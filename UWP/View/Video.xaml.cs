using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Video : Page
    {
        private ObservableCollection<StorageFile> videos = new ObservableCollection<StorageFile>();
        private StorageFile selectedVideo = null;

        public ObservableCollection<StorageFile> Videos
        {
            get => videos;
            set
            {
                if (videos != value)
                {
                    videos = value;
                    OnPropertyChanged();

                }
            }
        }

        public StorageFile SelectedVideo
        {
            get => selectedVideo;
            set
            {
                if (selectedVideo != value)
                {
                    selectedVideo = value;
                    OnPropertyChanged();

                }
            }
        }

        public Video()
        {
            this.InitializeComponent();
        }

        private async void OnSelectFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderPicker oFolderPicker = new FolderPicker();
                oFolderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                oFolderPicker.FileTypeFilter.Add(".mp4");
                StorageFolder folder = await oFolderPicker.PickSingleFolderAsync();

                IReadOnlyList<StorageFile> listFiles = await folder.GetFilesAsync();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.InnerException);
            }
        }

        private async void OnSelectVideo(object sender, SelectionChangedEventArgs e)
        {
            ListView mysender = (ListView)sender;
            SelectedVideo = (StorageFile)mysender.SelectedItem;
            VideoPlayer.SetSource(await SelectedVideo.OpenAsync(FileAccessMode.Read), SelectedVideo.ContentType);
            VideoPlayer.Play();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
