namespace MathToolsApp.Services;

public class UnitConverter
{
    private Dictionary<string, Dictionary<string, double>> conversionRates = new()
    {
        ["Length"] = new()
        {
            ["Meters"] = 1,
            ["Kilometers"] = 0.001,
            ["Miles"] = 0.000621371,
            ["Feet"] = 3.28084,
            ["Inches"] = 39.3701,
            ["Centimeters"] = 100,
            ["Millimeters"] = 1000
        },
        ["Weight"] = new()
        {
            ["Kilograms"] = 1,
            ["Grams"] = 1000,
            ["Pounds"] = 2.20462,
            ["Ounces"] = 35.274,
            ["Tons"] = 0.001
        },
        ["Temperature"] = new()
        {
            ["Celsius"] = 1,
            ["Fahrenheit"] = 1,
            ["Kelvin"] = 1
        },
        ["Area"] = new()
        {
            ["Square Meters"] = 1,
            ["Square Kilometers"] = 0.000001,
            ["Square Miles"] = 3.861e-7,
            ["Acres"] = 0.000247105,
            ["Hectares"] = 0.0001
        },
        ["Volume"] = new()
        {
            ["Liters"] = 1,
            ["Milliliters"] = 1000,
            ["Gallons"] = 0.264172,
            ["Cubic Meters"] = 0.001
        },
        ["Speed"] = new()
        {
            ["Meters/Second"] = 1,
            ["Kilometers/Hour"] = 3.6,
            ["Miles/Hour"] = 2.23694,
            ["Knots"] = 1.94384
        }
    };
    
    public List<string> GetCategories() => conversionRates.Keys.ToList();
    
    public List<string> GetUnits(string category) => conversionRates[category].Keys.ToList();
    
    public double Convert(string category, string fromUnit, string toUnit, double value)
    {
        if (category == "Temperature")
            return ConvertTemperature(fromUnit, toUnit, value);
        
        var units = conversionRates[category];
        double inBaseUnit = value / units[fromUnit];
        return inBaseUnit * units[toUnit];
    }
    
    private double ConvertTemperature(string from, string to, double value)
    {
        // Convert to Celsius first
        double celsius = from switch
        {
            "Fahrenheit" => (value - 32) * 5 / 9,
            "Kelvin" => value - 273.15,
            _ => value
        };
        
        // Convert from Celsius to target
        return to switch
        {
            "Fahrenheit" => celsius * 9 / 5 + 32,
            "Kelvin" => celsius + 273.15,
            _ => celsius
        };
    }
}