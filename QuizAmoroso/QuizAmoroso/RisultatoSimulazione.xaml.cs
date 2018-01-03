using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuizAmoroso.DataModel;

namespace QuizAmoroso
{
    /**
     * @Authors: Antonio Fabrizio Fiume, Alessio Calabrese, Antonio Saverio Valente
     * La page seguente implementa i risultati che l'utente, dopo 
     * la simulazione e simulazione assistita, ha conseguito.
     * la visualizzazione dei risultati è implementata tramite tre
     * label contenute in tre frame.
     */
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RisultatoSimulazione : ContentPage
    {
        public static int conteggioDomandeSvoltePerSimulazione;
        int contEs, contSb = 0;
        int contNonRisp = 0;
        string tempoTotale;
        public List<DatiRisultati> lstdatirisultati = new List<DatiRisultati>();
        float coefSb;
        float coefEs;
        float risultatoFinale;
        private int contST;
        private int contET;
        private string v;
        public bool clickBtnRisultati = true;
        public float punteggioQuizVeloce;
        public float punteggioQuizVeloceSelezioneSlider;

        /** Costruttore
         * Al costruttore vengono passati i parametri dalle classi simulazione e simulazione assistita, questi parametri 
         * dopo vengono fatti visualizzare nelle label corrispondenti.
         * @param contS: è il parametro che conta le risposte sbagliate;
         * @param contE: è il parametro che conta le risposte sbagliate;
         * @param tempoTot: è il parametro che restituisce il tempo totale della simulazione;
         */
        public RisultatoSimulazione(int contS, int contE, string tempoTot, List<DatiRisultati> lstRisultati,bool flagPunteggio, int contNonRisposteTot)
        {
            InitializeComponent();
            btnRisultatiDettaglio.IsEnabled = clickBtnRisultati;
            this.contEs = contE;
            this.contNonRisp = contNonRisposteTot;
            this.contSb = contS;
            this.tempoTotale = tempoTot;
            this.lstdatirisultati = lstRisultati;
            
            float puntSb = QuizVeloce.valoreCorrezioneRispostaErrata;
            float puntEs = QuizVeloce.valoreCorrezioneRispostaEsatta;

            // Da modificare prelevandolo direttamente dal server nella modalità quizveloce
            int puntNonRisp = 0;

            if (coefSb > 0){
                risultatoFinale = ((contNonRisp * puntNonRisp) + (contEs * puntEs) - (contSb * puntSb));
            }
            else {
                risultatoFinale = ((contNonRisp * puntNonRisp) + (contEs * puntEs) + (contSb * puntSb));
            }

            LblTitolo.Text = "Ciao " + Utente.Instance.getNomeDiBattesimo + ". Hai terminato la simulazione, ecco il tuo risultato!";
            LabelEsatte.Text = contEs.ToString();
            LabelSbagliate.Text = contSb.ToString();
            LabelNonRisposte.Text = contNonRisp.ToString();
            LabelTempo.Text = tempoTotale.ToString();
            labelPunteggio.Text = risultatoFinale.ToString();

            int domandeTotaliSvolte = contE + contS + contNonRisp;

            // conteggioDomandeSvoltePerSimulazione a che serve?
            punteggioQuizVeloceSelezioneSlider =
                (100 * (contE * puntEs + contS * puntSb + contNonRisp * puntNonRisp)) /
                (conteggioDomandeSvoltePerSimulazione * puntEs);
            punteggioQuizVeloceSelezioneSlider = (punteggioQuizVeloceSelezioneSlider * QuizVeloce.numeroDomandeMassimoDelTestQuizVeloce) / 100;
            LabelPunteggioTotalizzato.Text = punteggioQuizVeloceSelezioneSlider.ToString();

            if (flagPunteggio==false)
            {

                stackPunteggio.IsVisible = false;
                stackPunteggioNormalizzato.IsVisible = false;
            }
        }

        private void Button_Clicked_1(object sender, System.EventArgs e)
        {
            btnRitornaHome.IsEnabled = false;
            Navigation.PopToRootAsync();
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            clickBtnRisultati = false;
            btnRisultatiDettaglio.IsEnabled = clickBtnRisultati;
            await  Navigation.PushAsync(new DettagliTest(lstdatirisultati));
            clickBtnRisultati = true;
            btnRisultatiDettaglio.IsEnabled = clickBtnRisultati;

        }
    }
}