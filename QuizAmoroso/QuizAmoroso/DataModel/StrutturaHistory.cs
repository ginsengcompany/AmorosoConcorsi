using QuizAmoroso.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAmoroso
{
   public class StrutturaHistory
    {
        public string dataSessione { get; set; }
        public List<Sessioni> sessioni { get; set; }
    }
}
