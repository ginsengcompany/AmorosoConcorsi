using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAmoroso.DataModel
{
    public class VideoLezioni
    {
        public string Nome { set; get; }
        public string VideoSource { set; get; }
        public string Descrizione { set; get; }
        public string sottoCategoria { set; get; }
        public string Materia { set; get; }
    }
    public class MaterieVideo
    {
        public string Materia { set; get; }
    }
}
