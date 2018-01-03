using System;
using Xamarin.Forms;

namespace QuizAmoroso.DataModel
{
    public class Costanti
    {
        /**
         * Link ai servizi esposti lato piattaforma
         * */
        public const string sitoAK12 = 
            "http://www.ak12srl.it";

        public const string paginaFacebook = 
            "https://www.facebook.com/centro.militari/?fref=ts";

        public const string urlLocation =
                "https://www.google.it/maps/place/Viale+Italia,+53,+81020+San+Nicola+La+Strada+CE/@41.0507431,14.3264049,17z/data=!3m1!4b1!4m5!3m4!1s0x133a551ccb708adb:0xb134a991e304809a!8m2!3d41.0507431!4d14.3285936";

        public const string urlBase = 
            "https://amorosoconcorsi.ak12srl.it/services/";

        public const string domandeNew = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/domandeNew";

        public const string login = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/login";

        public const string logout = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/logout";

        public const string domconcorsorandomNew = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/domconcorsorandomNew";

        public const string domconcorsosequenzaNew = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/domconcorsosequenzaNew";

        public const string sessione = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/sessione";

        public const string sessionePerTuttiConcorsi = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/sessionepertutticoncorsi";

        public const string pianoformativo = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/pianoformativo";

        public const string setdomande = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/setdomande";

        public const string concorsi = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/concorsi";

        public const string materieconcorso = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/materieconcorso";

        public const string statisticheURL = 
            "https://amorosoconcorsi.ak12srl.it/services/statistiche/homes.html?username=";

        public const string cronologia = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/cronologia";

        public const string commenti = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/commenti";

        public const string materietotali = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/materietotali";

        public const string domconcorsorandomtotaliNew = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/domconcorsorandomtotaliNew";

        public const string invioTempiGlobali = 
            "https://amorosoconcorsi.ak12srl.it/services/servizioapp/invioTempi";

        /**
         * Costanti di servizio
         * */
        public static char[] alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /**
         * Costanti per il limite di domande ammesse per eseguire il test 
         * sulle domande contenute sull'intera banca dati amministratore
         * */
        public const int numeroMassimoDomandeAmmesso = 150;
        public const int numeroMassimoDomandeDelTestSuInteroDB = 100;

        public const string eseguiTestSuInteroDb = "Esercitazione su tutti i Concorsi";
    }
}