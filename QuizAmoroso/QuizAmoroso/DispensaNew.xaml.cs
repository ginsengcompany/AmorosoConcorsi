using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;

/**
 *@Authors: Fiume Antonio Fabrizio, Alessio Calabrese, Antonio  Saverio Valente
 * 
 * Questa page serve a implementare la modalità dispensa. Codesta modalità avrà come scopo quello
 * di mostrare a video solo la risposta esatta, le quali saranno colorate di verde, gli altri button
 * non saranno abilitati e nemmeno visibili.
 */
namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DispensaNew : ContentPage
    {
        // Instanziamo un nuovo oggetto di tipo list di nome struttura, che sarà riempito con le domande e risposte arrivate dal server
        public List<Domande> struttura;
        //Instanziamo un nuovo oggetto di tipo list di nome strutturaAppoggio, che ci servirà da contenitore per muoverci all'indietro tra le domande
        public List<Domande> strutturaAppoggio = new List<Domande>();
        //Instanziamo un nuovo oggetto di tipo StrutturaJson di nome recordCampiDomandaRisposte
        public Domande recordCampiDomandaRisposte = new Domande();
        //Instanziamo un nuovo oggetto di tipo StopWatch di nome stopwatch, che ci servirà per stoppare il timer
        public Stopwatch stopwatch = new Stopwatch();
        //Instanziamo una nuova variabile di tipo char di nome risposte esatte, che riempiremo con  le risposte esatte
        char[] risposteEsatte;
        //Instanziamo un nuovo oggetto di tipo Random di nome rnd, che utilizzeremo per far visualizzare le domande in maniera casuale;
        private static Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        //inizializziamo l'indice della struttura a zero;
        private int indiceAccessoStruttura = 0;
        //Contatore domanda rispetto al totoale del set
        private int numeroAttualeDomanda = 0;
        //inizializziamo l'indice che utilizzeremo per andare indietro tra le domande a zero;
        private int indiceBack;
        //inizializziamo l'indice per andare avanti tra le domande a zero;
        private int indiceNext = 0;
        //inizializziamo l'indice della struttura d'appoggio a zero;
        private int indiceStrutturaAppoggio = 0;
        //numero totale del set di domande
        private int numeroTotaleDelSetDiDomande = 0;
        //dichiariamo una stringa result content che ci restituirà il risultato della risposta del server;
        string resultcontent;
        //dichiariamo una variabile di tipo string, chiamandola nomeset e la inizializziamo uguale al nome set che ci verrà inviato dal server
        public string nomeset = SetDomande.setDomandeSelezionato.nome_set;
        //dichiariamo una variabile booleana di nome flag che useremo a true quando la connesione andrà a buon fine.
        bool flag = false;
        string urlRisorsa = "";
        public Timer timer = new Timer();

        /**
         * COSTRUTTORE
         * Inizializza i componenti(Label,button ecc.),
         * Inizializza il metodo ingresso pagina();
         */
        public DispensaNew()
        {
            InitializeComponent();
        }

        /**
         * il metodo ingresso pagina permette all'utente che accederà alle dispense 
         * di visualizzare un' activity indicator mentre ci sarà la connessione al server, quando la connessione
         * sarà avvenuta con successo allora tutti i button e i frame si visualizzeranno a video.
         */
        protected async override void OnAppearing()
        {
            LabelTitoloHeader.Text = "Dispensa: " + nomeset;
            caricamentoPagina.IsRunning = true;
            caricamentoPagina.IsVisible = true;
            FrameDomandaDispensa.IsVisible = false;
            btn.IsVisible = false;
            btnIndietro.IsVisible = false;
            btnAvanti.IsVisible = false;
            await ConnessioneDomande();
            caricamentoPagina.IsRunning = false;
            caricamentoPagina.IsVisible = false;
            stackCaricamentoPagina.IsVisible = false;
            FrameDomandaDispensa.IsVisible = true;
            btnIndietro.IsVisible = false;
            btnAvanti.IsVisible = true;
            timer.TempoDispensa(true);
        }

        /**
         * Il metodo ConnessioneDomande servirà a creare la connessione al server, 
         * e esso dovrà restituire le domande e le quattro risposte
         **/
        public async Task ConnessioneDomande()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("nome_set", nomeset));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.domandeNew, content);
                resultcontent = await result.Content.ReadAsStringAsync();

                if (resultcontent.ToString() == "Impossibile connettersi al servizio")
                {
                    throw new Exception();
                }
                else
                {
                    flag = false;
                    struttura = JsonConvert.DeserializeObject<List<Domande>>(resultcontent);
                    risposteEsatte = new char[struttura.Count + 1];
                    numeroTotaleDelSetDiDomande = struttura.Count;
                    DomandaSuccessiva();
                }
            }
            catch (Exception e)
            {
                resultcontent = "Impossibile connettersi al servizio";
                await DisplayAlert("Errore", resultcontent.ToString(), "Ok");
                await Navigation.PopToRootAsync();
            }
        }

        /**
         * Il metodo domanda successiva, ogni qualvolta richiamato, 
         * permette di visualizzare a video una domanda, presa in maniera casuale, 
         * dal server.
         **/
        public void DomandaSuccessiva()
        {
            if (indiceStrutturaAppoggio == 0)
            {
                btnIndietro.IsVisible = false;
                btnAvanti.IsVisible = true;
            }
            else
            {
                btnIndietro.IsVisible = true;
            }

            if (struttura.Count > 0)
            {
                indiceAccessoStruttura = rnd.Next(struttura.Count);
                recordCampiDomandaRisposte = struttura[indiceAccessoStruttura];
                visualizzaRispostaEsatta(recordCampiDomandaRisposte);
                strutturaAppoggio.Add(recordCampiDomandaRisposte);
                indiceBack = indiceStrutturaAppoggio;
                indiceStrutturaAppoggio++;
                struttura.RemoveAt(indiceAccessoStruttura);
                numeroAttualeDomanda++;
                ContatoreDomande.Text = "Domanda " + numeroAttualeDomanda.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
            }
            else
            {
                DisplayAlert("Complimenti!", "La sessione è terminata.", "Esci");
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
            }
        }

        /**
         * Il metoto in questione visualizzerà a video, colorando di colore verde, 
         * la risposta esatta
         **/
        private async void visualizzaRispostaEsatta(Domande prova)
        {
            lblDomanda.Text = prova.Domanda;
            await CaricamentoImmagine(prova.tipo, Costanti.urlBase, prova.link);
            btn.IsEnabled = true;
            btn.BackgroundColor = Color.FromHex("#0069c0");

            for (int i = 0; i <= Costanti.alfabeto.Length; i++)
            {
                if (Costanti.alfabeto[i].ToString() == prova.Risposta)
                {
                    btn.IsVisible = true;
                    btn.Text = prova.Quesiti[i];
                    btn.BackgroundColor = Color.Green;
                    break;
                }
            }
        }

        private async Task CaricamentoImmagine(string tipo, string indBase, string link)
        {
            if (tipo == "img")
            {
                btn_ApriPDF.IsVisible = false;
                immagine.IsVisible = true;
                urlRisorsa = Costanti.urlBase + link;
                var urlProva = new System.Uri(urlRisorsa);
                Task<ImageSource> result = Task<ImageSource>.Factory.StartNew(() => ImageSource.FromUri(urlProva));
                immagine.Source = await result;
            }
            else if (tipo == "pdf")
            {
                immagine.IsVisible = false;
                btn_ApriPDF.IsEnabled = true;
                btn_ApriPDF.IsVisible = true;
                urlRisorsa = Costanti.urlBase + link;
            }
            else
            {
                immagine.IsVisible = false;
                btn_ApriPDF.IsEnabled = false;
                btn_ApriPDF.IsVisible = false;
            }
        }

        /**
         * Il metodo in questione permette di ritornare alla page modalità quiz
         **/
        private async Task TornaAlleModalita_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /**
         * Il metodo in questione, ogni qualvolta richiamato, permette all'utente
         * di andare indietro per sfogliare le domande già visualizzate in precedenza
         **/
        private void btnIndietro_Clicked(object sender, EventArgs e)
        {
            if (btnAvanti.IsVisible == false)
            {
                btnAvanti.IsVisible = true;
            }
            if (indiceBack > 0)
            {
                recordCampiDomandaRisposte = strutturaAppoggio[indiceBack - 1];
                visualizzaRispostaEsatta(recordCampiDomandaRisposte);
                indiceBack--;
                if (indiceBack == 0)
                {
                    btnIndietro.IsVisible = false;
                }
                numeroAttualeDomanda = numeroAttualeDomanda - 1;
                ContatoreDomande.Text = "Domanda " + numeroAttualeDomanda.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
            }
            else
            {
                btnIndietro.IsVisible = false;
                numeroAttualeDomanda = numeroAttualeDomanda - 1;
                ContatoreDomande.Text = "Domanda " + numeroAttualeDomanda.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
            }
        }

        /**
         * Il metodo in questione, ogni qualvolta richiamato, permette ogni volta che sarà
         * cliccato il button avanti di sfogliare le domande successive
         **/
        private void btnAvanti_Clicked(object sender, EventArgs e)
        {
            indiceBack++;
            indiceNext = indiceStrutturaAppoggio - indiceBack;
            if (indiceNext >= 1)
            {
                recordCampiDomandaRisposte = strutturaAppoggio[indiceBack];
                visualizzaRispostaEsatta(recordCampiDomandaRisposte);
                numeroAttualeDomanda++;
                ContatoreDomande.Text = "Domanda " + numeroAttualeDomanda.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
            }
            else if (indiceNext < 1)
            {
                btn.IsEnabled = false;
                btn.BackgroundColor = Color.FromHex("#0069c0");
                DomandaSuccessiva();
            }
            if (btnIndietro.IsVisible == false)
            {
                btnIndietro.IsVisible = true;
            }
        }

        private void btnA_Clicked(object sender, EventArgs e)
        {

        }

        private void btnB_Clicked(object sender, EventArgs e)
        {

        }

        private void btnC_Clicked(object sender, EventArgs e)
        {

        }

        private void btnD_Clicked(object sender, EventArgs e)
        {

        }

        private void btn_ApriPDF_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(urlRisorsa));
            btnAvanti.IsVisible = true;
            btnIndietro.IsVisible = true;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            timer.FermaTempoDispensa();
            await InvioTempoTotale();
        }

        public async Task InvioTempoTotale()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", Utente.Instance.getUserName));
                values.Add(new KeyValuePair<string, string>("tempoDispensa", timer.tempoTotaleDispensa));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.invioTempiGlobali, content);
                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}