using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.DrawerLayout.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "pieceoflife")]
    public class pieceoflife : Activity
    {
        private const string dat = "https://api.github.com/repos/Bionic-Reading-Library/PDFs-BRL/contents/";
        private ListView datlist;
        private string difficultyLevel;
        private List<GitHubContent> gitHubContents;
        private Color textColor = Color.ParseColor("#272727");
        private Typeface urbanistfont;
        private ProgressBar pb;
        private DrawerLayout overlayDrawer;
        private ListView drawerlist;
        private ArrayAdapter<string> adapterpanel;
        private string selval;
        private AndroidX.AppCompat.Widget.AppCompatButton about;
        private TextView t1;
        private TextView t2;
        private TextView t3;
        private TextView t4;
        private TextView vercon;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.pieceoflife);
            vercon = FindViewById<TextView>(Resource.Id.vercon);
            var ltr = AnimationUtils.LoadAnimation(this, Resource.Animation.ltr_transition);
            var rtl = AnimationUtils.LoadAnimation(this, Resource.Animation.rtl_transition);
            urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
            about = FindViewById<AndroidX.AppCompat.Widget.AppCompatButton>(Resource.Id.about);
            overlayDrawer = FindViewById<DrawerLayout>(Resource.Id.drawer);
            drawerlist = FindViewById<ListView>(Resource.Id.drawerlist2);
            List<string> sidpanel = new List<string> { "Home", "About", "Report an Issue", "Exit" };
            adapterpanel = new CustomArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, sidpanel, urbanistfont, textColor);
            string versionName = $"Version: {AppInfo.VersionString}";
            vercon.Text = versionName;
            vercon.Typeface = urbanistfont;
            pb = FindViewById<ProgressBar>(Resource.Id.pb);
            t1 = FindViewById<TextView>(Resource.Id.textView3);
            t2 = FindViewById<TextView>(Resource.Id.textView2);
            t3 = FindViewById<TextView>(Resource.Id.textView4);
            t4 = FindViewById<TextView>(Resource.Id.textView5);
            t1.Typeface = t2.Typeface = t3.Typeface = t4.Typeface = urbanistfont;
            datlist = FindViewById<ListView>(Resource.Id.drawerlist);
            difficultyLevel = Intent.GetStringExtra("diff") ?? "DefaultDifficulty";
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
            // Get Data from GitHub
            FetchGitHubContents();
        }
        private async void FetchGitHubContents()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    string apiUrl = $"{dat}/{difficultyLevel}/Piece%20of%20Life";
                    var json = await httpClient.GetStringAsync(new Uri(apiUrl));
                    gitHubContents = JsonConvert.DeserializeObject<List<GitHubContent>>(json);

                    // Display GitHub contents in ListView
                    DisplayGitHubContents();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            }
        }
        private class GitHubContent
        {
            public string Name { get; set; }
            public string download_url { get; set; }
            public string path { get; set; }
        }

        // ...

        private void DisplayGitHubContents()
        {
            try
            {
                if (gitHubContents != null)
                {
                    var adapter = new CustomArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, gitHubContents.Select((content, index) => $"{index + 1}. {content.Name.Replace(".txt", "")}").ToList(), urbanistfont, textColor);
                    datlist.Adapter = adapter;
                    pb.Visibility = ViewStates.Gone;
                    // Handle item click event for ListView
                    datlist.ItemClick += (sender, args) =>
                    {
                        var selectedContent = gitHubContents[args.Position];

                        // Access the selected content's DownloadUrl
                        string downloadUrl = selectedContent.download_url;
                        string[] titlearr = selectedContent.Name.Split('.');
                        string path = selectedContent.path;
                        string[] data = { difficultyLevel, "Fiction", downloadUrl, titlearr[0], path };

                        // Do something with the selected GitHub content (e.g., open the download URL)
                        Intent intent = new Intent(this, typeof(pdfreader));
                        intent.PutExtra("data", data);
                        StartActivity(intent);

                    };
                }
            }
            catch (Exception ex)
            {
                Intent intent = new Intent(this, typeof(Error));
                intent.PutExtra("error", ex.Message);
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
    }
}