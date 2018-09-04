using Newtonsoft.Json;
using QuizAmoroso.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GalleriaVideo : ContentPage
    {

        List<VideoLezioniNuovo> listaVideoLezioni = new List<VideoLezioniNuovo>();
        string materia;
        public GalleriaVideo(string materiaVideo)
        {
            InitializeComponent();
            materia = materiaVideo;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ConnessioneMaterie();
        }
        protected override bool OnBackButtonPressed()
        {
            return false;
        }
        public async Task ConnessioneMaterie()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("materia", materia));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.ListaLezioniVideo, content);
                var resultcontent = await result.Content.ReadAsStringAsync();
                listaVideoLezioni = JsonConvert.DeserializeObject<List<VideoLezioniNuovo>>(resultcontent);
                ListaVideo.ItemsSource = listaVideoLezioni;
            }
            catch
            {
                await DisplayAlert("Errore", "errore nel prelievo dei dati!", "OK");
            }
        }

        private async void ListaVideo_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var elementoTappato = e.Item as VideoLezioniNuovo;
            await Navigation.PushAsync(new VideoLezioni(elementoTappato.VideoSource));

        }
    }
    public class VideoLezioniNuovo
    {
        public string Nome { set; get; }
        public string VideoSource { set; get; }
        public string Descrizione { set; get; }
        public string sottoCategoria { set; get; }
        public string Materia { set; get; }
    }
}