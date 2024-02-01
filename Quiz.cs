using Android.App;
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
using Felipecsl.GifImageViewLibrary;
using System.Threading;
using System.IO;
using Xamarin.Essentials;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "Quiz")]
    public class Quiz : Activity
    {
        private string api = "https://api.github.com/repos/Bionic-Reading-Library/Quizes-BRL/contents/";
        private string apiresponse;
        private List<QAP> questions;
        private Typeface urbanistfont;
        private Color textColor = Color.ParseColor("#272727");
        private DrawerLayout overlayDrawer;
        private ListView drawerlist;
        private ArrayAdapter<string> adapterpanel;
        private string selval;
        private AndroidX.AppCompat.Widget.AppCompatButton about;
        private string[] datap;
        private TextView debug;
        private TextInputEditText inop;
        private FloatingActionButton fab;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private TextView scoreView;
        private TextView Title;
        private TextView DiGen;
        private TextInputLayout TextInputLayout;
        private RelativeLayout complete;
        private GifImageView gifImageView;
        private TextView info1;
        private TextView info2;
        private TextView info3;
        private TextView info4;
        private TextView info5;
        private TextView info6;
        private TextView status;
        private TextView t2;
        private TextView t3;
        private AndroidX.AppCompat.Widget.AppCompatButton rhome;
        private TextView vercon;
        private Stream input;
        private int cq;
        private int clickcount = 0 ;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Quiz);
            urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
            complete = FindViewById<RelativeLayout>(Resource.Id.complete);
            TextInputLayout = FindViewById<TextInputLayout>(Resource.Id.textInputLayout);
            vercon = FindViewById<TextView>(Resource.Id.vercon);
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
            gifImageView = FindViewById<GifImageView>(Resource.Id.gifImageView);
            info1 = FindViewById<TextView>(Resource.Id.info1);
            info2 = FindViewById<TextView>(Resource.Id.info2);
            info3 = FindViewById<TextView>(Resource.Id.info3);
            info4 = FindViewById<TextView>(Resource.Id.info4);
            info5 = FindViewById<TextView>(Resource.Id.info5);
            info6 = FindViewById<TextView>(Resource.Id.info6);
            status = FindViewById<TextView>(Resource.Id.status);
            rhome = FindViewById<AndroidX.AppCompat.Widget.AppCompatButton>(Resource.Id.homee);
            t2 = FindViewById<TextView>(Resource.Id.textView2);
            t3 = FindViewById<TextView>(Resource.Id.textView3);
            string versionName = $"Version: {AppInfo.VersionString}";
            vercon.Text = versionName;
            vercon.Typeface = urbanistfont;
            debug.Text = $"{datap[0]} | {datap[1]} | {datap[2]} | {datap[3]}";
            inop.Typeface = urbanistfont;
            t2.Typeface = t3.Typeface = rhome.Typeface = info5.Typeface = info3.Typeface = info1.Typeface = status.Typeface = info4.Typeface = info6.Typeface = info2.Typeface = Title.Typeface = DiGen.Typeface = scoreView.Typeface = inop.Typeface = debug.Typeface = urbanistfont;
            Title.Text= datap[2].Replace(".txt", "");
            DiGen.Text = $"{datap[0]} | {datap[1]}";
            info1.SetTypeface(info1.Typeface, TypefaceStyle.Bold);
            info2.SetTypeface(info2.Typeface, TypefaceStyle.Bold);
            info3.SetTypeface(info3.Typeface, TypefaceStyle.Bold);
            info4.SetTypeface(info4.Typeface, TypefaceStyle.Bold);
            info5.SetTypeface(info5.Typeface, TypefaceStyle.Bold);
            info6.SetTypeface(info6.Typeface, TypefaceStyle.Bold);


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
            public string ql { get; set; }
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
                    DcData(); //Display and Compare Data

                }
                catch (Exception ex)
                {
                    Intent intent = new Intent(this, typeof(Error));
                    intent.PutExtra("error", ex.Message);
                    StartActivity(intent);
                }
            }
        }

        private async void DcData()
        {
            try
            {
                scoreView.Text = $"{score} / {questions.Count}";

                cq = Convert.ToInt32(questions[currentQuestionIndex].ql);

                if (currentQuestionIndex == cq)
                {
                    debug.Text = questions[currentQuestionIndex].Question;
                    inop.Text = string.Empty;

                    // Subscribe to the Click event outside the loop to avoid multiple subscriptions
                    fab.Click -= FabClickHandler;
                    fab.Click += FabClickHandler;
                    await Task.Delay(100); // Introduce a small delay to allow the UI to update

                }
            }
            catch (Exception ex)
            {
                Intent intent = new Intent(this, typeof(Error));
                intent.PutExtra("error", ex.Message);
                StartActivity(intent);
            }
        }

        private void FabClickHandler(object sender, EventArgs e)
        {
            string inputText = inop.Text.Trim();
            cq = Convert.ToInt32(questions[currentQuestionIndex].ql);
            if (string.Equals(inputText, questions[currentQuestionIndex].Answer, StringComparison.OrdinalIgnoreCase))
            {
                // Correct answer
                score++;
                scorestat();
            }
            else { scorestat(); }


            if (currentQuestionIndex < 4)
            {
                currentQuestionIndex++;
            }
            DcData();
        }
        private void display()
        {
            TextInputLayout.Visibility = ViewStates.Gone;
            inop.Visibility = ViewStates.Gone;
            fab.Visibility = ViewStates.Gone;
            complete.Visibility = ViewStates.Visible;
        }
        private void scorestat()
        {
            try
            {
                if (currentQuestionIndex == 4)
                {   
                    display();
                    input = Resources.OpenRawResource(Resource.Drawable.complete);
                    byte[] bytes = ConvertByteArray(input);
                    gifImageView.SetBytes(bytes);
                    gifImageView.StartAnimation();
                    status.Text = "Congrats, You Finished the Quiz!";
                    status.SetTypeface(status.Typeface, TypefaceStyle.Bold);
                    info1.SetTypeface(info1.Typeface, TypefaceStyle.Bold);
                    info2.SetTypeface(info2.Typeface, TypefaceStyle.Bold);
                    info3.SetTypeface(info3.Typeface, TypefaceStyle.Bold);
                    rhome.SetTypeface(rhome.Typeface, TypefaceStyle.Bold);
                    info2.Text = datap[2].Replace(".txt", "");
                    info4.Text = $"{datap[0]} | {datap[1]}";
                    info6.Text = $"{score} / {questions.Count}";
                    OnBackPressed();
                }
            }catch(Exception ex)
            {
                Intent intent = new Intent(this, typeof(Error));
                intent.PutExtra("error", ex.Message);
                StartActivity(intent);
            }
        }


        private byte[] ConvertByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
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
        public override void OnBackPressed()
        {
            if(score == questions.Count)
            {
                rhome.Click += (sender, args) =>
                {
                    Intent intent = new Intent(this, typeof(home));
                    StartActivity(intent);
                    Finish();
                };
            }
            else
            {
                rhome.Click += (sender, args) =>
                {
                    Intent intent = new Intent(this, typeof(home));
                    StartActivity(intent);
                    Finish();
                };
            }
        }
    }
}
