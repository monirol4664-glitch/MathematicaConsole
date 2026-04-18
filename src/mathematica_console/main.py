"""
Mathematica Console - Offline Android App
Entry point for the application
"""

from kivy.config import Config
Config.set('kivy', 'keyboard_mode', 'systemandmulti')  # Better keyboard support
Config.set('graphics', 'resizable', False)

from kivy.core.window import Window
from .console import ConsoleApp

def main():
    """Main entry point"""
    # Set window size for better mobile experience
    Window.size = (400, 700)
    Window.clearcolor = (0.1, 0.1, 0.12, 1)
    
    app = ConsoleApp()
    app.run()

if __name__ == '__main__':
    main()