using System;
using BookPhotocopyApp;
using Android.OS;
using BookPhotocopyApp.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(EnvironmentHelper))]
namespace BookPhotocopyApp.Droid
{
    public class EnvironmentHelper : IEnvironmentHelper
    {
        public string GetExternalStorageDirectory()
        {
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }
        public string checkManageFilePermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R && !Android.OS.Environment.IsExternalStorageManager)
            {
                // If the app does not have the "Manage External Storage" permission, show a Snackbar.
                return "Permission needed!";
            }
            else
            {
                // Return empty string if permission is not needed
                return "";
            }
        }
    }
}
