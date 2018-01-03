using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using QuizAmoroso.DataModel;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    /**
     * Author:: Antonio Saverio Valente
     * 
     * Questa classe gestisce la fase di logout dal sistema andando a richiedere al server
     * di disconnettere il device portando il bit che tiene traccia degli utenti connessi sul
     * sistema a zero.
     * */
    public partial class LogOut : ContentPage
    {
        // Variabile di ritorno della post
        private string resultContent;
        // Variabile che tiene traccia del fatto che la post sia andata a buon fine
        private bool flag = false;

        public LogOut()
        {
            InitializeComponent();
            logOut();
        }

        /**
         * Questo metodo di logout ci permetterà di disconnetterci dal sistema.
         * In questo modo è possibile far sapere al server che l'utente ha finito la sessione 
         * di utilizzo dell'app
         * */
        public async void logOut()
        {
            var client = new HttpClient();
            /** 
             * Ci si connette con il server, si inviano username e password, si aspetta la risposta, 
             * se tutto andrà positivamente allora il sistema ti disconnetterà
             * */
            try
            {
                var values = new List<KeyValuePair<string, string>>();
                values.Add(new KeyValuePair<string, string>("username", Utente.Instance.getUserName));
                values.Add(new KeyValuePair<string, string>("password", Utente.Instance.getPassword));
                values.Add(new KeyValuePair<string, string>("devInfo", Utente.Instance.getDevInfo));
                var content = new FormUrlEncodedContent(values);
                var result = await client.PostAsync(Costanti.logout, content);
                resultContent = await result.Content.ReadAsStringAsync();
                if (resultContent.ToString() == "Logout non riuscito")
                {
                    flag = true;
                }
                else if (resultContent.ToString() == "Logout effettuato con successo")
                {
                    flag = false;
                    await DisplayAlert("Attenzione!", "Sei stato disconnesso dal sistema!", "OK");
                    var Mainpage = new Login();
                   await Navigation.PopModalAsync();
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Errore", "Non è stato possibile effettuare il Log Out!", "riprova");
            }
        }
    }
}