﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "Quiz")]
    public class Quiz : Activity
    {
        private string api = "https://api.github.com/repos/Bionic-Reading-Library/Quizes-BRL/contents/";
        private string apiresponse;
        private List<QAP> questions;
        private Typeface urbanistfont;
        private Color textColor = Color.White;
        private DrawerLayout overlayDrawer;
        private ListView drawerlist;
        private ArrayAdapter<string> adapterpanel;
        private string selval;
        private AndroidX.AppCompat.Widget.AppCompatButton about;
        private string[] datap;
        private TextView debug;
        private TextInputEditText inop;
        private FloatingActionButton fab;
        private int currentQuestionIndex;
        private int score = 0;
        private TextView scoreView;
        private TextView Title;
        private TextView DiGen;
        private TextInputLayout TextInputLayout;
        private RelativeLayout complete;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Quiz);
            urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
            complete = FindViewById<RelativeLayout>(Resource.Id.complete);
            TextInputLayout = FindViewById<TextInputLayout>(Resource.Id.textInputLayout);
            Title = FindViewById<TextView>(Resource.Id.Title);
            DiGen = FindViewById<TextView>(Resource.Id.difficulty);
            scoreView = FindViewById<TextView>(Resource.Id.Score);
            about = FindViewById<AndroidX.AppCompat.Widget.AppCompatButton>(Resource.Id.about);
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            overlayDrawer = FindViewById<DrawerLayout>(Resource.Id.drawer);
            drawerlist = FindViewById<ListView>(Resource.Id.drawerlist2);
            List<string> sidpanel = new List<string> { "Home", "About", "Report an Issue", "Exit" };
            inop = FindViewById<TextInputEditText>(Resource.Id.inop);
            adapterpanel = new CustomArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, sidpanel, urbanistfont, textColor);
            debug = FindViewById<TextView>(Resource.Id.debug);
            datap = Intent.GetStringArrayExtra("datap") ?? new string[0];

            debug.Text = $"{datap[0]} | {datap[1]} | {datap[2]} | {datap[3]}";
            Title.Typeface = DiGen.Typeface = scoreView.Typeface = inop.Typeface = debug.Typeface = urbanistfont;
            Title.Text= datap[2].Replace(".txt", "");
            DiGen.Text = $"{datap[0]} | {datap[1]}";

            //Code for Side panel
            drawerlist.Adapter = adapterpanel;
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

            // Fetch quiz data and Display and Compare Data (AKA MAIN PROCESS)
            GetData();
        }

        private class QAP
        {
            public string Question { get; set; }
            public string Answer { get; set; }
        }

        private class GitHubContent
        {
            public string Name { get; set; }
            public string download_url { get; set; }
        }

        private async void GetData()
        {
            using (var httpclient = new HttpClient())
            {
                try
                {
                    string path = datap[3].Replace(".txt", ".json");
                    string apiurl = $"{api}/{path}";
                    var response = await httpclient.GetAsync(apiurl);
                    apiresponse = await response.Content.ReadAsStringAsync();
                    GitHubContent gitHubContents = JsonConvert.DeserializeObject<GitHubContent>(apiresponse);

                    string data = await httpclient.GetStringAsync(gitHubContents.download_url);
                    questions = JsonConvert.DeserializeObject<List<QAP>>(data);
                    currentQuestionIndex = 0;
                    DCData(); //Display and Compare Data

                }
                catch (Exception ex)
                {
                    Intent intent = new Intent(this, typeof(Error));
                    intent.PutExtra("error", ex.Message);
                    StartActivity(intent);
                }
            }
        }

        private void DCData()
        {
            try
            {
                scoreView.Text = $"{score} / {questions.Count}";
                if (currentQuestionIndex < questions.Count)
                {
                    debug.Text = questions[currentQuestionIndex].Question;
                }
                else
                {
                    TextInputLayout.Visibility = ViewStates.Gone;
                    inop.Visibility = ViewStates.Gone;
                    fab.Visibility = ViewStates.Gone;
                    complete.Visibility = ViewStates.Visible;
                }

                fab.Click += (sender, args) =>
                {
                    string input = inop.Text.Trim();
                    if (currentQuestionIndex < questions.Count)
                    {
                        if (string.Equals(input, questions[currentQuestionIndex].Answer, StringComparison.OrdinalIgnoreCase))
                        {
                            Toast.MakeText(this, "Correct", ToastLength.Short).Show();
                            currentQuestionIndex++;
                            score++;
                            DCData();
                        }
                        else
                        {
                            currentQuestionIndex++;
                            DCData();
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Intent intent = new Intent(this, typeof(Error));
                intent.PutExtra("error", ex.Message);
                StartActivity(intent);
                Finish();
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
