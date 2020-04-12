using GrooveClone.Models;
using MediaManager;
using MediaManager.Library;
using MediaManager.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GrooveClone
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerPage : ContentPage
    {
        private double posval;
        int Current = 0;

        public TimeSpan Pausetime { get; private set; }
        public bool Pausedornot { get; private set; }
        public IList<string> songs = new List<string> { };
        public List<Song> Songs { get; set; }
        public PlayerPage(int songindex,List<Song> songList)
        {
            
            Songs = songList;
            Song song = Songs[songindex];
            
            for(int i = songindex; i < Songs.Count; i++)
            {
                songs.Add(Songs[i].Path);
                if(i==Songs.Count - 1)
                {
                    for(int c = songindex-1; c >= 0; c--)
                    {
                        songs.Add(Songs[c].Path);
                    }
                }
            }
            CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            InitializeComponent();
            ImageSource image = song.imageSource();
            BackImage.Source = image;
            MainImage.Source = image;
            TitleLabel.Text = song.Title;
            AlbumLabel.Text = song.Album;
            ArtistLabel.Text = song.Artist;
            TotalLabel.Text = song.Duration;
            CrossMediaManager.Current.Play(songs);
            Pausedornot = false;
            Pausetime = new TimeSpan(0, 0, 0);

        }

        private async void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            if (e.State == MediaPlayerState.Playing)
            {
                if (Pausetime != new TimeSpan(0, 0, 0))
                {
                    await CrossMediaManager.Current.SeekTo(Pausetime);
                    if (Pausedornot)
                    {
                        Pausetime = new TimeSpan(0, 0, 0);
                    }
                }
            }
        }

        private void Current_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
            posval = CrossMediaManager.Current.Position.TotalSeconds / CrossMediaManager.Current.Duration.TotalSeconds;

            if (posval > 0 || posval < 1)
            {
                MainSlider.Value = posval;
                Device.BeginInvokeOnMainThread(() =>
                {
                    ElapsedLabel.Text = CrossMediaManager.Current.Position.ToString().Substring(4, 4);
                });

            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            CrossMediaManager.Current.PlayPause();
        }

        private void MainSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            TimeSpan timeSpan1 = CrossMediaManager.Current.Position;
            TimeSpan timeSpan = TimeSpan.FromSeconds(e.NewValue * CrossMediaManager.Current.Duration.TotalSeconds);
            if (timeSpan1 - timeSpan > new TimeSpan(0, 0, 2) || timeSpan1 - timeSpan < new TimeSpan(0, 0, 0))
            {

                CrossMediaManager.Current.SeekTo(timeSpan);
            }
            if (MainSlider.Value >= .99)
            {
                MainSlider.Value = 0;
            }
        }

        private async void PreviousTapped(object sender, EventArgs e)
        {
            try
            {
                await CrossMediaManager.Current.PlayPrevious();
                if(CrossMediaManager.Current.Position < new TimeSpan(0, 0, 3))
                {
                    Current--;
                    Populate(songs[Current]);
                }
                
            }
            catch(Exception u)
            {
                Current++;
            }
            
        }
        private async void NextTapped(object sender, EventArgs e)
        {
            try
            {
                await CrossMediaManager.Current.PlayNext();
                Current++;
                Populate(songs[Current]);
            }
            catch (Exception u)
            {
                Current--;
            }
        }
        private async void PlayTapped(object sender, EventArgs e)
        {
            if (CrossMediaManager.Current.IsPlaying())
            {
                Pausetime = CrossMediaManager.Current.Position;
                await CrossMediaManager.Current.Pause();
                Pausedornot = false;
                PlayPauseSvg.Source = "Play.svg";
            }
            else
            {

                await CrossMediaManager.Current.Play();
                Pausedornot = true;
                PlayPauseSvg.Source = "Pause.svg";
            }
        }

        

        public void Populate(string path)
        {
            Song song = Songs.First(item => item.Path == path);
            ImageSource image = song.imageSource();
            BackImage.Source = image;
            MainImage.Source = image;
            TitleLabel.Text = song.Title;
            AlbumLabel.Text = song.Album;
            ArtistLabel.Text = song.Artist;
            TotalLabel.Text = song.Duration;
        }

        private void ShuffleTapped(object sender, EventArgs e)
        {
            ShufleBox.IsVisible = true;
        }

        private void RepeatTapped(object sender, EventArgs e)
        {
            RepeatBox.IsVisible = true;
        }
    }
}