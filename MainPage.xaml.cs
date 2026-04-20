using MathToolsApp.Services;

namespace MathToolsApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        
        AlgebraButton.Clicked += OnAlgebraClicked;
        CalculusButton.Clicked += OnCalculusClicked;
        StatsButton.Clicked += OnStatsClicked;
        ConverterButton.Clicked += OnConverterClicked;
        MatrixButton.Clicked += OnMatrixClicked;
    }
    
    private async void OnAlgebraClicked(object sender, EventArgs e)
    {
        string input = await DisplayPromptAsync("Algebra Solver", 
            "Enter quadratic equation (a b c):\nExample: 1 -5 6 for x² -5x + 6 = 0",
            placeholder: "a b c", keyboard: Keyboard.Numeric);
            
        if (!string.IsNullOrEmpty(input))
        {
            var parts = input.Split(' ');
            if (parts.Length == 3 && 
                double.TryParse(parts[0], out double a) &&
                double.TryParse(parts[1], out double b) &&
                double.TryParse(parts[2], out double c))
            {
                var solver = new AlgebraSolver();
                var result = solver.SolveQuadratic(a, b, c);
                await DisplayAlert("Solution", result.message, "OK");
            }
            else
            {
                await DisplayAlert("Error", "Please enter 3 numbers separated by spaces", "OK");
            }
        }
    }
    
    private async void OnCalculusClicked(object sender, EventArgs e)
    {
        string input = await DisplayPromptAsync("Calculus - Derivative", 
            "Enter x value:",
            placeholder: "Example: 2", keyboard: Keyboard.Numeric);
            
        if (!string.IsNullOrEmpty(input) && double.TryParse(input, out double x))
        {
            var calc = new CalculusEngine();
            // f(x) = x²
            double derivative = calc.Derivative(t => t * t, x);
            await DisplayAlert("Derivative", $"For f(x)=x² at x={x}\nDerivative = {derivative:F4}", "OK");
        }
    }
    
    private async void OnStatsClicked(object sender, EventArgs e)
    {
        string input = await DisplayPromptAsync("Statistics", 
            "Enter numbers separated by commas:",
            placeholder: "Example: 2,4,6,8,10");
            
        if (!string.IsNullOrEmpty(input))
        {
            try
            {
                var numbers = input.Split(',')
                    .Select(n => double.Parse(n.Trim()))
                    .ToArray();
                
                var stats = new StatisticsEngine();
                var mean = stats.Mean(numbers);
                var median = stats.Median(numbers);
                var stdDev = stats.StandardDeviation(numbers);
                
                await DisplayAlert("Statistics Results",
                    $"Mean: {mean:F2}\nMedian: {median:F2}\nStd Dev: {stdDev:F2}\nCount: {numbers.Length}",
                    "OK");
            }
            catch
            {
                await DisplayAlert("Error", "Invalid input. Use comma-separated numbers", "OK");
            }
        }
    }
    
    private async void OnConverterClicked(object sender, EventArgs e)
    {
        var converter = new UnitConverter();
        var categories = converter.GetCategories();
        
        string category = await DisplayActionSheet("Select Category", "Cancel", null, categories);
        
        if (category != "Cancel" && !string.IsNullOrEmpty(category))
        {
            var units = converter.GetUnits(category);
            string fromUnit = await DisplayActionSheet("From Unit", "Cancel", null, units);
            
            if (fromUnit != "Cancel" && !string.IsNullOrEmpty(fromUnit))
            {
                string toUnit = await DisplayActionSheet("To Unit", "Cancel", null, units);
                
                if (toUnit != "Cancel" && !string.IsNullOrEmpty(toUnit))
                {
                    string valueInput = await DisplayPromptAsync("Convert", 
                        $"Enter value in {fromUnit}:",
                        keyboard: Keyboard.Numeric);
                        
                    if (!string.IsNullOrEmpty(valueInput) && double.TryParse(valueInput, out double value))
                    {
                        double result = converter.Convert(category, fromUnit, toUnit, value);
                        await DisplayAlert("Result", 
                            $"{value} {fromUnit} = {result:F4} {toUnit}", "OK");
                    }
                }
            }
        }
    }
    
    private async void OnMatrixClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Matrix Calculator", 
            "2x2 Matrix Operations:\n" +
            "| a  b |\n" +
            "| c  d |\n\n" +
            "Enter a,b,c,d values", "OK");
            
        string input = await DisplayPromptAsync("Matrix", 
            "Enter values (a b c d):",
            placeholder: "Example: 1 2 3 4");
            
        if (!string.IsNullOrEmpty(input))
        {
            var parts = input.Split(' ');
            if (parts.Length == 4 &&
                double.TryParse(parts[0], out double a) &&
                double.TryParse(parts[1], out double b) &&
                double.TryParse(parts[2], out double c) &&
                double.TryParse(parts[3], out double d))
            {
                var matrix = new MatrixService();
                double[,] m = { { a, b }, { c, d } };
                double det = matrix.Determinant(m);
                
                await DisplayAlert("Matrix Result",
                    $"Matrix:\n| {a:F2}  {b:F2} |\n| {c:F2}  {d:F2} |\n\n" +
                    $"Determinant: {det:F4}\n" +
                    $"Inverse exists: {(det != 0 ? "Yes" : "No")}", "OK");
            }
        }
    }
}