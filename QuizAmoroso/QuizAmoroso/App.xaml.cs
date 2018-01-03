using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace QuizAmoroso
{
    /**
     * Author: Antonio Fabrizio Fiume
     * 
     * Questa classe inizializza l'applicazione e ne gestisce il passaggio di stati (onSleep,
     * onResume, onStart).
     * */
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Indica la prima pagina che verrà visualizzata 
            MainPage = new Login();
        }

        protected override void OnStart()
        {

        }

        // Handle when your app sleeps
        protected override void OnSleep()
        {
           
        }

        // Handle when your app resumes
        protected override void OnResume()
        {
            
        }
    }
}
