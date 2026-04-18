using Android.App;
using Android.OS;
using Android.Widget;

namespace MathematicaConsole;

[Activity(Label = "Mathematica Console", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Create a simple UI
        var layout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };
        
        var textView = new TextView(this)
        {
            Text = "Hello from Mathematica Console!",
            TextSize = 24
        };
        
        var button = new Button(this)
        {
            Text = "Click Me"
        };
        
        button.Click += (s, e) => 
        {
            Toast.MakeText(this, "Button Clicked!", ToastLength.Short).Show();
        };
        
        layout.AddView(textView);
        layout.AddView(button);
        
        SetContentView(layout);
    }
}