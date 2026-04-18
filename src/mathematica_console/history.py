"""
Command history management like Mathematica's % command
"""

import json
import os
from datetime import datetime
from collections import deque

class HistoryManager:
    """Manage command history with persistent storage"""
    
    def __init__(self, max_size=1000):
        self.max_size = max_size
        self.history = deque(maxlen=max_size)
        self.current_index = -1
        self.temp_input = ""
        self.history_file = None
        
        # Try to set up persistent storage
        try:
            self.history_file = '/sdcard/mathematica_history.json'
            self.load_history()
        except:
            pass
    
    def add_command(self, command):
        """Add command to history"""
        if command and command.strip():
            # Don't duplicate consecutive identical commands
            if self.history and self.history[-1]['command'] == command:
                return
            
            entry = {
                'command': command,
                'timestamp': datetime.now().isoformat(),
                'index': len(self.history)
            }
            self.history.append(entry)
            self.current_index = len(self.history)
            self.save_history()
    
    def get_previous(self, current_input=""):
        """Get previous command in history (like up arrow)"""
        if not self.history:
            return current_input
        
        if self.current_index == len(self.history):
            self.temp_input = current_input
        
        if self.current_index > 0:
            self.current_index -= 1
            return self.history[self.current_index]['command']
        
        return self.history[0]['command']
    
    def get_next(self):
        """Get next command in history (like down arrow)"""
        if not self.history:
            return ""
        
        if self.current_index < len(self.history) - 1:
            self.current_index += 1
            return self.history[self.current_index]['command']
        elif self.current_index == len(self.history) - 1:
            self.current_index = len(self.history)
            return self.temp_input
        
        return ""
    
    def search_history(self, query):
        """Search history for commands containing query"""
        results = []
        for entry in self.history:
            if query.lower() in entry['command'].lower():
                results.append(entry)
        return results
    
    def get_last(self, n=1):
        """Get last n commands"""
        if n <= 0:
            return []
        return list(self.history)[-n:]
    
    def clear_history(self):
        """Clear all history"""
        self.history.clear()
        self.current_index = -1
        self.temp_input = ""
        self.save_history()
    
    def save_history(self):
        """Save history to file for persistence"""
        if self.history_file:
            try:
                with open(self.history_file, 'w') as f:
                    json.dump(list(self.history), f, indent=2)
            except:
                pass
    
    def load_history(self):
        """Load history from file"""
        if self.history_file and os.path.exists(self.history_file):
            try:
                with open(self.history_file, 'r') as f:
                    data = json.load(f)
                    self.history = deque(data, maxlen=self.max_size)
                    self.current_index = len(self.history)
            except:
                pass
    
    def format_history(self, limit=20):
        """Format history for display"""
        if not self.history:
            return "No command history."
        
        lines = []
        start = max(0, len(self.history) - limit)
        for i in range(start, len(self.history)):
            entry = self.history[i]
            lines.append(f"{entry['index']+1}: {entry['command']}")
        
        return "\n".join(lines)