using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAmoroso.DataModel
{
    public class Sessioni
    {
        public string idSessione { get; set; }
        public string idConcorso { get; set; }
        public string corpoConcorso { get; set; }
        public string codiceConcorso { get; set; }
        public string oraSessione { get; set; }
        public string nomeSet { get; set; }
        public List<RisultatoDomande> domande {get; set;}
    }
}
