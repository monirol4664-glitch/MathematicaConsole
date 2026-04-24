import 'dart:io';
import 'package:flutter/services.dart';
import 'package:path_provider/path_provider.dart';
import 'package:dio/dio.dart';

/// Handles Python execution and package management at runtime
class PythonService {
  static final PythonService _instance = PythonService._internal();
  factory PythonService() => _instance;
  PythonService._internal();
  
  bool _isInitialized = false;
  String? _pythonPath;
  
  /// Downloads and initializes Python on first run
  Future<bool> initializePython() async {
    if (_isInitialized) return true;
    
    try {
      final appDir = await getApplicationDocumentsDirectory();
      final pythonDir = Directory('${appDir.path}/python');
      
      if (!await pythonDir.exists()) {
        await pythonDir.create(recursive: true);
        await _downloadPythonRuntime(pythonDir.path);
      }
      
      _pythonPath = '${pythonDir.path}/bin/python3';
      _isInitialized = true;
      return true;
    } catch (e) {
      print('Python init error: $e');
      return false;
    }
  }
  
  /// Downloads Python runtime from CDN (only once)
  Future<void> _downloadPythonRuntime(String targetDir) async {
    final dio = Dio();
    
    try {
      // Download Python embedded distribution for Android
      final response = await dio.download(
        'https://github.com/indygreg/python-build-standalone/releases/download/20240415/cpython-3.11.8%2B20240415-aarch64-unknown-linux-gnu-install_only.tar.gz',
        '$targetDir/python.tar.gz',
      );
      
      // Extract Python
      await Process.run('tar', ['-xzf', '$targetDir/python.tar.gz', '-C', targetDir]);
      
      print('Python downloaded to $targetDir');
    } catch (e) {
      print('Download failed: $e');
      rethrow;
    }
  }
  
  /// Install Python packages dynamically
  Future<bool> installPackage(String packageName) async {
    if (!_isInitialized) await initializePython();
    
    try {
      final result = await Process.run(
        _pythonPath!,
        ['-m', 'pip', 'install', packageName, '--target', '${_pythonPath}/../lib/python3.11/site-packages'],
      );
      
      if (result.exitCode == 0) {
        print('Installed $packageName');
        return true;
      } else {
        print('Failed: ${result.stderr}');
        return false;
      }
    } catch (e) {
      print('Install error: $e');
      return false;
    }
  }
  
  /// Execute Python code
  Future<String> executeCode(String code) async {
    if (!_isInitialized) {
      final initSuccess = await initializePython();
      if (!initSuccess) return 'Error: Python initialization failed';
    }
    
    try {
      final tempFile = await _createTempFile(code);
      final result = await Process.run(_pythonPath!, [tempFile.path]);
      
      await tempFile.delete();
      
      if (result.exitCode == 0) {
        return result.stdout.toString();
      } else {
        return 'Error: ${result.stderr}';
      }
    } catch (e) {
      return 'Exception: $e';
    }
  }
  
  Future<File> _createTempFile(String code) async {
    final dir = await getTemporaryDirectory();
    final file = File('${dir.path}/temp_${DateTime.now().millisecondsSinceEpoch}.py');
    await file.writeAsString(code);
    return file;
  }
  
  /// Check if a package is installed
  Future<bool> isPackageInstalled(String packageName) async {
    try {
      final result = await Process.run(
        _pythonPath!,
        ['-c', f'import {packageName}; print(True)'],
      );
      return result.exitCode == 0;
    } catch (e) {
      return false;
    }
  }
  
  /// Get list of installed packages
  Future<List<String>> getInstalledPackages() async {
    final result = await Process.run(
      _pythonPath!,
      ['-m', 'pip', 'list', '--format=freeze'],
    );
    
    if (result.exitCode == 0) {
      return (result.stdout as String)
          .split('\n')
          .where((line) => line.isNotEmpty)
          .toList();
    }
    return [];
  }
}
