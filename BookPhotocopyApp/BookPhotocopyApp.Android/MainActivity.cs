using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Xamarin.Essentials;
using Google.Android.Material.Snackbar;
using Android.Net;
using Uri = Android.Net.Uri;
using Android.Provider;
using Android.Runtime;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using Android.Telecom;
using Java.Lang.Reflect;
using static Android.Provider.Telephony.Mms;

namespace BookPhotocopyApp.Droid
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int ManageFilesPermissionRequestCode = 100;
        private AlertDialog permissionAlertDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());

            CheckAndRequestPermission();
        }

        private void CheckAndRequestPermission()
        {
            // Check build version and if manage all files permission is given
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R && !Android.OS.Environment.IsExternalStorageManager)
            {
                ShowPermissionAlertDialog();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Check if the permission is granted
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R && !Android.OS.Environment.IsExternalStorageManager)
            {
                // Show the permission dialog
                ShowPermissionAlertDialog();
            }
            else
            {

            }
        }


        // Go to manage all files
        private void ShowPermissionAlertDialog()
        {
            // Create a custom dialog
            using (AlertDialog.Builder builder = new AlertDialog.Builder(this))
            {
                builder.SetMessage("Permission needed!")
                    .SetCancelable(false)
                    .SetPositiveButton("Go to Settings", (senderAlert, args) => OpenAppSettings())
                    .SetNegativeButton("Cancel", (senderAlert, args) => Finish());

                // Show the dialog
                permissionAlertDialog = builder.Create();
                permissionAlertDialog.Show();
            }
        }

        private void OpenAppSettings()
        {
            try
            {
                Uri uri = Android.Net.Uri.Parse("package:" + Application.Context.ApplicationInfo.PackageName);
                Intent intent = new Intent(Settings.ActionManageAppAllFilesAccessPermission, uri);
                StartActivityForResult(intent, ManageFilesPermissionRequestCode);
            }
            catch
            {
                Intent intent = new Intent();
                intent.SetAction(Settings.ActionManageAppAllFilesAccessPermission);
                StartActivityForResult(intent, ManageFilesPermissionRequestCode);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

}
