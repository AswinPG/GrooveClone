using GrooveClone.Interfaces;
using GrooveClone.Models;
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

        private void SwipeItem_Invoked(object sender, EventArgs e)
        {
            Song x =(Song)((SwipeItem)sender).CommandParameter;
        }
    }
}