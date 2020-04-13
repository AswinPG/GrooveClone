using GrooveClone.Models;
using MediaManager;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GrooveClone
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //CrossMediaManager.Current.Init();
            QueueData.Init();
            MainPage = new NavigationPage(new MainPage());

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
