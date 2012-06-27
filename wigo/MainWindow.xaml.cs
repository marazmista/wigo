
// copy marazmista 1.10.2011 //


// ================
// Main //
// ================


using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Collections;

using commonStrings;
using klasyakcjowe;

namespace mm_gielda
    {
    // ================
    // stałe używane w kodzie, żeby były w jednym miejscu //
    // ================
    public struct staleapki
        {
        public const string tmpdir = @"\tmp\";
        public const string bazadir = @"\baza\";
        public const string danedir = @"\dane\";
        public static readonly string appdir = Environment.CurrentDirectory;

        public const string formatDaty = "dd-MM-yyyy HH:mm:ss";
        public const byte iloscNaj = 6;  // ilość w tabelach wzrosty,spadki i najaktywniejsze na podsumowaniu //

        // powiedzmy że zmienna, do metody pobierającej składy indeksów //
        public static bool pierwszePobranie = true;
        }


    public partial class MainWindow : Window
        {
        // ================
        // cały nasz mainwindow, okienko //
        // ================
        public MainWindow()
            {
            try
                {
                InitializeComponent();
                DirCheck();
                }
            catch { };

            // obsługa zdarzenia logującego w klasie mainwindow //
            Loger.updateLogbox += new EventHandler(Loger_updateLogbox);
            Loger.updateTotalDown += new EventHandler(Loger_updateTotalDown);

            #region timery, deklaracja ====
            var timerGPW = new System.Windows.Threading.DispatcherTimer();
            timerGPW.Tick += new EventHandler(timerGPW_Tick);
            timerGPW.Interval = new TimeSpan(0, 2, 0);
            timerGPW.Start();

            var timerSwiat = new System.Windows.Threading.DispatcherTimer();
            timerSwiat.Tick += new EventHandler(timerSwiat_Tick);
            timerSwiat.Interval = new TimeSpan(0, 2, 0);
            timerSwiat.Start();

            var timerNewsy = new System.Windows.Threading.DispatcherTimer();
            timerNewsy.Tick += new EventHandler(timerNewsy_Tick);
            timerNewsy.Interval = new TimeSpan(0, 5, 0);
            timerNewsy.Start();
            #endregion

            Loger.dodajDoLogaInfo("Start logowania");
            }// zamkniecie maina

        #region === inne zdarzenia ===
        private void Window_Initialized(object sender, EventArgs e)
            {
            timerGPW_Tick(null, null);
            timerSwiat_Tick(null, null);
            timerNewsy_Tick(null, null);
            }

        // zdarzenie aktualizujace loga //
        void Loger_updateLogbox(object sender, EventArgs e)
            {
            Action append = delegate() { logbox.AppendText(Loger.returnLastLogLine() + "\n"); };
            this.Dispatcher.Invoke(append);
            }

        // zdarzenie aktualizujące labelkę z pobranymi danymi //
        void Loger_updateTotalDown(object sender, EventArgs e)
            {
            Action a = delegate() { totalDownL.Content = "Łącznie pobrano danych: " + Loger.returnTotalDownloaded().ToString(); };
            this.Dispatcher.Invoke(a);
            }
        #endregion

        //private void wiadomosciGrid_scrool_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //    {
        //    wiadomosciGrid_scrool.ScrollToVerticalOffset(wiadomosciGrid_scrool.VerticalOffset - e.Delta / 3);
        //    }

        }
    }
