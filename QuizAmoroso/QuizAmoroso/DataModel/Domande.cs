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
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string Risposta { get; set; }
        public List<string> Quesiti { get; set; }
        public string tipo { get; set; }
        public string link { get; set; }
        public Color colorA { get; set; }
        public Color colorB { get; set; }
        public Color colorC { get; set; }
        public Color colorD { get; set; }
        public FontAttributes GrassettoA { get; set; }
        public FontAttributes GrassettoB { get; set; }
        public FontAttributes GrassettoC { get; set; }
        public FontAttributes GrassettoD { get; set; }
        public string SizeA { get; set; }
        public string SizeB { get; set; }
        public string SizeC { get; set; }
        public string SizeD { get; set; }
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