using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Net.Http;
using QuizAmoroso.DataModel;

namespace QuizAmoroso
{
    /**
     * @Authors: Antonio Fabrizio Fiume, Alessio Calabrese, Antonio Saverio Valente
     * La page set domande implementa una list view, riempiendola con il titolo e la descrizione dei set di domande, 
     * preventivamente presi dal server tramite una connessione precedente avvenuta in Piani formativi, durante la login.
     *
     */

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetDomande : ContentPage
    {
        //variabile pubblica statica di tipo strutturajson
        public static DatiSetDomande setDomandeSelezionato = new DatiSetDomande();
        //variabile statica di tipo string, implementa il nome del set
        public static string nomeDelSet;
        // variabile di tipo stringa che restituisce il risulato della connessione al server
        private string resultContent = "";
        //variabile booleana che restituisce, se true, l'avvenimento con successo della connessione.
        private bool flag = false;

        //Costruttore, inizializza i componenti e il caricamento della list view.
        public SetDomande()
        {
            InitializeComponent();
            InizializzaCaricamentoCategoria();
        }

        /**
         * il metodo seguente inizializza la list view e la riempie con i campi nome set e descrizione
        */
        public void InizializzaCaricamentoCategoria()
        {
            StackActivityIndicator.IsVisible = false;
            if (PianoFormativo.risultatoSetDiDomandeAssociateAlPiano != "Impossibile connettersi al servizio")
            {
                List<DatiSetDomande> items = JsonConvert.DeserializeObject<List<DatiSetDomande>>(PianoFormativo.risultatoSetDiDomandeAssociateAlPiano);
                lstCategorie.ItemsSource = items; 
            }
        }

        /**
         * Il metodo seguente gestisce il touch della list view.
         * Quando l'elemento scelto dall'utente sarà "tappato" allora si visualizzerà un Activity indicator che sottoporrà l'utente ad un'attesa di pochi secondi,
         * per caricare la page modalità quiz.
         */
        private async void lstCategorie_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            lstCategorie.IsEnabled = false;

            var elementoTappato = e.Item as DatiSetDomande;

            setDomandeSelezionato.nome_set = elementoTappato.nome_set;
            setDomandeSelezionato.Descrizione = elementoTappato.Descrizione;
            setDomandeSelezionato.simulazione = elementoTappato.simulazione;
            setDomandeSelezionato.dispensa = elementoTappato.dispensa;
            setDomandeSelezionato.apprendimento = elementoTappato.apprendimento;
            setDomandeSelezionato.simulazione_assistita = elementoTappato.simulazione_assistita;

            try
            {
                StackActivityIndicator.IsVisible = true;
                caricamentoPagina.IsRunning = true;
                caricamentoPagina.IsVisible = true;

                lstCategorie.SelectedItem = Color.Blue;
                await Navigation.PushAsync(new ModalitaQuiz());

                caricamentoPagina.IsRunning = false;
                caricamentoPagina.IsVisible = false;
                StackActivityIndicator.IsVisible = false;
                lstCategorie.IsEnabled = true;
            }
            catch (Exception b)
            {
                await DisplayAlert("Attenzione", "Connessione non riuscita", "riprova");
            }
        }
    }
}