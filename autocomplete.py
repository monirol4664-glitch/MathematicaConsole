"""
Auto-completion engine using Trie data structure
"""

import re


class TrieNode:
    """Node for Trie (prefix tree) data structure"""
    
    def __init__(self):
        self.children = {}
        self.is_word = False


class AutoCompleter:
    """Fast prefix-based word completion using Trie"""
    
    def __init__(self):
        self.root = TrieNode()
        self._load_python_keywords()
        self._load_common_stdlib()
    
    def _load_python_keywords(self):
        """Add Python keywords to the trie"""
        keywords = [
            # Control flow
            'and', 'or', 'not', 'if', 'elif', 'else', 'for', 'while',
            'break', 'continue', 'pass', 'return', 'yield',
            
            # Functions and classes
            'def', 'class', 'lambda', 'async', 'await',
            
            # Imports
            'import', 'from', 'as', 'with', 'assert',
            
            # Exception handling
            'try', 'except', 'finally', 'raise',
            
            # Booleans and None
            'True', 'False', 'None',
            
            # Other
            'global', 'nonlocal', 'is', 'in', 'del'
        ]
        
        for keyword in keywords:
            self.insert(keyword)
    
    def _load_common_stdlib(self):
        """Add common Python builtins and stdlib functions"""
        builtins = [
            'print', 'len', 'range', 'str', 'int', 'float', 'bool',
            'list', 'dict', 'set', 'tuple', 'type', 'isinstance',
            'issubclass', 'hasattr', 'getattr', 'setattr', 'delattr',
            'abs', 'all', 'any', 'bin', 'hex', 'oct', 'chr', 'ord',
            'divmod', 'enumerate', 'filter', 'map', 'zip', 'reversed',
            'sorted', 'sum', 'min', 'max', 'round', 'pow'
        ]
        
        common_functions = [
            'open', 'input', 'format', 'repr', 'ascii', 'hash',
            'help', 'dir', 'vars', 'locals', 'globals', 'id'
        ]
        
        for func in builtins + common_functions:
            self.insert(func)
        
        # Add common module names
        modules = ['math', 'random', 'datetime', 'json', 'os', 'sys',
                  're', 'time', 'collections', 'itertools', 'functools']
        
        for module in modules:
            self.insert(module)
    
    def insert(self, word):
        """Insert a word into the trie"""
        word = word.lower()
        node = self.root
        
        for char in word:
            if char not in node.children:
                node.children[char] = TrieNode()
            node = node.children[char]
        
        node.is_word = True
    
    def search(self, word):
        """Check if word exists in trie"""
        word = word.lower()
        node = self.root
        
        for char in word:
            if char not in node.children:
                return False
            node = node.children[char]
        
        return node.is_word
    
    def get_suggestions(self, prefix, max_suggestions=10):
        """Get autocomplete suggestions for given prefix"""
        prefix = prefix.lower()
        node = self.root
        
        # Navigate to prefix node
        for char in prefix:
            if char not in node.children:
                return []
            node = node.children[char]
        
        # Collect all words starting with prefix
        suggestions = []
        self._collect_words(node, prefix, suggestions)
        
        # Sort by length and alphabetically
        suggestions.sort(key=lambda x: (len(x), x))
        
        return suggestions[:max_suggestions]
    
    def _collect_words(self, node, current_word, suggestions):
        """Recursively collect all words from node"""
        if node.is_word and current_word not in suggestions:
            suggestions.append(current_word)
        
        for char, child_node in sorted(node.children.items()):
            self._collect_words(child_node, current_word + char, suggestions)
    
    def get_context_suggestions(self, text, cursor_position):
        """Get suggestions based on code context (improved)"""
        # Get current line and word being typed
        lines = text.split('\n')
        
        # Find line at cursor
        line_num = 0
        char_count = 0
        for i, line in enumerate(lines):
            if char_count + len(line) + 1 > cursor_position:
                line_num = i
                break
            char_count += len(line) + 1
        
        current_line = lines[line_num]
        line_pos = cursor_position - char_count
        
        # Get current word
        word_start = line_pos
        while word_start > 0 and current_line[word_start - 1].isalnum():
            word_start -= 1
        
        current_word = current_line[word_start:line_pos]
        
        if len(current_word) >= 2:
            return self.get_suggestions(current_word)
        
        return []