"""
Mathematica-style Console Interface
Similar to Wolfram's notebook input/output cells
"""

from kivy.app import App
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.gridlayout import GridLayout
from kivy.uix.scrollview import ScrollView
from kivy.uix.textinput import TextInput
from kivy.uix.label import Label
from kivy.uix.button import Button
from kivy.uix.popup import Popup
from kivy.clock import Clock
from kivy.core.clipboard import Clipboard
from kivy.graphics import Color, Rectangle
from kivy.metrics import dp

from .math_engine import MathEngine

class CellInput(TextInput):
    """Input cell - like Wolfram's In[n] cell"""
    def __init__(self, console, **kwargs):
        super().__init__(**kwargs)
        self.console = console
        self.multiline = True
        self.size_hint_y = None
        self.height = dp(60)
        self.background_color = (0.15, 0.15, 0.2, 1)
        self.foreground_color = (0.9, 0.9, 0.9, 1)
        self.hint_text = "In[?]:= Enter expression..."
        self.bind(height=self._set_height)
        self.bind(on_text_validate=self.on_enter)
    
    def _set_height(self, instance, value):
        self.height = max(dp(60), value)
    
    def on_enter(self, instance):
        self.console.evaluate_current_cell()

class CellOutput(Label):
    """Output cell - like Wolfram's Out[n] cell"""
    def __init__(self, console, text="", **kwargs):
        super().__init__(**kwargs)
        self.console = console
        self.text = text
        self.size_hint_y = None
        self.height = dp(40)
        self.text_size = (self.width, None)
        self.bind(width=lambda *x: self._set_text_size())
        self.bind(texture_size=self._set_height)
        self.color = (0.6, 0.8, 0.9, 1)
        self.halign = 'left'
        self.valign = 'top'
        self.padding = (dp(10), dp(5))
        self.font_size = dp(14)
    
    def _set_text_size(self):
        self.text_size = (self.width - dp(20), None)
    
    def _set_height(self, instance, value):
        self.height = max(dp(40), value[1] + dp(10))

class ConsoleCell(BoxLayout):
    """Single cell containing input and output"""
    def __init__(self, console, cell_number, **kwargs):
        super().__init__(**kwargs)
        self.console = console
        self.cell_number = cell_number
        self.orientation = 'vertical'
        self.size_hint_y = None
        self.spacing = dp(5)
        self.padding = [dp(10), dp(5), dp(10), dp(5)]
        
        # Input area
        input_layout = BoxLayout(size_hint_y=None, height=dp(30))
        input_label = Label(text=f"In[{cell_number}]:=", size_hint_x=0.15, 
                           color=(0.7, 0.7, 0.7, 1), font_size=dp(14))
        self.input_field = CellInput(console, size_hint_x=0.85)
        input_layout.add_widget(input_label)
        input_layout.add_widget(self.input_field)
        
        # Output area
        self.output_area = CellOutput(console, "")
        
        self.add_widget(input_layout)
        self.add_widget(self.output_area)
    
    def set_output(self, text):
        self.output_area.text = text
        self.output_area.height = max(dp(40), len(text.split('\n')) * dp(20))

class ConsoleWidget(ScrollView):
    """Main console widget managing all cells"""
    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.do_scroll_x = False
        self.do_scroll_y = True
        
        self.content = GridLayout(cols=1, size_hint_y=None, spacing=dp(10))
        self.content.bind(minimum_height=self.content.setter('height'))
        self.add_widget(self.content)
        
        self.math_engine = MathEngine()
        self.cell_counter = 1
        
        # Add first cell
        self.add_new_cell()
    
    def add_new_cell(self, insert_before=False):
        """Add a new input cell"""
        cell = ConsoleCell(self, self.cell_counter)
        self.cell_counter += 1
        self.content.add_widget(cell)
        
        # Scroll to new cell
        Clock.schedule_once(lambda dt: self.scroll_to(cell), 0.1)
        
        return cell
    
    def evaluate_current_cell(self):
        """Evaluate the current cell - like pressing Shift+Enter in Mathematica"""
        # Find current cell (last one with input)
        for child in reversed(self.content.children):
            if isinstance(child, ConsoleCell):
                expr = child.input_field.text.strip()
                if expr:
                    # Evaluate
                    result = self.math_engine.evaluate(expr)
                    child.set_output(result)
                    
                    # Create new cell for next input
                    self.add_new_cell()
                    return
        
        # If no cell with input found, add new
        self.add_new_cell()
    
    def clear_all(self):
        """Clear all cells"""
        self.content.clear_widgets()
        self.cell_counter = 1
        self.add_new_cell()
    
    def show_history(self):
        """Show command history"""
        history = self.math_engine.show_history()
        popup = Popup(title='History', content=Label(text=history), size_hint=(0.8, 0.6))
        popup.open()

class ConsoleApp(App):
    """Main Kivy application"""
    def build(self):
        self.title = "Mathematica Console - Offline"
        
        main_layout = BoxLayout(orientation='vertical')
        
        # Toolbar
        toolbar = BoxLayout(size_hint_y=0.08, padding=dp(5), spacing=dp(5))
        
        clear_btn = Button(text="Clear All", on_press=self.clear_console)
        history_btn = Button(text="History", on_press=self.show_history)
        help_btn = Button(text="Help", on_press=self.show_help)
        
        toolbar.add_widget(clear_btn)
        toolbar.add_widget(history_btn)
        toolbar.add_widget(help_btn)
        
        # Console
        self.console = ConsoleWidget()
        
        main_layout.add_widget(toolbar)
        main_layout.add_widget(self.console)
        
        return main_layout
    
    def clear_console(self, instance):
        self.console.clear_all()
    
    def show_history(self, instance):
        self.console.show_history()
    
    def show_help(self, instance):
        help_text = """
        Mathematica Console - Offline Edition
        
        Basic Commands:
        • 2+2 → 4
        • Integrate[x^2, x] → x^3/3
        • D[Sin[x], x] → cos(x)
        • Solve[x^2-4==0, x] → [-2, 2]
        • Simplify[(x^2-1)/(x-1)] → x+1
        • Expand[(x+y)^3] → x^3+3x^2y+3xy^2+y^3
        
        Variables:
        • a = 5 (assign)
        • a + 3 (use)
        
        Functions:
        • f(x) = x^2 (define)
        • f(5) → 25
        
        Constants:
        • pi, e, I (imaginary), oo (infinity)
        
        Special Commands:
        • clear - Clear console
        • history - Show history
        • vars - Show variables
        
        All computation is done OFFLINE!
        """
        popup = Popup(title='Help', content=Label(text=help_text), size_hint=(0.9, 0.8))
        popup.open()