using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAmoroso.DataModel
{       
    /**
         * Creiamo una classe struttura Json, dove strutturiamo il json che 
         * ci servirà per inviare dei campi al server, 
         * in questo caso il nome set
    */
    public class DatiSetDomande
    {
        public string nome_set { get; set; }
        public string Descrizione { get; set; }
        public string simulazione { get; set; }
        public string dispensa { get; set; }
        public string simulazione_assistita { get; set; }
        public string apprendimento { get; set; }
    }
}
