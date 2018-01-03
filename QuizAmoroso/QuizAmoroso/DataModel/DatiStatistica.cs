namespace QuizAmoroso.DataModel
{
    public class DatiStatistica
    {
        public bool rispostaEsattaYN { get; set; }
        public string codice { get; set; }
        public string materia { get; set; }
        public string sottocategoria { get; set; }
        public string data { get; set; }
        public string ora { get; set; }
        public string tempoRisposta { get; set; }
        public string nomeSet { get; set; }
        public string id_concorso { get; set; }
        public string risposta_utente { get; set; }
        public string username = Utente.Instance.getUserName;
    }
}
