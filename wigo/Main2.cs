
// copy marazmista 16.05.2012 //


// ================
// Rozbity main window żeby nie było burdelu zbytniego //
// ================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;

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

        // ogólne tabele //
        internal static List<daneAkcji> tAkcje;
        internal static List<daneAkcji> tIndeksyGPW;
        internal static List<daneNumTabeli> tIndeksy;
        internal static List<daneNumTabeli> tIndeksyFut;
        internal static List<daneNumTabeli> tTowary;
        internal static List<daneNumTabeli> tWaluty;

        // newsy //
        internal static List<daneNewsa> NK;

        // tabele indeksów gpw //
        internal static List<daneAkcji> tWig;
        internal static List<daneAkcji> tWig20;
        internal static List<daneAkcji> tmWig40;
        internal static List<daneAkcji> tsWig80;
        internal static List<daneAkcji> tNewConnect;
        internal static List<daneAkcji> tWigBanki;
        internal static List<daneAkcji> tWigBudow;
        internal static List<daneAkcji> tWigCee;
        internal static List<daneAkcji> tWigChemia;
        internal static List<daneAkcji> tWigDewel;
        internal static List<daneAkcji> tWigEnerg;
        internal static List<daneAkcji> tWigInfo;
        internal static List<daneAkcji> tWigMedia;
        internal static List<daneAkcji> tWigPaliwa;
        internal static List<daneAkcji> tWigPlus;
        internal static List<daneAkcji> tWigPoland;
        internal static List<daneAkcji> tWigSpozyw;
        internal static List<daneAkcji> tWigSurowc;
        internal static List<daneAkcji> tWigTelkom;
        internal static List<daneAkcji> tWigUkrain;
        internal static List<daneAkcji> tRespect;
        }

    #region === logująca klasa ===
    public static class Loger
        {
        private static List<string> log = new List<string>();
        private static long pobranoBytes;

        public static event EventHandler updateLogbox;
        public static event EventHandler updateTotalDown;

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
            updateTotalDown(null, null);
            }

        public static string returnTotalDownloaded()
            {
            long pobB = (pobranoBytes / 1024);
            if (pobB < 2048)
                return pobB.ToString() + "kB";
            else
                return (pobB / 1024).ToString() + "." + (pobB % 1024).ToString().Remove(1) + "MB";
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
        //  lista indeksów //
        List<string> listaIndeksyGPW = new List<string> { "wig", "wig20","mwig40","swig80","respect","wig_banki","wig_budow","wig_cee","wig_chemia",
                "wig_dewel","wig_energ","wig_info","wig_media","wig_paliwa","wig_plus","wig_poland","wig_spozyw","wig_surowc","wig_telkom","wig_ukrain" };

        void Pobierz(string url, string sciezka)
            {
            try
                {
                var pobieracz = new WebClient();
                pobieracz.DownloadFile(url, sciezka);

                var fi = new FileInfo(sciezka);
                Loger.dodajPobrane(fi.Length);
                }
            catch { }
            }

        // sprawdzanie czy są katalogi programu (żeby potem nie było że nie da się zapisać) //
        void DirCheck()
            {
            try
                {
                if (!(Directory.Exists(staleapki.appdir + staleapki.tmpdir)))
                    Directory.CreateDirectory("tmp");
                if (!(Directory.Exists(staleapki.appdir + staleapki.bazadir)))
                    Directory.CreateDirectory("Baza");
                if (!(Directory.Exists(staleapki.appdir + staleapki.danedir)))
                    Directory.CreateDirectory("Dane");

                Loger.dodajDoLogaInfo("Katalogi OK");
                }
            catch { Loger.dodajDoLogaError("Błąd katalgów programu"); }
            }

        // metodka pobierająca albo odświeżająca skład indeksów //
        void pobierzSkladIndeksow()
            {
            // sprawdza czy to jest pierwsze pobieranie indeksów po odpaleniu apki, żeby zapobiec
            // pobieraniu lub sprawdzaniu każdego indeksy przy każdym odświeżaniu akcji //
            if (staleapki.pierwszePobranie)
                {
                Loger.dodajDoLogaInfo(messages.indOdswStart);
                try
                    {
                    // przerabianie wszystkich indeksów z listy powyższej //
                    foreach (var s in listaIndeksyGPW)
                        {
                        // ścieżka do pliku //
                        string filepath = staleapki.appdir + staleapki.bazadir + s + ".txt";

                        // jeśli pliczku nie ma, albo jest starszy niż tydzien //
                        if (!File.Exists(filepath) | (File.GetCreationTime(filepath) - DateTime.Now).TotalDays > 5)
                            {
                            Loger.dodajDoLogaInfo(messages.indAktual + s.ToUpper());
                            // tempowa lista z symbolami należącymi do indeksu //
                            List<string> tmpl = new List<string>();

                            // pobieranie pliczku z komponentami indeksu //
                            string adres = "http://stooq.pl/q/i/?s=" + s;
                            Pobierz(adres, staleapki.appdir + staleapki.tmpdir + "tmpfile.html");

                            // kod tej stronki //
                            string[] tmpf = File.ReadAllLines(staleapki.appdir + staleapki.tmpdir + "tmpfile.html");

                            // szukanie symboli spółek w indeksie //
                            var reg = new Regex(@"<font id=f13><a href=q/\?s=(.+?)>");
                            MatchCollection match = reg.Matches(tmpf[0]);

                            // dodawanie do tej tempowej tabelki a potem zapis całości do pliku //
                            foreach (var m in match)
                                {
                                var tmps = m.ToString();
                                tmps = tmps
                                            .Substring(26, 3)
                                            .ToUpper();

                                tmpl.Add(tmps);
                                }
                            File.WriteAllLines(filepath, tmpl.ToArray());
                            File.SetCreationTime(filepath, DateTime.Now);
                            }
                        }
                    Loger.dodajDoLogaInfo(messages.indOdswKoniec);
                    staleapki.pierwszePobranie = false;
                    }
                catch { Loger.dodajDoLogaError(messages.indOdswFail); }
                }
            }

        //Action<string, List<daneAkcji>> wczytajTabeleIndeksu = (indeks, listaCel) =>
        void wczytajTabeleIndeksu(string indeks, List<daneAkcji> listaCel,DataGrid grid)
            {
            if (File.Exists(staleapki.appdir + staleapki.bazadir + indeks + ".txt"))
                {
                try
                    {
                    // wczytuje skład indeksu z pliku
                    string[] sklad = File.ReadAllLines(staleapki.appdir + staleapki.bazadir + indeks + ".txt");
                    List<daneAkcji> tmpl = new List<daneAkcji>();   //tempowa lista, żeby potem przypisać do static listy //

                    // leci po tablicy i kopiuje te akcje, które są w indeksie //
                    for (int i = 0; i < sklad.Length; i++)
                        {
                        var row = daneTabel.tAkcje.Find(a => a.Symbol == sklad[i]);
                        tmpl.Add(row);
                        }
                    listaCel = tmpl;

                    // wczytanie do grida
                    this.Dispatcher.Invoke(wAkcje, grid, listaCel);
                    }
                catch { Loger.dodajDoLogaError(indeks.ToUpper() + messages.indLoadFail); }
                }
            else
                Loger.dodajDoLogaError(staleapki.appdir + staleapki.bazadir + indeks + ".txt - Plik nie istnieje!");
            }

        #region === delegary ogólne dla wątków ===
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
        
        Action<DataGrid, List<daneNewsa>> wNewsy = delegate(DataGrid wpfDataGrid, List<daneNewsa> lista)
            {
                wpfDataGrid.ItemsSource = lista;
            };
        #endregion


        #region === metody timerowe, wczytujące tabele ===
        // ================
        // metody wrzucane do timerów, które w wątkach organizują infromację dla tabel
        // ================
        void tabelujGPW()
            {
            genDelegate.BeginInvoke(generujAkcje, done => { this.Dispatcher.Invoke(wAkcje, akcjeGrid, daneTabel.tAkcje); }, null);
            genDelegate.BeginInvoke(generujIndeksyGPW, done => { this.Dispatcher.Invoke(wAkcje, indeksyGPWGrid, daneTabel.tIndeksyGPW); }, null);
            }

        void tabelujSwiat()
            {
            genDelegate.BeginInvoke(generujIndeksy, done => { this.Dispatcher.Invoke(wInne, indeksyGrid, daneTabel.tIndeksy); }, null);
            genDelegate.BeginInvoke(generujIndeksyFut, done => { this.Dispatcher.Invoke(wInne, indeksyFutGrid, daneTabel.tIndeksyFut); }, null);
            genDelegate.BeginInvoke(generujTowary, done => { this.Dispatcher.Invoke(wInne, towaryGrid, daneTabel.tTowary); }, null);
            genDelegate.BeginInvoke(generujWaluty, done => { this.Dispatcher.Invoke(wInne, walutyGrid, daneTabel.tWaluty); }, null);
            }

        void tabelujNewsy()
            {
            genDelegate.BeginInvoke(generujNK, done => { this.Dispatcher.Invoke(wNewsy, wiadomosciGrid, daneTabel.NK); }, null);
            }
        #endregion


        #region ==== action odświeżający te dolne kwadraciki ====
        Action<string, Label, Label, Label, Label, Label, Label, Label, Label, Grid> odswiezDolInfoIndeksy = (item, kurs, otwarcie, odniesienie, obrot, wolumen, zmiana, zmianaproc, godz, bgGrid) =>
            {
                var oo = daneTabel.tIndeksyGPW.Find(row => row.Nazwa == item);

                kurs.Content = oo.Kurs;
                otwarcie.Content = oo.Otwarcie;
                odniesienie.Content = oo.Odniesienie;
                zmiana.Content = oo.Zmiana;
                obrot.Content = oo.Obrot;
                wolumen.Content = oo.Wolumen;
                zmianaproc.Content = oo.ZmianaProc;

                var dt = new DateTime();
                dt = DateTime.ParseExact(oo.Data, staleapki.formatDaty, null);

                godz.Content = dt.ToString("HH:mm");

                // Zmiana koloru tła, można w XAMLu, ale na razie jest tu //
                if (Convert.ToSingle(oo.Zmiana) > 0.00)
                    bgGrid.Background = Brushes.Green;
                else
                    bgGrid.Background = Brushes.Red;
                if (Convert.ToSingle(oo.Zmiana) == 0.00)
                    bgGrid.Background = Brushes.Aqua;
            };
        Action<string, Label, Label, Label, Label, Label, Label, Grid> odswiezDolInfoWaluty = (item, kurs, otwarcie, odniesienie, zmiana, zmianaproc, godz, bgGrid) =>
            {
            var oo = daneTabel.tWaluty.Find(row => row.Symbol == item);

            kurs.Content = oo.Kurs;
            otwarcie.Content = oo.Otwarcie;
            odniesienie.Content = oo.Odniesienie;
            zmiana.Content = oo.Zmiana;
            zmianaproc.Content = oo.ZmianaProc;

            var dt = new DateTime();
            dt = DateTime.ParseExact(oo.Data, staleapki.formatDaty, null);

            godz.Content = dt.ToString("HH:mm");

            // Zmiana koloru tła, można w XAMLu, ale na razie jest tu //
            if (Convert.ToSingle(oo.Zmiana) > 0.0000)
                bgGrid.Background = Brushes.Green;  // albo FF009500
            else
                bgGrid.Background = Brushes.Red;
            if (Convert.ToSingle(oo.Zmiana) == 0.0000)
                bgGrid.Background = Brushes.Aqua;
            };
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
            var komentarzeMoney = new komentarzeMoney(serwisy.Money, typy.Komentarze);
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

                //=============
                // mając tabelę z akcjami można filtrować na indeksy (w tle, żeby główna tab z akcjami się wczytała //
                Action wczytajTab = delegate()
                    {
                    // odswieżanie składu indeksów //
                    pobierzSkladIndeksow();

                    // wczytywanie tabel do gridów w interfejsie
                    wczytajTabeleIndeksu(listaIndeksyGPW[1], daneTabel.tWig20, wig20Grid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[2], daneTabel.tmWig40, mwig40Grid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[3], daneTabel.tsWig80, swig80Grid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[4], daneTabel.tWigBanki, wigbankiGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[5], daneTabel.tWigBudow, wigbudowGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[6], daneTabel.tWigCee, wigceeGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[7], daneTabel.tWigChemia, wigchemiaGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[8], daneTabel.tWigDewel, wigdewelGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[8], daneTabel.tWigEnerg, wigenergGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[9], daneTabel.tWigInfo, wiginfoGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[10], daneTabel.tWigMedia, wigmediaGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[11], daneTabel.tWigPaliwa, wigpaliwaGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[12], daneTabel.tWigPlus, wigplusGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[13], daneTabel.tWigPoland, wigpolandGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[14], daneTabel.tWigSpozyw, wigspozywGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[15], daneTabel.tWigSurowc, wigsurowcGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[16], daneTabel.tWigTelkom, wigtelkomGrid);
                    wczytajTabeleIndeksu(listaIndeksyGPW[17], daneTabel.tWigUkrain, wigukrainGrid);
                    };

                wczytajTab.BeginInvoke(null, null);
                }
            catch { }
            }

        private void generujIndeksyGPW()
            {
            var stooqIndeksyGPW = new IndeksyGPW(serwisy.Stooq, typy.IndeksyGPW);
            try
                {
                daneTabel.tIndeksyGPW = stooqIndeksyGPW.generujTabele();

                // wczytanie dolnych kwadracików
                this.Dispatcher.Invoke(odswiezDolInfoIndeksy, "WIG", wigkursl, wigotwl, wigodnl, wigobrl, wigwoll, wigzmianal, wigprocentl, wiggodzl,wiggrid);
                this.Dispatcher.Invoke(odswiezDolInfoIndeksy, "WIG20", wig20kursl, wig20otwl, wig20odnl, wig20obrl, wig20woll, wig20zmianal, wig20procentl, wig20godzl, wig20grid);
                this.Dispatcher.Invoke(odswiezDolInfoIndeksy, "MWIG40", mwigkursl, mwigotwl, mwigodnl, mwigobrl, mwigwoll, mwigzmianal, mwigprocentl, mwiggodzl, mwiggrid);
                this.Dispatcher.Invoke(odswiezDolInfoIndeksy, "SWIG80", swigkursl, swigotwl, swigodnl, swigobrl, swigwoll, swigzmianal, swigprocentl, swiggodzl, swiggrid);
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

                // wczytanie dolnych kwadracików
                this.Dispatcher.Invoke(odswiezDolInfoWaluty, "EURPLN", eurplnkursl, eurplnotwarciel, eurplnodniesieniel, eurplnzmianal, eurplnprocentl, eurplngodzl, eurplngrid);
                this.Dispatcher.Invoke(odswiezDolInfoWaluty, "USDPLN", usdplnkursl, usdplnotwarciel, usdplnodniesieniel, usdplnzmianal, usdplnprocentl, usdplngodzl, usdplngrid);
                this.Dispatcher.Invoke(odswiezDolInfoWaluty, "EURUSD", eurusdkursl, eurusdotwarciel, eurusdodniesieniel, eurusdzmianal, eurusdprocentl, eurusdgodzl, eurusdgrid);
                }
            catch { }
            }
        }
    }
