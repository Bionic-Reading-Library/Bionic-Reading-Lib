using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.DrawerLayout.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "home")]
    public class home : Activity
    {
        private RelativeLayout easy;
        private RelativeLayout avg;
        private RelativeLayout hard;
        private ListView liste;
        private ListView lista;
        private ListView listh;
        private TextView title;
        private TextView title2;
        private TextView title3;
        private TextView bdesc;
        private TextView bdesc2;
        private TextView bdesc3;
        private TextView not;
        private AppCompatButton about;
        private ArrayAdapter<string> adapter;
        private ArrayAdapter<string> adapterpanel;
        private Color textColor = Color.ParseColor("#272727");// Change this to your desired text color
        private string selval;
        private DrawerLayout overlayDrawer;
        private ListView drawerlist;
        private TextView vercon;
        private TextView t2;
        private TextView t3;
        private bool isExpandedEasy = false;
        private bool isExpandedAvg = false;
        private bool isExpandedHard = false;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.home);
                var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
                var ltr = AnimationUtils.LoadAnimation(this, Resource.Animation.ltr_transition);
                var rtl = AnimationUtils.LoadAnimation(this, Resource.Animation.rtl_transition);
                var up = AnimationUtils.LoadAnimation(this, Resource.Animation.up_transition);
                var down = AnimationUtils.LoadAnimation(this, Resource.Animation.down_transition);
                // Create your application here
                easy = FindViewById<RelativeLayout>(Resource.Id.Container);
                avg = FindViewById<RelativeLayout>(Resource.Id.Container2);
                hard = FindViewById<RelativeLayout>(Resource.Id.Container3);
                overlayDrawer = FindViewById<DrawerLayout>(Resource.Id.drawer);
                drawerlist = FindViewById<ListView>(Resource.Id.drawerlist2);
                vercon = FindViewById<TextView>(Resource.Id.vercon);
                liste = FindViewById<ListView>(Resource.Id.liste);
                lista = FindViewById<ListView>(Resource.Id.lista);
                listh = FindViewById<ListView>(Resource.Id.listh);
                title = FindViewById<TextView>(Resource.Id.Title);
                title2 = FindViewById<TextView>(Resource.Id.Title2);
                title3 = FindViewById<TextView>(Resource.Id.Title3);
                not = FindViewById<TextView>(Resource.Id.not);
                bdesc = FindViewById<TextView>(Resource.Id.bdesc);
                bdesc2 = FindViewById<TextView>(Resource.Id.bdesc2);
                bdesc3 = FindViewById<TextView>(Resource.Id.bdesc3);
                t2 = FindViewById<TextView>(Resource.Id.textView2);
                t3 = FindViewById<TextView>(Resource.Id.textView3);
                t2.Typeface = t3.Typeface = vercon.Typeface = title.Typeface = bdesc.Typeface = title2.Typeface = title3.Typeface = bdesc2.Typeface = not.Typeface = bdesc3.Typeface = urbanistfont;
                title.SetTypeface(title.Typeface, TypefaceStyle.Bold);
                title2.SetTypeface(title2.Typeface, TypefaceStyle.Bold);
                title3.SetTypeface(title3.Typeface, TypefaceStyle.Bold);
                about = FindViewById<AppCompatButton>(Resource.Id.about);

                //Version Value:
                string versionName = $"Version: {AppInfo.VersionString}";
                vercon.Text = versionName;
                vercon.Typeface = urbanistfont;

                List<string> dataList = new List<string> { "Fantasy", "Horror", "Piece of Life", "Science Fiction" };
                List<string> sidpanel = new List<string> { "Home", "About", "Report an Issue", "Exit" };

                // Initialize the ArrayAdapter with the sample data
                adapter = new CustomArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, dataList, urbanistfont, textColor);
                adapterpanel = new CustomArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, sidpanel, urbanistfont, textColor);

                // Set the adapter for the ListView
                liste.Adapter = adapter;
                lista.Adapter = adapter;
                listh.Adapter = adapter;
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
                        overlayDrawer.StartAnimation(rtl);
                        overlayDrawer.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        overlayDrawer.StartAnimation(ltr);
                        overlayDrawer.Visibility = ViewStates.Visible;
                    }

                };
                easy.Click += (sender, args) =>
                {
                    
                    if (liste.Visibility == Android.Views.ViewStates.Visible)
                    {
                        
                        liste.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        
                        liste.Visibility = Android.Views.ViewStates.Visible;
                    }
                };
                avg.Click += (sender, args) =>
                {
                    
                    if (lista.Visibility == Android.Views.ViewStates.Visible)
                    {
                        
                        lista.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        
                        lista.Visibility = Android.Views.ViewStates.Visible;
                    }
                };
                hard.Click += (sender, args) =>
                {
                    if (listh.Visibility == Android.Views.ViewStates.Visible)
                    {
                        
                        listh.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {
                        
                        listh.Visibility = Android.Views.ViewStates.Visible;
                    }
                };

                // Handle Item Click on List.
                liste.ItemClick += (sender, args) =>
                {
                    selval = adapter.GetItem(args.Position);
                    if (selval == "Fantasy")
                    {
                        Intent intent = new Intent(this, typeof(fantasy));
                        intent.PutExtra("diff", "Easy");
                        StartActivity(intent);
                    }
                    else if (selval == "Horror")
                    {
                        Intent intent = new Intent(this, typeof(Horror));
                        intent.PutExtra("diff", "Easy");
                        StartActivity(intent);
                    }
                    else if (selval == "Piece of Life")
                    {
                        Intent intent = new Intent(this, typeof(pieceoflife));
                        intent.PutExtra("diff", "Easy");
                        StartActivity(intent);
                    }
                    else if (selval == "Science Fiction")
                    {
                        Intent intent = new Intent(this, typeof(sciencefiction));
                        intent.PutExtra("diff", "Easy");
                        StartActivity(intent);
                    }
                };
                lista.ItemClick += (sender, args) =>
                {
                    selval = adapter.GetItem(args.Position);
                    if (selval == "Fantasy")
                    {
                        Intent intent = new Intent(this, typeof(fantasy));
                        intent.PutExtra("diff", "Average");
                        StartActivity(intent);
                    }
                    else if (selval == "Horror")
                    {
                        Intent intent = new Intent(this, typeof(Horror));
                        intent.PutExtra("diff", "Average");
                        StartActivity(intent);
                    }
                    else if (selval == "Piece of Life")
                    {
                        Intent intent = new Intent(this, typeof(pieceoflife));
                        intent.PutExtra("diff", "Average");
                        StartActivity(intent);
                    }
                    else if (selval == "Science Fiction")
                    {
                        Intent intent = new Intent(this, typeof(sciencefiction));
                        intent.PutExtra("diff", "Average");
                        StartActivity(intent);
                    }
                };
                listh.ItemClick += (sender, args) =>
                {
                    selval = adapter.GetItem(args.Position);
                    if (selval == "Fantasy")
                    {
                        Intent intent = new Intent(this, typeof(fantasy));
                        intent.PutExtra("diff", "Hard");
                        StartActivity(intent);
                    }
                    else if (selval == "Horror")
                    {
                        Intent intent = new Intent(this, typeof(Horror));
                        intent.PutExtra("diff", "Hard");
                        StartActivity(intent);
                    }
                    else if (selval == "Piece of Life")
                    {
                        Intent intent = new Intent(this, typeof(pieceoflife));
                        intent.PutExtra("diff", "Hard");
                        StartActivity(intent);
                    }
                    else if (selval == "Science Fiction")
                    {
                        Intent intent = new Intent(this, typeof(sciencefiction));
                        intent.PutExtra("diff", "Hard");
                        StartActivity(intent);
                    }
                };
            }catch(Exception ex)
            {
                Intent intent = new Intent(this, typeof(Error));
                intent.Extras.PutString("error", ex.Message);
                StartActivity(intent);
            }

            }

        public class CustomArrayAdapter : ArrayAdapter<string>
        {
            private readonly Typeface typeface;
            private readonly Color textColor;

            public CustomArrayAdapter(Context context, int resource, List<string> objects, Typeface typeface, Color textColor)
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
                    textView.Typeface = typeface;
                    textView.SetTextColor(textColor);
                }

                return view;
            }
        }

    }
}