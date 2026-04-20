using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;

namespace MyAndroidApp;

[Activity(Label = "My Android App", MainLauncher = true)]
public class MainActivity : Activity
{
    int clickCount = 0;
    
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // Create main layout
        var layout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            SetPadding = (50, 100, 50, 100)
        };
        
        // Title
        var title = new TextView(this)
        {
            Text = "My Android App",
            TextSize = 32,
            Gravity = GravityFlags.CenterHorizontal
        };
        title.SetTextColor(Color.ParseColor("#6200EE"));
        title.SetTypeface(null, TypefaceStyle.Bold);
        title.SetPadding(0, 0, 0, 40);
        
        // Subtitle
        var subtitle = new TextView(this)
        {
            Text = "Built with GitHub Actions",
            TextSize = 16,
            Gravity = GravityFlags.CenterHorizontal
        };
        subtitle.SetTextColor(Color.ParseColor("#666666"));
        subtitle.SetPadding(0, 0, 0, 60);
        
        // Counter display
        var counterText = new TextView(this)
        {
            Text = "Ready to click!",
            TextSize = 28,
            Gravity = GravityFlags.CenterHorizontal
        };
        counterText.SetTextColor(Color.ParseColor("#333333"));
        counterText.SetPadding(0, 0, 0, 40);
        
        // Button
        var button = new Button(this)
        {
            Text = "Click Me!",
            TextSize = 20
        };
        button.SetBackgroundColor(Color.ParseColor("#6200EE"));
        button.SetTextColor(Color.White);
        button.SetPadding(40, 20, 40, 20);
        
        // Status message
        var statusText = new TextView(this)
        {
            Text = "Tap the button to start",
            TextSize = 14,
            Gravity = GravityFlags.CenterHorizontal
        };
        statusText.SetTextColor(Color.ParseColor("#999999"));
        statusText.SetPadding(0, 60, 0, 0);
        
        // Button click handler
        button.Click += (sender, e) =>
        {
            clickCount++;
            
            // Update counter text with emojis
            if (clickCount == 1)
            {
                counterText.Text = "🎉 1 click! 🎉";
                statusText.Text = "Great start!";
            }
            else if (clickCount <= 5)
            {
                counterText.Text = $"⚡ {clickCount} clicks! ⚡";
                statusText.Text = "You're on fire!";
            }
            else if (clickCount <= 10)
            {
                counterText.Text = $"🔥 {clickCount} clicks! 🔥";
                statusText.Text = "Amazing! Keep going!";
            }
            else
            {
                counterText.Text = $"🏆 {clickCount} clicks! 🏆";
                statusText.Text = "You're a champion!";
            }
            
            // Change button text occasionally
            if (clickCount == 10)
                button.Text = "You're Awesome!";
            else if (clickCount == 20)
                button.Text = "Unstoppable!";
        };
        
        // Add all views to layout
        layout.AddView(title);
        layout.AddView(subtitle);
        layout.AddView(counterText);
        layout.AddView(button);
        layout.AddView(statusText);
        
        SetContentView(layout);
    }
}