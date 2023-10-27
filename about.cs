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

namespace Bionic_Reading_Lib
{
    [Activity(Label = "about")]
    public class about : Activity
    {
        private const string ReleasesUrl = "https://api.github.com/repos/Bionic-Reading-Library/BRL-Release/releases/latest";
        private const int RequestCodeStoragePermission = 100;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.about);
                var urbanistfont = Typeface.CreateFromAsset(Assets, "fonts/UrbanistNonItalic.ttf");


                // Create your application here
                TextView header = FindViewById<TextView>(Resource.Id.textView1);
                TextView app = FindViewById<TextView>(Resource.Id.textView2);
                TextView desc = FindViewById<TextView>(Resource.Id.textView3);
                TextView rver = FindViewById<TextView>(Resource.Id.textView4);
                TextView ver = FindViewById<TextView>(Resource.Id.textView5);
                TextView cn = FindViewById<TextView>(Resource.Id.textView6);
                TextView cnr = FindViewById<TextView>(Resource.Id.textView7);

                //Buttons
                AppCompatButton back = FindViewById<AppCompatButton>(Resource.Id.btn);
                AppCompatButton update = FindViewById<AppCompatButton>(Resource.Id.appCompatButton1);
                AppCompatButton project = FindViewById<AppCompatButton>(Resource.Id.appCompatButton2);
                AppCompatButton issue = FindViewById<AppCompatButton>(Resource.Id.appCompatButton4);

                //Setting up Fonts
                header.Typeface = app.Typeface = desc.Typeface = rver.Typeface = ver.Typeface = cn.Typeface = cnr.Typeface = update.Typeface = project.Typeface =
                    issue.Typeface = urbanistfont;

                //Version Control info
                string versionName = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
                ver.Text = versionName;

                // Functions
                back.Click += (sender, args) =>
                {
                    Intent intent = new Intent(this, typeof(home));
                    StartActivity(intent);
                };
                project.Click += (sender, args) =>
                {
                    Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/Bionic-Reading-Library/Bionic-Reading-Lib"));
                    browserIntent.SetFlags(ActivityFlags.NewTask);
                    StartActivity(browserIntent);

                };
                issue.Click += (sender, args) =>
                {
                    Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://github.com/Bionic-Reading-Library/Bionic-Reading-Lib/issues/new"));
                    browserIntent.SetFlags(ActivityFlags.NewTask);
                    StartActivity(browserIntent);
                };

                //Check For updates
                update.Click += CheckUpdates_Click;
            }catch(Exception ex)
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
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(responseContent);
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                return null;
            }
            catch (JsonReaderException ex)
            {
                // Handle JSON reader exception
                return null;
            }
        }

        private async void CheckUpdates_Click(object sender, EventArgs e)
        {

            int version = (int)Build.VERSION.SdkInt;

            //Checks What API is the device is (For Compatibility Reasons)
            if (version >= 33)
            {
                //API 33 and above uses ReadMediaImages
                try
                {
                   
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadMediaImages) == Permission.Granted)
                    {
                        var latestRelease = await GetLatestReleaseAsync();

                        if (latestRelease != null)
                        {
                            var currentVersion = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
                            var latestVersion = latestRelease.Value<string>("tag_name").TrimStart('v');
                            var releaseNotes = latestRelease.Value<string>("body");
                            var downloadUrl = latestRelease.Value<JArray>("assets")[0].Value<string>("browser_download_url");

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
                    else
                    {
                        ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage, Manifest.Permission.ReadMediaImages }, RequestCodeStoragePermission);
                    }
                }
                catch (Exception ex)
                {
                    Intent intent = new Intent(this, typeof(Error));
                    StartActivity(intent);
                }
            }
            else
            {
                //API 32 and Below uses WriteExternalStorage
                try
                {
                    //Switch this to Manifest.Permission.WriteExternalStorage for android 12 and below.
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == Permission.Granted)
                    {
                        var latestRelease = await GetLatestReleaseAsync();

                        if (latestRelease != null)
                        {
                            var currentVersion = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
                            var latestVersion = latestRelease.Value<string>("tag_name").TrimStart('v');
                            var releaseNotes = latestRelease.Value<string>("body");
                            var downloadUrl = latestRelease.Value<JArray>("assets")[0].Value<string>("browser_download_url");

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
                    else
                    {
                        ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.ReadExternalStorage }, RequestCodeStoragePermission);
                    }
                }
                catch (Exception ex)
                {
                    Intent intent = new Intent(this, typeof(Error));
                    StartActivity(intent);
                }
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

                public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
                {
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                    if (requestCode == RequestCodeStoragePermission)
                    {
                        if (grantResults[0] == Android.Content.PM.Permission.Granted)
                        {
                            var checkUpdatesButton = FindViewById<Button>(Resource.Id.appCompatButton2);
                            checkUpdatesButton.PerformClick();
                        }
                        else
                        {
                            Toast.MakeText(this, "Cannot check for updates without storage permission", ToastLength.Short).Show();
                        }
                    }
                }


    }
    }
