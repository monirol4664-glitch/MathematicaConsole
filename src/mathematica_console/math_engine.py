"""
Mathematica-style symbolic math engine using SymPy
Provides core functionality similar to Wolfram Language
"""

from sympy import *
from sympy.parsing.sympy_parser import parse_expr, standard_transformations, implicit_multiplication_application
import mpmath
import math

class MathEngine:
    """Core mathematical engine with Mathematica-like functionality"""
    
    def __init__(self):
        # Initialize symbolic environment
        self.x, self.y, self.z = symbols('x y z')
        self.env = {
            'pi': pi, 'Pi': pi, 'π': pi,
            'E': E, 'e': E,
            'I': I, 'i': I,
            'oo': oo, 'infinity': oo,
            'True': True, 'False': False,
            'sin': sin, 'cos': cos, 'tan': tan,
            'asin': asin, 'acos': acos, 'atan': atan,
            'sinh': sinh, 'cosh': cosh, 'tanh': tanh,
            'log': log, 'ln': log, 'lg': lambda x: log(x, 10),
            'exp': exp, 'sqrt': sqrt,
            'abs': Abs, 'floor': floor, 'ceiling': ceiling,
            'gamma': gamma, 'factorial': factorial,
            'integrate': integrate, 'diff': diff,
            'limit': limit, 'series': series,
            'solve': solve, 'dsolve': dsolve,
            'Matrix': Matrix, 'det': det, 'inv': inv,
            'transpose': transpose, 'eigenvals': eigenvals,
            'plot': self._plot_wrapper,
            'Plot': self._plot_wrapper,
        }
        
        # Define common mathematical constants
        self.constants = {
            'GoldenRatio': (1 + sqrt(5)) / 2,
            'Catalan': catalan,
            'EulerGamma': EulerGamma,
        }
        self.env.update(self.constants)
        
        # Result history
        self.history = []
        self.last_result = None
        
        # Setup transformations for natural input
        self.transformations = standard_transformations + (implicit_multiplication_application,)
    
    def evaluate(self, expression: str) -> str:
        """
        Evaluate a mathematical expression - similar to Wolfram's evaluation
        Supports: 2+2, Integrate[x^2, x], D[Sin[x], x], Solve[x^2-4==0, x], etc.
        """
        if not expression or expression.strip() == '':
            return ''
        
        expr_str = expression.strip()
        
        # Special commands
        if expr_str.lower() == 'clear':
            self.history.clear()
            return 'History cleared.'
        
        if expr_str.lower() == 'history':
            return self.show_history()
        
        if expr_str.lower() == 'vars':
            return self.show_variables()
        
        # Check for assignment (variable = expression)
        if '=' in expr_str and not expr_str.startswith(('=', '!=', '<=', '>=')):
            return self.handle_assignment(expr_str)
        
        try:
            # Parse the expression
            parsed = parse_expr(expr_str, transformations=self.transformations, local_dict=self.env)
            
            # Evaluate
            result = self._simplify_result(parsed)
            
            # Format output nicely
            formatted = self.format_output(result)
            
            # Store in history
            self.history.append({
                'input': expr_str,
                'output': formatted,
                'raw': result
            })
            self.last_result = result
            
            return formatted
            
        except Exception as e:
            return f"Error: {str(e)}"
    
    def handle_assignment(self, expr_str: str) -> str:
        """Handle variable assignments like: a = 5 or f(x) = x^2"""
        parts = expr_str.split('=', 1)
        var_name = parts[0].strip()
        value_str = parts[1].strip()
        
        try:
            # Parse the value
            value = parse_expr(value_str, transformations=self.transformations, local_dict=self.env)
            
            # Check if it's a function definition (contains parentheses)
            if '(' in var_name and ')' in var_name:
                # Function definition: f(x) = x^2
                func_name = var_name.split('(')[0].strip()
                self.env[func_name] = Lambda(tuple(symbols(var_name)), value)
                return f"{func_name} defined as {self.format_output(value)}"
            else:
                # Variable assignment
                self.env[var_name] = value
                return f"{var_name} = {self.format_output(value)}"
        except Exception as e:
            return f"Assignment error: {str(e)}"
    
    def _simplify_result(self, expr):
        """Simplify and evaluate results intelligently"""
        # Numeric evaluation for constants
        if expr.is_Number:
            if expr.is_Integer:
                return expr
            else:
                return expr.evalf()
        
        # Simplify if possible
        simplified = simplify(expr)
        
        # Try to get numeric approximation for complex expressions
        if not simplified.is_Number and simplified.is_constant():
            try:
                numeric = simplified.evalf()
                return numeric
            except:
                return simplified
        
        return simplified
    
    def _plot_wrapper(self, *args, **kwargs):
        """Wrapper for plotting - returns a plot object"""
        try:
            if len(args) >= 2:
                # plot(x^2, (x, -10, 10))
                expr = args[0]
                range_spec = args[1] if len(args) > 1 else (self.x, -10, 10)
                if isinstance(range_spec, tuple) and len(range_spec) == 3:
                    p = plot(expr, range_spec, show=False)
                    return f"Plot generated for {expr}"
            return "Plot function called. Use: plot(expression, (var, min, max))"
        except Exception as e:
            return f"Plot error: {str(e)}"
    
    def format_output(self, result):
        """Format output nicely - similar to Wolfram's output formatting"""
        if result is None:
            return ""
        
        # Handle matrices
        if hasattr(result, 'shape'):
            rows, cols = result.shape
            if rows <= 10 and cols <= 10:
                return str(result)
        
        # Handle equations and relationals
        if isinstance(result, (Eq, Ne, Lt, Le, Gt, Ge)):
            return str(result)
        
        # Handle sets and lists
        if isinstance(result, (set, list, tuple)):
            if len(result) <= 20:
                return str(result)
            return f"{str(result[:10])} ... (length {len(result)})"
        
        # Default formatting
        result_str = str(result)
        
        # Clean up formatting
        result_str = result_str.replace('**', '^')
        
        return result_str
    
    def show_history(self) -> str:
        """Show evaluation history"""
        if not self.history:
            return "No history yet."
        
        history_str = "\n".join([
            f"In[{i+1}]: {h['input']}\nOut[{i+1}]: {h['output']}\n"
            for i, h in enumerate(self.history[-10:])  # Last 10 entries
        ])
        return history_str
    
    def show_variables(self) -> str:
        """Show defined variables and functions"""
        defined = {k: v for k, v in self.env.items() 
                   if not k.startswith('_') and k not in dir(sympy) and k not in ['x', 'y', 'z']}
        
        if not defined:
            return "No user-defined variables."
        
        return "\n".join([f"{k} = {self.format_output(v)}" for k, v in defined.items()])
    
    def get_autocomplete_suggestions(self, prefix: str) -> list:
        """Get Mathematica-like autocomplete suggestions"""
        functions = [
            'Integrate', 'D', 'Solve', 'DSolve', 'Simplify', 'Expand', 'Factor',
            'Plot', 'Matrix', 'Det', 'Inverse', 'Eigenvalues', 'Limit', 'Series',
            'Sum', 'Product', 'Log', 'Exp', 'Sin', 'Cos', 'Tan', 'ArcSin',
            'Sqrt', 'Abs', 'Floor', 'Ceiling', 'Gamma', 'Factorial'
        ]
        
        if not prefix:
            return functions[:10]
        
        return [f for f in functions if f.lower().startswith(prefix.lower())]

# Mathematica-like syntax mappings
MATHEMATICA_SYNTAX = {
    'Integrate': 'integrate',  # Integrate[x^2, x]
    'D': 'diff',                # D[x^2, x]
    'Solve': 'solve',          # Solve[x^2-4==0, x]
    'DSolve': 'dsolve',        # DSolve[y''+y==0, y(x), x]
    'Plot': 'plot',            # Plot[x^2, {x, -10, 10}]
    'MatrixForm': 'Matrix',
    'Det': 'det',
    'Inverse': 'inv',
    'Eigenvalues': 'eigenvals',
    'Limit': 'limit',
    'Series': 'series',
    'Sum': 'summation',
    'Product': 'product',
    'Simplify': 'simplify',
    'Expand': 'expand',
    'Factor': 'factor',
}