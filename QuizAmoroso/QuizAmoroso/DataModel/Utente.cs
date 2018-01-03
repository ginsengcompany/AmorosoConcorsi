using Xamarin.Forms;

/*
 * @Author: Antonio Saverio Valente
 * 
 * Questa classe serve a tenere traccia dei dati personali di accesso e 
 * dei dati anagrafici dell'utente. Gestisce inoltre il salvataggio delle credenziali sul
 * dispositivo definendo delle classi capaci di memorizzare le informazioni
 * all'interno del dizionario dell'applicazione (Application.Current.Properties)
 * */

namespace QuizAmoroso.DataModel
{
    public class Utente
    {
        // Indica l'username dell'utente
        private string userName;
        // Indica la Password dell'utente
        private string password;
        // Indica il nome di battesimo dell'utente
        private string nomeDiBattesimo;
        // Costituisce la dichiarazione di un oggetto di tipo Utente
        private static Utente instance;
        private string devInfo;
        //Il bit di login assumerà valore 1 durante l'esecuzione della loginpage;
        private int bitLogin = 0;
        
        /**
         * @param us: parametro che indica l'username, serve per inizializzare l'oggetto
         * @param pass: è un parametro che viene passato per inizializzare la variabile di istanza password
         * dell'oggetto Utente
         * Il costruttore privato permette i generare un signleton della classe utente.
         * */
        private Utente(string us, string pass,string devInfo)
        {
            this.userName = us;
            this.password = pass;
            this.devInfo = devInfo;
        }

        public string getUserName { get; set ;}
        public string getPassword { get; set ;}
        public string getNomeDiBattesimo { get; set ;}
        public int getBitLogin { get; set ; }

        public string getDevInfo { get; set; }
        
        /**
         * Questo metodo genera un singleton della classe Utente
         * */
        public static Utente Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Utente("","","");
                }
                return instance;
            }
        }

        /**
         * Questo metodo permette di salvare le credenziali (in particolare l'username) dell'utente
         * all'interno del dizionario gestito dal metodo statico Application.Current.Properties.Add
         * */
        public void salvaCredenzialiAccesso(string user)
        {
            if (!string.IsNullOrWhiteSpace(user))
            {
                this.userName = user;
                Application.Current.Properties.Add("userName",user);
                Application.Current.SavePropertiesAsync();
            }
        }

        /**
         * Questo metodo permette di recuperare l'username utente dalla memoria interna.
         * */
        public string recuperaUserName()
        {
            string u;
            /**
             * Se è già presente una chiave userName in memoria viene recuperato il campo userName 
             * dal dizionario interno dell'applicazione che è stato in precedenza settatto 
             * al valore inserito in fase di login. 
             * Tale dizionario (Application.Current.Properties) viene usato per memorizzare i dati
             * sul dispositivo.
             * */
            if (Application.Current.Properties.ContainsKey("userName"))
            {
                u = Application.Current.Properties["userName"].ToString();
            } else
            {
                u = null;
            }
            return u;
        }

        /**
         * Questo metodo agisce in caso di aggiornamento da parte dell'utente dell'username.
         * In tal modo viene salvato il nuovo username inserito in memoria del telefono, in 
         * particolare all'interno dell'Application.Current...
         * */
        public void cancellaEdAggiornaUsername(string nuovo)
        {
            this.userName = nuovo;
            Application.Current.Properties.Clear();
            Application.Current.Properties.Add("userName", nuovo);
            Application.Current.SavePropertiesAsync();
        }
    }
}
