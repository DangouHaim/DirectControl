using BLL;
using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DirectControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            IScreenShareService client = new ScreenShareService(new NetworkService(6789));
            client.OnFrame += (s, e) =>
            {
                try
                {
                    Stream stream = new MemoryStream(e.Capture);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Image.Source = ImageSource.FromStream(() => stream);
                    });
                }
                catch { }
            };

            client.StartReceive();
        }
    }
}