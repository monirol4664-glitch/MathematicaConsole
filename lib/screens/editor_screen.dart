import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../models/notebook_model.dart';
import '../services/python_service.dart';
import '../widgets/code_editor.dart';

class EditorScreen extends StatefulWidget {
  const EditorScreen({super.key});
  
  @override
  State<EditorScreen> createState() => _EditorScreenState();
}

class _EditorScreenState extends State<EditorScreen> {
  final PythonService _pythonService = PythonService();
  
  @override
  Widget build(BuildContext context) {
    final notebook = Provider.of<NotebookModel>(context);
    
    return Scaffold(
      appBar: AppBar(
        title: const Text('🐍 Python IDE'),
        centerTitle: true,
        actions: [
          IconButton(
            icon: const Icon(Icons.play_arrow),
            onPressed: () => _runAllCells(notebook),
            tooltip: 'Run All',
          ),
          IconButton(
            icon: const Icon(Icons.cleaning_services),
            onPressed: () => notebook.clearAllOutputs(),
            tooltip: 'Clear Outputs',
          ),
        ],
      ),
      body: ReorderableListView(
        onReorder: (oldIndex, newIndex) => notebook.reorderCells(oldIndex, newIndex),
        children: [
          for (var cell in notebook.cells)
            Container(
              key: Key(cell.id),
              margin: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: Colors.grey[850],
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildToolbar(notebook, cell),
                  Padding(
                    padding: const EdgeInsets.all(8),
                    child: CodeEditor(
                      code: cell.code,
                      onChanged: (code) => notebook.updateCode(cell.id, code),
                    ),
                  ),
                  if (cell.output.isNotEmpty || cell.isRunning)
                    Container(
                      margin: const EdgeInsets.all(8),
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: Colors.grey[900],
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: cell.isRunning
                          ? const Row(
                              children: [
                                SizedBox(
                                  width: 16,
                                  height: 16,
                                  child: CircularProgressIndicator(strokeWidth: 2),
                                ),
                                SizedBox(width: 8),
                                Text('Running...'),
                              ],
                            )
                          : SelectableText(
                              cell.output,
                              style: const TextStyle(
                                fontFamily: 'monospace',
                                color: Colors.green,
                                fontSize: 12,
                              ),
                            ),
                    ),
                ],
              ),
            ),
          const SizedBox(height: 80),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => notebook.addCell(),
        child: const Icon(Icons.add),
      ),
    );
  }
  
  Widget _buildToolbar(NotebookModel notebook, CodeCell cell) {
    return Container(
      padding: const EdgeInsets.all(8),
      decoration: BoxDecoration(
        color: Colors.grey[800],
        borderRadius: const BorderRadius.vertical(top: Radius.circular(12)),
      ),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
            decoration: BoxDecoration(
              color: Colors.deepPurple[700],
              borderRadius: BorderRadius.circular(12),
            ),
            child: Text(
              'In [${cell.id.split('_').last}]',
              style: const TextStyle(fontSize: 12),
            ),
          ),
          const Spacer(),
          IconButton(
            icon: const Icon(Icons.play_arrow, color: Colors.green, size: 20),
            onPressed: () => _runCell(notebook, cell),
          ),
          IconButton(
            icon: const Icon(Icons.add, color: Colors.blue, size: 20),
            onPressed: () => notebook.addCell(afterId: cell.id),
          ),
          IconButton(
            icon: const Icon(Icons.delete, color: Colors.red, size: 20),
            onPressed: () => notebook.deleteCell(cell.id),
          ),
        ],
      ),
    );
  }
  
  Future<void> _runCell(NotebookModel notebook, CodeCell cell) async {
    notebook.setRunning(cell.id, true);
    final output = await _pythonService.executeCode(cell.code);
    notebook.updateOutput(cell.id, output);
  }
  
  Future<void> _runAllCells(NotebookModel notebook) async {
    for (var cell in notebook.cells) {
      if (cell.code.trim().isNotEmpty) {
        await _runCell(notebook, cell);
      }
    }
  }
}
