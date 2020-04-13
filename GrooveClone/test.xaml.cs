using GrooveClone.Interfaces;
using GrooveClone.Models;
using MediaManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GrooveClone
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class test : ContentPage
    {
        private double posval = 0;

        List<Song> songs { get; set; }
        public test()
        {
            InitializeComponent();
            songs = new List<Song>() { };
            string path = Path.Combine(FileSystem.AppDataDirectory, "SongsData");
            if (System.IO.File.Exists(path))
            {
                try
                {
                    string jsondata = System.IO.File.ReadAllText(Path.Combine(FileSystem.AppDataDirectory, "SongsData"));
                    songs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Song>>(jsondata);
                    MainCollectionView.ItemsSource = songs;
                }
                catch (Exception t)
                {

                }
            }
            else
            {
                GetData();
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new test());
        }

        private void GetData()
        {
            List<string> mylist = (List<string>)DependencyService.Get<IMyfile>().GetFileLocation();
            try
            {
                string artists = "";
                songs.Clear();
                for (int i = 0; i < mylist.Count; i++)
                {
                    var tfile = TagLib.File.Create(mylist[i]);
                    artists = "";
                    for (int c = 0; c < tfile.Tag.Artists.Count(); c++)
                    {
                        artists = artists + tfile.Tag.Artists[c];
                    }
                    Song song = new Song(tfile.Tag.Pictures[0].Data.Count)
                    {
                        Album = tfile.Tag.Album,
                        Index = i,
                        Artist = artists,
                        Duration = tfile.Properties.Duration.ToString().Substring(4, 4),
                        Img = tfile.Tag.Pictures[0].Data.ToArray(),
                        Title = tfile.Tag.Title,
                        Path = mylist[i]
                    };
                    songs.Add(song);
                }
                
                MainCollectionView.ItemsSource = songs;
                string jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(songs);
                System.IO.File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, "SongsData"), jsondata);
                
            }
            catch (Exception t)
            {

            }
            

            //var listView = new ListView();
            //if (mylist != null)
            //{
            //    //listView.ItemsSource = mylist;
            //}

            //Content = listView;
        }

        private async void MainCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainCollectionView.SelectedItem != null)
            {
                //await Navigation.PushAsync());
                Song song = (Song)e.CurrentSelection[0];
                await Navigation.PushAsync(new PlayerPage(song.Index,songs));
            }

            MainCollectionView.SelectedItem = null;

        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            GetData();
        }

        

        private void MainSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            QueueData.Player.MainSlider_ValueChanged(sender, e);
            TitleLabel.Text = QueueData.Player.PlayingTitle; 
            BackImage.Source = QueueData.Player.image;
        }

        private void Previous(object sender, EventArgs e)
        {
            QueueData.Player.PreviousTapped(sender, e);
            TitleLabel.Text = QueueData.Player.PlayingTitle;
            BackImage.Source = QueueData.Player.image;
        }

        private void Play(object sender, EventArgs e)
        {
            QueueData.Player.PlayTapped(sender, e);
            TitleLabel.Text = QueueData.Player.PlayingTitle;
            BackImage.Source = QueueData.Player.image;
            if (CrossMediaManager.Current.IsPlaying())
            {
                PlayPauseSvg.Source = "Play.svg";
            }
            else
            {
                PlayPauseSvg.Source = "Pause.svg";
            }
        }

        private void Next(object sender, EventArgs e)
        {
            QueueData.Player.NextTapped(sender, e);
            TitleLabel.Text = QueueData.Player.PlayingTitle;
            BackImage.Source = QueueData.Player.image;
        }

        private void Shuffle(object sender, EventArgs e)
        {
            QueueData.Player.ShuffleTapped(sender, e);
            TitleLabel.Text = QueueData.Player.PlayingTitle;
            BackImage.Source = QueueData.Player.image;
        }

        private void Repeat(object sender, EventArgs e)
        {
            QueueData.Player.RepeatTapped(sender, e);
            TitleLabel.Text = QueueData.Player.PlayingTitle;
            BackImage.Source = QueueData.Player.image;
        }

        private void TapGestureRecognizer_Tapped_5(object sender, EventArgs e)
        {
            QueueData.Player.PreviousTapped(sender, e);
            TitleLabel.Text = QueueData.Player.PlayingTitle;
            BackImage.Source = QueueData.Player.image;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            try
            {
                CrossMediaManager.Current.PositionChanged += Current_PositionChanged;
                CrossMediaManager.Current.StateChanged += Current_StateChanged;
                Device.BeginInvokeOnMainThread(() => {
                    try
                    {
                        TitleLabel.Text = QueueData.Player.PlayingTitle;
                        //TotalLabel.Text = QueueData.Player.
                        BackImage.Source = QueueData.Player.image;
                        ControlGrid.IsVisible = true;
                    }
                    catch(Exception n)
                    {
                        ControlGrid.IsVisible = false;
                    }
                    
                });
                
            }
            catch
            {
                
            }
        }

        private void Current_StateChanged(object sender, MediaManager.Playback.StateChangedEventArgs e)
        {
            QueueData.Player.Current_StateChanged(sender, e);
        }

        private void Current_PositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e)
        {
            try
            {
                posval = CrossMediaManager.Current.Position.TotalSeconds / CrossMediaManager.Current.Duration.TotalSeconds;

                if (posval > 0 || posval < 1)
                {

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MainSlider.Value = posval;
                        ElapsedLabel.Text = CrossMediaManager.Current.Position.ToString().Substring(4, 4);
                    });

                }
            }
            catch(Exception w)
            {

            }
            
            
        }

        private async void NavigatePage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(QueueData.Player);
        }
    }
}