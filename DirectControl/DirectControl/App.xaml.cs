using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DirectControl
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            MainPage = new MainPage();
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
