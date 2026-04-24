import 'package:chaquopy/chaquopy.dart';

class ExecutionService {
  Future<String> executeCode(String code) async {
    try {
      final result = await Chaquopy.executeCode(code);
      return result["textOutputOrError"]?.toString() ?? "Executed successfully (no output).";
    } catch (e) {
      return "Python Error: ${e.toString()}";
    }
  }
}