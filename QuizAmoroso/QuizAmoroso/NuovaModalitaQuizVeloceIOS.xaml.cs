using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NuovaModalitàQuizVeloceIOS : ContentPage
    {
        public int contEsatteA, contEsatteB, contEsatteC, contEsatteD, contEsatteTot, contSbagliateA, contSbagliateB, contSbagliateC, contSbagliateD, contSbagliateTot, contNonRisposteTot = 0;
        private static Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        public List<Domande> struttura;
        public DateTime dateTime = DateTime.Now;
        public List<DatiStatistica> lstDatiStatistica = new List<DatiStatistica>();
        public List<DatiRisultati> lstdatirisultati = new List<DatiRisultati>();
        public DatiStatistica datiStatistica;
        public DatiRisultati datiRisultati;
        public Domande recordCampiDomandaRisposte = new Domande();
        public Stopwatch stopwatch = new Stopwatch();
        public Stopwatch stopwatchDue = new Stopwatch();
        private bool btnA_Cliccato = false;
        private bool btnB_Cliccato = false;
        private bool btnC_Cliccato = false;
        private bool btnD_Cliccato = false;
        private string modalitàSelezionata;
        public TimeSpan tempo;
        public TimeSpan tempoDomanda;
        private int index = 0;
        //Contatore domanda rispetto al totoale del set
        private int numeroAttualeDomanda = 0;
        private int indiceAppoggio = 0;
        //numero totale del set di domande
        private int numeroTotaleDelSetDiDomande = 0;
        private string deltaTemporale;
        private string tempoTrascorso;
        public string TmpTotale;
        public int indiceIntervalliTemporali = 0;
        bool click = true;
        string resultcontent;
        bool flag = false;
        bool fermaAvviaTempoGlobale = true;
        string urlRisorsa = "";

        public NuovaModalitàQuizVeloceIOS(string modalitàselezionata)
        {
            this.modalitàSelezionata = modalitàselezionata;
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
            btnA.IsVisible = false;
            btnB.IsVisible = false;
            btnC.IsVisible = false;
            btnD.IsVisible = false;
            lblA.IsVisible = false;
            lblB.IsVisible = false;
            lblC.IsVisible = false;
            lblD.IsVisible = false;
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
                if (modalitàSelezionata == "Modalità Casuale" && QuizVeloce.idConcorsoSelezionato != Costanti.eseguiTestSuInteroDb)
                {
                    content = new FormUrlEncodedContent(values);
                    result = await client.PostAsync(Costanti.domconcorsorandom, content);
                }
                else if (modalitàSelezionata == "Modalità Casuale" && QuizVeloce.idConcorsoSelezionato == Costanti.eseguiTestSuInteroDb)
                {
                    content = new FormUrlEncodedContent(values);
                    result = await client.PostAsync(Costanti.domconcorsorandomtotali, content);
                }
                else
                {
                    values.Add(new KeyValuePair<string, string>("domandainiziale", QuizVeloce.numeroselezionato.ToString()));
                    content = new FormUrlEncodedContent(values);
                    result = await client.PostAsync(Costanti.domconcorsosequenza, content);
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
                    if (modalitàSelezionata == "Modalità Casuale")
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
                recordCampiDomandaRisposte = struttura[0];
                await CaricamentoImmagine(recordCampiDomandaRisposte.tipo, Costanti.urlBase, recordCampiDomandaRisposte.link);
                lblDomanda.Text = recordCampiDomandaRisposte.Domanda;
                btnA.Text = recordCampiDomandaRisposte.A;
                btnB.Text = recordCampiDomandaRisposte.B;
                btnC.Text = recordCampiDomandaRisposte.C;
                btnD.Text = recordCampiDomandaRisposte.D;
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
                fermaAvviaTempoGlobale = false;
                StopTempoTrascorsoGlobale();
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
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
                    datiStatistica.codice = el.Codice;
                    datiStatistica.materia = el.Materia;
                    datiStatistica.sottocategoria = el.Sottocategoria;
                    datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                    datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                    datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                    datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
                    // DA modificare con non risposto per ora lascio la A
                    datiStatistica.risposta_utente = "Non Risposta";
                    datiRisultati.Domanda = el.Domanda;
                    datiRisultati.tuaRisposta = "Non Risposta";
                    // Per ora lascio Red potrei cambiare
                    datiRisultati.color = Color.Red;
                    datiRisultati.rispostaEsattaYN = "Non Risposta";
                    switch (el.Risposta)
                    {
                        case "A":
                            datiRisultati.risposta = el.A;
                            break;
                        case "B":
                            datiRisultati.risposta = el.B;
                            break;
                        case "C":
                            datiRisultati.risposta = el.C;
                            break;
                        case "D":
                            datiRisultati.risposta = el.D;
                            break;
                    }
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
            contEsatteTot = contEsatteA + contEsatteB + contEsatteC + contEsatteD;
            contSbagliateTot = contSbagliateA + contSbagliateB + contSbagliateC + contSbagliateD;
            var page = Navigation.NavigationStack.ElementAtOrDefault(Navigation.NavigationStack.Count - 1);
            bool flagPunteggio = true;
            await Navigation.PushAsync(new RisultatoSimulazione(contSbagliateTot, contEsatteTot, TmpTotale, lstdatirisultati, flagPunteggio, contNonRisposteTot));
            Navigation.RemovePage(page);
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
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

        private void btnA_Clicked(object sender, EventArgs e)
        {
            btnA_Cliccato = true;
            if (btnA_Cliccato == true)
            {
                click = false;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "B" || recordCampiDomandaRisposte.Risposta.ToString() == "C" || recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = false;
                contSbagliateA++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.A;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.A;
                datiRisultati.color = Color.Red;
                datiRisultati.rispostaEsattaYN = "errata";
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "A")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = true;
                contEsatteA++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.A;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.A;
                datiRisultati.color = Color.Green;
                datiRisultati.rispostaEsattaYN = "esatta";
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            indiceIntervalliTemporali++;
        }

        private void btnB_Clicked(object sender, EventArgs e)
        {
            btnB_Cliccato = true;
            if (btnB_Cliccato == true)
            {
                click = false;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "A" || recordCampiDomandaRisposte.Risposta.ToString() == "C" || recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = false;
                contSbagliateB++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.B;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.B;
                datiRisultati.rispostaEsattaYN = "errata";
                datiRisultati.color = Color.Red;
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "B")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = true;
                contEsatteB++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.B;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.B;
                datiRisultati.rispostaEsattaYN = "esatta";
                datiRisultati.color = Color.Green;
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            indiceIntervalliTemporali++;
        }

        private void btnC_Clicked(object sender, EventArgs e)
        {
            btnC_Cliccato = true;
            if (btnC_Cliccato == true)
            {
                click = false;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "A" || recordCampiDomandaRisposte.Risposta.ToString() == "B" || recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = false;
                contSbagliateC++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.C;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.C;
                datiRisultati.color = Color.Red;
                datiRisultati.rispostaEsattaYN = "errata";
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "C")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = true;
                contEsatteC++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.C;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.C;
                datiRisultati.rispostaEsattaYN = "esatta";
                datiRisultati.color = Color.Green;
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            indiceIntervalliTemporali++;
        }

        private void btnD_Clicked(object sender, EventArgs e)
        {
            btnD_Cliccato = true;
            if (btnD_Cliccato == true)
            {
                click = false;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "A" || recordCampiDomandaRisposte.Risposta.ToString() == "B" || recordCampiDomandaRisposte.Risposta.ToString() == "C")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = false;
                contSbagliateD++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.D;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.D;
                datiRisultati.rispostaEsattaYN = "errata";
                datiRisultati.color = Color.Red;
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datiStatistica = new DatiStatistica();
                datiRisultati = new DatiRisultati();
                datiStatistica.rispostaEsattaYN = true;
                contEsatteD++;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = "null";
                datiStatistica.id_concorso = QuizVeloce.idConcorsoSelezionato;
                datiStatistica.risposta_utente = recordCampiDomandaRisposte.D;
                datiRisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datiRisultati.tuaRisposta = recordCampiDomandaRisposte.D;
                datiRisultati.rispostaEsattaYN = "esatta";
                datiRisultati.color = Color.Green;
                struttura.RemoveAt(0);
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datiRisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datiRisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datiRisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datiRisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                DomandaSuccessiva();
            }
            indiceIntervalliTemporali++;
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
            btnA.IsVisible = true;
            btnB.IsVisible = true;
            btnC.IsVisible = true;
            btnD.IsVisible = true;
            lblA.IsVisible = true;
            lblB.IsVisible = true;
            lblC.IsVisible = true;
            lblD.IsVisible = true;
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
                    await InvioDatiStatistiche();
                    break;
            }
        }
    }
}