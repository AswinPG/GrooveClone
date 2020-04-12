using GrooveClone.Models;
using MediaManager;
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

        public TimeSpan Pausetime { get; private set; }
        public bool Pausedornot { get; private set; }

        public PlayerPage(Song song)
        {
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
            CrossMediaManager.Current.Play(song.Path);
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
                    ElapsedLabel.Text = CrossMediaManager.Current.Position.ToString().Substring(0, 8);
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

        private void NextTapped(object sender, EventArgs e)
        {

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