using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using System.Threading.Tasks;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;
using System.Linq;

namespace QuizAmoroso
{
    /**
     * @Authors: Antonio Fabrizio Fiume, Alessio Calabrese, Antonio Saverio Valente
     */
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Simulazione : ContentPage
    {
        // DICHIARAZIONE VARIABILI
        public  int contEsatteA,contEsatteB,contEsatteC,contEsatteD,contEsatteTot,contSbagliateA, contSbagliateB, contSbagliateC, contSbagliateD, contSbagliateTot, contNonRisposteTot = 0;
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
        public string nomeset = SetDomande.setDomandeSelezionato.nome_set;
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
        bool click = true;
        string resultcontent;
        bool flag = false;
        bool fermaAvviaTempoGlobale = true;
        string urlRisorsa = "";
        public Timer timer = new Timer();

        public Simulazione()
        {
            InitializeComponent();
            IngressoPagina();
            timer.TempoSimulazione(true);
        }

        /** 
         * Questo metodo va aggiunto se si decide di eliminare il frame che sostituisce 
         * la navigation bar. Gli aggiunge il titolo con il nome del set
         * 
         * protected override void OnAppearing()
         * {
         * base.OnAppearing();
         * Title = "Simulazione: " + nomeset;
         * }
         * Ricordarsi anche di settare a true il parametro hasnavigation bar nello xaml
         **/
        private void IngressoPagina()
        {
            // Va eliminata la gira sotto se si vuole tornare alla modalità con navigation bar (vedi commento precedente)
            LabelTitoloHeader.Text = "Simulazione: " + nomeset;
            btn_ApriPDF.IsEnabled = false;
            btn_ApriPDF.IsVisible = false;
            immagine.IsVisible = false;
            lblDomanda.IsVisible = false;
            btnA.IsVisible = false;
            btnB.IsVisible = false;
            btnC.IsVisible = false;
            btnD.IsVisible = false;
            lblTempo.IsVisible = false;
            FooterContatoreDomande.IsVisible = false;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            timer.FermaTempoSimulazione();
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
                var risposta = await result.Content.ReadAsStringAsync();
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
                    struttura = ShuffleList.Shuffle<Domande>(struttura);
                    numeroTotaleDelSetDiDomande = struttura.Count;
                    RisultatoSimulazione.conteggioDomandeSvoltePerSimulazione = numeroTotaleDelSetDiDomande;
                }
            }
            catch (Exception e)
            {
                resultcontent = "Impossibile connettersi al servizio";
                await DisplayAlert("Errore", resultcontent.ToString(), "Ok");
                await Navigation.PopToRootAsync();
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
                TmpTotale= string.Format("{0:00}:{1:00}:{2:00}", tempo.Hours, tempo.Minutes, tempo.Seconds);
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
            contEsatteTot = contEsatteA + contEsatteB + contEsatteC + contEsatteD;
            contSbagliateTot = contSbagliateA + contSbagliateB + contSbagliateC + contSbagliateD;
            //var page = Navigation.NavigationStack.ElementAtOrDefault(Navigation.NavigationStack.Count - 1);
            bool flagPunteggio = false;
            await Navigation.PushAsync(new RisultatoSimulazione(contSbagliateTot, contEsatteTot, TmpTotale,lstdatirisultati,flagPunteggio,contNonRisposteTot));
            //Navigation.RemovePage(page);
        }

        public async Task InvioDatiStatistiche()
        {
            try
            {
                string output = JsonConvert.SerializeObject(lstDatiStatistica, Formatting.Indented);
                StringContent stringContent = new StringContent(output, UnicodeEncoding.UTF8, "application/json");
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync(Costanti.sessione, stringContent);
                string resultContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(resultContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
                datiStatistica.codice = recordCampiDomandaRisposte.Codice;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datiStatistica.nomeSet = SetDomande.setDomandeSelezionato.nome_set;
                datiStatistica.id_concorso = PianoFormativo.pianoFormativoSelezionato.id_concorso;
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
        }

        private async void avvioQuiz_Clicked(object sender, EventArgs e)
        {
            try
            {
              await ConnessioneDomande();
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
            lblTempo.IsVisible = true;
            avvioQuiz.IsVisible = false;
            FooterContatoreDomande.IsVisible = true;
            RelativeBottoneAvvio.IsVisible = false;
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
            CaricamentoPaginaSimulazione.IsRunning = true;
            CaricamentoPaginaSimulazione.IsVisible = true;
            LayoutPrincipalePaginaSimulazione.IsVisible = false;
            await InvioDatiStatistiche();
            CaricamentoPaginaSimulazione.IsRunning = false;
            CaricamentoPaginaSimulazione.IsVisible = false;
            await Navigation.PopAsync();
        }
    }
}