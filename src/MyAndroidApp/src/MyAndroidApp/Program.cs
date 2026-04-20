using Android.App;
using Android.Runtime;

namespace MyAndroidApp;

[Application]
public class MainApplication : Application
{
    public MainApplication(IntPtr handle, JniHandleOwnership transfer) 
        : base(handle, transfer)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
    }
}