using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using System.Threading.Tasks;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;

namespace QuizAmoroso
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SimulazioneAssistitaNew : ContentPage
    {
        private int contSbagliateTot, contEsatteTot;
        private int contNonRisposteTot = 0;
        private List<Button> listaBottoni;
        private List<Label> listaLabel;
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
        private bool btn_Cliccato = false;
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
        bool click = true;
        string resultcontent;
        bool flag = false;
        public Timer timer = new Timer();

        public SimulazioneAssistitaNew ()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            Title = "Simulazione Assistita: " + SetDomande.setDomandeSelezionato.nome_set;
            LabelTitoloHeader.Text = "Simulazione Assistita: " + SetDomande.setDomandeSelezionato.nome_set;
            CaricamentoPaginaSimulazioneAssistita.IsRunning = true;
            CaricamentoPaginaSimulazioneAssistita.IsVisible = true;
            btn_ApriPDF.IsEnabled = false;
            btn_ApriPDF.IsVisible = false;
            immagine.IsVisible = false;
            lblDomanda.IsVisible = false;
            GrigliaDomande.IsVisible = false;
            btnAvanti.IsVisible = false;
            lblTempo.IsVisible = false;
            avvioQuiz.IsVisible = false;
            FooterContatoreDomande.IsVisible = false;
            await ConnessioneDomande();
            CaricamentoPaginaSimulazioneAssistita.IsRunning = false;
            CaricamentoPaginaSimulazioneAssistita.IsVisible = false;
            avvioQuiz.IsVisible = true;
            timer.TempoSimulazioneAssistita(true);
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
                recordCampiDomandaRisposte = null;
                index = rnd.Next(struttura.Count);
                recordCampiDomandaRisposte = struttura[index];
                lblDomanda.Text = recordCampiDomandaRisposte.Domanda;
                Griglia(recordCampiDomandaRisposte);
                strutturaAppoggio.Add(recordCampiDomandaRisposte);
                indiceStrutturaAppoggio++;
                struttura.RemoveAt(index);
                numeroAttualeDomanda++;
                ContatoreDomande.Text = "Domanda " + numeroAttualeDomanda.ToString() + " di " + numeroTotaleDelSetDiDomande.ToString();
            }
            else
            {
                DisplayAlert("Complimenti!", "La sessione di simulazione è terminata.", "Esci");
                //var page = Navigation.NavigationStack.ElementAtOrDefault(Navigation.NavigationStack.Count - 1);
                bool flagPunteggio = false;
                Navigation.PushAsync(new RisultatoSimulazione(contEsatteTot, contSbagliateTot, lblTempo.Text, lstdatirisultati, flagPunteggio, contNonRisposteTot));
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

        public void Griglia(Domande recordCampiDomandaRisposte)
        {
            GrigliaDomande.RowDefinitions = new RowDefinitionCollection();
            GrigliaDomande.ColumnDefinitions = new ColumnDefinitionCollection();
            listaBottoni = new List<Button>();
            listaLabel = new List<Label>();
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
                btnAvanti.IsVisible = true;
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
                datirisultati = new DatiRisultati();
                lblBtn.BackgroundColor = Color.Green;
                contEsatteTot++;
                GrigliaDomande.IsEnabled = false;
                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = true;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = lista[lblBtn.Text];
                datirisultati.color = Color.Green;
                datirisultati.rispostaEsattaYN = "esatta";
                datirisultati.risposta = lista[recordCampiDomandaRisposte.Risposta];
                lstdatirisultati.Add(datirisultati);
            }
            else
            {
                datirisultati = new DatiRisultati();
                lblBtn.BackgroundColor = Color.Red;
                contSbagliateTot++;
                // Dovrei disabilitare solo i bottoni
                GrigliaDomande.IsEnabled = false;
                datirisultati.Domanda = recordCampiDomandaRisposte.Domanda;
                datirisultati.tuaRisposta = lista[lblBtn.Text];
                datirisultati.color = Color.Red;
                datirisultati.rispostaEsattaYN = "errata";
                datirisultati.risposta = lista[recordCampiDomandaRisposte.Risposta];
                lstdatirisultati.Add(datirisultati);

                listaBottoni.Remove(lblBtn);

                foreach (var el in listaBottoni)
                {
                    if (recordCampiDomandaRisposte.Risposta.ToString() == el.Text)
                    {
                        el.BackgroundColor = Color.Green;
                        GrigliaDomande.IsEnabled = false;
                    }
                }

                datiStatistica = new DatiStatistica();
                datiStatistica.rispostaEsattaYN = false;
                datiStatistica.codice = recordCampiDomandaRisposte.id_domanda;
                datiStatistica.materia = recordCampiDomandaRisposte.Materia;
                datiStatistica.sottocategoria = recordCampiDomandaRisposte.Sottocategoria;
                datiStatistica.data = dateTime.ToString("dd/MM/yyyy");
                datiStatistica.ora = dateTime.ToString("HH:mm:ss");
            }
        }

        private void avvioQuiz_Clicked(object sender, EventArgs e)
        {
            frameDomanda.IsVisible = true;
            lblDomanda.IsVisible = true;
            GrigliaDomande.IsVisible = true;
            lblTempo.IsVisible = true;
            avvioQuiz.IsVisible = false;
            RelativeBottoneAvvio.IsVisible = false;
            btnAvanti.IsVisible = false;
            FooterContatoreDomande.IsVisible = true;
            TempoStartDomanda();
            TempoTrascorsoGlobale();
            click = false;
        }

        private async void btnAvanti_Clicked(object sender, EventArgs e)
        {
            GrigliaDomande.Children.Clear();
            DomandaSuccessiva();
            await CaricamentoImmagine(recordCampiDomandaRisposte.tipo, Costanti.urlBase, recordCampiDomandaRisposte.link);
            GrigliaDomande.IsEnabled = true;
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
                values.Add(new KeyValuePair<string, string>("tempoSimulazioneAssistita", timer.tempoTotaleSimulazioneAssistita));
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