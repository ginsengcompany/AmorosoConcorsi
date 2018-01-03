using Plugin.DeviceInfo;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using QuizAmoroso.DataModel;
using System.Threading.Tasks;

/**
 * @Authors:Antonio Fabrizio Fiume, Alessio Calabrese, Antonio Saverio Valente
 * In questa page, sarà permesso all'utente di visualizzare le 
 * informazioni dell'attività "Amoroso Concorsi", saranno implementati 
 * i servizi di geolocalizzazione, si potrà, tramite una tapgesture, chiamare al numero di telefono direttamente
 * senza ricopiare il numero, mandare una mail, o iscriversi alla pagina facebook.
 */
namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
  
    public partial class Info : ContentPage
    {   //COSTRUTTORE
        /**
         * Inizializza la mappa che geolocalizzerà l'attività "Amoroso Concorsi" e anche 
         * l'utente che utilizza l'applicazione(Avendo opportunamente attivato il servizio di geolocalizzazione), 
         * e le label di informazioni.
         **/
        public Info()
        {
            InitializeComponent();
            labelLuogo.FormattedText = "Viale Italia n° 53"+"\n"+"San Nicola La Strada (CE)";
            labelBenvenuto.FormattedText = "BENVENUTO " + Utente.Instance.getNomeDiBattesimo;
           LabelInformazioneLog.Text=  Utente.Instance.getNomeDiBattesimo+ " sei connesso con il dispositivo " + CrossDeviceInfo.Current.Model;
            
            var tapGestureLuogo = new TapGestureRecognizer();
            tapGestureLuogo.Tapped += (s, e) =>
            {
                Device.OpenUri(new Uri(Costanti.urlLocation));
            };
            labelLuogo.GestureRecognizers.Add(tapGestureLuogo);

            var tapGestureWebSite = new TapGestureRecognizer();
            tapGestureWebSite.Tapped += (s, e) =>
            {
                Device.OpenUri(new Uri("https://www.amorosoconcorsi.com/"));
            };
            sitoWeb.GestureRecognizers.Add(tapGestureWebSite);

            /* La variabile tap gesture Phone ci permetterà di cliccare sul numero di telefono e chiamare il negozio in questione*/
            var tapGesturePhone = new TapGestureRecognizer();
            tapGesturePhone.Tapped += (s, e) => {
                    Device.OpenUri(new Uri(String.Format("tel:{0}", "08231545081")));
                };
           numeroTelefonoFisso.GestureRecognizers.Add(tapGesturePhone);

            var tapGestureCellulare = new TapGestureRecognizer();
            tapGestureCellulare.Tapped += (s, e) => {
                Device.OpenUri(new Uri(String.Format("tel:{0}", "3925224680")));
            };
            numeroCellulareUno.GestureRecognizers.Add(tapGestureCellulare);

            var tapGestureCellulareDue = new TapGestureRecognizer();
            tapGestureCellulareDue.Tapped += (s, e) => {
                Device.OpenUri(new Uri(String.Format("tel:{0}", "3474856700")));
            };
            numeroCellulareDue.GestureRecognizers.Add(tapGestureCellulareDue);
            /* La variabile tap gesture Facebook ci permetterà di cliccare e navigare sulla pagina facebook del negozio in questione*/
            var tapGestureFacebook = new TapGestureRecognizer();
            tapGestureFacebook.Tapped += (s, e) => {
                Device.OpenUri(new Uri(Costanti.paginaFacebook));
            };

           facebook.GestureRecognizers.Add(tapGestureFacebook);
            /* La variabile tap gesture mail ci permetterà di cliccare e scrivere una mail al negozio in questione*/
            var tapGestureMail = new TapGestureRecognizer();
            tapGestureMail.Tapped += (s, e) => {
                Device.OpenUri(new Uri(String.Format("mailto:{0}", "cfpcm@hotmail.it")));
            };
            LinkSitoWebAk12();
           indirizzoMail.GestureRecognizers.Add(tapGestureMail);
            /* La variabile positon ci permetterà di indicare sulla mappa l'indirizzo esatto del negozio*/
            
        }
       

        public void LinkSitoWebAk12()
        {
            var tapGestureLinkSito = new TapGestureRecognizer();
            tapGestureLinkSito.Tapped += (s, e) => {
                Device.OpenUri(new Uri(Costanti.sitoAK12));
            };
           logoFooter.GestureRecognizers.Add(tapGestureLinkSito);
        }

        private async void LogOut_Clicked(object sender, EventArgs e)
        {
            /**
             * In questa parte di codice si controllerà se il bit di login in ogni pagina 
             * è zero o uno. 
             * Se il bit assume valore uguale 1 allora il sistema non effettuerà il logout.
             * Si ricora che il bit di login vale 1 solo quando è attiva la pagina di login e 
             * solo quando viene eseguita una onResume (ovvero quando l'app viene richiamata 
             * dal background). In questi casi (ossia quando Utente.Instance.getBitLogin == 1) 
             * non viene eseguito il logOut utente quando quando si va in modalità di onSleep 
             * ma viene aperta la pagina Login.
             * */
            
            if (Utente.Instance.getBitLogin == 1)
            {
                await Navigation.PushModalAsync(new Login());
            }
            else
            {
                await Navigation.PushAsync(new LogOut());
            }
        }
    }
}
//https://www.google.it/maps/place/Viale+Italia,+53,+81020+San+Nicola+La+Strada+CE/@41.0507431,14.3264049,17z/data=!3m1!4b1!4m5!3m4!1s0x133a551ccb708adb:0xb134a991e304809a!8m2!3d41.0507431!4d14.3285936