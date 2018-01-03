using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;
using QuizAmoroso.DataModel;

/*
 * @Author: Alessio Calabrese
 * 
 * Questa classe effettua la connessione al database inviando l'username dell'utente e
 * restituendo i piani formativi associati a quest'ultimo. Successivamente è possibile 
 * cliccare su di essi tramite listview. Facendo questo il piano formativo verrà inviato
 * al server e si aprirà la pagina setDomande
 * */



namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PianoFormativo : ContentPage
    {
        // Variabile per la risposta del metodo invioDatiSelezionati()
        public static string risultatoSetDiDomandeAssociateAlPiano = "";
        // Inizializzazione dell'oggetto della classe StrutturaJsonPianoFormativo
        public static DatiPianoFormativo pianoFormativoSelezionato = new DatiPianoFormativo();
        // Variabile per la risposta del metodo ConnessioniConcorso()
        private string risultatoChiamataPianoFormativo = "";
        // Variabile per controllare la connessione nei metodi invioDatiSelezionati() e ConnessioniConcorso()
        private bool  flagConnessionePiano = false;
        private bool flag = false;



        public PianoFormativo()
        {
            InitializeComponent();
            LinkSitoWebAk12();
        }

        /*
         * Questo metodo invia l'username dell'utente e vengono restituiti in formato json i piani formativi a lui associati
         * */
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                DisabilitaLayoutActivityIndicator.IsVisible = true;
               
                caricamentoPagina.IsRunning = true;
                caricamentoPagina.IsVisible = true;
                await ConnessioneConcorsi();
                caricamentoPagina.IsVisible = false;
                caricamentoPagina.IsRunning = false;
                DisabilitaLayoutActivityIndicator.IsVisible = false;
                lstConcorsi.IsVisible = true;
                lstConcorsi.IsEnabled = true;
            } catch (Exception errore)
            {
                await DisplayAlert("Errore", "Errore nel caricamento del set di domande!", "Ok");
                DisabilitaLayoutActivityIndicator.IsVisible = false;
                caricamentoPagina.IsVisible = false;
                caricamentoPagina.IsRunning = false;
                lstConcorsi.IsVisible = false;
                lstConcorsi.IsEnabled = false;
            }
        }

        public async Task ConnessioneConcorsi()
        {
            string username = Utente.Instance.getUserName;
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", username));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.pianoformativo, content);
                risultatoChiamataPianoFormativo = await result.Content.ReadAsStringAsync();
                if (risultatoChiamataPianoFormativo.ToString() == "Impossibile connettersi al servizio")
                {
                    flagConnessionePiano = true;
                    throw new Exception();
                }
                else
                {
                    flagConnessionePiano = false;
                    List<DatiPianoFormativo> items = JsonConvert.DeserializeObject<List<DatiPianoFormativo>>(risultatoChiamataPianoFormativo);
                    lstConcorsi.ItemsSource = items;
                }
            }
            catch (Exception e)
            {
                flagConnessionePiano = true;
                risultatoChiamataPianoFormativo = "Impossibile connettersi al servizio";
                await DisplayAlert("Errore", risultatoChiamataPianoFormativo.ToString(), "Ok");
                await Navigation.PopToRootAsync();
            }
        }

        /*
         * Questo metodo invia il piano formativo selezionato dall'utente  
         * */
        public async Task invioDatiSelezionati()
        {
            string nomeDelPiano = pianoFormativoSelezionato.nome_piano;
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("nome_piano", pianoFormativoSelezionato.nome_piano));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.setdomande, content);
                risultatoSetDiDomandeAssociateAlPiano = await result.Content.ReadAsStringAsync();
                if (risultatoSetDiDomandeAssociateAlPiano.ToString() == "Impossibile connettersi al servizio")
                {
                    flag = true;
                    throw new Exception();
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception e)
            {
                flag = true;
                risultatoSetDiDomandeAssociateAlPiano = "Impossibile connettersi al servizio";
                await DisplayAlert("Errore", risultatoSetDiDomandeAssociateAlPiano.ToString(), "Ok");
                
            }
        }

        /*
        * Questo metodo assegna alle proprietà dell'oggetto pianoFormativoSelezionato i valori del piano formativo scelto dalla listview
        * */
        private async void lstConcorsi_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (flagConnessionePiano != true)
            {
                lstConcorsi.IsEnabled = false;
                var elementoTappato = e.Item as DatiPianoFormativo;
                pianoFormativoSelezionato.Corpo = elementoTappato.Corpo;
                pianoFormativoSelezionato.nome_piano = elementoTappato.nome_piano;
                pianoFormativoSelezionato.id_concorso = elementoTappato.id_concorso;
                await invioDatiSelezionati();

                DisabilitaLayoutActivityIndicator.IsVisible = true;
                caricamentoPagina.IsRunning = true;
                caricamentoPagina.IsVisible = true;
                lstConcorsi.SelectedItem = Color.Blue;

                await Navigation.PushAsync(new SetDomande());
                DisabilitaLayoutActivityIndicator.IsVisible = false;
                caricamentoPagina.IsRunning = false;
                caricamentoPagina.IsVisible = false;
                lstConcorsi.IsEnabled = true;
            }
            else
            {
                await Navigation.PopToRootAsync();
            }
        }

        public void LinkSitoWebAk12()
        {
            var tapGestureLinkSito = new TapGestureRecognizer();
            tapGestureLinkSito.Tapped += (s, e) => {
                Device.OpenUri(new Uri(Costanti.sitoAK12));
            };
            logoFooter.GestureRecognizers.Add(tapGestureLinkSito);
        }
    }
}