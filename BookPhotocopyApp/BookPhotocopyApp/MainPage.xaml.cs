using BookPhotocopyApp.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace BookPhotocopyApp
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<string> pdfFiles;
        private readonly string checkManageFilePermission;

        public MainPage()
        {
            InitializeComponent();

            //Get exteranl storage arndrois os
            var environmentHelper = DependencyService.Get<IEnvironmentHelper>();
            checkManageFilePermission = environmentHelper.checkManageFilePermission();


            pdfFiles = new ObservableCollection<string>();
            pdfListView.ItemsSource = pdfFiles;
        }


        private async void AddStoryBook(object sender, EventArgs e)
        {
            // Check if "Manage External Storage" permission is needed
            if (!string.IsNullOrEmpty(checkManageFilePermission))
            {
                // Permission needed, show a Snackbar
                await DisplayAlert("Permission Needed", checkManageFilePermission, "OK");
            }
            else
            {
                // Permission not needed, navigate to AddStoryBookDetails page
                Navigation.PushAsync(new AddStoryBookDetails());
            }
        }
    }
}
