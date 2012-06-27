
// copy marazmista 26.06.2012 //

// ================
// Rozbity main window, wyodrębnione zdarzenia //
// ================

using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;


using klasyakcjowe;

namespace mm_gielda
    {
    public partial class MainWindow
        {
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

        #region === zdarzenie wczytujace dane obok tabel po wcisnieciu na gridzie ===
		void wczytajAkcjeDetails(List<daneAkcji> tabela, DataGrid grid, bool czyAkcja)
            {
            int i = grid.SelectedIndex;

            // sprawdza czy wczytuje akcje, czy indeks gpw //
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

                if (Convert.ToSingle(tabela[i].Zmiana) < 0.00)
                    aBorder.Background = Brushes.Red;
                else
                    aBorder.Background = Brushes.Green;

                if (Convert.ToSingle(tabela[i].Zmiana) == 0.00)
                    aBorder.Background = Brushes.Aqua;
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

                if (Convert.ToSingle(tabela[i].Zmiana) < 0.00)
                    iBorder.Background = Brushes.Red;
                else
                    iBorder.Background = Brushes.Green;

                if (Convert.ToSingle(tabela[i].Zmiana) == 0.00)
                    iBorder.Background = Brushes.Aqua;

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

                if (Convert.ToSingle(tabela[i].Zmiana) < 0.00)
                    wtBorder.Background = Brushes.Red;
                else
                    wtBorder.Background = Brushes.Green;

                if (Convert.ToSingle(tabela[i].Zmiana) == 0.00)
                    wtBorder.Background = Brushes.Aqua;
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

                if (Convert.ToSingle(tabela[i].Zmiana) < 0.00)
                    iBorder.Background = Brushes.Red;
                else
                    iBorder.Background = Brushes.Green;

                if (Convert.ToSingle(tabela[i].Zmiana) == 0.00)
                    iBorder.Background = Brushes.Aqua;
                }
            }

        private void akcjeGrid_Selected(object sender, RoutedEventArgs e)
            {
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
        }
    }