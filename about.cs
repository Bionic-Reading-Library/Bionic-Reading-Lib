using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Android.Content.PM;
using Exception = System.Exception;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Android;
using Xamarin.Essentials;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "about")]
    public class about : Activity
    {
        private const string ReleasesUrl = "https://api.github.com/repos/Bionic-Reading-Library/BRL-Release/releases/latest";
        private string versioncode = $"{AppInfo.VersionString}";
        private string CodeName = "";
        private string Failup = "Network Returned 404 - Error Checking for Updates.";
        private string downloadUrl = "";
        private Typeface urbanistfont;
        private TextView header;
        private TextView app;
        private TextView desc;
        private TextView rver;
        private TextView ver;
        private TextView cn;
        private TextView cnr;
        private AppCompatButton back;
        private AppCompatButton update;
        private AppCompatButton project;
        private AppCompatButton issue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.about);
                urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
                CodeName = GetString(Resource.String.vbuild);

                // Create your application here
                header = FindViewById<TextView>(Resource.Id.textView1);
                app = FindViewById<TextView>(Resource.Id.textView2);
                desc = FindViewById<TextView>(Resource.Id.textView3);
                rver = FindViewById<TextView>(Resource.Id.textView4);
                ver = FindViewById<TextView>(Resource.Id.textView5);
                cn = FindViewById<TextView>(Resource.Id.textView6);
                cnr = FindViewById<TextView>(Resource.Id.textView7);

                //Buttons
                back = FindViewById<AppCompatButton>(Resource.Id.btn);
                update = FindViewById<AppCompatButton>(Resource.Id.appCompatButton1);
                project = FindViewById<AppCompatButton>(Resource.Id.appCompatButton2);
                issue = FindViewById<AppCompatButton>(Resource.Id.appCompatButton4);

                //Setting up Fonts
                header.Typeface = app.Typeface = desc.Typeface = rver.Typeface = ver.Typeface = cn.Typeface = cnr.Typeface = update.Typeface = project.Typeface =
                    issue.Typeface = urbanistfont;

                //Version Control info
                ver.Text = versioncode;
                cnr.Text = CodeName;

                // Functions
                back.Click += (sender, args) =>
                {
                    Intent intent = new Intent(this, typeof(home));
                    StartActivity(intent);
                    Finish();
                };
                project.Click += (sender, args) =>
                {
                    Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/Bionic-Reading-Library/Bionic-Reading-Lib"));
                    browserIntent.SetFlags(ActivityFlags.NewTask);
                    StartActivity(browserIntent);
                    Finish();

                };
                issue.Click += (sender, args) =>
                {
                    Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/Bionic-Reading-Library/Bionic-Reading-Lib/issues/new"));
                    browserIntent.SetFlags(ActivityFlags.NewTask);
                    StartActivity(browserIntent);
                    Finish();
                };

                //Check For updates
                update.Click += CheckUpdates_Click;
            }
            catch (Exception)
            {
                Intent intent = new Intent(this, typeof(Error));
                StartActivity(intent);
            }
        }
        private async Task<JObject> GetLatestReleaseAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(ReleasesUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.Write(response.StatusCode);
                        return null;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(responseContent);
                }
            }
            catch (Exception)
            {
                Intent intent = new Intent(this, typeof(Error));
                intent.Extras.PutString("error", Failup);
                StartActivity(intent);
                return null;
            }
        }

        private async void CheckUpdates_Click(object sender, EventArgs e)
        {
            try
            {
                var latestRelease = await GetLatestReleaseAsync();

                if (latestRelease != null)
                {
                    var currentVersion = versioncode;
                    var latestVersion = latestRelease.Value<string>("tag_name").TrimStart('v');
                    var releaseNotes = latestRelease.Value<string>("body");
                    downloadUrl = latestRelease.Value<JArray>("assets")[0].Value<string>("browser_download_url");



                    if (currentVersion != latestVersion)
                    {
                        ShowUpdatePopup(latestVersion, releaseNotes, downloadUrl);
                    }
                    else
                    {
                        Toast.MakeText(this, "You are using the latest version", ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Unable to check for updates", ToastLength.Short).Show();
                }
            }
            catch (Exception)
            {
                Intent intent = new Intent(this, typeof(Error));
                StartActivity(intent);
            }
        }


        private void ShowUpdatePopup(string latestVersion, string releaseNotes, string downloadUrl)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Update Available");
            builder.SetMessage($"A new version ({latestVersion}) is available.\n\nRelease Notes:\n{releaseNotes}");
            builder.SetPositiveButton("Download", (s, e) =>
            {
                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(downloadUrl));
                StartActivity(intent);
            });
            builder.SetNegativeButton("Cancel", (s, e) => { });
            var dialog = builder.Create();
            dialog.Show();
        }


    }
}
