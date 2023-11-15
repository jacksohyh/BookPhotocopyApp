using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using Tesseract;
using net.vieapps.Components.Utility.Epub;

namespace BookPhotocopyApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddStoryBookDetails : ContentPage
    {
        private readonly string externalStorageDirectory;
        private string extractedText;
        private byte[] coverImageBinaryData;

        public AddStoryBookDetails()
        {
            InitializeComponent();

            // Get external storage Android OS
            var environmentHelper = DependencyService.Get<IEnvironmentHelper>();
            externalStorageDirectory = environmentHelper.GetExternalStorageDirectory();
        }

        private async void OnCameraButtonClick(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync();

                if (result != null)
                {
                    using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile(result.FullPath))
                        {
                            using (var page = engine.Process(img))
                            {
                                extractedText = page.GetText();
                                // Now you have the text from the image (variable 'extractedText')
                                var imageSource = ImageSource.FromStream(() => result.OpenReadAsync().Result);
                                // YourImageControl.Source = imageSource;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., if the user denies camera access
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async void OnUploadButtonClick(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Pick an image"
                });

                if (fileResult != null)
                {
                    using (Stream stream = await fileResult.OpenReadAsync())
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        coverImageBinaryData = ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle file picking errors
                Console.WriteLine($"Error picking file: {ex.Message}");
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            // Retrieve values from the Entry fields
            string title = TitleEntry.Text;
            string author = AuthorEntry.Text;
            string description = DescriptionEntry.Text;
            string publisher = PublisherEntry.Text;

            // Generate EPUB with the entered data, extracted text, and cover image
            GenerateEpub(title, description, author, publisher, extractedText, coverImageBinaryData);
        }

        private void GenerateEpub(string title, string description, string author, string publisher, string extractedText, byte[] coverImageBinaryData)
        {
            try
            {
                // Create EPUB document
                var epub = new net.vieapps.Components.Utility.Epub.Document();
                epub.AddBookIdentifier(Guid.NewGuid().ToString());
                epub.AddLanguage("en"); // Set language code as needed
                epub.AddTitle(title);
                epub.AddAuthor(author);
                epub.AddPublisher(publisher); // Set publisher as needed

                // Add cover image
                if (coverImageBinaryData != null)
                {
                    var coverImageId = epub.AddImageData("cover.jpg", coverImageBinaryData);
                    epub.AddMetaItem("cover", coverImageId);
                }

                // Add content
                var contentPage = $@"<!DOCTYPE html>
                    <html xmlns=""http://www.w3.org/1999/xhtml"">
                        <head>
                            <title>{title}</title>
                            <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
                        </head>
                        <body>
                            <h1>{title}</h1>
                            <p>Author: {author}</p>
                            <p>Description: {description}</p>
                            <p>Extracted Text: {extractedText}</p>
                        </body>
                    </html>";
                epub.AddXhtmlData("content.xhtml", contentPage);

                // Specify the subdirectory (e.g., "Downloads")
                string subdirectory = "BookPhotocopy";

                // Combine the external storage directory with the subdirectory and the EPUB file name
                string epubFileName = title + ".epub";
                string epubPath = Path.Combine(externalStorageDirectory, subdirectory, epubFileName);

                // Ensure that the subdirectory exists, create it if not
                Directory.CreateDirectory(Path.Combine(externalStorageDirectory, subdirectory));

                // Save the EPUB to the specified directory
                epub.Generate(epubPath);

                DisplayAlert("EPUB Saved", "EPUB saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                // Handle the EPUB creation or file saving error
                Console.WriteLine($"Error saving EPUB: {ex.Message}");
                DisplayAlert("Error", "An error occurred while saving the EPUB.", "OK");
            }
        }
    }
}
