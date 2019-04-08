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
using Windows.Media.Playback;
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
    public sealed partial class Music : Page
    {
        private ObservableCollection<StorageFile> musics = new ObservableCollection<StorageFile>();
        private StorageFile selectedMusic = null;

        public ObservableCollection<StorageFile> Musics
        {
            get => musics;
            set
            {
                if (musics != value)
                {
                    musics = value;
                    OnPropertyChanged();
                }
            }
        }

        public StorageFile SelectedMusic
        {
            get => selectedMusic;
            set
            {
                if (selectedMusic != value)
                {
                    selectedMusic = value;
                    OnPropertyChanged();
                }
            }
        }

        public Music()
        {
            this.InitializeComponent();
            volumeSlider.Value = 100;

        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private async void OnSelectFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderPicker NewFolderPicker = new FolderPicker();
                NewFolderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                NewFolderPicker.FileTypeFilter.Add(".mp3");
                StorageFolder folder = await NewFolderPicker.PickSingleFolderAsync();

                IReadOnlyList<StorageFile> listFiles = await folder.GetFilesAsync();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.InnerException);
            }
        }

        private void NextOrPrev(object sender, RoutedEventArgs e)
        {
            int indexChange = Convert.ToInt32(((AppBarButton)sender).Tag);
            MediaPlayer.Stop();
            ListViewMusic.SelectedItem = ListViewMusic.Items[ListViewMusic.SelectedIndex + indexChange];
            ChangeSelectedMusic(ListViewMusic.SelectedItem as StorageFile);
            PlayButton.Icon = new SymbolIcon(Symbol.Pause);
        }

        private void CheckVisibleNav()
        {
            PrevBtn.IsEnabled = true;
            NextBtn.IsEnabled = true;
            if (ListViewMusic.SelectedIndex == 0)
            {
                PrevBtn.IsEnabled = false;
            }
            if (ListViewMusic.SelectedIndex == Musics.Count - 1)
            {
                NextBtn.IsEnabled = false;
            }

            if (ListViewMusic.SelectedItem == null)
            {
                PrevBtn.IsEnabled = false;
                NextBtn.IsEnabled = false;
            }
        }

        private void volumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider vol = sender as Slider;

            if (vol != null)
            {
                MediaPlayer.Volume = vol.Value / 100;

                this.volume.Text = vol.Value.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSelectMusic(object sender, SelectionChangedEventArgs e)
        {
            ListView mysender = (ListView)sender;
            ChangeSelectedMusic((StorageFile)mysender.SelectedItem);
        }


        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Pause();
            PlayButton.Icon = new SymbolIcon(Symbol.Play);
        }

        private async void ChangeSelectedMusic(StorageFile val)
        {
            MediaPlayer.Stop();
            SelectedMusic = val;
            CheckVisibleNav();
            MediaPlayer.SetSource(await val.OpenAsync(FileAccessMode.Read), val.ContentType);
            MediaPlayer.Play();
            PlayButton.Icon = new SymbolIcon(Symbol.Pause);
        }

        private void timer_Tick(object sender, object e)
        {
            if (MediaPlayer.Source != null && MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                Progress.Minimum = 0;
                Progress.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                Progress.Value = MediaPlayer.Position.TotalSeconds;

            }
        }
    }
}

