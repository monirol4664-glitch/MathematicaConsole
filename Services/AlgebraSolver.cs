using System.Text.RegularExpressions;

namespace MathToolsApp.Services;

public class AlgebraSolver
{
    // Solve quadratic equation: ax² + bx + c = 0
    public (double? x1, double? x2, string message) SolveQuadratic(double a, double b, double c)
    {
        if (a == 0)
            return (null, null, "Not a quadratic equation (a cannot be 0)");

        double discriminant = b * b - 4 * a * c;
        
        if (discriminant < 0)
            return (null, null, "No real solutions (discriminant < 0)");
        
        if (discriminant == 0)
        {
            double x = -b / (2 * a);
            return (x, null, $"One solution: x = {x:F4}");
        }
        
        double x1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
        double x2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
        return (x1, x2, $"Two solutions: x₁ = {x1:F4}, x₂ = {x2:F4}");
    }

    // Solve linear equation: ax + b = 0
    public (double? x, string message) SolveLinear(double a, double b)
    {
        if (a == 0)
        {
            if (b == 0)
                return (null, "Infinite solutions (identity)");
            return (null, "No solution (contradiction)");
        }
        
        double x = -b / a;
        return (x, $"x = {x:F4}");
    }

    // Solve system of 2 linear equations
    // a1x + b1y = c1
    // a2x + b2y = c2
    public (double? x, double? y, string message) SolveSystem(
        double a1, double b1, double c1,
        double a2, double b2, double c2)
    {
        double determinant = a1 * b2 - a2 * b1;
        
        if (Math.Abs(determinant) < 1e-10)
            return (null, null, "No unique solution (lines are parallel or coincident)");
        
        double x = (c1 * b2 - c2 * b1) / determinant;
        double y = (a1 * c2 - a2 * c1) / determinant;
        
        return (x, y, $"x = {x:F4}, y = {y:F4}");
    }

    // Evaluate mathematical expression
    public double EvaluateExpression(string expression)
    {
        // Remove whitespace
        expression = Regex.Replace(expression, @"\s+", "");
        
        // Basic implementation - can be extended with Shunting-yard algorithm
        var dataTable = new System.Data.DataTable();
        var result = dataTable.Compute(expression, "");
        return Convert.ToDouble(result);
    }
}