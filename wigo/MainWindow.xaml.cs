
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

        #region === wyswietlanie info obok tabel ====
        // Action wrzucający dane z tabeli do labelek // (dwie niżej to samo) //
         Action<Label, Label, Label, Label, Label, Label, Label, List<daneNumTabeli>, DataGrid, Image> selectedAction = (nazwa, data, kurs, zmiana, maxmin, otwarcie, odniesienie, tabela, grid, image) =>
             {
             int i = grid.SelectedIndex;

             nazwa.Content = tabela[i].Nazwa;
             data.Content = tabela[i].Data;
             kurs.Content = tabela[i].Kurs;
             zmiana.Content = tabela[i].Zmiana + " (" + tabela[i].ZmianaProc + ")";
             maxmin.Content = tabela[i].MaxMin;
             otwarcie.Content = tabela[i].Otwarcie;
             odniesienie.Content = tabela[i].Odniesienie;

             if (Convert.ToSingle(tabela[i].Zmiana) < 0.00)
                 zmiana.Foreground = Brushes.Red;
             else
                 zmiana.Foreground = Brushes.Green;

             if (Convert.ToSingle(tabela[i].Zmiana) == 0.00)
                 zmiana.Foreground = Brushes.Aqua;
             };

         Action<Label, Label, Label, Label, Label, Label, Label, Label, Label, Label, List<daneAkcji>, DataGrid, Image> selectedActionAkcje = (nazwa, data, kurs, zmiana, maxmin, otwarcie, odniesienie, obrot, transakcje, wolumen, tabela, grid, image) =>
             {
             int i = grid.SelectedIndex;

             nazwa.Content = tabela[i].Nazwa;
             data.Content = tabela[i].Data;
             kurs.Content = tabela[i].Kurs;
             zmiana.Content = tabela[i].Zmiana + " (" + tabela[i].ZmianaProc + ")";
             maxmin.Content = tabela[i].MaxMin;
             otwarcie.Content = tabela[i].Otwarcie;
             odniesienie.Content = tabela[i].Odniesienie;
             obrot.Content = tabela[i].Obrot;
             transakcje.Content = tabela[i].Transakcje;
             wolumen.Content = tabela[i].Wolumen;

             if (Convert.ToSingle(tabela[i].Zmiana) < 0.00)
                 zmiana.Foreground = Brushes.Red;
             else
                 zmiana.Foreground = Brushes.Green;

             if (Convert.ToSingle(tabela[i].Zmiana) == 0.00)
                 zmiana.Foreground = Brushes.Aqua;
            };

         Action<Label, Label, Label, Label, Label, Label, Label, List<daneWaluty>, DataGrid, Image> selectedActionWaluty = (nazwa, data, kurs, zmiana, maxmin, otwarcie, odniesienie, tabela, grid, image) =>
             {
             int i = grid.SelectedIndex;

             nazwa.Content = tabela[i].Nazwa;
             data.Content = tabela[i].Data;
             kurs.Content = tabela[i].Kurs;
             zmiana.Content = tabela[i].Zmiana + " (" + tabela[i].ZmianaProc + ")";
             maxmin.Content = tabela[i].MaxMin;
             otwarcie.Content = tabela[i].Otwarcie;
             odniesienie.Content = tabela[i].Odniesienie;

             if (Convert.ToSingle(tabela[i].Zmiana) < 0.00)
                 zmiana.Foreground = Brushes.Red;
             else
                 zmiana.Foreground = Brushes.Green;

             if (Convert.ToSingle(tabela[i].Zmiana) == 0.00)
                 zmiana.Foreground = Brushes.Aqua;
             };
         private void indeksyGPWGrid_Selected(object sender, RoutedEventArgs e)
             {
             selectedAction(iNazwa, iData, iKurs, iZmiana, iMaxMin, iOtwarcie, iOdniesienie, daneTabel.tIndeksyGPW, indeksyGPWGrid, iImage);
             }

         private void indeksyGrid_Selected(object sender, RoutedEventArgs e)
             {
             selectedAction(iNazwa, iData, iKurs, iZmiana, iMaxMin, iOtwarcie, iOdniesienie, daneTabel.tIndeksy, indeksyGrid, iImage);
             }

         private void indeksyFutGrid_Selected(object sender, RoutedEventArgs e)
             {
             selectedAction(iNazwa, iData, iKurs, iZmiana, iMaxMin, iOtwarcie, iOdniesienie, daneTabel.tIndeksyFut, indeksyFutGrid, iImage);
             }

         private void akcjeGrid_Selected(object sender, RoutedEventArgs e)
             {
             selectedActionAkcje(aNazwa, aData, aKurs, aZmiana, aMaxMin, aOtwarcie, aOdniesienie, aObrot, aTransakcje, aWolumen, daneTabel.tAkcje, akcjeGrid, aImage);
             }

         private void walutyGrid_Selected(object sender, RoutedEventArgs e)
             {
             selectedActionWaluty(wtNazwa, wtData, wtKurs, wtZmiana, wtMaxMin, wtOtwarcie, wtOdniesienie, daneTabel.tWaluty, walutyGrid, wtImage);
             }

         private void towaryGrid_Selected(object sender, RoutedEventArgs e)
             {
             selectedAction(wtNazwa, wtData, wtKurs, wtZmiana, wtMaxMin, wtOtwarcie, wtOdniesienie, daneTabel.tTowary, towaryGrid, wtImage);
             } 
         #endregion

        //private void wiadomosciGrid_scrool_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //    {
        //    wiadomosciGrid_scrool.ScrollToVerticalOffset(wiadomosciGrid_scrool.VerticalOffset - e.Delta / 3);
        //    }


        }
    }
