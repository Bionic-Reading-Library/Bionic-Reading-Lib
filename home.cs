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
            TextView title;
            TextView bdesc;
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.home);
                var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");

                // Create your application here
                title = FindViewById<TextView>(Resource.Id.Title);
                bdesc = FindViewById<TextView>(Resource.Id.bdesc);
                TextView end = FindViewById<TextView>(Resource.Id.end);
                title.Typeface = bdesc.Typeface = end.Typeface = urbanistfont;
                title.SetTypeface(title.Typeface, TypefaceStyle.Bold);

                AppCompatButton about = FindViewById<AppCompatButton>(Resource.Id.about);
                about.Click += (sender, args) =>
                {
                    Intent intent = new Intent(this, typeof(about));
                    StartActivity(intent);
                };

        }
    }
}