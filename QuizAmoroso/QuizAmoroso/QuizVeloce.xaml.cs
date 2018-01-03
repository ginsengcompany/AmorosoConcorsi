using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using QuizAmoroso.DataModel;
using QuizAmoroso.Model;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuizVeloce : ContentPage
    {
        public List<StrutturaPickerConcorso> listaConcorsi = new List<StrutturaPickerConcorso>();
        public List<StrutturaPickerMaterie> listaMaterie = new List<StrutturaPickerMaterie>();
        public List<string> listaIdConcorsi = new List<string>();
        public List<float> listaCorrezioneEsatte = new List<float>();
        public List<float> listaCorrezioneErrate = new List<float>();
        public List<int> listaNumeroDomandeDelConcorso = new List<int>();
        public List<int> listaNumeroDomandeMassimoDelTestQuizVeloce = new List<int>();
        public List<string> listaMaterieQuizVeloce = new List<string>();
        public static float valoreCorrezioneRispostaEsatta = 0.0F;
        public static float valoreCorrezioneRispostaErrata = 0.0F;
        public static int numeroDomandeMassimoDelTestQuizVeloce = 0;
        public static int numeroDomandeQuizVeloceSelezionato = 0;
        public static int numeroselezionato = 1;
        public static int numeroSelezionatoArrotondato;
        public static string idConcorsoSelezionato;
        public static string materiaSelezionata;
        public int numeroMassimoDomandeAmmessoPerMateria = 0;
        public int differenza;
        public int StepValue = 5;
        public int flag = 1;
        const int minDomandeSlider = 10;
        public string risualtatoChiamataMaterie;
        public string risultatoChiamataQuizVeloce;
        public string corpoSelezionato;
        public string corpo;
        public string idConcorso;
        public string modalitaSelezionata;
        public string materie;
        public string estremoMinimoSlider = minDomandeSlider.ToString();
        public string estremoMassimoSlider = numeroDomandeMassimoDelTestQuizVeloce.ToString();
        public string nomeMateriaScelta;
        public StrutturaPickerConcorso testSuInteroDB = new StrutturaPickerConcorso
        {
            Corpo = Costanti.eseguiTestSuInteroDb,
            id_concorso = Costanti.eseguiTestSuInteroDb,
            codice_concorso = Costanti.eseguiTestSuInteroDb,
            rispostaerrata = "0",
            rispostaesatta = "1",
            numerodomande = Costanti.numeroMassimoDomandeDelTestSuInteroDB.ToString(),
            domandemax = Costanti.numeroMassimoDomandeAmmesso.ToString()
        };
        Timer t = new Timer();

        /*
         * Struttura dati contenente le informazioni del Concorso
         * */
        public class StrutturaPickerConcorso
        {
            // denominazione per esteso del concorso (Es. Esercito Marina Aeronautica", Corpo json)
            public string Corpo { get; set; }
            // identificativo numerico del concorso (Es. 5, id_concorso json)
            public string id_concorso { get; set; }
            // codice alfanumerico del concorso (Es. VFP4, codice_concorso json) 
            public string codice_concorso { get; set; }
            // punteggio associato alla risposta esatta (rispostaesatta json)
            public string rispostaesatta { get; set; }
            // punteggio associato alla risposta errata (rispostaerrata json)
            public string rispostaerrata { get; set; }
            // numedo delle domande contenute in una prova di tipo quiz veloce (numerodomande json)
            public string numerodomande { get; set; }
            // numero totale delle domande contenute nell'intero concorso (domandemax json)
            public string domandemax { get; set; }
        }

        /*
         * Struttura dati contenente le informazioni delle Materie
         **/
        public class StrutturaPickerMaterie
        {
            // denominazione della materia (es. GEOGRAFIA, materia json)
            public string materia { get; set; }
            // numero delle domande totali contenute nella materia (domandemateriamax json)
            public string domandemateriamax { get; set; }
        }

        public QuizVeloce()
        {
            InitializeComponent();
            LinkSitoWebAk12();
        }

        protected async override void OnAppearing()
        {
            if (picker_selezioneConcorso.Items.Any())
            {
                picker_selezioneConcorso.Items.Clear();
                picker_selezioneConcorso.IsVisible = false;
                labelSelezionaConcorso.IsVisible = false;
                listaConcorsi.Clear();
                listaIdConcorsi.Clear();
                listaCorrezioneEsatte.Clear();
                listaCorrezioneErrate.Clear();
                listaNumeroDomandeMassimoDelTestQuizVeloce.Clear();
                listaNumeroDomandeDelConcorso.Clear();
                if (picker_selezioneMateria.Items.Any())
                {
                    picker_selezioneMateria.Items.Clear();
                    listaMaterie.Clear();                    
                    listaMaterieQuizVeloce.Clear();

                    picker_selezioneMateria.IsVisible = false;
                    labelSelezionaMateria.IsVisible = false;
                    if (picker_selezioneModalita.Items.Any())
                    {
                        picker_selezioneModalita.Items.Clear();
                        AttivaSelezioneManualeNumeroDomande.IsToggled = false;
                        picker_selezioneModalita.IsVisible = false;
                        labelSelezionaModalita.IsVisible = false;
                        AttivaSelezioneManualeNumeroDomande.IsVisible = false;
                        LabelAttivaSwitchSelezioneDomande.IsVisible = false;
                        LabelNumeroPartenza.IsVisible = false;
                        EntryNumeroPartenza.IsVisible = false;
                        LabelSliderNumeroDomande.IsVisible = false;
                        SliderSelezioneNumeroDomande.IsVisible = false;
                        LabelValoriSelezionatiConSlider.IsVisible = false;
                        LabelMinimoSlider.IsVisible = false;
                        LabelMassimoSlider.IsVisible = false;
                        buttonQuizVeloce.IsVisible = false;
                    }
                }
            }
            caricamentoPagina.IsRunning = true;
            caricamentoPagina.IsVisible = true;
            await AttesaRicezioneConcorsi();
            picker_selezioneConcorso.IsVisible = true;
            labelSelezionaConcorso.IsVisible = true;
            caricamentoPagina.IsRunning = false;
            caricamentoPagina.IsVisible = false;
        }

        public async Task AttesaRicezioneConcorsi()
        {
            try
            {
                string username = Utente.Instance.getUserName;
                var client = new HttpClient();
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", username));
                var content = new FormUrlEncodedContent(values);
                try
                {
                    var result = await client.PostAsync(Costanti.concorsi, content);
                    risultatoChiamataQuizVeloce = await result.Content.ReadAsStringAsync();
                    if (risultatoChiamataQuizVeloce == "null")
                    {
                        listaConcorsi.Add(testSuInteroDB);
                        listaIdConcorsi.Add(testSuInteroDB.id_concorso);
                        picker_selezioneConcorso.Items.Add(Costanti.eseguiTestSuInteroDb);
                        listaCorrezioneEsatte.Add(Single.Parse(testSuInteroDB.rispostaesatta, CultureInfo.InvariantCulture));
                        listaCorrezioneErrate.Add(Single.Parse(testSuInteroDB.rispostaerrata, CultureInfo.InvariantCulture));
                        listaNumeroDomandeMassimoDelTestQuizVeloce.Add(Convert.ToInt16(testSuInteroDB.numerodomande));
                        listaNumeroDomandeDelConcorso.Add(Convert.ToInt16(testSuInteroDB.domandemax));
                        Exception errore = new Exception();
                        throw errore;
                    }
                    else
                    {
                        listaConcorsi = JsonConvert.DeserializeObject<List<StrutturaPickerConcorso>>(risultatoChiamataQuizVeloce);
                        if (listaConcorsi.Any())
                        {
                            if (picker_selezioneConcorso.Items.Any())
                            {
                                picker_selezioneConcorso.Items.Clear();
                            }

                            foreach (StrutturaPickerConcorso i in listaConcorsi)
                            {
                                corpo = i.Corpo;
                                idConcorso = i.id_concorso;
                                string codice_concorso = i.codice_concorso;
                                listaIdConcorsi.Add(idConcorso);
                                picker_selezioneConcorso.Items.Add(" Concorso: " + corpo + "\n" + " Codice: " + codice_concorso);
                                listaCorrezioneEsatte.Add(Single.Parse(i.rispostaesatta, CultureInfo.InvariantCulture));
                                listaCorrezioneErrate.Add(float.Parse(i.rispostaerrata, CultureInfo.InvariantCulture));
                                listaNumeroDomandeMassimoDelTestQuizVeloce.Add(Convert.ToInt16(i.numerodomande));
                                listaNumeroDomandeDelConcorso.Add(Convert.ToInt16(i.domandemax));
                            }
                            listaIdConcorsi.Add(Costanti.eseguiTestSuInteroDb);
                            picker_selezioneConcorso.Items.Add(Costanti.eseguiTestSuInteroDb);
                            listaCorrezioneEsatte.Add(1);
                            listaCorrezioneErrate.Add(0);
                            listaNumeroDomandeMassimoDelTestQuizVeloce.Add(Costanti.numeroMassimoDomandeAmmesso);
                            listaNumeroDomandeDelConcorso.Add(Costanti.numeroMassimoDomandeAmmesso);
                        }
                    }
                }
                catch (Exception errore)
                {
                    //await DisplayAlert("Attenzione", "Non sono presenti concorsi. E' comunque possibile esercitarsi sull'intera banca dati.", "Ok");
                }
            }
            catch (Exception errore)
            {
                await DisplayAlert("Attenzione", "Connessione persa durante il caricamento dei concorsi.", "Ok");
            }
        }

        private async void picker_selezioneConcorso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker_selezioneConcorso.Items.Any())
            {
                caricamentoPagina.IsRunning = true;
                caricamentoPagina.IsVisible = true;
                await AttesaRicezioneMaterie();
                caricamentoPagina.IsRunning = false;
                caricamentoPagina.IsVisible = false;

                valoreCorrezioneRispostaErrata = listaCorrezioneErrate[picker_selezioneConcorso.SelectedIndex];
                valoreCorrezioneRispostaEsatta = listaCorrezioneEsatte[picker_selezioneConcorso.SelectedIndex];

                if (listaMaterie == null)
                {
                    picker_selezioneMateria.IsVisible = false;
                    labelSelezionaMateria.IsVisible = false;
                    picker_selezioneModalita.IsVisible = false;
                    LabelSliderNumeroDomande.IsVisible = false;
                    SliderSelezioneNumeroDomande.IsVisible = false;
                    LabelValoriSelezionatiConSlider.IsVisible = false;
                    AttivaSelezioneManualeNumeroDomande.IsVisible = false;
                    LabelAttivaSwitchSelezioneDomande.IsVisible = false;
                }
                else if (listaMaterie != null && listaIdConcorsi[picker_selezioneConcorso.SelectedIndex] == Costanti.eseguiTestSuInteroDb)
                {
                    picker_selezioneMateria.IsVisible = true;
                    labelSelezionaMateria.IsVisible = true;
                    picker_selezioneModalita.IsVisible = false;
                    LabelSliderNumeroDomande.IsVisible = false;
                    SliderSelezioneNumeroDomande.IsVisible = false;
                    LabelValoriSelezionatiConSlider.IsVisible = false;
                    AttivaSelezioneManualeNumeroDomande.IsVisible = false;
                    LabelAttivaSwitchSelezioneDomande.IsVisible = false;
                }
                else
                {
                    picker_selezioneMateria.IsVisible = true;
                    labelSelezionaMateria.IsVisible = true;
                    picker_selezioneModalita.IsVisible = false;
                    LabelSliderNumeroDomande.IsVisible = false;
                    SliderSelezioneNumeroDomande.IsVisible = false;
                    LabelValoriSelezionatiConSlider.IsVisible = false;
                    AttivaSelezioneManualeNumeroDomande.IsVisible = false;
                    LabelAttivaSwitchSelezioneDomande.IsVisible = false;
                }
            }
        }

        public async Task AttesaRicezioneMaterie()
        {
            corpoSelezionato = picker_selezioneConcorso.Items[picker_selezioneConcorso.SelectedIndex];
            idConcorsoSelezionato = listaIdConcorsi[picker_selezioneConcorso.SelectedIndex];

            try
            {
                listaMaterie.Clear(); 
                var client = new HttpClient();
                var values = new List<KeyValuePair<string, string>>();
                string rotta = "";

                if (corpoSelezionato == Costanti.eseguiTestSuInteroDb)
                {
                    values.Add(new KeyValuePair<string, string>("id_concorso", Costanti.eseguiTestSuInteroDb));
                    rotta = Costanti.materietotali;
                }
                else
                {
                    values.Add(new KeyValuePair<string, string>("id_concorso", idConcorsoSelezionato));
                    rotta = Costanti.materieconcorso;
                }

                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(rotta, content);
                risualtatoChiamataMaterie = await result.Content.ReadAsStringAsync();
                listaMaterie = JsonConvert.DeserializeObject<List<StrutturaPickerMaterie>>(risualtatoChiamataMaterie);

                if (listaMaterie.Count != 0)
                {
                    if (picker_selezioneMateria.Items.Any())
                    {
                        picker_selezioneMateria.Items.Clear();
                        listaNumeroDomandeDelConcorso.Clear();
                        listaMaterieQuizVeloce.Clear();
                    }
                    picker_selezioneMateria.Items.Add("NESSUNA MATERIA");
                    listaNumeroDomandeDelConcorso.Add(Costanti.numeroMassimoDomandeAmmesso);
                    listaMaterieQuizVeloce.Add("NESSUNA MATERIA");
                    foreach (StrutturaPickerMaterie i in listaMaterie)
                    {
                        materie = i.materia;
                        picker_selezioneMateria.Items.Add(materie);
                        listaNumeroDomandeDelConcorso.Add(Convert.ToInt16(i.domandemateriamax));
                        listaMaterieQuizVeloce.Add(i.materia);
                    }
                    
                } else {
                    Exception errore = new ArgumentNullException();
                    await DisplayAlert("Attenzione", "Il concorso non ha materie o non sono state assegnate.", "Ok");
                    throw errore;
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Attenzione", "Connessione persa durante il caricamento delle materie.", "riprova");
            }
        }

        private async void picker_selezioneMateria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker_selezioneMateria.Items.Any())
            {
                if (picker_selezioneModalita.Items.Any())
                {
                    picker_selezioneModalita.Items.Clear();
                    picker_selezioneModalita.IsVisible = false;
                }

                labelSelezionaModalita.IsVisible = true;
                picker_selezioneModalita.IsVisible = true;

                if (listaIdConcorsi[picker_selezioneConcorso.SelectedIndex] == Costanti.eseguiTestSuInteroDb)
                {
                    materiaSelezionata = picker_selezioneMateria.Items[picker_selezioneMateria.SelectedIndex];
                    numeroMassimoDomandeAmmessoPerMateria = listaNumeroDomandeDelConcorso[picker_selezioneMateria.SelectedIndex + 1];
                    LabelNumeroPartenza.Text = "Inserisci il numero della domanda da cui vuoi partire, ricordandoti che il  massimo e': " + numeroMassimoDomandeAmmessoPerMateria;
                }
                else
                {
                    materiaSelezionata = picker_selezioneMateria.Items[picker_selezioneMateria.SelectedIndex];
                    numeroMassimoDomandeAmmessoPerMateria = listaNumeroDomandeDelConcorso[picker_selezioneMateria.SelectedIndex + 1];
                    LabelNumeroPartenza.Text = "Inserisci il numero della domanda da cui vuoi partire, ricordandoti che il  massimo e': " + numeroMassimoDomandeAmmessoPerMateria;
                }

                if (picker_selezioneModalita.Items.Any())
                {
                    picker_selezioneModalita.Items.Clear();
                }

                if (listaIdConcorsi[picker_selezioneConcorso.SelectedIndex] == Costanti.eseguiTestSuInteroDb)
                {
                    picker_selezioneModalita.Items.Add("Modalità Casuale");
                }
                else
                {
                    picker_selezioneModalita.Items.Add("Modalità Casuale");
                    picker_selezioneModalita.Items.Add("Modalità Sequenziale");
                }
            }
        }

        private void picker_selezioneModalita_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker_selezioneModalita.Items.Any())
            {
                LabelAttivaSwitchSelezioneDomande.IsVisible = true;
                AttivaSelezioneManualeNumeroDomande.IsVisible = true;
                numeroDomandeMassimoDelTestQuizVeloce = listaNumeroDomandeMassimoDelTestQuizVeloce[picker_selezioneConcorso.SelectedIndex];

                if (listaIdConcorsi[picker_selezioneConcorso.SelectedIndex] == Costanti.eseguiTestSuInteroDb)
                {
                    modalitaSelezionata = picker_selezioneModalita.Items.FirstOrDefault<string>();
                    if (picker_selezioneModalita.SelectedIndex != -1)
                    {
                        buttonQuizVeloce.IsVisible = true;
                    }
                    if (modalitaSelezionata == "Modalità Casuale")
                    {
                        LabelNumeroPartenza.IsVisible = false;
                        EntryNumeroPartenza.IsVisible = false;
                    }
                }
                else
                {
                    modalitaSelezionata = picker_selezioneModalita.Items[picker_selezioneModalita.SelectedIndex];
                    if (picker_selezioneModalita.SelectedIndex != -1)
                    {
                        buttonQuizVeloce.IsVisible = true;
                    }
                    if (modalitaSelezionata == "Modalità Casuale")
                    {
                        LabelNumeroPartenza.IsVisible = false;
                        EntryNumeroPartenza.IsVisible = false;
                    }
                    if (modalitaSelezionata == "Modalità Sequenziale")
                    {
                        LabelNumeroPartenza.IsVisible = true;
                        EntryNumeroPartenza.IsVisible = true;
                    }
                }
            }
        }

        private void AttivaSelezioneManualeNumeroDomande_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value == false)
            {
                LabelSliderNumeroDomande.IsVisible = false;
                SliderSelezioneNumeroDomande.IsVisible = false;
                LabelValoriSelezionatiConSlider.IsVisible = false;
                LabelMinimoSlider.IsVisible = false;
                LabelMassimoSlider.IsVisible = false;
                LabelAttivaSwitchSelezioneDomande.IsVisible = true;
            }
            else
            {
                LabelSliderNumeroDomande.IsVisible = true;
                SliderSelezioneNumeroDomande.IsVisible = true;
                LabelValoriSelezionatiConSlider.IsVisible = true;
                LabelMinimoSlider.IsVisible = true;
                LabelMassimoSlider.IsVisible = true;
                numeroDomandeMassimoDelTestQuizVeloce = listaNumeroDomandeMassimoDelTestQuizVeloce[picker_selezioneConcorso.SelectedIndex];
                SliderSelezioneNumeroDomande.Maximum = numeroDomandeMassimoDelTestQuizVeloce;
                SliderSelezioneNumeroDomande.Minimum = minDomandeSlider;
                LabelMassimoSlider.Text = "Max " + numeroDomandeMassimoDelTestQuizVeloce;
                LabelMinimoSlider.Text = "Min " + estremoMinimoSlider;
                LabelAttivaSwitchSelezioneDomande.IsVisible = true;
            }
        }

        private void SliderSelezioneNumeroDomande_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue / StepValue);
            SliderSelezioneNumeroDomande.Value = newStep * StepValue;
            numeroDomandeQuizVeloceSelezionato = Convert.ToInt16(SliderSelezioneNumeroDomande.Value);
            LabelMassimoSlider.Text = "Max " + numeroDomandeMassimoDelTestQuizVeloce.ToString();
            LabelMinimoSlider.Text = "Min " + minDomandeSlider.ToString();
            LabelValoriSelezionatiConSlider.Text = "Numero selezionato: " + SliderSelezioneNumeroDomande.Value.ToString();
        }

        private async void buttonQuizVeloce_Clicked(object sender, EventArgs e)
        {
            if (picker_selezioneConcorso.SelectedIndex != -1 && picker_selezioneMateria.SelectedIndex != -1 && picker_selezioneModalita.SelectedIndex != -1)
            {
                if ((materiaSelezionata == null) || (materiaSelezionata == "NESSUNA MATERIA"))
                {
                    materiaSelezionata = "null";
                }

                if (string.IsNullOrEmpty(EntryNumeroPartenza.Text))
                {
                    EntryNumeroPartenza.Text = "1";
                    numeroselezionato = Convert.ToInt16(EntryNumeroPartenza.Text);
                    flag = 1;
                }
                else
                {
                    if (numeroselezionato % 1 == 1)
                    {
                        numeroselezionato = Convert.ToInt16(EntryNumeroPartenza.Text);
                        if (numeroselezionato > numeroMassimoDomandeAmmessoPerMateria)
                        {
                            flag = 0;
                        }
                    }
                    else
                    if (EntryNumeroPartenza.Text.Contains(".") || EntryNumeroPartenza.Text.Contains(","))
                    {
                        await DisplayAlert("Attenzione", "Non puoi utilizzare numeri decimali", "riprova");
                    }
                }

                caricamentoPagina.IsRunning = true;
                caricamentoPagina.IsVisible = true;
                if (flag == 1)
                {
                    numeroselezionato = Convert.ToInt16(EntryNumeroPartenza.Text);

                    differenza = numeroMassimoDomandeAmmessoPerMateria - numeroselezionato;
                    if (numeroDomandeQuizVeloceSelezionato != 0)
                    {
                        if (numeroDomandeQuizVeloceSelezionato <= differenza)
                        {
                            await Navigation.PushAsync(new NuovaModalitaQuizVeloceNew(modalitaSelezionata));
                        }
                        else
                        {
                            numeroDomandeQuizVeloceSelezionato = differenza;
                            await Navigation.PushAsync(new NuovaModalitaQuizVeloceNew(modalitaSelezionata));
                        }
                    }
                    else
                    {
                        if (numeroDomandeMassimoDelTestQuizVeloce <= differenza)
                        {
                            await Navigation.PushAsync(new NuovaModalitaQuizVeloceNew(modalitaSelezionata));
                        }
                        else
                        {
                            numeroDomandeMassimoDelTestQuizVeloce = differenza;
                            await Navigation.PushAsync(new NuovaModalitaQuizVeloceNew(modalitaSelezionata));
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Attenzione", "Numero selezionato supera il limite delle domande", "riprova");
                }
                caricamentoPagina.IsRunning = false;
                caricamentoPagina.IsVisible = false;
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