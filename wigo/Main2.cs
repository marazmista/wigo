
// copy marazmista 16.05.2012 //


// ================
// Rozbity main window żeby nie było burdelu zbytniego //
// ================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Controls;

using klasynewsowe;
using commonStrings;
using klasyakcjowe;

namespace mm_gielda
    {
    
    // ================
    // klasa z danymi dla tabel żeby wątki sobie je generowały i wypełniały kiedy chcą //
    // ================
    static class daneTabel
        {
        internal static List<daneAkcji> tAkcje;
        internal static List<daneNumTabeli> tIndeksy;
        internal static List<daneNumTabeli> tIndeksyGPW;
        internal static List<daneNumTabeli> tIndeksyFut;
        internal static List<daneNumTabeli> tTowary;
        internal static List<daneWaluty> tWaluty;

        internal static List<daneNewsa> NK;
        }

    #region logująca klasa
    public static class Loger
        {
        private static List<string> log = new List<string>();
        private static long pobranoBytes;

        public static event EventHandler updateLogbox;

        public static void dodajDoLogaInfo(string logInfoMessage)
            {
            log.Add(Convert.ToString(DateTime.Now) + " INFO: " + logInfoMessage);
            updateLogbox(null, null);
            }

        public static void dodajDoLogaError(string logErrorMessage)
            {
            log.Add(Convert.ToString(DateTime.Now) + " ERROR: " + logErrorMessage);
            updateLogbox(null, null);
            }

        public static void dodajPobrane(long d)
            {
            pobranoBytes += d;
            }

        public static string returnTotalDownloaded()
            {
            long pobB = (pobranoBytes / 1024);
            if (pobB < 2048) 
                return pobB.ToString() + "kB";
            else
                return (pobB / 1024).ToString() + "MB";
            }

        public static string returnLastLogLine()
            {
            if (log.Count != 0)
                return log[log.Count - 1].ToString();
            else
                return " ";
            }
        } 
    #endregion

    public partial class MainWindow
        {
        // ================
        // sprawdzanie czy są katalogi programu (żeby potem nie było że nie da się zapisać) //
        // ================
        public bool DirCheck()
            {
            byte spr = 0;
            spr = (Directory.Exists(staleapki.appdir + staleapki.tmpdir)) ? spr += 1 : spr = 0;
            spr = (Directory.Exists(staleapki.appdir + staleapki.bazadir)) ? spr += 1 : spr = 0;
            spr = (Directory.Exists(staleapki.appdir + staleapki.danedir)) ? spr += 1 : spr = 0;

            if (spr == 3)
                return true;
            else
                return false;
            }


        #region === delegary wątkowe ===
        Action<Action> genDelegate = (genMetoda) =>
            {
            try
                {
                genMetoda();
                }
            catch { }
            };

        // z lambdą, edukacyjnie:  Action<DataGrid,List<daneAkcji>> wA = (wpfDataGrid,lista) => { ... }
        Action<DataGrid, List<daneAkcji>> wAkcje = delegate(DataGrid wpfDataGrid, List<daneAkcji> lista)
            {
            wpfDataGrid.ItemsSource = lista;
            };

        Action<DataGrid, List<daneNumTabeli>> wInne = delegate(DataGrid wpfDataGrid, List<daneNumTabeli> lista)
            {
            wpfDataGrid.ItemsSource = lista;
            };

        Action<DataGrid, List<daneWaluty>> wWaluty = delegate(DataGrid wpfDataGrid, List<daneWaluty> lista)
            {
            wpfDataGrid.ItemsSource = lista;
            };

        Action<DataGrid, List<daneNewsa>> wNewsy = delegate(DataGrid wpfDataGrid, List<daneNewsa> lista)
            {
            wpfDataGrid.ItemsSource = lista;
            }; 
        #endregion


        #region === metody timerowe ===
        // ================
        // metody wrzucane do timerów, które w wątkach oprganizują infromację dla tabel
        // ================
        void tabelujGPW()
            {
            genDelegate.BeginInvoke(generujAkcje, done => { this.Dispatcher.Invoke(wAkcje, akcjeGrid, daneTabel.tAkcje); }, null);
            genDelegate.BeginInvoke(generujIndeksyGPW, done => { this.Dispatcher.Invoke(wInne, indeksyGPWGrid, daneTabel.tIndeksyGPW); }, null);
            }

        void tabelujSwiat()
            {
            genDelegate.BeginInvoke(generujIndeksy, done => { this.Dispatcher.Invoke(wInne, indeksyGrid, daneTabel.tIndeksy); }, null);
            genDelegate.BeginInvoke(generujIndeksyFut, done => { this.Dispatcher.Invoke(wInne, indeksyFutGrid, daneTabel.tIndeksyFut); }, null);
            genDelegate.BeginInvoke(generujTowary, done => { this.Dispatcher.Invoke(wInne, towaryGrid, daneTabel.tTowary); }, null);
            genDelegate.BeginInvoke(generujWaluty, done => { this.Dispatcher.Invoke(wWaluty, walutyGrid, daneTabel.tWaluty); }, null);
            }

        void tabelujNewsy()
            {
            genDelegate.BeginInvoke(generujNK, done => { this.Dispatcher.Invoke(wNewsy, wiadomosciGrid, daneTabel.NK); }, null);
            } 
        #endregion

//===============================================================================
//===============================================================================

        // ================
        // procka zwracająca połaczone kolekcje z newsami z różnych źródeł, żeby
        // potem wrzucic newsy jako jedna lista do datagrida (przyjmuje tylko jedną listę)//
        // ================
        private void generujNK()
            {
            var bankierRynki = new newsBankier(serwisy.Bankier, typy.Wiadomosci);
            var komentarzeMoney = new komentarzeMoney(serwisy.Money,typy.Komentarze);
            var tmptab = new List<daneNewsa>();

            try
                {
                tmptab = bankierRynki.generujTabele();
                }
            catch { }

            try
                {
                tmptab = tmptab.Concat(komentarzeMoney.generujTabele()).ToList();
                }
            catch { }

            daneTabel.NK = tmptab;
            }

        // ================
        // procki wywołujące pobieranie akcji, indeksów, towarów, walut, fut //
        // ================
        private void generujAkcje()
            {
            var stooqAkcje = new Akcje(serwisy.Stooq, typy.Akcje);
            try
                {
                daneTabel.tAkcje = stooqAkcje.generujTabele();
                }
            catch { }
            }

        private void generujIndeksyGPW()
            {
            var stooqIndeksyGPW = new IndeksyGPW(serwisy.Stooq, typy.IndeksyGPW);
            try
                {
                daneTabel.tIndeksyGPW = stooqIndeksyGPW.generujTabele();    
                }
            catch { }
            }

        private void generujIndeksy()
            {
            var stooqIndeksy = new Indeksy(serwisy.Stooq, typy.Indeksy);
            try
                {
                daneTabel.tIndeksy = stooqIndeksy.generujTabele();
                }
            catch { }
            }
        
        private void generujIndeksyFut()
            {
            var stooqIndeksyFut = new IndeksyFut(serwisy.Stooq, typy.IndeksyFut);
            try
                {
                daneTabel.tIndeksyFut = stooqIndeksyFut.generujTabele();
                }
            catch { }
            }

        private void generujTowary()
            {
            var stooqTowary = new Towary(serwisy.Stooq, typy.Towary);
            try
                {
                daneTabel.tTowary = stooqTowary.generujTabele();
                }
            catch { }
            }

        private void generujWaluty()
            {
            var stooqWaluty = new Waluty(serwisy.Stooq, typy.Waluty);
            try
                {
                daneTabel.tWaluty = stooqWaluty.generujTabele();
                }
            catch { }
            }
        }
    }
