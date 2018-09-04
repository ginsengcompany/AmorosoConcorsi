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

        List<VideoLezioni> listaVideoLezioni = new List<VideoLezioni>();
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
        public async Task ConnessioneMaterie()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("Materia", materia));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.ListaLezioniVideo, content);
                var resultcontent = await result.Content.ReadAsStringAsync();
                listaVideoLezioni = JsonConvert.DeserializeObject<List<VideoLezioni>>(resultcontent);
                ListaVideo.ItemsSource = listaVideoLezioni;
            }
            catch
            {
                await DisplayAlert("Errore", "errore nel prelievo dei dati!", "OK");
            }
        }
    }
}