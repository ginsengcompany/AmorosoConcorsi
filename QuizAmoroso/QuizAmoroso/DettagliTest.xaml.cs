using QuizAmoroso.DataModel;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DettagliTest : ContentPage
    { 
       public List<DatiRisultati> lstdatirisultati = new List<DatiRisultati>();
        public DettagliTest(List<DatiRisultati> Risultati)

        {
            this.lstdatirisultati = Risultati;
            InitializeComponent();
            visualizza();
        }

        public void visualizza()
        {
            lstRisultati.ItemsSource = lstdatirisultati;
            lstRisultati.SelectedItem = Color.Blue;   
        }
    }
}