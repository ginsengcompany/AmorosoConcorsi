using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace QuizAmoroso.Model
{
    public class Timer
    {
        // Timer tempo totale simulazione
        private Stopwatch tempoGlobaleSimulazione = new Stopwatch();
        private TimeSpan tempoSimulazione;
        public string tempoTotaleSimulazione;
        private bool avviaTempoSimulazione;

        // Timer tempo totale simulazione assistita
        private Stopwatch tempoGlobaleSimulazioneAssistita = new Stopwatch();
        private TimeSpan tempoSimulazioneAssistita;
        public string tempoTotaleSimulazioneAssistita;
        private bool avviaTempoSimulazioneAssistita;

        // Timer tempo totale dispensa
        private Stopwatch tempoGlobaleDispensa = new Stopwatch();
        private TimeSpan tempoDispensa;
        public string tempoTotaleDispensa;
        private bool avviaTempoDispensa;

        // Timer tempo totale apprendimento
        private Stopwatch tempoGlobaleApprendimento = new Stopwatch();
        private TimeSpan tempoApprendimento;
        public string tempoTotaleApprendimento;
        private bool avviaTempoApprendimento;

        /**
         * TEMPO TRASCORSO IN MODALITA SIMULAZIONE
         * */
        public void TempoSimulazione(bool start)
        {
            avviaTempoSimulazione = start;
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                tempoGlobaleSimulazione.Start();
                tempoSimulazione = tempoGlobaleSimulazione.Elapsed;
                tempoTotaleSimulazione = string.Format("{0:00}:{1:00}:{2:00}", tempoSimulazione.Hours, tempoSimulazione.Minutes, tempoSimulazione.Seconds);
                return avviaTempoSimulazione;
            });
        }

        public void FermaTempoSimulazione()
        {
            tempoGlobaleSimulazione.Stop();
        }

        public void ResetTempoSimulazione()
        {
            avviaTempoSimulazione = false;
            tempoGlobaleSimulazione.Reset();
        }

        public void RestartTempoSimulazione()
        {
            avviaTempoSimulazione = true;
            tempoGlobaleSimulazione.Restart();
        }

        /**
         * TEMPO TRASCORSO IN SIMULAZIONE ASSISTITA
         * */
        public void TempoSimulazioneAssistita(bool start)
        {
            avviaTempoSimulazioneAssistita = start;
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                tempoGlobaleSimulazioneAssistita.Start();
                tempoSimulazioneAssistita = tempoGlobaleSimulazioneAssistita.Elapsed;
                tempoTotaleSimulazioneAssistita = string.Format("{0:00}:{1:00}:{2:00}", tempoSimulazioneAssistita.Hours, tempoSimulazioneAssistita.Minutes, tempoSimulazioneAssistita.Seconds);
                return avviaTempoSimulazioneAssistita;
            });
        }

        public void FermaTempoSimulazioneAssistita()
        {
            tempoGlobaleSimulazioneAssistita.Stop();
        }

        public void ResetTempoSimulazioneAssistita()
        {
            avviaTempoSimulazioneAssistita = false;
            tempoGlobaleSimulazioneAssistita.Reset();
        }

        public void RestartTempoSimulazioneAssistita()
        {
            avviaTempoSimulazioneAssistita = true;
            tempoGlobaleSimulazioneAssistita.Restart();
        }

        /**
         * TEMPO TRASCORSO IN DISPENSA
         * */
        public void TempoDispensa(bool start)
        {
            avviaTempoDispensa = start;
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                tempoGlobaleDispensa.Start();
                tempoDispensa = tempoGlobaleDispensa.Elapsed;
                tempoTotaleDispensa = string.Format("{0:00}:{1:00}:{2:00}", tempoDispensa.Hours, tempoDispensa.Minutes, tempoDispensa.Seconds);
                return avviaTempoDispensa;
            });
        }

        public void FermaTempoDispensa()
        {
            tempoGlobaleDispensa.Stop();
        }

        public void ResetTempoDispensa()
        {
            avviaTempoDispensa = false;
            tempoGlobaleDispensa.Reset();
        }

        public void RestartTempoDispensa()
        {
            avviaTempoDispensa = true;
            tempoGlobaleDispensa.Restart();
        }

        /**
        * TEMPO TRASCORSO IN APPRENDIMENTO
        * */
        public void TempoApprendimento(bool start)
        {
            avviaTempoApprendimento = start;
            Device.StartTimer(TimeSpan.FromSeconds(0), () =>
            {
                tempoGlobaleApprendimento.Start();
                tempoApprendimento = tempoGlobaleApprendimento.Elapsed;
                tempoTotaleApprendimento = string.Format("{0:00}:{1:00}:{2:00}", tempoApprendimento.Hours, tempoApprendimento.Minutes, tempoApprendimento.Seconds);
                return avviaTempoApprendimento;
            });
        }

        public void FermaTempoApprendimento()
        {
            tempoGlobaleApprendimento.Stop();
        }

        public void ResetTempoApprendimento()
        {
            avviaTempoApprendimento = false;
            tempoGlobaleApprendimento.Reset();
        }

        public void RestartTempoApprendimento()
        {
            avviaTempoApprendimento = true;
            tempoGlobaleApprendimento.Restart();
        }
    }
}
