using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SimulazioneAssistita : ContentPage
    {
        private int contSA, contSB, contSC, contSD,contST;
        private int contEA, contEB, contEC, contED,contET;
        private int contNonRisposteTot = 0;
        private static Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        public List<Domande> struttura;
        public List<Domande> strutturaAppoggio = new List<Domande>();
        public DateTime dateTime = DateTime.Now;
        public List<DatiStatistica> lstDatiStatistica = new List<DatiStatistica>();
        public List<DatiRisultati> lstdatirisultati = new List<DatiRisultati>();
        public DatiRisultati datirisultati;
        public DatiStatistica datiStatistica;
        string urlRisorsa = "";
        public Domande recordCampiDomandaRisposte = new Domande();
        public Stopwatch stopwatch = new Stopwatch();
        public Stopwatch stopwatchDue = new Stopwatch();
        private bool btnA_Cliccato = false;
        private bool btnB_Cliccato = false;
        private bool btnC_Cliccato = false;
        public string nomeset = SetDomande.setDomandeSelezionato.nome_set;
        private bool btnD_Cliccato = false;
        public TimeSpan tempo;
        public TimeSpan tempoDomanda;
        private int index = 0;
        private int indiceStrutturaAppoggio = 0;
        //Contatore domanda rispetto al totoale del set
        private int numeroAttualeDomanda = 0;
        //numero totale del set di domande
        private int numeroTotaleDelSetDiDomande = 0;
        private string deltaTemporale;
        private string tempoTrascorso;
        private string[] arrayIntervalliTemporali;
        public int indiceIntervalliTemporali = 0;
        bool click = true;
        string resultcontent;
        bool flag = false;
        public Timer timer = new Timer();

        public SimulazioneAssistita()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            Title = "Simulazione Assistita: " + nomeset;
            LabelTitoloHeader.Text = "Simulazione Assistita: " + nomeset;
            CaricamentoPaginaSimulazioneAssistita.IsRunning = true;
            CaricamentoPaginaSimulazioneAssistita.IsVisible = true;
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
            btnAvanti.IsVisible = false;
            lblTempo.IsVisible = false;
            avvioQuiz.IsVisible = false;
            FooterContatoreDomande.IsVisible = false;
            await ConnessioneDomande();
            CaricamentoPaginaSimulazioneAssistita.IsRunning = false;
            CaricamentoPaginaSimulazioneAssistita.IsVisible = false;
            avvioQuiz.IsVisible = true;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            timer.FermaTempoSimulazioneAssistita();
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

        public async Task ConnessioneDomande()
        {
            var client = new HttpClient();
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("nome_set", nomeset));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.domande, content);
                resultcontent = await result.Content.ReadAsStringAsync();
                if (resultcontent.ToString() == "Impossibile connettersi al servizio")
                {
                    flag = true;
                    throw new Exception();
                }
                else
                {
                    flag = false;
                    struttura = JsonConvert.DeserializeObject<List<Domande>>(resultcontent);
                    arrayIntervalliTemporali = new string[struttura.Count + 1];
                    numeroTotaleDelSetDiDomande = struttura.Count;
                    RisultatoSimulazione.conteggioDomandeSvoltePerSimulazione = numeroTotaleDelSetDiDomande;
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

        public void DomandaSuccessiva()
        {
            if (struttura.Count > 0)
            {
                index = rnd.Next(struttura.Count);
                recordCampiDomandaRisposte = struttura[index];
                lblDomanda.Text = recordCampiDomandaRisposte.Domanda;
                lblA.Text = recordCampiDomandaRisposte.A;
                lblB.Text = recordCampiDomandaRisposte.B;
                lblC.Text = recordCampiDomandaRisposte.C;
                lblD.Text = recordCampiDomandaRisposte.D;
                strutturaAppoggio.Add(recordCampiDomandaRisposte);
                indiceStrutturaAppoggio++;
                struttura.RemoveAt(index);
                numeroAttualeDomanda++;
                ContatoreDomande.Text = "Domanda " + numeroAttualeDomanda.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
            }
            else
            {
                DisplayAlert("Complimenti!", "La sessione di simulazione è terminata.", "Esci");
                contST = contSA + contSB + contSC + contSD;
                contET = contEA + contEB + contEC + contED;

                //var page = Navigation.NavigationStack.ElementAtOrDefault(Navigation.NavigationStack.Count - 1);
                bool flagPunteggio = false;

                Navigation.PushAsync(new RisultatoSimulazione(contST, contET, lblTempo.Text, lstdatirisultati,flagPunteggio, contNonRisposteTot));
                //Navigation.RemovePage(page);
            }
            btnAvanti.IsVisible = false;
        }

        public void TempoTrascorsoGlobale()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                stopwatch.Start();
                tempo = stopwatch.Elapsed;
                lblTempo.Text = string.Format("{0:00}:{1:00}:{2:00}", tempo.Hours, tempo.Minutes, tempo.Seconds);
                return true;
            });

        }

        public void TempoStartDomanda()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                stopwatchDue.Start();
                Debug.WriteLine(stopwatchDue.Elapsed);
                return click;
            });
        }

        private void btn_ApriPDF_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(urlRisorsa));
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
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
                lstDatiStatistica.Add(datiStatistica);
                stopwatchDue.Restart();
                return click;
            });
        }

        private void btnA_Clicked(object sender, EventArgs e)
        {
            btnA_Cliccato = true;
            if (btnA_Cliccato == true)
            {
                click = false;
                btnAvanti.IsVisible = true;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "B" || recordCampiDomandaRisposte.Risposta.ToString() == "C" || recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datirisultati = new DatiRisultati();
                btnA.BackgroundColor = Color.Red;
                contSA++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.A;
                datirisultati.color = Color.Red;
                datirisultati.rispostaEsattaYN = "errata";
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                lstdatirisultati.Add(datirisultati);
                if (recordCampiDomandaRisposte.Risposta.ToString() == "B")
                {
                    btnB.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "C")
                {
                    btnC.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "D")
                {
                    btnD.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = false;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "A")
            {
                datirisultati = new DatiRisultati();
                btnA.BackgroundColor = Color.Green;
                contEA++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = true;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.A;
                datirisultati.color = Color.Green;
                datirisultati.rispostaEsattaYN = "esatta";
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;

                }
                lstdatirisultati.Add(datirisultati);
            }
            indiceIntervalliTemporali++;
        }

        private void btnB_Clicked(object sender, EventArgs e)
        {
            btnB_Cliccato = true;
            if (btnB_Cliccato == true)
            {
                click = false;
                btnAvanti.IsVisible = true;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "A" || recordCampiDomandaRisposte.Risposta.ToString() == "C" || recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datirisultati = new DatiRisultati();
                btnB.BackgroundColor = Color.Red;
                contSB++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.B;
                datirisultati.color = Color.Red;
                datirisultati.rispostaEsattaYN = "errata";
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;

                }
                lstdatirisultati.Add(datirisultati);
                if (recordCampiDomandaRisposte.Risposta.ToString() == "A")
                {
                    btnA.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "C")
                {
                    btnC.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "D")
                {
                    btnD.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = false;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "B")
            {
                datirisultati = new DatiRisultati();
                btnB.BackgroundColor = Color.Green;
                contEB++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = true;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.B;
                datirisultati.color = Color.Green;
                datirisultati.rispostaEsattaYN = "esatta";
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;

                }
                lstdatirisultati.Add(datirisultati);
            }
            indiceIntervalliTemporali++;
        }

        private void btnC_Clicked(object sender, EventArgs e)
        {   
            btnC_Cliccato = true;
            if (btnC_Cliccato == true)
            {
                click = false;
                btnAvanti.IsVisible = true;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "A" || recordCampiDomandaRisposte.Risposta.ToString() == "B" || recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datirisultati = new DatiRisultati();
                btnC.BackgroundColor = Color.Red;
                contSC++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.C;
                datirisultati.rispostaEsattaYN = "errata";
                datirisultati.color = Color.Red;
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;

                }
                lstdatirisultati.Add(datirisultati);
                if (recordCampiDomandaRisposte.Risposta.ToString() == "A")
                {
                    btnA.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "B")
                {
                    btnB.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "D")
                {
                    btnD.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = false;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "C")
            {
                datirisultati = new DatiRisultati();
                btnC.BackgroundColor = Color.Green;
                contEC++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = true;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.C;
                datirisultati.rispostaEsattaYN = "esatta";
                datirisultati.color = Color.Green;
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                lstdatirisultati.Add(datirisultati);
            }
            indiceIntervalliTemporali++;
        }

        private void btnD_Clicked(object sender, EventArgs e)
        {
            btnD_Cliccato = true;
            if (btnD_Cliccato == true)
            {
                click = false;
                btnAvanti.IsVisible = true;
            }
            TempoRestartDomanda();
            if (recordCampiDomandaRisposte.Risposta.ToString() == "A" || recordCampiDomandaRisposte.Risposta.ToString() == "B" || recordCampiDomandaRisposte.Risposta.ToString() == "C")
            {
                datirisultati = new DatiRisultati();
                btnD.BackgroundColor = Color.Red;
                contSD++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.D;
                datirisultati.color = Color.Red;
                datirisultati.rispostaEsattaYN = "errata";
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;

                }
                lstdatirisultati.Add(datirisultati);

                if (recordCampiDomandaRisposte.Risposta.ToString() == "A")
                {
                    btnA.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "B")
                {
                    btnB.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                else if (recordCampiDomandaRisposte.Risposta.ToString() == "C")
                {
                    btnC.BackgroundColor = Color.Green;
                    btnA.IsEnabled = false;
                    btnB.IsEnabled = false;
                    btnC.IsEnabled = false;
                    btnD.IsEnabled = false;
                }
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = false;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
            }
            else if (recordCampiDomandaRisposte.Risposta.ToString() == "D")
            {
                datirisultati = new DatiRisultati();
                btnD.BackgroundColor = Color.Green;
                contED++;
                btnA.IsEnabled = false;
                btnB.IsEnabled = false;
                btnC.IsEnabled = false;
                btnD.IsEnabled = false;
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = true;
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = recordCampiDomandaRisposte.D;
                datirisultati.rispostaEsattaYN = "esatta";
                datirisultati.color = Color.Green;
                switch (recordCampiDomandaRisposte.Risposta)
                {
                    case "A":
                        datirisultati.risposta = recordCampiDomandaRisposte.A;
                        break;
                    case "B":
                        datirisultati.risposta = recordCampiDomandaRisposte.B;
                        break;
                    case "C":
                        datirisultati.risposta = recordCampiDomandaRisposte.C;
                        break;
                    case "D":
                        datirisultati.risposta = recordCampiDomandaRisposte.D;
                        break;
                }
                lstdatirisultati.Add(datirisultati);
            }
            indiceIntervalliTemporali++;
        }

        private void avvioQuiz_Clicked(object sender, EventArgs e)
        {
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
            RelativeBottoneAvvio.IsVisible = false;
            FooterContatoreDomande.IsVisible = true;
            TempoStartDomanda();
            TempoTrascorsoGlobale();
            click = false;
        }

        private async void btnAvanti_Clicked(object sender, EventArgs e)
        {
            DomandaSuccessiva();
            await CaricamentoImmagine(recordCampiDomandaRisposte.tipo, Costanti.urlBase, recordCampiDomandaRisposte.link);
            btnA.IsEnabled = true;
            btnB.IsEnabled = true;
            btnC.IsEnabled = true;
            btnD.IsEnabled = true;
            btnA.BackgroundColor = Color.FromHex("#0069c0");
            btnB.BackgroundColor = Color.FromHex("#0069c0");
            btnC.BackgroundColor = Color.FromHex("#0069c0");
            btnD.BackgroundColor = Color.FromHex("#0069c0");
        }
    }
}