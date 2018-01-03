using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class Historyquiz : ContentPage
    {
        public static string risultatoRispostaCronologia;
        public static List<StrutturaHistory> lista = new List<StrutturaHistory>();
        public static List<Sessioni> SessionedataSelezionata = new List<Sessioni>();
        
        public Historyquiz()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await ConnessioneCronologia();
            VisualizzaDate();
        }

        public async Task ConnessioneCronologia()
        {
            var client = new HttpClient();
            StackLabelCronologiaVuota.IsVisible = false;
            LabelCronologia.IsVisible = false;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", Utente.Instance.getUserName));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.cronologia, content);
                risultatoRispostaCronologia = await result.Content.ReadAsStringAsync();
                if(risultatoRispostaCronologia.Contains("connessione non riuscita"))
                {
                    throw new Exception();
                }
                var isValid = JToken.Parse(risultatoRispostaCronologia);
                JArray jObject = JArray.Parse(risultatoRispostaCronologia);
                lista = JsonConvert.DeserializeObject<List<StrutturaHistory>>(risultatoRispostaCronologia);
                lista.Reverse();
            }
            catch (Exception)
            {
                if (risultatoRispostaCronologia == "null")
                {
                    StackLabelCronologiaVuota.IsVisible = true;
                    LabelCronologia.IsVisible = true;
                    LabelCronologia.Text = "cronologia vuota";
                }
                else
                    await DisplayAlert("Attenzione", "Errore caricamento cronologia", "riprova");
            }
        }

        public async void VisualizzaDate()
        {
            lstCronologiaDate.ItemsSource = lista;
        }

        private async void lstCronologiaDate_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var  elementoTappato = e.Item as StrutturaHistory;
            SessionedataSelezionata = elementoTappato.sessioni;
            lstCronologiaDate.SelectedItem = Color.Blue;
            await Navigation.PushAsync(new HistorySessione());
        }
    }
}