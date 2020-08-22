using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using PertukAndroid.UI.Activities.Main;

namespace PertukAndroid.UI.Activities.Splash
{
    [Activity(Theme = "@style/AppThemeSplashScreen", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Handler splashHandler = new Handler();
            splashHandler.PostDelayed(new System.Action(() =>
            {
                Intent mainIntent = new Intent(this, typeof(MainActivity));
                StartActivity(mainIntent);
                Finish();
            }), 2000);
        }
    }
}