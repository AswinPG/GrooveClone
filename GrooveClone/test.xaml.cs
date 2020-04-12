using GrooveClone.Interfaces;
using GrooveClone.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
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
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new test());
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            List<string> mylist = (List<string>)DependencyService.Get<IMyfile>().GetFileLocation();
            try
            {
                string artists = "";
                
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

                        Artist = artists,
                        Duration = tfile.Properties.Duration.ToString().Substring(4, 4),
                        Img = tfile.Tag.Pictures[0].Data.ToArray(),
                        Title = tfile.Tag.Title,
                        Path = mylist[i]
                    };
                    songs.Add(song);
                }
                MainCollectionView.ItemsSource = songs;
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
                await Navigation.PushAsync(new PlayerPage(song));
            }

            MainCollectionView.SelectedItem = null;

        }
    }
}