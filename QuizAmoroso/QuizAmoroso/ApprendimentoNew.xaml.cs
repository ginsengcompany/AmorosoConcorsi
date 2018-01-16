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
    public partial class ApprendimentoNew : ContentPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="struttura"> Lista per salvare i dati del json deserializzato prelevato in remoto </param>
        /// <param name="listaDomandeApprendimento"> Lista di appoggio per salvare le domande già visualizzate </param>
        /// <param name="recordCampiDomandaRisposte"> Oggetto per accedere ai campi del json deserializzato </param>
        /// <param name="timer"> Oggetto della classe timer per la misuradei tempi totali in modalità apprendimento </param>
        /// 

        public List<Domande> struttura;
        public List<Quesiti> quesiti = new List<Quesiti>();
        public List<Domande> listaDomandeApprendimento = new List<Domande>();
        public Domande recordCampiDomandaRisposte = new Domande();
        public Timer timer = new Timer();

        /// <summary>
        /// Costruttore della modalità apprendimento
        /// </summary>
        public ApprendimentoNew()
        {
            InitializeComponent();
        }
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
                values.Add(new KeyValuePair<string, string>("tempoApprendimento", timer.tempoTotaleApprendimento));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.invioTempiGlobali, content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task ConnessioneDomande()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("nome_set", SetDomande.setDomandeSelezionato.nome_set));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.domandeNew, content);
                var resultcontent = await result.Content.ReadAsStringAsync();

                if (resultcontent.ToString() == "Impossibile connettersi al servizio")
                {
                    throw new Exception();
                }
                else
                {
                    ListView appoggio = new ListView();
                    
                    struttura = JsonConvert.DeserializeObject<List<Domande>>(resultcontent);
                    for (int i = 0; i < struttura.Count; i++)
                    {
                        int y = 0;
                        int cont = 0;
                        struttura[i].lstQuesiti = new List<Quesiti>();
                        foreach (var el in struttura[i].Quesiti)
                        {
                            Quesiti temp = new Quesiti();
                            Color colore = new Color();
                            FontAttributes attribute = new FontAttributes();
                            string risp = Costanti.alfabeto[y].ToString();
                            if (risp.Equals(struttura[i].Risposta)){
                                colore = Color.Green;
                                attribute = FontAttributes.Bold;
                            }
                            else
                            {
                                colore = Color.Black;
                                attribute = FontAttributes.None;
                            }
                            temp.colore = colore;
                            temp.attribute = attribute;
                            temp.quesito = el;
                            temp.visible = "true";
                            temp.lettera = Costanti.alfabeto[y].ToString();
                            struttura[i].lstQuesiti.Add(temp);
                            y++;
                            cont++;
                        }
                        while(cont<=7)
                        {
                            Quesiti temp = new Quesiti();
                            Color colore = new Color();
                            FontAttributes attribute = new FontAttributes();
                            attribute = FontAttributes.None;

                            colore = Color.Black;
                            temp.colore = colore;
                            temp.attribute = attribute;
                            temp.quesito = " ";
                            
                            struttura[i].lstQuesiti.Add(temp);

                            cont++;
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
    }
}


