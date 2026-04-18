"""
Mathematica-style plotting widget using matplotlib
"""

from kivy.uix.boxlayout import BoxLayout
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.garden.matplotlib import FigureCanvasKivyAgg
import matplotlib.pyplot as plt
import numpy as np
from sympy import symbols, lambdify, sympify
import io
from kivy.core.image import Image as CoreImage
from kivy.uix.image import Image
from PIL import Image as PILImage

class PlotWidget(BoxLayout):
    """Widget for displaying mathematical plots like Mathematica"""
    
    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self.orientation = 'vertical'
        self.spacing = 10
        self.padding = 10
        
        # Plot area
        self.figure = plt.figure(figsize=(8, 6), dpi=80)
        self.canvas = FigureCanvasKivyAgg(self.figure)
        self.canvas.size_hint = (1, 0.9)
        
        # Control panel
        controls = BoxLayout(size_hint=(1, 0.1), spacing=5)
        close_btn = Button(text="Close", size_hint=(0.3, 1))
        close_btn.bind(on_press=self.close_plot)
        save_btn = Button(text="Save", size_hint=(0.3, 1))
        save_btn.bind(on_press=self.save_plot)
        
        controls.add_widget(Label(text="Plot Controls", size_hint=(0.4, 1)))
        controls.add_widget(save_btn)
        controls.add_widget(close_btn)
        
        self.add_widget(self.canvas)
        self.add_widget(controls)
    
    def plot_2d(self, expression, var_range, **kwargs):
        """Plot 2D function - like Mathematica's Plot[]"""
        self.figure.clear()
        ax = self.figure.add_subplot(111)
        
        try:
            # Parse expression
            x = symbols('x')
            expr = sympify(expression)
            f = lambdify(x, expr, modules=['numpy'])
            
            # Generate x values
            x_min, x_max = var_range
            x_vals = np.linspace(x_min, x_max, 1000)
            y_vals = f(x_vals)
            
            # Plot
            ax.plot(x_vals, y_vals, 'b-', linewidth=2, label=str(expression))
            ax.grid(True, alpha=0.3)
            ax.set_xlabel('x')
            ax.set_ylabel('y')
            ax.set_title(f'Plot of {expression}')
            ax.legend()
            
            # Set limits
            y_min, y_max = np.min(y_vals), np.max(y_vals)
            y_padding = (y_max - y_min) * 0.1
            ax.set_ylim(y_min - y_padding, y_max + y_padding)
            
            self.canvas.draw()
            return True
            
        except Exception as e:
            ax.text(0.5, 0.5, f'Plot Error: {str(e)}', 
                   horizontalalignment='center', verticalalignment='center')
            self.canvas.draw()
            return False
    
    def plot_3d(self, expression, x_range, y_range, **kwargs):
        """Plot 3D surface - like Mathematica's Plot3D[]"""
        self.figure.clear()
        ax = self.figure.add_subplot(111, projection='3d')
        
        try:
            from sympy import symbols, lambdify, sympify
            import numpy as np
            
            x, y = symbols('x y')
            expr = sympify(expression)
            f = lambdify((x, y), expr, modules=['numpy'])
            
            # Generate grid
            x_min, x_max = x_range
            y_min, y_max = y_range
            x_vals = np.linspace(x_min, x_max, 50)
            y_vals = np.linspace(y_min, y_max, 50)
            X, Y = np.meshgrid(x_vals, y_vals)
            Z = f(X, Y)
            
            # Plot surface
            surf = ax.plot_surface(X, Y, Z, cmap='viridis', alpha=0.8)
            self.figure.colorbar(surf, ax=ax, shrink=0.5, aspect=5)
            
            ax.set_xlabel('x')
            ax.set_ylabel('y')
            ax.set_zlabel('z')
            ax.set_title(f'3D Plot of {expression}')
            
            self.canvas.draw()
            return True
            
        except Exception as e:
            ax.text2D(0.5, 0.5, f'Plot Error: {str(e)}', 
                     transform=ax.transAxes)
            self.canvas.draw()
            return False
    
    def plot_parametric(self, x_expr, y_expr, param_range, **kwargs):
        """Plot parametric equations"""
        self.figure.clear()
        ax = self.figure.add_subplot(111)
        
        try:
            t = symbols('t')
            x_f = lambdify(t, sympify(x_expr), modules=['numpy'])
            y_f = lambdify(t, sympify(y_expr), modules=['numpy'])
            
            t_min, t_max = param_range
            t_vals = np.linspace(t_min, t_max, 1000)
            x_vals = x_f(t_vals)
            y_vals = y_f(t_vals)
            
            ax.plot(x_vals, y_vals, 'r-', linewidth=2)
            ax.grid(True, alpha=0.3)
            ax.set_xlabel('x')
            ax.set_ylabel('y')
            ax.set_title(f'Parametric Plot: x={x_expr}, y={y_expr}')
            ax.axis('equal')
            
            self.canvas.draw()
            return True
            
        except Exception as e:
            ax.text(0.5, 0.5, f'Plot Error: {str(e)}', 
                   horizontalalignment='center', verticalalignment='center')
            self.canvas.draw()
            return False
    
    def close_plot(self, instance):
        """Close the plot widget"""
        self.parent.remove_widget(self)
    
    def save_plot(self, instance):
        """Save plot to file"""
        try:
            from kivy.storage.jsonstore import JsonStore
            import os
            
            # Save to external storage
            filename = f'/sdcard/mathematica_plot_{len(os.listdir("/sdcard"))}.png'
            self.figure.savefig(filename, dpi=150, bbox_inches='tight')
            
            # Show confirmation
            from kivy.uix.popup import Popup
            from kivy.uix.label import Label
            popup = Popup(title='Saved', 
                         content=Label(text=f'Plot saved to\n{filename}'),
                         size_hint=(0.8, 0.3))
            popup.open()
            
        except Exception as e:
            print(f"Save error: {e}")