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
        int TotalCount;
        Song PreviousSong { get; set; }
        Song NextSong { get; set; }
        Song CurrentSong { get; set; }

        public TimeSpan Pausetime { get; private set; }
        public bool Pausedornot { get; private set; }
        public IList<string> songs = new List<string> { };
        private double HalfWidth;
        private int x;
        private int Songindex;
        public List<Song> Songs = new List<Song> { };


        public PlayerPage(int songindex,List<Song> songList)
        {
            InitializeComponent();
            Songindex = songindex;
            TranslateOriginal();
            HalfWidth = Application.Current.MainPage.Width / 2 + Application.Current.MainPage.Width / 4;
            
            for (int i = songindex; i < songList.Count; i++)
            {
                Songs.Add(songList[i]);
                if (i == songList.Count - 1)
                {
                    for (int c = songindex - 1; c >= 0; c--)
                    {
                        Songs.Add(songList[c]);
                    }
                }
            }
            TotalCount = Songs.Count();
            Song song = Songs[0];
            CurrentSong = song;
            
            NextSong = Songs[1];
            
            for(int i = 0; i < Songs.Count; i++)
            {
                songs.Add(Songs[i].Path);
            }
            CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
            CrossMediaManager.Current.StateChanged += Current_StateChanged;
            
            ImageSource image = song.imageSource();
            BackImage.Source = image;
            MainImage.Source = image;
            TitleLabel.Text = song.Title;
            AlbumLabel.Text = song.Album;
            ArtistLabel.Text = song.Artist;
            TotalLabel.Text = song.Duration;
            CrossMediaManager.Current.Play(songs);
            LoadView();
            Pausedornot = false;
            Pausetime = new TimeSpan(0, 0, 0);
            
            MainCollectionView.ItemsSource = Songs;
            
            
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
                await CrossMediaManager.Current.Play();
                await CrossMediaManager.Current.PlayPrevious();
                if(CrossMediaManager.Current.Position < new TimeSpan(0, 0, 3))
                {
                    Current--;
                    Populate(Current);
                    LoadView();
                }
                
            }
            catch(Exception u)
            {
                Current++;
                LoadView();
            }
            
        }
        public async void PlayPreviousitem()
        {
            try
            {
                await CrossMediaManager.Current.Play();
                Current--;
                Populate(Current);
                await CrossMediaManager.Current.SeekTo( new TimeSpan(0, 0, 0));
                await CrossMediaManager.Current.PlayPrevious();
                
                LoadView();
            }
            catch(Exception t)
            {
                Current++;
                LoadView();
            }
        }
        private async void NextTapped(object sender, EventArgs e)
        {
            try
            {
                await CrossMediaManager.Current.Play();
                Current++;
                Populate(Current);
                await CrossMediaManager.Current.PlayNext();
                
                LoadView();
            }
            catch (Exception u)
            {
                Current--;
                LoadView();
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

        

        public void Populate(int path)
        {
            Song song = Songs[path];
            ImageSource image = song.imageSource();
            BackImage.Source = image;
            MainImage.Source = image;
            TitleLabel.Text = song.Title;
            AlbumLabel.Text = song.Album;
            ArtistLabel.Text = song.Artist;
            TotalLabel.Text = song.Duration;
            try
            {
                PreviousSong = Songs[path - 1];
            }
            catch(Exception e)
            {
                PreviousSong = null;
            }
            try
            {
                CurrentSong = Songs[path];
            }
            catch (Exception e)
            {

            }
            try
            {
                NextSong = Songs[path + 1];
            }
            catch (Exception e)
            {
                NextSong = null;
            }
            
            
            
        }

        private void ShuffleTapped(object sender, EventArgs e)
        {
            ShufleBox.IsVisible = true;
        }

        private void RepeatTapped(object sender, EventArgs e)
        {
            RepeatBox.IsVisible = true;
        }

        private async void TranslateOriginal()
        {
            MainGrid.IsVisible = false;
            MainCollectionView.FadeTo(0);
            MainCollectionView.IsVisible = false;
            MainCollectionView.FadeTo(1);
            
            MainGrid.TranslationX = 0;
            MainGrid.TranslateTo(0, 0);
            ControlsLayout.TranslateTo(0, 0);
            bool c = await AwaiterF();
            MainGrid.IsVisible = true;
            MainGrid2.TranslationX = Application.Current.MainPage.Width;
            MainGrid1.TranslationX = -(Application.Current.MainPage.Width);
            //CollectionStack.TranslationY = Application.Current.MainPage.Height;
        }
        private async void TranslateOriginalAnimate()
        {
            MainGrid2.TranslateTo(Application.Current.MainPage.Width, 0);
            MainGrid1.TranslateTo(-(Application.Current.MainPage.Width), 0);
            //CollectionStack.TranslationY = Application.Current.MainPage.Height;
            await MainGrid.TranslateTo(0, 0);
        }
        private async Task<bool> AwaiterF()
        {
            await Task.Delay(5);
            return true;
        }
        private void LoadView()
        {
            if (PreviousSong != null)
            {
                ImageSource image = PreviousSong.imageSource();
                BackImage1.Source = image;
                MainImage1.Source = image;
            }
            if(NextSong != null)
            {
                ImageSource image = NextSong.imageSource();
                BackImage2.Source = image;
                MainImage2.Source = image;
            }
        }

        private async void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    if (Math.Abs(e.TotalX) > Math.Abs(e.TotalY) && MainGrid.TranslationY == 0)
                    {
                        MainGrid.TranslationX =
                        Math.Min(MainGrid.TranslationX + e.TotalX, Application.Current.MainPage.Width);
                        MainGrid1.TranslationX =
                        Math.Min(MainGrid1.TranslationX + e.TotalX, Application.Current.MainPage.Width);
                        MainGrid2.TranslationX =
                        Math.Max(MainGrid2.TranslationX + e.TotalX, 0);
                        //Content.TranslationY =
                        //  Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs(Content.Height - App.ScreenHeight));
                        break;
                    }
                    else if(MainGrid.TranslationX == 0 && e.TotalY < 0)
                    {
                        MainGrid.TranslationY =
                        Math.Max(MainGrid.TranslationY + e.TotalY, -Application.Current.MainPage.Height);
                        break;
                    }
                    break;
                    
                    

                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    if (-MainGrid.TranslationX > (Application.Current.MainPage.Width / 2) && (MainGrid.TranslationX < 0))
                    {
                        NextTapped(null, null);
                        TranslateOriginal();
                        
                    }
                    else if (MainGrid.TranslationX > (HalfWidth))
                    {
                        PlayPreviousitem();
                        TranslateOriginal();
                        
                    }
                    else if(-MainGrid.TranslationY > App.Current.MainPage.Height/4)
                    {
                        MainGrid.TranslateTo(0, ControlsLayout.Height   - App.Current.MainPage.Height);
                        await ControlsLayout.TranslateTo(0, ControlsLayout.Height - App.Current.MainPage.Height);
                        UpSvg.RotateTo(180);
                        MainCollectionView.Margin = new Thickness(0, ControlsLayout.Height, 0, 0);
                        MainCollectionView.IsVisible = true;
                        //CollectionStack.TranslateTo(0,-((ControlsLayout.Height) - App.Current.MainPage.Height));
                    }
                    else
                    {
                        TranslateOriginalAnimate();
                    }
                    break;
            }
        }

        private async void MainCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = Math.Abs((((Song)e.CurrentSelection[0]).Index - Songindex) % TotalCount);
            await CrossMediaManager.Current.PlayQueueItem(index);
            Current = index;
            Populate(Current);
            LoadView();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TranslateOriginal(); 
        }
    }
}