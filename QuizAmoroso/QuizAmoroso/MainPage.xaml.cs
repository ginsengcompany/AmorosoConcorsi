using QuizAmoroso.DataModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuizAmoroso
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    /**
     * @Authors: Antonio Fabrizio Fiume, Alessio Calabrese, Antonio Saverio Valente.
     * La Page seguente implementa il menu tabbed, che permetterà all'utente 
     * di spostarsi tra le page in maniera facile e veloce;
     */
    public partial class MainPage : TabbedPage
    {
        
        public MainPage()
        {
            Utente.Instance.getBitLogin = 0;
            InitializeComponent();
            
        }
    }
}