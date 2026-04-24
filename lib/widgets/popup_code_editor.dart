import 'package:flutter/material.dart';
import 'package:flutter_code_editor/flutter_code_editor.dart';
import 'package:highlight/languages/python.dart';

class PopupCodeEditor extends StatefulWidget {
  final String code;
  final Function(String) onChanged;
  
  const PopupCodeEditor({
    super.key,
    required this.code,
    required this.onChanged,
  });
  
  @override
  State<PopupCodeEditor> createState() => _PopupCodeEditorState();
}

class _PopupCodeEditorState extends State<PopupCodeEditor> {
  late CodeController _controller;
  final FocusNode _focusNode = FocusNode();
  final TextEditingController _textController = TextEditingController();
  
  final List<String> _pythonKeywords = [
    'def', 'class', 'import', 'from', 'as', 'if', 'elif', 'else',
    'for', 'while', 'break', 'continue', 'return', 'yield', 'try',
    'except', 'finally', 'raise', 'with', 'lambda', 'and', 'or',
    'not', 'is', 'in', 'True', 'False', 'None', 'print', 'len',
    'range', 'str', 'int', 'float', 'list', 'dict', 'set', 'tuple',
    'open', 'file', 'input', 'abs', 'sum', 'min', 'max', 'sorted',
    'append', 'extend', 'insert', 'remove', 'pop', 'clear', 'index',
    'count', 'sort', 'reverse', 'copy', 'keys', 'values', 'items',
    'numpy', 'pandas', 'DataFrame', 'Series', 'array',
  ];
  
  OverlayEntry? _suggestionsOverlay;
  List<String> _currentSuggestions = [];
  int _selectedSuggestionIndex = 0;
  
  @override
  void initState() {
    super.initState();
    _textController.text = widget.code;
    _textController.addListener(_onTextChanged);
    _focusNode.addListener(_onFocusChanged);
  }
  
  void _onTextChanged() {
    widget.onChanged(_textController.text);
    _showSuggestionsPopup();
  }
  
  void _onFocusChanged() {
    if (!_focusNode.hasFocus) {
      _hideSuggestions();
    }
  }
  
  void _showSuggestionsPopup() {
    final text = _textController.text;
    final cursorPos = _textController.selection.baseOffset;
    if (cursorPos < 0) {
      _hideSuggestions();
      return;
    }
    
    // Get current word being typed
    int start = cursorPos;
    while (start > 0 && _isWordChar(text[start - 1])) {
      start--;
    }
    final currentWord = text.substring(start, cursorPos);
    
    if (currentWord.length >= 2) {
      final matches = _pythonKeywords
          .where((kw) => kw.toLowerCase().startsWith(currentWord.toLowerCase()))
          .toList();
      
      if (matches.isNotEmpty && _focusNode.hasFocus) {
        _currentSuggestions = matches.take(5).toList();
        _selectedSuggestionIndex = 0;
        _showPopupOverlay();
        return;
      }
    }
    
    _hideSuggestions();
  }
  
  bool _isWordChar(String char) {
    return RegExp(r'[a-zA-Z0-9_]').hasMatch(char);
  }
  
  void _showPopupOverlay() {
    _hideSuggestions();
    
    final RenderBox renderBox = context.findRenderObject() as RenderBox;
    final position = renderBox.localToGlobal(Offset.zero);
    
    _suggestionsOverlay = OverlayEntry(
      builder: (context) => Positioned(
        left: position.dx + 16,
        top: position.dy + 60,
        child: Material(
          elevation: 8,
          borderRadius: BorderRadius.circular(8),
          child: Container(
            width: 200,
            decoration: BoxDecoration(
              color: Colors.grey[850],
              borderRadius: BorderRadius.circular(8),
              border: Border.all(color: Colors.grey[700]!),
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: _currentSuggestions.asMap().entries.map((entry) {
                final index = entry.key;
                final suggestion = entry.value;
                final isSelected = index == _selectedSuggestionIndex;
                return InkWell(
                  onTap: () => _insertSuggestion(suggestion),
                  onMouseEnter: (_) => setState(() => _selectedSuggestionIndex = index),
                  child: Container(
                    padding: const EdgeInsets.all(12),
                    color: isSelected ? Colors.blue[900] : null,
                    child: Row(
                      children: [
                        const Icon(Icons.code, size: 16, color: Colors.grey),
                        const SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            suggestion,
                            style: TextStyle(
                              color: isSelected ? Colors.white : Colors.cyan,
                              fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              }).toList(),
            ),
          ),
        ),
      ),
    );
    
    Overlay.of(context).insert(_suggestionsOverlay!);
  }
  
  void _hideSuggestions() {
    _suggestionsOverlay?.remove();
    _suggestionsOverlay = null;
  }
  
  void _insertSuggestion(String suggestion) {
    final text = _textController.text;
    final cursorPos = _textController.selection.baseOffset;
    
    int start = cursorPos;
    while (start > 0 && _isWordChar(text[start - 1])) {
      start--;
    }
    
    final newText = text.substring(0, start) + suggestion + text.substring(cursorPos);
    _textController.text = newText;
    _textController.selection = TextSelection.collapsed(offset: start + suggestion.length);
    widget.onChanged(newText);
    _hideSuggestions();
  }
  
  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: Colors.grey[300]!),
      ),
      child: TextField(
        controller: _textController,
        focusNode: _focusNode,
        maxLines: null,
        style: const TextStyle(
          fontFamily: 'monospace',
          fontSize: 14,
          color: Colors.black87,
        ),
        decoration: const InputDecoration(
          border: InputBorder.none,
          contentPadding: EdgeInsets.all(12),
          hintText: '# Write Python code here',
          hintStyle: TextStyle(color: Colors.grey),
        ),
      ),
    );
  }
  
  @override
  void dispose() {
    _hideSuggestions();
    _textController.dispose();
    _focusNode.dispose();
    super.dispose();
  }
}
