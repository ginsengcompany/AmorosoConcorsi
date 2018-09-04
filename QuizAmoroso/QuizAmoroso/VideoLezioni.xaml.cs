using FormsVideoLibrary;
using QuizAmoroso.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoLezioni : ContentPage
    {
        public VideoLezioni(string urlVideo)
        {
            InitializeComponent();
           var video = urlVideo.Replace(" ", "%20");
            urlVideo = Costanti.urlBase + video;
            videoView.Source = VideoSource.FromUri(urlVideo); 
            //videoView.Source = VideoSource.FromUri("https://amorosoconcorsi.ak12srl.it/services/video/2%20Mcd%20Mcm%20Insiemi%20Numerici-1.mp4");
        }
    }
}