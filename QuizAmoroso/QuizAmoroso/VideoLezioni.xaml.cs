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
            urlVideo = Costanti.urlBase + urlVideo;
            videoView.Source = VideoSource.FromUri(urlVideo);

        }
    }
}