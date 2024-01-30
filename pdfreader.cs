﻿using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.DrawerLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using PdfViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Google.Android.Material.Tabs.TabLayout;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "pdfreader")]
    public class pdfreader : Activity
    {
        private Typeface urbanistfont;
        private Color textColor = Color.White;
        private string[] pdfurl;
        private TextView tv;
        private ProgressBar pb;
        private DrawerLayout overlayDrawer;
        private ListView drawerlist;
        private ArrayAdapter<string> adapterpanel;
        private string selval;
        private AndroidX.AppCompat.Widget.AppCompatButton about;
        private FloatingActionButton fab;
        private TextView contentid;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.pdfreader);
                urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
                about = FindViewById<AndroidX.AppCompat.Widget.AppCompatButton>(Resource.Id.about);
                overlayDrawer = FindViewById<DrawerLayout>(Resource.Id.drawer);
                drawerlist = FindViewById<ListView>(Resource.Id.drawerlist2);
                List<string> sidpanel = new List<string> { "Home", "About", "Report an Issue", "Exit" };
                adapterpanel = new CustomArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, sidpanel, urbanistfont, textColor);
                fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

                tv = FindViewById<TextView>(Resource.Id.Textview);
                contentid = FindViewById<TextView>(Resource.Id.contentid);
                pb = FindViewById<ProgressBar>(Resource.Id.pb);

                pdfurl = Intent.GetStringArrayExtra("data");           
                contentid.Text = $"{pdfurl[0]} | {pdfurl[1]} | {pdfurl[3]}";
                contentid.Typeface = urbanistfont;
                drawerlist.Adapter = adapterpanel;

                drawerlist.ItemClick += (sender, args) =>
                {
                    selval = adapterpanel.GetItem(args.Position);
                    if (selval == "Home")
                    {
                        Intent intent = new Intent(this, typeof(home));
                        StartActivity(intent);
                        Finish();
                    }
                    else if (selval == "About")
                    {
                        Intent intent = new Intent(this, typeof(about));
                        StartActivity(intent);
                        Finish();
                    }
                    else if (selval == "Report an Issue")
                    {
                        Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/Bionic-Reading-Library/Bionic-Reading-Lib/issues/new"));
                        browserIntent.SetFlags(ActivityFlags.NewTask);
                        StartActivity(browserIntent);
                        Finish();
                    }
                    else if (selval == "Exit")
                    {
                        Process.KillProcess(Process.MyPid());
                    }
                };
                about.Click += (sender, args) =>
                {
                    if (overlayDrawer.Visibility == ViewStates.Visible)
                    {
                        overlayDrawer.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        overlayDrawer.Visibility = ViewStates.Visible;
                    }

                };
                fab.Click += (sender, args) =>
                {
                    Intent intent = new Intent(this, typeof(Quiz));
                    string[] datap = { pdfurl[0], pdfurl[1], pdfurl[3], pdfurl[4] };
                    intent.PutExtra("datap", datap);
                    StartActivity(intent);
                };
                _ = DisplayTextFromLinkAsync(pdfurl[2]);
            }
            catch (Exception ex)
            {
                Intent intent = new Intent(this, typeof(Error));
                intent.Extras.PutString("error", ex.Message);
                StartActivity(intent);
            }


        }
        public class CustomArrayAdapter<T> : ArrayAdapter<T>
        {
            private readonly Typeface typeface;
            private readonly Color textColor;

            public CustomArrayAdapter(Context context, int resource, List<T> objects, Typeface typeface, Color textColor)
                : base(context, resource, objects)
            {
                this.typeface = typeface;
                this.textColor = textColor;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = base.GetView(position, convertView, parent);

                if (view is TextView textView)
                {
                    // Set the custom typeface and text color
                    textView.Typeface = typeface;
                    textView.SetTextColor(textColor);
                }

                return view;
            }
        }

        async Task DisplayTextFromLinkAsync(string url)
        {
            pb.Visibility = ViewStates.Visible;
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {

                    string chunk;
                    while ((chunk = await reader.ReadLineAsync()) != null)
                    {
                        tv.Text += chunk + "\n";
                        tv.Typeface = urbanistfont;
                        pb.Visibility = ViewStates.Gone;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                Toast.MakeText(this, "Error fetching text: " + ex.Message, ToastLength.Long).Show();
            }
        }


    }
}