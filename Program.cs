using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using Android.Text;
using Android.Text.Style;
using Android.Text.Method;
using System.Text.RegularExpressions;

namespace MathematicaConsole;

[Activity(Label = "Mathematica Console", MainLauncher = true, Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
public class MainActivity : Activity
{
    private EditText inputField;
    private TextView outputArea;
    private ScrollView scrollView;
    private LinearLayout consoleLayout;
    private List<string> history = new List<string>();
    private int historyIndex = -1;
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Main layout
        var rootLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };
        rootLayout.SetBackgroundColor(Color.ParseColor("#1E1E1E"));
        
        // Title bar
        var titleBar = new TextView(this)
        {
            Text = "📐 Mathematica Console v1.0",
            TextSize = 18,
            SetBackgroundColor = Color.ParseColor("#2D2D2D"),
            Gravity = GravityFlags.Center
        };
        titleBar.SetTextColor(Color.ParseColor("#4EC9B0"));
        titleBar.SetPadding(20, 20, 20, 20);
        
        // Console output area (scrollable)
        scrollView = new ScrollView(this);
        consoleLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        consoleLayout.SetPadding(20, 20, 20, 20);
        scrollView.AddView(consoleLayout);
        
        // Welcome message
        AddConsoleOutput("Mathematica Console for Android", "#4EC9B0", 20, true);
        AddConsoleOutput("Type mathematical expressions and press Enter\n", "#888888", 14);
        AddConsoleOutput("Examples:", "#888888", 14);
        AddConsoleOutput("  > 2+2", "#FFD700");
        AddConsoleOutput("  > Integrate[x^2, x]", "#FFD700");
        AddConsoleOutput("  > Plot[Sin[x], {x,0,2Pi}]", "#FFD700");
        AddConsoleOutput("  > MatrixForm[{{1,2},{3,4}}]\n", "#FFD700");
        
        // Input area
        var inputLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal,
            SetBackgroundColor = Color.ParseColor("#252526")
        };
        inputLayout.SetPadding(10, 5, 10, 5);
        
        var prompt = new TextView(this)
        {
            Text = "> ",
            TextSize = 18,
            Typeface = Typeface.Create("monospace", TypefaceStyle.Normal)
        };
        prompt.SetTextColor(Color.ParseColor("#4EC9B0"));
        
        inputField = new EditText(this)
        {
            TextSize = 18,
            Hint = "Enter expression...",
            InputType = Android.Text.InputTypes.TextFlagNoSuggestions,
            Typeface = Typeface.Create("monospace", TypefaceStyle.Normal)
        };
        inputField.SetTextColor(Color.ParseColor("#D4D4D4"));
        inputField.SetHintTextColor(Color.ParseColor("#6A6A6A"));
        inputField.SetBackgroundColor(Color.Transparent);
        
        // Enter key handling
        inputField.EditorAction += (s, e) => {
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Send)
            {
                ProcessInput();
                e.Handled = true;
            }
        };
        
        // History navigation
        inputField.KeyPress += (s, e) => {
            if (e.KeyCode == Keycode.DpadUp)
            {
                NavigateHistory(-1);
                e.Handled = true;
            }
            else if (e.KeyCode == Keycode.DpadDown)
            {
                NavigateHistory(1);
                e.Handled = true;
            }
        };
        
        // Buttons layout
        var buttonLayout = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };
        
        var evalButton = new Button(this)
        {
            Text = "Evaluate",
            SetBackgroundColor = Color.ParseColor("#0E639C")
        };
        evalButton.SetTextColor(Color.White);
        evalButton.Click += (s, e) => ProcessInput();
        
        var clearButton = new Button(this)
        {
            Text = "Clear",
            SetBackgroundColor = Color.ParseColor("#3E3E42")
        };
        clearButton.SetTextColor(Color.White);
        clearButton.Click += (s, e) => ClearConsole();
        
        buttonLayout.AddView(evalButton, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 1));
        buttonLayout.AddView(clearButton, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 1));
        buttonLayout.SetPadding(10, 0, 10, 10);
        
        inputLayout.AddView(prompt);
        inputLayout.AddView(inputField, new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, 1));
        
        rootLayout.AddView(titleBar);
        rootLayout.AddView(scrollView, new LinearLayout.LayoutParams(
            LinearLayout.LayoutParams.MatchParent, 0, 1));
        rootLayout.AddView(inputLayout);
        rootLayout.AddView(buttonLayout);
        
        SetContentView(rootLayout);
        
        // Show keyboard
        inputField.RequestFocus();
    }
    
    private void ProcessInput()
    {
        string input = inputField.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(input)) return;
        
        // Add to history
        history.Add(input);
        historyIndex = history.Count;
        
        // Show input
        AddConsoleOutput($"> {input}", "#4EC9B0", 16);
        
        // Evaluate
        string result = EvaluateMathematica(input);
        AddConsoleOutput(result, result.StartsWith("Error") ? "#F48771" : "#CE9178", 16);
        AddConsoleOutput("", "#888888", 12);
        
        // Clear input
        inputField.Text = "";
        
        // Auto-scroll
        scrollView.Post(() => scrollView.FullScroll(FocusSearchDirection.Down));
    }
    
    private string EvaluateMathematica(string expression)
    {
        try
        {
            // Basic arithmetic
            if (expression.Contains("+") || expression.Contains("-") || 
                expression.Contains("*") || expression.Contains("/") || expression.Contains("^"))
            {
                return EvaluateArithmetic(expression);
            }
            
            // Calculus
            if (expression.StartsWith("Integrate", StringComparison.OrdinalIgnoreCase))
                return HandleIntegrate(expression);
            
            if (expression.StartsWith("Derivative", StringComparison.OrdinalIgnoreCase) ||
                expression.StartsWith("D[", StringComparison.OrdinalIgnoreCase))
                return HandleDerivative(expression);
            
            // Algebra
            if (expression.StartsWith("Simplify", StringComparison.OrdinalIgnoreCase))
                return HandleSimplify(expression);
            
            if (expression.StartsWith("Expand", StringComparison.OrdinalIgnoreCase))
                return HandleExpand(expression);
            
            if (expression.StartsWith("Factor", StringComparison.OrdinalIgnoreCase))
                return HandleFactor(expression);
            
            // Linear Algebra
            if (expression.StartsWith("MatrixForm", StringComparison.OrdinalIgnoreCase))
                return HandleMatrixForm(expression);
            
            if (expression.StartsWith("Det[", StringComparison.OrdinalIgnoreCase))
                return HandleDeterminant(expression);
            
            // Plotting
            if (expression.StartsWith("Plot", StringComparison.OrdinalIgnoreCase))
                return HandlePlot(expression);
            
            // Special functions
            if (expression.StartsWith("Sin", StringComparison.OrdinalIgnoreCase))
                return EvaluateTrig(expression, Math.Sin);
            if (expression.StartsWith("Cos", StringComparison.OrdinalIgnoreCase))
                return EvaluateTrig(expression, Math.Cos);
            if (expression.StartsWith("Tan", StringComparison.OrdinalIgnoreCase))
                return EvaluateTrig(expression, Math.Tan);
            if (expression.StartsWith("Sqrt", StringComparison.OrdinalIgnoreCase))
                return EvaluateFunction(expression, "Sqrt", Math.Sqrt);
            if (expression.StartsWith("Log", StringComparison.OrdinalIgnoreCase))
                return EvaluateLog(expression);
            
            // Constants
            if (expression == "Pi" || expression == "π")
                return $"π = {Math.PI}";
            if (expression == "E")
                return $"e = {Math.E}";
            if (expression == "GoldenRatio")
                return $"φ = {(1 + Math.Sqrt(5)) / 2}";
            
            // Direct evaluation
            return EvaluateArithmetic(expression);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
    
    private string EvaluateArithmetic(string expr)
    {
        // Replace ^ with Math.Pow
        expr = Regex.Replace(expr, @"(\d+)\^(\d+)", "Math.Pow($1,$2)");
        expr = expr.Replace("Math.Pow", "System.Math.Pow");
        
        // Replace common functions
        expr = expr.Replace("Sin", "Math.Sin")
                   .Replace("Cos", "Math.Cos")
                   .Replace("Tan", "Math.Tan")
                   .Replace("Sqrt", "Math.Sqrt")
                   .Replace("Pi", "Math.PI")
                   .Replace("E", "Math.E");
        
        // Use DataTable for safe evaluation
        var table = new System.Data.DataTable();
        var result = table.Compute(expr, "");
        return $"{expr} = {result}";
    }
    
    private string EvaluateTrig(string expr, Func<double, double> func)
    {
        var match = Regex.Match(expr, @"(Sin|Cos|Tan)\((.+)\)");
        if (match.Success)
        {
            double val = Convert.ToDouble(new System.Data.DataTable().Compute(match.Groups[2].Value, ""));
            double result = func(val * Math.PI / 180); // Convert to degrees
            return $"{expr} = {result}";
        }
        return $"Error: Invalid format. Use {expr.Substring(0, 3)}(value)";
    }
    
    private string EvaluateFunction(string expr, string funcName, Func<double, double> func)
    {
        var match = Regex.Match(expr, $@"{funcName}\((.+)\)");
        if (match.Success)
        {
            double val = Convert.ToDouble(new System.Data.DataTable().Compute(match.Groups[1].Value, ""));
            double result = func(val);
            return $"{expr} = {result}";
        }
        return $"Error: Invalid format. Use {funcName}(value)";
    }
    
    private string EvaluateLog(string expr)
    {
        var match = Regex.Match(expr, @"Log(?:\[|\((.+)\))");
        if (match.Success)
        {
            double val = Convert.ToDouble(new System.Data.DataTable().Compute(match.Groups[1].Value, ""));
            double result = Math.Log10(val);
            return $"{expr} = {result}";
        }
        return "Error: Invalid Log format. Use Log(value)";
    }
    
    private string HandleIntegrate(string expr)
    {
        var match = Regex.Match(expr, @"Integrate\[(.+),\s*(\w+)\]");
        if (match.Success)
        {
            string function = match.Groups[1].Value;
            string variable = match.Groups[2].Value;
            
            // Basic integration rules
            if (function == "x^2")
                return $"∫ x² dx = x³/3 + C";
            if (function == "x")
                return $"∫ x dx = x²/2 + C";
            if (function == "Sin[x]")
                return $"∫ sin(x) dx = -cos(x) + C";
            if (function == "Cos[x]")
                return $"∫ cos(x) dx = sin(x) + C";
            if (function == "1/x")
                return $"∫ 1/x dx = ln|x| + C";
            if (function == "E^x")
                return $"∫ eˣ dx = eˣ + C";
            
            return $"∫ {function} d{variable} = [Symbolic result not implemented]";
        }
        return "Error: Use Integrate[function, variable] (e.g., Integrate[x^2, x])";
    }
    
    private string HandleDerivative(string expr)
    {
        var match = Regex.Match(expr, @"D\[(.+),\s*(\w+)\]");
        if (match.Success)
        {
            string function = match.Groups[1].Value;
            string variable = match.Groups[2].Value;
            
            if (function == "x^3")
                return $"d/dx (x³) = 3x²";
            if (function == "Sin[x]")
                return $"d/dx sin(x) = cos(x)";
            if (function == "Cos[x]")
                return $"d/dx cos(x) = -sin(x)";
            if (function == "x^2")
                return $"d/dx (x²) = 2x";
            if (function == "E^x")
                return $"d/dx (eˣ) = eˣ";
            if (function == "Log[x]")
                return $"d/dx ln(x) = 1/x";
            
            return $"d/d{variable} ({function}) = [Derivative not implemented]";
        }
        return "Error: Use D[function, variable] (e.g., D[x^3, x])";
    }
    
    private string HandleSimplify(string expr)
    {
        var match = Regex.Match(expr, @"Simplify\[(.+)\]");
        if (match.Success)
        {
            string function = match.Groups[1].Value;
            
            var simplifications = new Dictionary<string, string>
            {
                { "Sin[x]^2 + Cos[x]^2", "1" },
                { "x + x", "2x" },
                { "x * x", "x²" },
                { "(x^2)^3", "x⁶" },
                { "Sin[2x]", "2 sin(x) cos(x)" },
                { "Cos[2x]", "cos²(x) - sin²(x)" }
            };
            
            if (simplifications.ContainsKey(function))
                return $"{function} simplifies to {simplifications[function]}";
            
            return $"{function} = [Cannot simplify further]";
        }
        return "Error: Use Simplify[expression]";
    }
    
    private string HandleExpand(string expr)
    {
        var match = Regex.Match(expr, @"Expand\[(.+)\]");
        if (match.Success)
        {
            string function = match.Groups[1].Value;
            
            if (function == "(x+1)^2")
                return "(x+1)² expands to x² + 2x + 1";
            if (function == "(x-1)^3")
                return "(x-1)³ expands to x³ - 3x² + 3x - 1";
            if (function == "(x+2)(x+3)")
                return "(x+2)(x+3) expands to x² + 5x + 6";
            
            return $"{function} = [Expansion not implemented]";
        }
        return "Error: Use Expand[expression]";
    }
    
    private string HandleFactor(string expr)
    {
        var match = Regex.Match(expr, @"Factor\[(.+)\]");
        if (match.Success)
        {
            string function = match.Groups[1].Value;
            
            if (function == "x^2+2x+1")
                return "x² + 2x + 1 factors to (x+1)²";
            if (function == "x^2-1")
                return "x² - 1 factors to (x-1)(x+1)";
            if (function == "x^2+5x+6")
                return "x² + 5x + 6 factors to (x+2)(x+3)";
            
            return $"{function} = [Prime or cannot factor]";
        }
        return "Error: Use Factor[expression]";
    }
    
    private string HandleMatrixForm(string expr)
    {
        var match = Regex.Match(expr, @"MatrixForm\[{{(.+),(.+)},{(.+),(.+)}}\]");
        if (match.Success)
        {
            return $@"Matrix:
┌ {match.Groups[1].Value}  {match.Groups[2].Value} ┐
└ {match.Groups[3].Value}  {match.Groups[4].Value} ┘";
        }
        return "Error: Use MatrixForm[{{a,b},{c,d}}]";
    }
    
    private string HandleDeterminant(string expr)
    {
        var match = Regex.Match(expr, @"Det\[{{(.+),(.+)},{(.+),(.+)}}\]");
        if (match.Success)
        {
            double a = double.Parse(match.Groups[1].Value);
            double b = double.Parse(match.Groups[2].Value);
            double c = double.Parse(match.Groups[3].Value);
            double d = double.Parse(match.Groups[4].Value);
            double det = a * d - b * c;
            return $"det({{{{{a},{b}}},{{{c},{d}}}}}) = {det}";
        }
        return "Error: Use Det[{{a,b},{c,d}}]";
    }
    
    private string HandlePlot(string expr)
    {
        var match = Regex.Match(expr, @"Plot\[(.+),\s*{(\w+),\s*(\d+),\s*(\d+)}\]");
        if (match.Success)
        {
            string function = match.Groups[1].Value;
            string variable = match.Groups[2].Value;
            double from = double.Parse(match.Groups[3].Value);
            double to = double.Parse(match.Groups[4].Value);
            
            // Generate ASCII plot
            var plot = GenerateAsciiPlot(function, from, to);
            return $"Plot of {function} from {from} to {to}:\n{plot}";
        }
        return "Error: Use Plot[function, {variable, min, max}] (e.g., Plot[Sin[x], {x, 0, 10}])";
    }
    
    private string GenerateAsciiPlot(string function, double from, double to)
    {
        int width = 40;
        int height = 10;
        char[,] plot = new char[height, width];
        
        // Initialize with spaces
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                plot[i, j] = ' ';
        
        // Calculate points
        List<double> yValues = new List<double>();
        double minY = double.MaxValue;
        double maxY = double.MinValue;
        
        for (int i = 0; i < width; i++)
        {
            double x = from + (to - from) * i / (width - 1);
            double y = EvaluateForPlot(function, x);
            yValues.Add(y);
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }
        
        // Plot points
        for (int i = 0; i < width; i++)
        {
            if (maxY == minY) continue;
            int yIndex = (int)((yValues[i] - minY) / (maxY - minY) * (height - 1));
            yIndex = Math.Max(0, Math.Min(height - 1, yIndex));
            plot[height - 1 - yIndex, i] = '•';
        }
        
        // Build string
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
                sb.Append(plot[i, j]);
            sb.AppendLine();
        }
        
        return sb.ToString();
    }
    
    private double EvaluateForPlot(string function, double x)
    {
        function = function.ToLower().Replace("sin", "Math.Sin")
            .Replace("cos", "Math.Cos")
            .Replace("x", x.ToString());
        
        try
        {
            var table = new System.Data.DataTable();
            return Convert.ToDouble(table.Compute(function, ""));
        }
        catch
        {
            return 0;
        }
    }
    
    private void AddConsoleOutput(string text, string colorHex, float textSize = 14, bool bold = false)
    {
        var textView = new TextView(this)
        {
            Text = text,
            TextSize = textSize,
            Typeface = Typeface.Create("monospace", bold ? TypefaceStyle.Bold : TypefaceStyle.Normal)
        };
        textView.SetTextColor(Color.ParseColor(colorHex));
        textView.SetPadding(0, 5, 0, 5);
        consoleLayout.AddView(textView);
    }
    
    private void ClearConsole()
    {
        consoleLayout.RemoveAllViews();
        AddConsoleOutput("Console cleared. Type new expressions below.\n", "#4EC9B0", 14);
    }
    
    private void NavigateHistory(int direction)
    {
        if (history.Count == 0) return;
        
        int newIndex = historyIndex + direction;
        if (newIndex >= 0 && newIndex < history.Count)
        {
            historyIndex = newIndex;
            inputField 

            }
             }
             }
