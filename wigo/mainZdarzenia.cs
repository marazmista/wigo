
// copy marazmista 26.06.2012 //

// ================
// Rozbity main window, wyodrębnione zdarzenia //
// ================

using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Input;
using System.Linq;

using klasyakcjowe;
using commonStrings;
using klasyAnalizyPDF;
using System.Windows.Media.Imaging;

namespace mm_gielda
    {
    public partial class MainWindow
        {
        #region === zdarzenia timerowe ===
        void timerGPW_Tick(object sender, EventArgs e)
            {
            new Action(delegate() { tabelujGPW(); }).BeginInvoke(null, null);
            timerGPW.Interval = new TimeSpan(0, config.gpwInterwal, 0);
            }
        public void timerSwiat_Tick(object sender, EventArgs e)
            {
            new Action(delegate() { tabelujSwiat(); }).BeginInvoke(null, null);
            timerSwiat.Interval = new TimeSpan(0, config.swiatInterwal, 0);
            }

        public void timerNewsy_Tick(object sender, EventArgs e)
            {
            new Action(delegate() { tabelujNewsy(); }).BeginInvoke(null, null);
            timerNewsy.Interval = new TimeSpan(0, config.newsyInterwal, 0);
            }
        #endregion

        // pobieranie tego małego wykresu obok tabeli //
        void wczytajMalyWykres(string symbol, Image wpfImage, bool czyGPW)
            {
            string wykresPlik = staleapki.appDir + staleapki.tmpWykresDir + symbol + "_mWykres.png";

            // sprawdza czy pobiera coś z gpw, jeśli nie to pobiera zawsze, nie patrzy na godzinę //
            if (czyGPW)
                {
                if (czyTrwaSesja())    // jeśli trwa sesja to pobieraj
                    Pobierz(adresy.StooqMWykres + symbol.ToLower(), wykresPlik);
                else if (!File.Exists(wykresPlik))  // jeśli pliku nie ma to pobieraj
                    Pobierz(adresy.StooqMWykres + symbol.ToLower(), wykresPlik);
                else if ((File.GetCreationTime(wykresPlik) == DateTime.Now) & (File.GetCreationTime(wykresPlik).Hour > 9) & (File.GetCreationTime(wykresPlik).Hour <= 17))
                    Pobierz(adresy.StooqMWykres + symbol.ToLower(), wykresPlik); // jeśli plik jest z sesji, a sesji już nie ma to też pobieraj (wykres zamknięcia) //
                }
            else
                Pobierz(adresy.StooqMWykres + symbol.ToLower(), wykresPlik);

            try
                {
                BitmapImage bp = new BitmapImage(new Uri(wykresPlik));
                wpfImage.Source = bp;
                }
            catch { }
            }

        // zmiana koloru paska i ikonki(góra, dół, kwadrat) na bocznym infie //
        void KolorISymbol(string zmiana, Border border, Grid grid, Rectangle rect)
            {
            var kolorUp = Brushes.Green;
            var kolorDown = Brushes.Red;
            var kolorZero = Brushes.Aqua;

            if (Convert.ToSingle(zmiana) > 0.00)
                {
                if (border != null)
                    border.Background = kolorUp;
                if (grid != null)
                    grid.Background = kolorUp;

                rect.Fill = (ImageBrush)Resources["up"];
                }
            else
                {
                if (border != null)
                    border.Background = kolorDown;
                if (grid != null)
                    grid.Background = kolorDown;

                rect.Fill = (ImageBrush)Resources["down"];
                };

            if (Convert.ToSingle(zmiana) == 0.00)
                {
                if (border != null)
                    border.Background = kolorZero;
                if (grid != null)
                    grid.Background = kolorZero;
                rect.Fill = (ImageBrush)Resources["zero"];
                }
            }

        #region === zdarzenie wczytujace dane obok tabel po wcisnieciu na gridzie ===
        void wczytajAkcjeDetails(List<daneAkcji> tabela, DataGrid grid, bool czyAkcja)
            {
            int i = grid.SelectedIndex;

            // sprawdza czy wczytuje akcje, czy indeks gpw (inne nazwy labelek) //
            if (czyAkcja)
                {
                aNazwa.Content = tabela[i].Nazwa;
                aData.Content = tabela[i].Data;
                aKurs.Content = tabela[i].Kurs;
                aZmiana.Content = tabela[i].Zmiana + " (" + tabela[i].ZmianaProc + ")";
                aMaxMin.Content = tabela[i].MaxMin;
                aOtwarcie.Content = tabela[i].Otwarcie;
                aOdniesienie.Content = tabela[i].Odniesienie;
                aWolumen.Content = tabela[i].Wolumen;
                aObrot.Content = tabela[i].Obrot;
                aTransakcje.Content = tabela[i].Transakcje;
                KolorISymbol(tabela[i].Zmiana, aBorder, null, aRect);

                aWykres.Source = null;
                this.Dispatcher.BeginInvoke(new Action(delegate() { wczytajMalyWykres(tabela[i].Symbol, aWykres, true); }), DispatcherPriority.Background);
                }
            else
                {
                iNazwa.Content = tabela[i].Nazwa;
                iData.Content = tabela[i].Data;
                iKurs.Content = tabela[i].Kurs;
                iZmiana.Content = tabela[i].Zmiana + " (" + tabela[i].ZmianaProc + ")";
                iMaxMin.Content = tabela[i].MaxMin;
                iOtwarcie.Content = tabela[i].Otwarcie;
                iOdniesienie.Content = tabela[i].Odniesienie;
                iWolumen.Content = tabela[i].Wolumen;
                iObrot.Content = tabela[i].Obrot;
                iTransakcje.Content = tabela[i].Transakcje;
                KolorISymbol(tabela[i].Zmiana, iBorder, null, iRect);

                iWykres.Source = null;
                this.Dispatcher.BeginInvoke(new Action(delegate() { wczytajMalyWykres(tabela[i].Symbol, iWykres, true); }), DispatcherPriority.Background);
                }
            }

        void wczytajInneDetails(List<daneNumTabeli> tabela, DataGrid grid, bool czyIndeks)
            {
            int i = grid.SelectedIndex;

            // sprawdza czy wczytuje indeks, czy walute albo towar (indeksyGPW które sa daneAkcji a nie daneNumTabeli to powodują)
            if (!czyIndeks)
                {
                wtNazwa.Content = tabela[i].Nazwa;
                wtData.Content = tabela[i].Data;
                wtKurs.Content = tabela[i].Kurs;
                wtZmiana.Content = tabela[i].Zmiana + " (" + tabela[i].ZmianaProc + ")";
                wtMaxMin.Content = tabela[i].MaxMin;
                wtOtwarcie.Content = tabela[i].Otwarcie;
                wtOdniesienie.Content = tabela[i].Odniesienie;
                KolorISymbol(tabela[i].Zmiana, wtBorder, null, wtRect);

                wtWykres.Source = null;
                this.Dispatcher.BeginInvoke(new Action(delegate() { wczytajMalyWykres(tabela[i].Symbol, wtWykres, false); }), DispatcherPriority.Background);
                }
            else
                {
                iNazwa.Content = tabela[i].Nazwa;
                iData.Content = tabela[i].Data;
                iKurs.Content = tabela[i].Kurs;
                iZmiana.Content = tabela[i].Zmiana + " (" + tabela[i].ZmianaProc + ")";
                iMaxMin.Content = tabela[i].MaxMin;
                iOtwarcie.Content = tabela[i].Otwarcie;
                iOdniesienie.Content = tabela[i].Odniesienie;
                KolorISymbol(tabela[i].Zmiana, iBorder, null, iRect);

                iWykres.Source = null;
                this.Dispatcher.BeginInvoke(new Action(delegate() { wczytajMalyWykres(tabela[i].Symbol, iWykres, false); }), DispatcherPriority.Background);
                }
            }

        private void akcjeGrid_Selected(object sender, RoutedEventArgs e)
            {
            //this.Dispatcher.BeginInvoke(new Action(() => wczytajAkcjeDetails(daneTabel.tAkcje, akcjeGrid, true)));
            wczytajAkcjeDetails(daneTabel.tAkcje, akcjeGrid, true);
            }

        private void indeksyGPWGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tIndeksyGPW, indeksyGPWGrid, false);

            iWolumen.IsEnabled = true;
            iObrot.IsEnabled = true;
            iTransakcje.IsEnabled = true;
            }

        private void indeksyGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajInneDetails(daneTabel.tIndeksy, indeksyGrid, true);

            iWolumen.IsEnabled = false;
            iObrot.IsEnabled = false;
            iTransakcje.IsEnabled = false;
            }

        private void indeksyFutGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajInneDetails(daneTabel.tIndeksyFut, indeksyFutGrid, true);

            iWolumen.IsEnabled = false;
            iObrot.IsEnabled = false;
            iTransakcje.IsEnabled = false;
            }

        private void walutyGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajInneDetails(daneTabel.tWaluty, walutyGrid, false);
            }

        private void towaryGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajInneDetails(daneTabel.tTowary, towaryGrid, false);
            }

        private void wigGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWig, wigGrid, true);
            }

        private void wig20Grid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWig20, wig20Grid, true);
            }

        private void mwig40Grid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tmWig40, mwig40Grid, true);
            }

        private void swig80Grid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tsWig80, swig80Grid, true);
            }

        private void wigbankiGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigBanki, wigbankiGrid, true);
            }

        private void wigbudowGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigBudow, wigbudowGrid, true);
            }

        private void wigceeGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigCee, wigceeGrid, true);
            }

        private void wigchemiaGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigChemia, wigchemiaGrid, true);
            }

        private void wigdewelGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigDewel, wigdewelGrid, true);
            }

        private void wigenergGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigEnerg, wigenergGrid, true);
            }

        private void wiginfoGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigInfo, wiginfoGrid, true);
            }

        private void wigmediaGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigMedia, wigmediaGrid, true);
            }

        private void wigpaliwaGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigPaliwa, wigpaliwaGrid, true);
            }

        private void wigplusGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigPlus, wigplusGrid, true);
            }

        private void wigpolandGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigPoland, wigpolandGrid, true);
            }

        private void wigspozywGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigSpozyw, wigspozywGrid, true);
            }

        private void wigsurowcGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigSurowc, wigsurowcGrid, true);
            }

        private void wigtelkomGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigTelkom, wigtelkomGrid, true);
            }

        private void wigukrainGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tWigUkrain, wigukrainGrid, true);
            }

        private void respectGrid_Selected(object sender, RoutedEventArgs e)
            {
            wczytajAkcjeDetails(daneTabel.tRespect, respectGrid, true);
            }
        #endregion

        #region === pobieranie list analiz ===
        // po wcisnięciu przycisku dla pobrania listy, wczytanie do tabeli //
        private void newsletterPobListe(object sender, RoutedEventArgs e)
            {
            tabelujNewsletteryGPW();
            }

        private void investorsPobListe(object sender, RoutedEventArgs e)
            {
            tabelujAnalizyInvestors();
            }
        #endregion

        #region === pobieranie i otwieranie pdf ===
        // actoion zmieniający labele i blokujący przycisk //
        Action<Label, string, Button, bool> zmbb = (label, napis, button, czyBlokowac) =>
        {
            label.Content = napis;
            button.IsEnabled = !czyBlokowac;
        };

        // pobieranie i otieranie pdf
        void pobierzPDF(int indeksWGridzie, List<daneAnalizy> lista, string serwis)
            {
            if (lista != null)
                {
                try
                    {
                    var link = new Uri(lista[indeksWGridzie].Link);
                    var fName = link.Segments[link.Segments.Length - 1];

                    if (!File.Exists(staleapki.appDir + staleapki.pdfDir + fName))
                        Pobierz(lista[indeksWGridzie].Link, staleapki.appDir + staleapki.pdfDir + fName);

                    System.Diagnostics.Process.Start(staleapki.appDir + staleapki.pdfDir + fName);
                    }
                catch { Loger.dodajDoLogaError(serwis + messages.docFail); };
                }
            }

        private void investorsPobPDF(object sender, RoutedEventArgs e)
            {
            var i = investorsGrid.SelectedIndex;

            this.Dispatcher.Invoke(zmbb, investorsPobPDFLabel, "Pobieranie...", investorsPobPDFButton, true);
            new Action(delegate()
            {
                pobierzPDF(i, daneTabel.invAnalizyList, serwisy.Investors);
                this.Dispatcher.Invoke(zmbb, investorsPobPDFLabel, "Pobierz i otwórz dokument", investorsPobPDFButton, false);
            }).BeginInvoke(null, null);
            }

        private void gpwButtonPobPDF(object sender, RoutedEventArgs e)
            {
            var i = gpwNewsletterGrid.SelectedIndex;

            this.Dispatcher.Invoke(zmbb, gpwPobPDFLabel, "Pobieranie...", newsletterGPWPobPDFButton, true);
            new Action(delegate()
            {
                pobierzPDF(i, daneTabel.gpwNewsletterList, serwisy.GPW);
                this.Dispatcher.Invoke(zmbb, gpwPobPDFLabel, "Pobierz i otwórz dokument", newsletterGPWPobPDFButton, false);
            }).BeginInvoke(null, null);
            }
        #endregion

        #region === inne ==
        // otwieranie newsów
        private void wiadomosciGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
            if (daneTabel.NK != null)
                System.Diagnostics.Process.Start(daneTabel.NK[wiadomosciGrid.SelectedIndex].Link);
            }

        private void ClearPDFDir(object sender, RoutedEventArgs e)
            {
            Directory.Delete(staleapki.appDir + staleapki.pdfDir, true);
            Directory.CreateDirectory(staleapki.appDir + staleapki.pdfDir);
            }
        #endregion

        #region == at przycisk ==
        private void akcjeATClick(object sender, RoutedEventArgs e)
            {
            if (daneTabel.tAkcje != null)
                {
                var oo = daneTabel.tAkcje.Where(a => a.Nazwa == aNazwa.Content).ToList();
                System.Diagnostics.Process.Start(adresy.StooqAt + oo[0].Symbol);
                }
            }

        private void indeksyATClick(object sender, RoutedEventArgs e)
            {
            try
                {
                if (indeksyTabControl.SelectedIndex == 0)
                    {
                    var oo = daneTabel.tIndeksyGPW.Where(a => a.Nazwa == iNazwa.Content).ToList();
                    System.Diagnostics.Process.Start(adresy.StooqAt + oo[0].Symbol);
                    }
                if (indeksyTabControl.SelectedIndex == 1)
                    {
                    var oo = daneTabel.tIndeksy.Where(a => a.Nazwa == iNazwa.Content).ToList();
                    System.Diagnostics.Process.Start(adresy.StooqAt + oo[0].Symbol);
                    }
                if (indeksyTabControl.SelectedIndex == 2)
                    {
                    var oo = daneTabel.tIndeksyFut.Where(a => a.Nazwa == iNazwa.Content).ToList();
                    System.Diagnostics.Process.Start(adresy.StooqAt + oo[0].Symbol);
                    }

                }
            catch { }
            }

        private void wtATClick(object sender, RoutedEventArgs e)
            {
            try
                {
                if (wtTabContol.SelectedIndex == 0)
                    {
                    var oo = daneTabel.tWaluty.Where(a => a.Nazwa == wtNazwa.Content).ToList();
                    System.Diagnostics.Process.Start(adresy.StooqAt + oo[0].Symbol);
                    }
                if (indeksyTabControl.SelectedIndex == 1)
                    {
                    var oo = daneTabel.tTowary.Where(a => a.Nazwa == wtNazwa.Content).ToList();
                    System.Diagnostics.Process.Start(adresy.StooqAt + oo[0].Symbol);
                    }
                }
            catch { }
            }
        #endregion

        #region == przyciski wykresow na dole ==
        void pokazWykreOkno(string link)
            {
            var wykWin = new wykresOkno(new BitmapImage(new Uri(link)));
            wykWin.Show();
            wykWin.wczytajWyk();
            }

        private void wigWykresShow(object sender, MouseButtonEventArgs e)
            {
            pokazWykreOkno("http://stooq.pl/c/?s=wig&c=1d&t=c&a=lg&b");
            }

        private void wig20WykresShow(object sender, MouseButtonEventArgs e)
            {
            pokazWykreOkno("http://stooq.pl/c/?s=wig20&c=1d&t=c&a=lg&b");
            }

        private void mwig40WykresShow(object sender, MouseButtonEventArgs e)
            {
            pokazWykreOkno("http://stooq.pl/c/?s=mwig40&c=1d&t=c&a=lg&b");
            }

        private void smwig80WykresShow(object sender, MouseButtonEventArgs e)
            {
            pokazWykreOkno("http://stooq.pl/c/?s=swig80&c=1d&t=c&a=lg&b");
            }
        #endregion
        }
    }