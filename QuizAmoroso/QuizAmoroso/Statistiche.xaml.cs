using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuizAmoroso.DataModel;

/**
 * Author: Antonio Fabrizio Fiume
 * 
 * Questa classe è stata pensata per riportare le statistiche dell'utente in una schermata di tipo
 * WebView. La scelta è stata fatta per snellire lo sviluppo utilizzando la tecnologia di ChartJS
 * */

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Statistiche : ContentPage
    {
        /**
         * Il costruttore inizializza sia il componente che la WebView
         * */
        public Statistiche()
        {
            InitializeComponent();
            /**
             * Viene indicato il link a cui la WebView punta. Si noti che viene fatta una post 
             * direttamente sulla pagina web homes.html
             * */
            Browser.Source = Costanti.statisticheURL + Utente.Instance.getUserName;
            LinkSitoWebAk12();
        }

        public void LinkSitoWebAk12()
        {
            var tapGestureLinkSito = new TapGestureRecognizer();
            tapGestureLinkSito.Tapped += (s, e) => {
                Device.OpenUri(new Uri(Costanti.sitoAK12));
            };
            logoFooter.GestureRecognizers.Add(tapGestureLinkSito);
        }
    }
}