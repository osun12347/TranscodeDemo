using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TranscodeDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");

            StorageFile source = await openPicker.PickSingleFileAsync();

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();

            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.VideosLibrary;

            savePicker.DefaultFileExtension = ".mp4";
            savePicker.SuggestedFileName = "New Video";

            savePicker.FileTypeChoices.Add("MPEG4", new string[] { ".mp4" });

            StorageFile destination = await savePicker.PickSaveFileAsync();


            MediaEncodingProfile profile =
    MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD720p);


            MediaTranscoder transcoder = new MediaTranscoder();

            if (source!=null && destination!=null && profile!=null)
            {
                PrepareTranscodeResult prepareOp = await
              transcoder.PrepareFileTranscodeAsync(source, destination, profile);

                if (prepareOp.CanTranscode)
                {
                    var transcodeOp = prepareOp.TranscodeAsync();

                    transcodeOp.Progress +=
                        new AsyncActionProgressHandler<double>(TranscodeProgress);
                    transcodeOp.Completed +=
                        new AsyncActionWithProgressCompletedHandler<double>(TranscodeComplete);
                }
                else
                {
                    switch (prepareOp.FailureReason)
                    {
                        case TranscodeFailureReason.CodecNotFound:
                            System.Diagnostics.Debug.WriteLine("Codec not found.");
                            break;
                        case TranscodeFailureReason.InvalidProfile:
                            System.Diagnostics.Debug.WriteLine("Invalid profile.");
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine("Unknown failure.");
                            break;
                    }
                }
            }

        }

        private void TranscodeComplete(IAsyncActionWithProgress<double> asyncInfo, AsyncStatus asyncStatus)
        {
            asyncInfo.GetResults();
            if (asyncInfo.Status == AsyncStatus.Completed)
            {
                // Display or handle complete info.
            }
            else if (asyncInfo.Status == AsyncStatus.Canceled)
            {
                // Display or handle cancel info.
            }
            else
            {
                // Display or handle error info.
            }

        }

        private void TranscodeProgress(IAsyncActionWithProgress<double> asyncInfo, double progressInfo)
        {
        }
    }
}
