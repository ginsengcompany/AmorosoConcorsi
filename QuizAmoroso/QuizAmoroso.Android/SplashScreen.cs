using Android.App;
using Android.OS;
using Android.Content.PM;

namespace QuizAmoroso.Droid
{
    [Activity(Label = "Amoroso Concorsi", Icon = "@drawable/icon", Theme = "@style/SplashTheme", MainLauncher = true,NoHistory =true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StartActivity(typeof(MainActivity));
        }
    }
}