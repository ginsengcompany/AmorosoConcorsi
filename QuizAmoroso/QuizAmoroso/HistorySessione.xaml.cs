using QuizAmoroso.DataModel;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistorySessione : ContentPage
    {
        public static List<RisultatoDomande> SessionedataSelezionata = new List<RisultatoDomande>();
        public HistorySessione()
        {
            InitializeComponent();
            VisualizzazioneSessioni();
        }

        public  void VisualizzazioneSessioni()
        {
            Historyquiz.SessionedataSelezionata.Reverse();
            lstCronologiaSessioni.ItemsSource = Historyquiz.SessionedataSelezionata; 
        }

        private async void lstCronologiaSessioni_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var elementoTappato = e.Item as Sessioni;
            SessionedataSelezionata = elementoTappato.domande;
            lstCronologiaSessioni.SelectedItem = Color.Blue;
            await Navigation.PushAsync(new HistoryDomande());
        }
    }
             
}