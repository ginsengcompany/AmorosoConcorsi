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
	public partial class ListaMaterieVideoLezioni : ContentPage
	{
        List<MaterieVideo> listaMaterieVideo = new List<MaterieVideo>();


        public ListaMaterieVideoLezioni()
		{
			InitializeComponent ();
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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await client.GetAsync(Costanti.listaMaterieVideo);
                var resultcontent = await result.Content.ReadAsStringAsync();
                listaMaterieVideo = JsonConvert.DeserializeObject<List<MaterieVideo>>(resultcontent);
                listaMaterie.ItemsSource = listaMaterieVideo;
            }
            catch
            {
                await DisplayAlert("Errore", "errore nel prelievo dei dati!", "OK");
            }
        }

        private async void listaMaterie_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var elementoTappato = e.Item as MaterieVideo;
            await Navigation.PushAsync(new GalleriaVideo(elementoTappato.Materia));
        }
    }
}