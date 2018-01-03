using Newtonsoft.Json;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NuovaModalitaQuizVeloceNew : ContentPage
    {
        public int contEsatteTot, contSbagliateTot, contNonRisposteTot = 0;
        public List<Domande> struttura;
        public DateTime dateTime = DateTime.Now;
        public List<DatiStatistica> lstDatiStatistica = new List<DatiStatistica>();
        public List<DatiRisultati> lstdatirisultati = new List<DatiRisultati>();
        public DatiStatistica datiStatistica;
        public DatiRisultati datiRisultati;
        public Domande recordCampiDomandaRisposte = new Domande();
        public Stopwatch stopwatch = new Stopwatch();
        public Stopwatch stopwatchDue = new Stopwatch();
        private bool btn_Cliccato = false;
        private string modalitaSelezionata;
        public TimeSpan tempo;
        public TimeSpan tempoDomanda;
        //Contatore domanda rispetto al totoale del set
        private int numeroAttualeDomanda = 0;
        private int indiceAppoggio = 0;
        //numero totale del set di domande
        private int numeroTotaleDelSetDiDomande = 0;
        private string deltaTemporale;
        private string tempoTrascorso;
        public string TmpTotale;
        bool click = true;
        string resultcontent;
        bool flag, flagInvioDatiStatistiche = false;
        bool fermaAvviaTempoGlobale = true;
        string urlRisorsa = "";

        public NuovaModalitaQuizVeloceNew(string modalitàselezionata)
        {
            this.modalitaSelezionata = modalitàselezionata;
            InitializeComponent();
            IngressoPagina();
        }

        private void IngressoPagina()
        {
            if (QuizVeloce.materiaSelezionata != Costanti.eseguiTestSuInteroDb)
                LabelTitoloHeader.Text = "Speed Quiz: " + Costanti.eseguiTestSuInteroDb;
            else
                LabelTitoloHeader.Text = "Speed Quiz: " + QuizVeloce.materiaSelezionata;
            btn_ApriPDF.IsEnabled = false;
            btn_ApriPDF.IsVisible = false;
            immagine.IsVisible = false;
            lblDomanda.IsVisible = false;
            GrigliaDomande.IsVisible = false;
            lblTempo.IsVisible = false;
            StackFooter.IsVisible = false;
        }

        public async Task ConnessioneDomande()
        {
            var client = new HttpClient();
            var result = new HttpResponseMessage();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("id_concorso", QuizVeloce.idConcorsoSelezionato));
                values.Add(new KeyValuePair<string, string>("materia", QuizVeloce.materiaSelezionata));
                if (QuizVeloce.numeroDomandeQuizVeloceSelezionato != 0)
                {
                    values.Add(new KeyValuePair<string, string>("numerodomande", QuizVeloce.numeroDomandeQuizVeloceSelezionato.ToString()));
                    RisultatoSimulazione.conteggioDomandeSvoltePerSimulazione = QuizVeloce.numeroDomandeQuizVeloceSelezionato;
                }
                else
                {
                    values.Add(new KeyValuePair<string, string>("numerodomande", QuizVeloce.numeroDomandeMassimoDelTestQuizVeloce.ToString()));
                    RisultatoSimulazione.conteggioDomandeSvoltePerSimulazione = QuizVeloce.numeroDomandeMassimoDelTestQuizVeloce;
                }
                var content = new FormUrlEncodedContent(values);
                if (modalitaSelezionata == "Modalità Casuale" && QuizVeloce.idConcorsoSelezionato != Costanti.eseguiTestSuInteroDb)
                {
                    content = new FormUrlEncodedContent(values);
                    result = await client.PostAsync(Costanti.domconcorsorandomNew, content);
                }
                else if (modalitaSelezionata == "Modalità Casuale" && QuizVeloce.idConcorsoSelezionato == Costanti.eseguiTestSuInteroDb)
                {
                    content = new FormUrlEncodedContent(values);
                    result = await client.PostAsync(Costanti.domconcorsorandomtotaliNew, content);
                }
                else
                {
                    values.Add(new KeyValuePair<string, string>("domandainiziale", QuizVeloce.numeroselezionato.ToString()));
                    content = new FormUrlEncodedContent(values);
                    result = await client.PostAsync(Costanti.domconcorsosequenzaNew, content);
                }

                resultcontent = await result.Content.ReadAsStringAsync();

                if (resultcontent.ToString() == "errore nella get")
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    struttura = JsonConvert.DeserializeObject<List<Domande>>(resultcontent);
                    if (modalitaSelezionata == "Modalità Casuale")
                    {
                        struttura = ShuffleList.Shuffle<Domande>(struttura);
                    }
                    numeroTotaleDelSetDiDomande = struttura.Count;
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Errore", resultcontent.ToString(), "Ok");
            }
        }

        public async void DomandaSuccessiva()
        {
            if (struttura.Count > 0)
            {
                recordCampiDomandaRisposte = null;
                recordCampiDomandaRisposte = struttura[0];
                await CaricamentoImmagine(recordCampiDomandaRisposte.tipo, Costanti.urlBase, recordCampiDomandaRisposte.link);
                lblDomanda.Text = recordCampiDomandaRisposte.Domanda;
                Griglia(recordCampiDomandaRisposte);
                numeroAttualeDomanda++;
                indiceAppoggio++;
                if (numeroAttualeDomanda <= numeroTotaleDelSetDiDomande)
                {
                    ContatoreDomande.Text = "Domanda " + numeroAttualeDomanda.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
                    saltaDomanda.IsEnabled = true;
                    saltaDomanda.IsVisible = true;
                }
                else
                {
                    ContatoreDomande.Text = "Domanda " + numeroTotaleDelSetDiDomande.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
                }
                if (indiceAppoggio > numeroTotaleDelSetDiDomande)
                {
                    consegnaSimulazione.IsVisible = true;
                    consegnaSimulazione.IsEnabled = true;
                }
            }
            else
            {
                StopTempoTrascorsoGlobale();
                fermaAvviaTempoGlobale = false;
                GrigliaDomande.IsEnabled = false;
                consegnaSimulazione.IsEnabled = false;
                consegnaSimulazione.IsVisible = false;
                saltaDomanda.IsEnabled = false;
                saltaDomanda.IsVisible = false;
                await DisplayAlert("Complimenti!", "La sessione di esercitazione è terminata.", "Guarda il risultato!");
                await attesaInvioDatiStatistiche();
            }
        }

        private void saltaDomanda_Clicked(object sender, EventArgs e)
        {
            if (numeroAttualeDomanda > 0)
            {
                numeroAttualeDomanda = numeroAttualeDomanda - 1;
            }
            struttura.RemoveAt(0);
            struttura.Add(recordCampiDomandaRisposte);
            GrigliaDomande.Children.Clear();
            DomandaSuccessiva();
        }

        private async Task consegnaSimulazione_Clicked(object sender, EventArgs e)
        {
            if (struttura.Count > 0)
            {
                contNonRisposteTot = struttura.Count;
                foreach (var el in struttura)
                {
                    datiStatistica = new DatiStatistica();
                    datiRisultati = new DatiRisultati();
                    datiStatistica.rispostaEsattaYN = false;
                    datiStatistica.tempoRisposta = "00:00:00:00";
                    datiStatistica.codice = el.id_domanda;
                    datiStatistica.materia = el.Materia;
                    datiStatistica.sottocategoria = el.Sottocategoria;
                    datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                    datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                    datiStatistica.nomeSet = "null";
                    datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                    // Da modificare con non risposto per ora lascio Non Risposta
                    datiStatistica.risposta_utente = "Non Risposta";
                    datiRisultati.Domanda = el.Domanda;
                    datiRisultati.tuaRisposta = "Non Risposta";
                    // Per ora lascio Red potrei cambiare
                    datiRisultati.color = Color.Red;
                    datiRisultati.rispostaEsattaYN = "Non Risposta";
                    datiRisultati.risposta = el.Risposta;

                    if (lstDatiStatistica.Any(elem => elem.codice == datiStatistica.codice))
                    {
                        int i = lstDatiStatistica.FindIndex(elem => elem.codice.Equals(datiStatistica.codice));
                        lstDatiStatistica[i] = datiStatistica;
                    }
                    else
                    {
                        lstDatiStatistica.Add(datiStatistica);
                        lstdatirisultati.Add(datiRisultati);
                    }
                }
            }
            await attesaInvioDatiStatistiche();
        }

        /**
         * Il metodo seguente tramite la classe Device e il metodo Start timer avvia il cronometro, che verrà visualizzato tramite una label.
         */
        public void TempoTrascorsoGlobale()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                stopwatch.Start();
                tempo = stopwatch.Elapsed;
                lblTempo.Text = string.Format("{0:00}:{1:00}:{2:00}", tempo.Hours, tempo.Minutes, tempo.Seconds);
                TmpTotale = string.Format("{0:00}:{1:00}:{2:00}", tempo.Hours, tempo.Minutes, tempo.Seconds);
                return fermaAvviaTempoGlobale;
            });
        }

        /**
         * Il metodo seguente mette fine al tempo tramite . 
         */
        public void StopTempoTrascorsoGlobale()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                stopwatch.Stop();
                tempo = stopwatch.Elapsed;
                return fermaAvviaTempoGlobale;
            });
        }

        public void TempoStartDomanda()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                stopwatchDue.Start();
                return click;
            });
        }

        private void btn_ApriPDF_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(urlRisorsa));
        }

        private async Task CaricamentoImmagine(string tipo, string indBase, string link)
        {
            if (tipo == "img")
            {
                btn_ApriPDF.IsEnabled = false;
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

        public void TempoRestartDomanda()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                tempoDomanda = stopwatchDue.Elapsed;
                deltaTemporale = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", tempoDomanda.Hours, tempoDomanda.Minutes, tempoDomanda.Seconds, tempoDomanda.Milliseconds);
                tempoTrascorso = deltaTemporale;
                datiStatistica.tempoRisposta = deltaTemporale;
                //lstDatiStatistica.Add(datiStatistica);
                //lstdatirisultati.Add(datiRisultati);
                if (lstDatiStatistica.Any(el => el.codice == datiStatistica.codice))
                {
                    int i = lstDatiStatistica.FindIndex(el => el.codice.Equals(datiStatistica.codice));
                    lstDatiStatistica[i] = datiStatistica;
                }
                else
                {
                    lstDatiStatistica.Add(datiStatistica);
                    lstdatirisultati.Add(datiRisultati);
                }
                stopwatchDue.Restart();
                return click;
            });
        }

        public async Task attesaInvioDatiStatistiche()
        {
            await InvioDatiStatistiche();
            //var page = Navigation.NavigationStack.ElementAtOrDefault(Navigation.NavigationStack.Count - 1);
            bool flagPunteggio = true;
            await Navigation.PushAsync(new RisultatoSimulazione(contSbagliateTot, contEsatteTot, TmpTotale, lstdatirisultati, flagPunteggio, contNonRisposteTot));
            //Navigation.RemovePage(page);
        }

        public async Task InvioDatiStatistiche()
        {
            try
            {
                string output = JsonConvert.SerializeObject(lstDatiStatistica, Formatting.Indented);
                StringContent stringContent = new StringContent(output, UnicodeEncoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                string rotta = "";

                if (QuizVeloce.idConcorsoSelezionato == Costanti.eseguiTestSuInteroDb)
                {
                    rotta = Costanti.sessionePerTuttiConcorsi;
                }
                else
                {
                    rotta = Costanti.sessione;
                }
                HttpResponseMessage response = await client.PostAsync(rotta, stringContent);
                string resultContent = await response.Content.ReadAsStringAsync();
                flagInvioDatiStatistiche = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void Griglia(Domande recordCampiDomandaRisposte)
        {
            GrigliaDomande.Children.Clear();
            GrigliaDomande.RowDefinitions = new RowDefinitionCollection();
            GrigliaDomande.ColumnDefinitions = new ColumnDefinitionCollection();
            var listaBottoni = new List<Button>();
            var listaLabel = new List<Label>();
            var lista = new List<string>();

            GrigliaDomande.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.2, GridUnitType.Star)
            });

            GrigliaDomande.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0.8, GridUnitType.Star)
            });

            int i = 0;
            foreach (var el in recordCampiDomandaRisposte.Quesiti)
            {
                lista.Add(el);

                GrigliaDomande.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Auto)
                });
                var btn = Addbutton(Costanti.alfabeto[i].ToString());

                var lbl = AddLabel();

                listaBottoni.Add(btn);
                listaLabel.Add(lbl);

                listaLabel[i].Text = lista[i];
                GrigliaDomande.Children.Add(listaBottoni[i], 0, i);
                GrigliaDomande.Children.Add(listaLabel[i], 1, i);
                i++;
            }
        }

        public Button Addbutton(string letBtn)
        {
            Button newButton = new Button();
            newButton.Text = letBtn;
            newButton.BackgroundColor = Color.FromHex("0069c0");
            newButton.TextColor = Color.White;
            GrigliaDomande.Children.Add(newButton);
            newButton.Clicked += NewButton_Clicked;
            return newButton;
        }

        private void NewButton_Clicked(object sender, EventArgs e)
        {
            btn_Cliccato = true;
            if (btn_Cliccato == true)
            {
                click = false;
            }

            TempoRestartDomanda();
            string lettera = "A";
            int cont = 0;
            Dictionary<string, string> lista = new Dictionary<string, string>();
            foreach (var el in recordCampiDomandaRisposte.Quesiti)
            {
                lista.Add(lettera, el);
                cont++;
                lettera = Costanti.alfabeto[cont].ToString();
            }

            var lblBtn = sender as Button;
            if (recordCampiDomandaRisposte.Risposta.ToString() == lblBtn.Text)
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = true;
                contEsatteTot++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = lista[lblBtn.Text];
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = lista[lblBtn.Text];
                datiRisultati.color = Color.Green;
                datiRisultati.rispostaEsattaYN = "esatta";

                datiRisultati.risposta = lista[recordCampiDomandaRisposte.Risposta];
                struttura.RemoveAt(0);
                GrigliaDomande.Children.Clear();
                DomandaSuccessiva();
            }
            else
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = false;
                contSbagliateTot++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = lista[lblBtn.Text];
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = lista[lblBtn.Text];
                datiRisultati.color = Color.Red;
                datiRisultati.rispostaEsattaYN = "errata";

                datiRisultati.risposta = lista[recordCampiDomandaRisposte.Risposta];
                struttura.RemoveAt(0);
                GrigliaDomande.Children.Clear();
                DomandaSuccessiva();
            }
        }

        public Label AddLabel()
        {
            Label newLabel = new Label();
            newLabel.BackgroundColor = Color.White;
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    newLabel.TextColor = Color.Black;
                    break;
            }
            return newLabel;
        }

        private async void avvioQuiz_Clicked(object sender, EventArgs e)
        {
            try
            {
                await ConnessioneDomande();
                RelativeBottoneAvvio.IsVisible = false;
                StackFooter.IsVisible = true;
            }
            catch (Exception err)
            {
                await DisplayAlert("errore", err.Message, "KO!");
            }
            frameDomanda.IsVisible = true;
            lblDomanda.IsVisible = true;
            GrigliaDomande.IsVisible = true;
            lblTempo.IsVisible = true;
            avvioQuiz.IsVisible = false;
            TempoStartDomanda();
            TempoTrascorsoGlobale();
            click = false;
            DomandaSuccessiva();
        }

        /**
         * Questa funzione va eliminata qualora si volesse tornare alla vecchia navigation bar e va a
         * eliminato anche il corrispondente frame ed immagine del button nello xaml
         **/
        private async Task TornaAlleModalita_Clicked(object sender, EventArgs e)
        {
            CaricamentoPaginaQuizVeloceCasuale.IsRunning = true;
            CaricamentoPaginaQuizVeloceCasuale.IsVisible = true;
            LayoutPrincipalePaginaQuizVeloceCasuale.IsVisible = false;
            await InvioDatiStatistiche();
            CaricamentoPaginaQuizVeloceCasuale.IsRunning = false;
            CaricamentoPaginaQuizVeloceCasuale.IsVisible = false;
            await Navigation.PushAsync(new MainPage());
        }

        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    if (flagInvioDatiStatistiche != true)
                        await InvioDatiStatistiche();
                    break;
            }
        }
    }
}