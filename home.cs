using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "home")]
    public class home : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.home);
            var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");

            // Create your application here
            TextView header = FindViewById<TextView>(Resource.Id.textView1);
            TextView end = FindViewById<TextView>(Resource.Id.end);
            header.Typeface = urbanistfont;
            header.SetTypeface(header.Typeface, TypefaceStyle.Bold);
            end.Typeface = urbanistfont;

            AppCompatButton about = FindViewById<AppCompatButton>(Resource.Id.about);
            about.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(about));
                StartActivity(intent);
            };

        }
    }
}