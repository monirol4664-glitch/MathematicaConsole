namespace MathToolsApp.Services;

public class UnitConverter
{
    private Dictionary<string, Dictionary<string, double>> _rates = new()
    {
        ["Length"] = new()
        {
            ["Meters"] = 1, ["Kilometers"] = 0.001, ["Miles"] = 0.000621371,
            ["Feet"] = 3.28084, ["Inches"] = 39.3701, ["Centimeters"] = 100
        },
        ["Weight"] = new()
        {
            ["Kilograms"] = 1, ["Grams"] = 1000, ["Pounds"] = 2.20462, ["Ounces"] = 35.274
        }
    };
    
    public List<string> GetCategories() => _rates.Keys.ToList();
    public List<string> GetUnits(string category) => _rates[category].Keys.ToList();
    
    public double Convert(string category, string fromUnit, string toUnit, double value)
    {
        var units = _rates[category];
        double inBaseUnit = value / units[fromUnit];
        return inBaseUnit * units[toUnit];
    }
}