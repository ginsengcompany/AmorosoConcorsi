using System.Collections.Generic;
using Xamarin.Forms;

namespace QuizAmoroso.DataModel
{
    public class Domande
    { 
        public string Materia { get; set; }
        public string Sottocategoria { get; set; }
        public string Codice { get; set; }
        public string id_domanda { get; set; }
        public string Domanda { get; set; }
        public string Risposta { get; set; }
        public List<string> Quesiti { get; set; }
        public string tipo { get; set; }
        public string link { get; set; }
        public string NumeroRisposte { get; set; }
        public List<Quesiti> lstQuesiti { get; set; }
    }

    public class Quesiti
    {
        public FontAttributes attribute { get; set; }
        public Color colore { get; set; }
        public string quesito { get; set; }
        public string lettera { get; set; }
        public string visible { get; set; } = "false";

    }
}