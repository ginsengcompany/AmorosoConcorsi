using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryDomande : ContentPage
    {
        public HistoryDomande()
        {
            InitializeComponent();
            VisualizzaDomandeHistory();
        }
        public void VisualizzaDomandeHistory()
        {
            lstCronologiaDomande.ItemsSource = HistorySessione.SessionedataSelezionata;
            foreach(var i in HistorySessione.SessionedataSelezionata)
            {
                if(i.esito=="0")
                {
                    i.esito = "errata";
                    i.color = Color.Red;
                }
                else if(i.esito=="1")
                {
                    i.esito = "esatta";
                    i.color = Color.Green;
                }
                if(i.risposta_utente == "Non Risposta")
                {
                    i.esito = "Saltata";
                    i.color = Color.FromHex("2196f3");
                }
                /*
                 * Da eliminare quando saranno attivi i servizi
                 * */
                if (String.IsNullOrEmpty(i.risposta_utente))
                {
                    i.lbl_RispostaUtente = false; 
                }
            }
        }
    }
}