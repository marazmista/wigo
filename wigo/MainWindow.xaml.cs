
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
using System.Windows.Threading;
using System.Threading;
using System.Linq;

using commonStrings;
using klasyakcjowe;

namespace mm_gielda
    {
    // ================
    // stałe używane w kodzie, żeby były w jednym miejscu //
    // ================
    public struct staleapki
        {
        public const string tmpDir = @"\tmp\";
        public const string tmpWykresDir = tmpDir + @"tmpWykresy\";
        public const string bazaDir = @"\baza\";
        public const string daneDir = @"\dane\";
        public const string pdfDir = tmpDir + @"pdfAnalizy\";
        public static readonly string appDir = Environment.CurrentDirectory;

        public const string formatDaty = "dd-MM-yyyy HH:mm:ss";
        public const byte iloscNaj = 6;  // ilość w tabelach wzrosty,spadki i najaktywniejsze na podsumowaniu //

        // stałe czasu, po kótym nie pobierać nowych danych z gpw //
        public const byte nieOdswGPWPoGodz = 17;
        public const byte nieOdswGPWPoMin = 50;
        public static readonly DateTime dataUruchmienia = DateTime.Now.Date;

        // powiedzmy że zmienna, do metody pobierającej składy indeksów //
        public static bool pierwszePobranie = true;
        }

    public partial class MainWindow: Window
        {

        //timery
        DispatcherTimer timerGPW = new DispatcherTimer();
        DispatcherTimer timerSwiat = new DispatcherTimer();
        DispatcherTimer timerNewsy = new DispatcherTimer();

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
            }// zamkniecie maina

        void setTimers()
            {
            timerGPW.Tick += new EventHandler(timerGPW_Tick);
            timerGPW.Interval = new TimeSpan(0, config.gpwInterwal, 0);

            timerSwiat.Tick += new EventHandler(timerSwiat_Tick);
            timerSwiat.Interval = new TimeSpan(0, config.swiatInterwal, 0);

            timerNewsy.Tick += new EventHandler(timerNewsy_Tick);
            timerNewsy.Interval = new TimeSpan(0, config.newsyInterwal, 0);

            // jesli odpalonew w weekend, to nie startuj timerów, jedynie wczytaj w WindowInitialized tabele jednokrotnie //
            if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday | DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
                {
                timerGPW.Start();
                timerSwiat.Start();
                timerNewsy.Start();
                }
            }

        //Action<Ellipse, Label> checkWork = (elipsa, labelka) =>
        //    //void checkWork(Ellipse elipsa,Label labelka)
        //    {
        //        if (envVar.akcjeTrwaOdsw == true)
        //            {
        //            do
        //                {
        //                labelka.Content = "Pracuje...";
        //                elipsa.Fill = Brushes.Red;
        //                elipsa.Visibility = (elipsa.IsVisible) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
        //                Thread.Sleep(500);
        //                }
        //            while (envVar.akcjeTrwaOdsw == true);
        //            }
        //        else
        //            {
        //            labelka.Content = "Gotowe";
        //            elipsa.Fill = Brushes.Green;
        //            elipsa.Visibility = System.Windows.Visibility.Visible;
        //            }
        //    };

        //void checW()
        //    {
        //    this.Dispatcher.BeginInvoke(checkWork, DispatcherPriority.Background, workElipse, workLabel);
        //    }

        #region === inne zdarzenia ===
        private void Window_Initialized(object sender, EventArgs e)
            {
            // obsługa zdarzenia logującego w klasie mainwindow //
            Loger.updateLogbox += new EventHandler(Loger_updateLogbox);
            Loger.updateTotalDown += new EventHandler(Loger_updateTotalDown);

            Loger.dodajDoLogaInfo("Start logowania");

            // załączenie timerów //
            setTimers();

            // wczytanie configa jeśli jest //
            if (File.Exists(configFilePath))
                {
                wczytajOpcje();
                Loger.dodajDoLogaInfo("Wczytano ustawienia");
                }
            else
                Loger.dodajDoLogaInfo("Brak pliku ustawień, wczytano domyślne");

            // odpalenie ticka, żeby wczytać tabele na starcie //
            timerGPW_Tick(null, null);
            timerSwiat_Tick(null, null);
            timerNewsy_Tick(null, null);
            }

        // zdarzenie aktualizujace loga //
        void Loger_updateLogbox(object sender, EventArgs e)
            {
            this.Dispatcher.Invoke(new Action(() => { logbox.AppendText(Loger.returnLastLogLine() + "\n"); }));
            }

        // zdarzenie aktualizujące labelkę z pobranymi danymi //
        void Loger_updateTotalDown(object sender, EventArgs e)
            {
            this.Dispatcher.Invoke(new Action(() => { totalDownL.Content = "Łącznie pobrano danych: " + Loger.returnTotalDownloaded().ToString(); }));
            }
        #endregion
        }
    }