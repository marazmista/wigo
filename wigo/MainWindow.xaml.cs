
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

            #region timery, deklaracja ====
            var timerGPW = new System.Windows.Threading.DispatcherTimer();
            timerGPW.Tick += new EventHandler(timerGPW_Tick);
            timerGPW.Interval = new TimeSpan(0, 2, 0);
            timerGPW.Start();

            var timerSwiat = new System.Windows.Threading.DispatcherTimer();
            timerGPW.Tick += new EventHandler(timerSwiat_Tick);
            timerGPW.Interval = new TimeSpan(0, 2, 0);
            timerGPW.Start();

            var timerNewsy = new System.Windows.Threading.DispatcherTimer();
            timerGPW.Tick += new EventHandler(timerNewsy_Tick);
            timerGPW.Interval = new TimeSpan(0, 5, 0);
            timerGPW.Start(); 
            #endregion

            Loger.dodajDoLogaInfo("Start logowania");
            }// zamkniecie maina

        #region === zdarzenia timerowe ===
        void timerGPW_Tick(object sender, EventArgs e)
            {
            Action a = () => tabelujGPW();
            a.BeginInvoke(null, null);
            }

        public void timerSwiat_Tick(object sender, EventArgs e)
            {
            Action a = () => tabelujSwiat();
            a.BeginInvoke(null, null);
            }

        public void timerNewsy_Tick(object sender, EventArgs e)
            {
            Action a = () => tabelujNewsy();
            a.BeginInvoke(null, null);
            } 
        #endregion

        #region === inne zdarzenia ===
        private void Window_Initialized(object sender, EventArgs e)
            {
            timerGPW_Tick(null, null);
            timerSwiat_Tick(null, null);
            timerNewsy_Tick(null, null);
            }

        void Loger_updateLogbox(object sender, EventArgs e)
            {
            Action append = delegate() { logbox.AppendText(Loger.returnLastLogLine() + "\n"); };
            this.Dispatcher.Invoke(append);
            } 
        #endregion

        //private void wiadomosciGrid_scrool_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //    {
        //    wiadomosciGrid_scrool.ScrollToVerticalOffset(wiadomosciGrid_scrool.VerticalOffset - e.Delta / 3);
        //    }
        }
    }
