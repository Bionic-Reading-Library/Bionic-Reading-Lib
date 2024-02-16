using Android;
using Xamarin.Essentials;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Linq;
using Android.Text;
using Android.Text.Style;

namespace Bionic_Reading_Lib
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private const string ReleasesUrl = "https://api.github.com/repos/Bionic-Reading-Library/BRL-Release/releases/latest";
        private string versioncode = $"{AppInfo.VersionString}";
        private string versionbuid = "";
        private string Failup = "Network Returned 404 - Error Checking for Updates.";
        private string downloadUrl = "nice";
        TextView header;
        TextView desc;
        TextView help;
        TextView udate;
        TextView dl;
        TextView verp;
        TextView verbuild;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            int appColor = ContextCompat.GetColor(this, Resource.Color.TextColor);
            Window.SetStatusBarColor(new Color(appColor));
            var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
            versionbuid = GetString(Resource.String.vbuild);
            header = FindViewById<TextView>(Resource.Id.textView1);
            verbuild = FindViewById<TextView>(Resource.Id.versionbuild);
            desc = FindViewById<TextView>(Resource.Id.textView2);
            help = FindViewById<TextView>(Resource.Id.nht);
            udate = FindViewById<TextView>(Resource.Id.update);
            dl = FindViewById<TextView>(Resource.Id.dl);
            verp = FindViewById<TextView>(Resource.Id.verp);
            verbuild.Typeface = header.Typeface = desc.Typeface = help.Typeface = udate.Typeface = dl.Typeface = verp.Typeface = urbanistfont;
            udate.SetTypeface(udate.Typeface, TypefaceStyle.Bold);
            var appmsg = "Discover a Better Reading Experience."; 

            // Create a SpannableString
            SpannableString spannableString = new SpannableString(appmsg);

            // Apply bold styling to specific parts of the text
            spannableString.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, 4, SpanTypes.ExclusiveExclusive); //Disc
            spannableString.SetSpan(new StyleSpan(TypefaceStyle.Bold), 9, 10, SpanTypes.ExclusiveExclusive); //a
            spannableString.SetSpan(new StyleSpan(TypefaceStyle.Bold), 11, 14, SpanTypes.ExclusiveExclusive); // "Bet"
            spannableString.SetSpan(new StyleSpan(TypefaceStyle.Bold), 18, 22, SpanTypes.ExclusiveExclusive); // "Rea"
            spannableString.SetSpan(new StyleSpan(TypefaceStyle.Bold), 26, 32, SpanTypes.ExclusiveExclusive); // "Experi"
            header.SetText(spannableString, TextView.BufferType.Spannable);
            verbuild.Text = "// " + versionbuid;
            AppCompatButton start = FindViewById<AppCompatButton>(Resource.Id.appCompatButton1);
            AppCompatButton exit = FindViewById<AppCompatButton>(Resource.Id.appCompatButton2);
            start.Typeface = exit.Typeface = urbanistfont;
            start.SetTypeface(start.Typeface, TypefaceStyle.Bold);
            exit.SetTypeface(exit.Typeface, TypefaceStyle.Bold);
            help.SetTypeface(help.Typeface, TypefaceStyle.Bold);


            exit.Click += (sender, args) =>
            {
                Finish();
                Process.KillProcess(Process.MyPid());
            };
            start.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(home));
                StartActivity(intent);
                Finish();
            };
            help.Click += (sender, args) =>
            {

                Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/Bionic-Reading-Library/Bionic-Reading-Lib/blob/master/README.md"));
                browserIntent.SetFlags(ActivityFlags.NewTask);
                StartActivity(browserIntent);
                Finish();
            };

            CheckUpdates_OnLoad(this, EventArgs.Empty);




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

        private async void CheckUpdates_OnLoad(object sender, EventArgs e)
        {
            var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");
            RelativeLayout btn = FindViewById<RelativeLayout>(Resource.Id.dlbtn);
            udate.Typeface = dl.Typeface = verp.Typeface = urbanistfont;
            udate.SetTypeface(udate.Typeface, TypefaceStyle.Bold);
            try
            {

                var latestRelease = await GetLatestReleaseAsync();

                if (latestRelease != null)
                {
                    var currentVersion = versioncode;
                    var latestVersion = latestRelease.Value<string>("tag_name").TrimStart('v');
                    downloadUrl = latestRelease.Value<JArray>("assets")[0].Value<string>("browser_download_url");


                    if (currentVersion != latestVersion)
                    {
                        var releasesUrl = latestRelease.Value<string>("html_url");
                        udate.Text = "Update Available";
                        verp.Text = $"V{latestVersion}";
                        btn.Click += (sender, args) =>
                        {
                            Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(downloadUrl));
                            browserIntent.SetFlags(ActivityFlags.NewTask);
                            StartActivity(browserIntent);
                        };

                    }
                    else
                    {
                        udate.Text = "No Updates";
                        dl.Text = "You are running the latest version of the app";
                        verp.Text = $"V{currentVersion}";
                    }
                }
                else
                {
                    udate.Text = "Unable to check updates";
                    dl.Text = "Something went wrong, Try again later.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }







        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }
    }
}