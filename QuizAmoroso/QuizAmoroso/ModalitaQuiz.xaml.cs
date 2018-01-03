using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    /** 
     * Author: Antonio Saverio Valente
     * 
     * In questa classe l'utente potrà selezionare la modalità di esecuzione dei quiz.
     * Le modalità selezionabili sono imposte all'pplicazione dal sistema.
     * Le modalità vengono scelta dal deocente che ha creato il piano formativo.
     * */
    public partial class ModalitaQuiz : ContentPage
    {
        // Variabile che indica la scelta della modalità eseguita dall'utente
        private int sceltaModalita = 0;
        // Queste variabili corrispondono a quanto viene restituito dal sistema ovvero a quanto imposto dal docente 
        // Durante il prelievo dei set domande vengono anche prelevate le modalità associate
        public string simulazione = SetDomande.setDomandeSelezionato.simulazione;
        public string simulazione_assistita = SetDomande.setDomandeSelezionato.simulazione_assistita;
        public string apprendimento = SetDomande.setDomandeSelezionato.apprendimento;
        public string dispensa = SetDomande.setDomandeSelezionato.dispensa;

        public ModalitaQuiz()
        {
            InitializeComponent();
            VisualizzazioneModalita();
            // Viene mostrato in una label il nome del set di domande selezionato in precedenza
            labelTitoloSet.Text = SetDomande.setDomandeSelezionato.nome_set;
        }

        /* *
         * Questo metodo si attiverà se l'utente selezionerà la modalità dispensa.
         * Appena avrà cliccato si verrà reinderizzati verso la page dispensa.
         * */
        private async void btnDispensa_Clicked(object sender, EventArgs e)
        {
            // Viene mostrta la label all'interno di un frame usato per la descrizione della modalità
            TitoloModalita.Text = "Dispensa";
            // Rende visibile il titolo nel frame
            TitoloModalita.IsVisible = true;
            // Corrisponde ad una descrizione del tipo di modalità
            labelSpiegazione.Text = "In questa modalità è possible sfogliare l'intero set di domande per apprenderne le risposte esatte.";
            // La label spiegazione viene messa a visibile
            labelSpiegazione.IsVisible = true;
            // Abilita un bottone che permette di proseguire all'esecuzione della modalità
            BottoneAvanti.IsEnabled = true;
            // E' una label che mostra il tempo trascorso, viene messa ad invisibile 
            lblTempo.IsVisible = false;
            // Viene messo a visibile il bottone che permette di proseguire
            BottoneAvanti.IsVisible = true;
            // La label del frame viene messa a visibile
            frameLabel.IsVisible = true;
            // Indica la modalità scelta
            sceltaModalita = 1;
        }

        /* * 
         * Questo metodo si attiverà se l'utente selezionerà la modalità simulazione.
         * Appena avrà cliccato si verrà reinderizzati verso la page simulazione.
         * In questo frammento di codice verranno disabilitati gli altri button, 
         * per evitare eventuali crash dell'app.
         * Per i commenti di dettaglio si veda la descrizione del metodo btnDispensa_Clicked
         * */
        private async void btnSimulazione_Clicked(object sender, EventArgs e)
        {
            TitoloModalita.Text = "Simulazione";
            TitoloModalita.IsVisible = true;
            labelSpiegazione.Text = "In questa modalità è possibile eseguire una simulazione d'esame del test, pertanto selezionata una risposta automaticamente si passa alla successiva. *Nota: premendo il tasto back in alto a sinistra le statistiche saranno contate.";
            lblTempo.Text = "In questa modalità è previsto una misura cronometrica dei tempi di risposta";
            lblTempo.IsVisible = true;
            labelSpiegazione.IsVisible = true;
            BottoneAvanti.IsEnabled = true;
            BottoneAvanti.IsVisible = true;
            frameLabel.IsVisible = true;
            sceltaModalita = 2;
        }

        /* * 
         * Questo metodo si attiverà se l'utente selezionerà la modalità apprendimento.
         * Appena avrà cliccato si verrà reinderizzati verso la page apprendimento. 
         * In questo frammento di codice verranno disabilitati gli altri button, 
         * per evitare eventuali crash dell'app.
         * Per i commenti di dettaglio si veda la descrizione del metodo btnDispensa_Clicked.
         * */
        private async void bntApprendimento_Clicked(object sender, EventArgs e)
        {
            TitoloModalita.Text = "Apprendimento";
            TitoloModalita.IsVisible = true;
            labelSpiegazione.Text = "In questa modalità è possibile sfogliare tutte le domande con le relative risposte. La risposta esatta sarà evidenziata in verde";
            labelSpiegazione.IsVisible = true;
            lblTempo.IsVisible = false;
            BottoneAvanti.IsVisible = true;
            BottoneAvanti.IsEnabled = true;
            frameLabel.IsVisible = true;
            sceltaModalita = 3;
        }

        /** Questo metodo si attiverà se l'utente selezionerà la modalità simulazione assistita.
         * Appena l'utente avrà cliccato si verrà reinderizzati verso la page simulazione assistita.
         * In questo frammento di codice verranno disabilitati gli altri button, per evitare 
         * eventuali crash dell'app.
         * Per i commenti di dettaglio si veda la descrizione del metodo btnDispensa_Clicked
         * */
        private void bntSimulazioneAssistita_Clicked(object sender, EventArgs e)
        {
            TitoloModalita.Text = "Simulazione assistita";
            TitoloModalita.IsVisible = true;
            labelSpiegazione.Text = "In questa modalità è possibile eseguire una simulazione d'esame del test. Rispetto alla modalità Simulazione, in caso di errore, si avrà il vantaggio di vedere indicata la risposta esatta.";
            lblTempo.Text = "In questa modalità è prevista una misurazione del tempo totale di risposta";
            lblTempo.IsVisible = true;
            labelSpiegazione.IsVisible = true;
            BottoneAvanti.IsVisible = true;
            BottoneAvanti.IsEnabled = true;
            frameLabel.IsVisible = true;
            sceltaModalita = 4;
        }

        /**
         * Questo metodo gestisce il reindirizzamento verso la giusta pagina in funzione della
         * scelta del tipo di modalità con cui si vuole eseguire il test.
         * Dopo aver pigiato il bottone Avanti in base alla selezione si aprirà la pagina
         * corrispondente.
         * */
        private async void BottoneAvanti_Clicked(object sender, EventArgs e)
        {
            if (sceltaModalita == 1)
            {
                // Bottone Avanti disabilitato dopo il suo click
                BottoneAvanti.IsEnabled = false;
                // Viene lanciata la pagina Dispensa
                await Navigation.PushAsync(new DispensaNew());
            }
            else if (sceltaModalita == 2)
            {
                // Bottone Avanti disabilitato dopo il suo click
                BottoneAvanti.IsEnabled = false;
                // Viene lanciata la pagina Simulazione
                await Navigation.PushAsync(new SimulazioneNew());
            }
            else if (sceltaModalita == 3)
            {
                // Bottone Avanti disabilitato dopo il suo click
                BottoneAvanti.IsEnabled = false;
                // Viene lanciata la pagina Apprendimento
                await Navigation.PushAsync(new ApprendimentoNew());
            }
            else if (sceltaModalita == 4)
            {
                // Bottone Avanti disabilitato dopo il suo click
                BottoneAvanti.IsEnabled = false;
                // Viene lanciata la pagina Simulazione
                await Navigation.PushAsync(new SimulazioneAssistitaNew());
            }
            // Vengono disattivate tutte le label ed i bottoni presenti nella pagina
            labelSpiegazione.Text = "";
            labelSpiegazione.IsVisible = false;
            BottoneAvanti.IsVisible = false;
            frameLabel.IsVisible = false;
        }

        /**
         * Il metodo gestisce le modalità che l'untente può visualizzare sul proprio 
         * device, abilitando i bottoni che ne garantiscono l'accesso.
         * */
        private void VisualizzazioneModalita()
        {
            if (simulazione == "1")
            {
                btnSimulazione.IsVisible = true;
            }
            if (apprendimento == "1")
            {
                bntApprendimento.IsVisible = true;
            }
            if (dispensa == "1")
            {
                btnDispensa.IsVisible = true;
            }
            if (simulazione_assistita == "1")
            {
                bntSimulazioneAssistita.IsVisible = true;
            }
        }
    }
}