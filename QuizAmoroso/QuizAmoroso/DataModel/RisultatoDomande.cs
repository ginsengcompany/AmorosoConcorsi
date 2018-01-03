using Xamarin.Forms;
 
namespace QuizAmoroso.DataModel
{
    public class RisultatoDomande
    {
        public string domanda { get; set; }
        public string risposta { get; set; }
        public string esito { get; set; }
        public Color color { get; set; }
        public bool lbl_RispostaUtente { get; set; } = true;
        public string risposta_utente { get; set; }
    }
}
