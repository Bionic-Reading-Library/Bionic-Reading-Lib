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
using System.Text;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "Error")]
    public class Error : Activity
    {
        TextView errmsg;
        TextView egg;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Error);
            var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");

            // Create your application here
            errmsg = FindViewById<TextView>(Resource.Id.Errmsg);
            egg = FindViewById<TextView>(Resource.Id.eagg);
            AppCompatButton home = FindViewById<AppCompatButton>(Resource.Id.appCompatButton1);
            AppCompatButton report = FindViewById<AppCompatButton>(Resource.Id.appCompatButton2);

            errmsg.Typeface = egg.Typeface = home.Typeface = report.Typeface = urbanistfont;
            errmsg.SetTypeface(errmsg.Typeface, TypefaceStyle.Bold);

            home.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(home));
                StartActivity(intent);
            };
            egg.Click += (sender, args) => 
            {
                Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/Bionic-Reading-Library/Bionic-Reading-Lib/issues/new"));
                browserIntent.SetFlags(ActivityFlags.NewTask);
                StartActivity(browserIntent);
            };
        }
    }
}