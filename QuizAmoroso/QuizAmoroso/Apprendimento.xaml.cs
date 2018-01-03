using Newtonsoft.Json;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /// <summary>
    /// Questa classe gestisce le domande da presentare all'utente in modalità apprendimento
    /// </summary>
    /// 
    public partial class Apprendimento : ContentPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="struttura"> Lista per salvare i dati del json deserializzato prelevato in remoto </param>
        /// <param name="listaDomandeApprendimento"> Lista di appoggio per salvare le domande già visualizzate </param>
        /// <param name="recordCampiDomandaRisposte"> Oggetto per accedere ai campi del json deserializzato </param>
        /// <param name="timer"> Oggetto della classe timer per la misuradei tempi totali in modalità apprendimento </param>

        public List<Domande> struttura;
        public List<Domande> listaDomandeApprendimento = new List<Domande>();
        public Domande recordCampiDomandaRisposte = new Domande();
        public Timer timer = new Timer();

        /// <summary>
        /// Costruttore della modalità apprendimento
        /// </summary>
        public Apprendimento()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Questo metodo viene eseguito al lancio dell'applicazione e prevede la connessione al sistema
        /// per il prelievo delle domande ed il riempimento della lista su che visualizza le domande all'utente
        /// </summary>
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            // Attivo l'activity indicator
            caricamentoPaginaApprendimentoNew.IsRunning = true;
            caricamentoPaginaApprendimentoNew.IsVisible = true;
            // Mi connetto al sistema per il prelievo delle domande e delle risposte da presentare all'utente
            await ConnessioneDomande();
            // Avvenuto il caricamento disattivo 'activity indicator
            caricamentoPaginaApprendimentoNew.IsRunning = false;
            caricamentoPaginaApprendimentoNew.IsVisible = false;
            // Associo alla lista le domande prelevate ed immagazzinate nella variabile Lista struttura
            lstApprendimento.ItemsSource = struttura;
            timer.TempoApprendimento(true);
        }

        public async Task ConnessioneDomande()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("nome_set", SetDomande.setDomandeSelezionato.nome_set));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.domande, content);
                var resultcontent = await result.Content.ReadAsStringAsync();

                if (resultcontent.ToString() == "Impossibile connettersi al servizio")
                {
                    throw new Exception();
                }
                else
                {
                    struttura = JsonConvert.DeserializeObject<List<Domande>>(resultcontent);
                    for(int i=0; i<struttura.Count;i++)
                    { 
                        switch(struttura[i].Risposta)
                        {
                            case "A":
                                struttura[i].colorA = Color.Green;
                                struttura[i].GrassettoA = FontAttributes.Bold;
                                struttura[i].colorB = Color.Black;
                                struttura[i].colorC = Color.Black;
                                struttura[i].colorD = Color.Black;
                                break;
                            case "B":
                                struttura[i].colorB = Color.Green;
                                struttura[i].GrassettoB = FontAttributes.Bold;
                                struttura[i].colorA = Color.Black;
                                struttura[i].colorC = Color.Black;
                                struttura[i].colorD = Color.Black;
                                break;
                            case "C":
                                struttura[i].colorC = Color.Green;
                                struttura[i].GrassettoC = FontAttributes.Bold;
                                struttura[i].colorA = Color.Black;
                                struttura[i].colorB = Color.Black;
                                struttura[i].colorD = Color.Black;
                                break;
                            case "D":
                                struttura[i].colorD = Color.Green;
                                struttura[i].GrassettoD = FontAttributes.Bold;
                                struttura[i].colorA = Color.Black;
                                struttura[i].colorB = Color.Black;
                                struttura[i].colorC = Color.Black;
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var resultcontent = "Impossibile connettersi al servizio";
                await DisplayAlert("Errore", resultcontent.ToString(), "Ok");
                await Navigation.PopToRootAsync();
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            timer.FermaTempoApprendimento();
            await InvioTempoTotale();
        }

        public async Task InvioTempoTotale()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", Utente.Instance.getUserName));
                values.Add(new KeyValuePair<string, string>("tempoSimulazione", timer.tempoTotaleSimulazione));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.invioTempiGlobali, content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}