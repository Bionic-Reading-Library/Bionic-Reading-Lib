using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");

            TextView header = FindViewById<TextView>(Resource.Id.textView1);
            TextView desc = FindViewById<TextView>(Resource.Id.textView2);
            header.Typeface = urbanistfont;
            header.SetTypeface(header.Typeface, TypefaceStyle.Bold);
            desc.Typeface = urbanistfont;

            AppCompatButton start = FindViewById<AppCompatButton>(Resource.Id.appCompatButton1);
            AppCompatButton exit = FindViewById<AppCompatButton>(Resource.Id.appCompatButton2);

            exit.Click += (sender, args) =>
            {
                Process.KillProcess(Process.MyPid());
            };
            start.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(home));
                StartActivity(intent);
            };
        }










        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}