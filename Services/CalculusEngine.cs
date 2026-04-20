namespace MathToolsApp.Services;

public class CalculusEngine
{
    public double Derivative(Func<double, double> f, double x, double h = 1e-6)
    {
        return (f(x + h) - f(x - h)) / (2 * h);
    }
}