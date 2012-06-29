
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
using System.Windows.Resources;
using System.Resources;
using System.Windows.Shapes;

using klasynewsowe;
using commonStrings;
using klasyakcjowe;


namespace mm_gielda
    {

    class daneWzrostySpadki
        {
        public string Nazwa { get; set; }
        public float Kurs { get; set; }
        public string ZmianaProc { get; set; }
        public float Zmiana { get; set; }

        public daneWzrostySpadki() { }

        public daneWzrostySpadki(string nazwa,float kurs, string zmianProc, float zmiana)
            {
            this.Nazwa = nazwa;
            this.Kurs = kurs;
            this.ZmianaProc = zmianProc;
            this.Zmiana = zmiana;
            }           
        }

    class daneNajaktyniejsze
        {
        // Obrot jest string tylko dlatego, żeby w tabeli na podsumowaniu nie było liczb typu 100000000.
        // I po to też te wszystkie ciągłe konwersje ToString() albo ToInt32() niżej w metodzie filtrujNajaktwyniejsze // 
        public string Nazwa { get; set; }
        public float Kurs { get; set; }
        public string Obrot { get; set; } 
        public string Wolumen { get; set; }

        public daneNajaktyniejsze() { }

        public daneNajaktyniejsze(string nazwa, float kurs, string obrot, string wolumen)
            {
            this.Nazwa = nazwa;
            this.Kurs = kurs;
            this.Obrot = obrot;
            this.Wolumen = wolumen;
            }
        }

    // ================
    // klasa z danymi dla tabel żeby wątki sobie je generowały i wypełniały kiedy chcą //
    // ================
    static class daneTabel
        {

        #region == ogólne tabele ==
        internal static List<daneAkcji> tAkcje;
        internal static List<daneAkcji> tIndeksyGPW;
        internal static List<daneNumTabeli> tIndeksy;
        internal static List<daneNumTabeli> tIndeksyFut;
        internal static List<daneNumTabeli> tTowary;
        internal static List<daneNumTabeli> tWaluty; 
        #endregion

        // newsy //
        internal static List<daneNewsa> NK;

        #region == tabele indeksów gpw ==
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
        #endregion

        #region == male tabele top indeksow ==
        internal static List<daneWzrostySpadki> tAkcjeGPWWzrosty;
        internal static List<daneWzrostySpadki> tAkcjeGPWSpadki;
        internal static List<daneNajaktyniejsze> tAkcjeGPWNajaktwyniejsze;

        internal static List<daneWzrostySpadki> tWigWzrosty;
        internal static List<daneWzrostySpadki> tWigSpadki;
        internal static List<daneNajaktyniejsze> tWigNajaktywniejsze;

        internal static List<daneWzrostySpadki> tWig20Wzrosty;
        internal static List<daneWzrostySpadki> tWig20Spadki;
        internal static List<daneNajaktyniejsze> tWig20Najaktywniejsze;

        internal static List<daneWzrostySpadki> tMwig40Wzrosty;
        internal static List<daneWzrostySpadki> tMwig40Spadki;
        internal static List<daneNajaktyniejsze> tMwig40Najaktywniejsze;

        internal static List<daneWzrostySpadki> tSwig80Wzrosty;
        internal static List<daneWzrostySpadki> tSwig80Spadki;
        internal static List<daneNajaktyniejsze> tSwig80Najaktywniejsze; 
        #endregion
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
        string[] listaIndeksyGPW = { "wig", "wig20","mwig40","swig80","wig_banki","wig_budow","wig_cee","wig_chemia","wig_dewel","wig_energ","wig_info","wig_media",
                                   "wig_paliwa","wig_plus","wig_poland","wig_spozyw","wig_surowc","wig_telkom","wig_ukrain","respect" };

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
                    Directory.CreateDirectory(staleapki.appdir + @"\tmp");
                if (!(Directory.Exists(staleapki.appdir + staleapki.bazadir)))
                    Directory.CreateDirectory(staleapki.appdir + @"\baza");
                if (!(Directory.Exists(staleapki.appdir + staleapki.danedir)))
                    Directory.CreateDirectory(staleapki.appdir + @"\dane");
                if (!(Directory.Exists(staleapki.appdir + staleapki.tmpWykresDir)))
                    Directory.CreateDirectory(staleapki.appdir + staleapki.tmpdir + "tmpWykresy");

                Loger.dodajDoLogaInfo("Katalogi OK");
                }
            catch { Loger.dodajDoLogaError("Błąd katalgów programu"); }
            }

        bool czyTrwaSesja()
            {
            if ((DateTime.Now.Date == staleapki.dataUruchmienia) & (DateTime.Now.Hour <= staleapki.nieOdswGPWPoGodz) & (DateTime.Now.Minute < staleapki.nieOdswGPWPoMin))
                return true;
            else
                return false;
            }

        bool czyOdswierzacAkcje()
            {
            // sprawdza czy tabela jest pusta, a potem jeśli nie jest, to sprawdza czy trwa sesja //
            if (daneTabel.tAkcje == null)
                return true;
            else
                {
                if (czyTrwaSesja())
                    return true;
                else
                    return false;
                }
            }

        bool czyOdswierzacIndeksyGPW()
            {
            if (daneTabel.tIndeksyGPW == null)
                return true;
            else
                {
                if (czyTrwaSesja())
                    return true;
                else
                    return false;
                }
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

        void wczytajTabeleIndeksu(string indeks,ref List<daneAkcji> listaCel,DataGrid grid)
            {
            if (File.Exists(staleapki.appdir + staleapki.bazadir + indeks + ".txt"))
                {
                try
                    {
                    // wczytuje skład indeksu z pliku
                    string[] sklad = File.ReadAllLines(staleapki.appdir + staleapki.bazadir + indeks + ".txt");
                    var tmpl = new List<daneAkcji>();   //tempowa lista, żeby potem przypisać do static listy //

                    // leci po tablicy i kopiuje te akcje, które są w indeksie //
                    for (int i = 0; i < sklad.Length; i++)
                        {
                        var row = daneTabel.tAkcje.Find(a => a.Symbol == sklad[i]);
                        if (row != null)
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

        // metodka filtrująca indeksy i wrzucająca do list te akcje które rosną i spadają najbardziej //
        void filtrujWzrostySpadki(List<daneAkcji> listaZrodlo, ref List<daneWzrostySpadki> listaCelWzrosty, ref List<daneWzrostySpadki> listaCelSpadki)
            {
            // sortowanie po zmianie porocentowej //
            IEnumerable<daneAkcji> tmp1 = listaZrodlo.OrderBy(x => Convert.ToSingle(x.ZmianaProc.Remove(x.ZmianaProc.Length - 1))).Take(staleapki.iloscNaj);
            IEnumerable<daneAkcji> tmp2 = listaZrodlo.OrderByDescending(x => Convert.ToSingle(x.ZmianaProc.Remove(x.ZmianaProc.Length - 1))).Take(staleapki.iloscNaj);

            // tempowe, żeby potem przypisać do staticów //
            var tmpW = new List<daneWzrostySpadki>(staleapki.iloscNaj);
            var tmpS = new List<daneWzrostySpadki>(staleapki.iloscNaj);

            // zbieranie 5 pierwszych i wpisanie do listy typu daneWzrostySpadki //
            for (byte i = 0; i < staleapki.iloscNaj; i++)
                {
                tmpS.Add(new daneWzrostySpadki(tmp1.ElementAt(i).Nazwa,tmp1.ElementAt(i).Kurs, tmp1.ElementAt(i).ZmianaProc, Convert.ToSingle(tmp1.ElementAt(i).Zmiana)));
                tmpW.Add(new daneWzrostySpadki(tmp2.ElementAt(i).Nazwa,tmp2.ElementAt(i).Kurs, tmp2.ElementAt(i).ZmianaProc, Convert.ToSingle(tmp2.ElementAt(i).Zmiana)));
                }
            listaCelWzrosty = tmpW;
            listaCelSpadki = tmpS;
            }

        void filtrujNajaktywniejsze(List<daneAkcji> listaZrodlo, ref List<daneNajaktyniejsze> listaCel)
            {
            // konwersja mnożników. Można kiedys poprawić jak znajdzie się lepszy sposób...
            #region = konwersja mnożników obrotu (k,m,g) na liczby, żeby posortować =
            var convAllTab = new List<daneNajaktyniejsze>(400);
            for (int i = 0; i < listaZrodlo.Count; i++)
                {
                float tmpflo;
                string tmps = listaZrodlo[i].Obrot.Replace('.', ',');
                if (tmps.Contains('k') | tmps.Contains('m') | tmps.Contains('g'))
                    {
                    tmpflo = Convert.ToSingle(tmps.Remove(tmps.Length - 1));
                    // identyfikacja mnożnika i przemnożenie
                    switch (tmps[tmps.Length - 1])
                        {
                        case 'k':
                            tmpflo *= 1000;
                            break;
                        case 'm':
                            tmpflo *= 1000000;
                            break;
                        case 'g':
                            tmpflo *= 1000000000;
                            break;
                        default:
                            break;
                        }
                    }
                else
                    {
                    tmpflo = Convert.ToInt32(tmps);
                    }
                int t = (int)tmpflo;
                convAllTab.Add(new daneNajaktyniejsze(listaZrodlo[i].Nazwa, listaZrodlo[i].Kurs, t.ToString("D"), listaZrodlo[i].Wolumen));
                }; 
            #endregion

            IEnumerable<daneNajaktyniejsze> sConvTab = convAllTab.OrderByDescending(x => Convert.ToInt32(x.Obrot)).Take(staleapki.iloscNaj);

            var tmpN = new List<daneNajaktyniejsze>(staleapki.iloscNaj);

            for (byte i = 0; i < staleapki.iloscNaj; i++)
                {
                // to jest po to, żeby w tabeli było nadal k,m,g a nie wiadomo jakie liczby np. 124000000 //
                var row = daneTabel.tAkcje.Find(a => a.Nazwa == sConvTab.ElementAt(i).Nazwa);
                string obrotStock = row.Obrot;

                tmpN.Add(new daneNajaktyniejsze(sConvTab.ElementAt(i).Nazwa, sConvTab.ElementAt(i).Kurs, obrotStock, sConvTab.ElementAt(i).Wolumen));
                }
            listaCel = tmpN;
            }

        #region === actiony ogólne dla wątków ===
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

        Action<DataGrid,DataGrid, List<daneWzrostySpadki>, List<daneWzrostySpadki>> wWS = delegate(DataGrid wpfDataGridWzrosty, DataGrid wpfDataGridSpadki, List<daneWzrostySpadki> listaWzrosty, List<daneWzrostySpadki> listaSpadki)
            {
            wpfDataGridWzrosty.ItemsSource = listaWzrosty;
            wpfDataGridSpadki.ItemsSource = listaSpadki;
            };

        Action<DataGrid, List<daneNajaktyniejsze>> wN = delegate(DataGrid wpfNajaktywniejszeGrid, List<daneNajaktyniejsze> listaNaj)
            {
            wpfNajaktywniejszeGrid.ItemsSource = listaNaj;
            };

        Action<DataGrid, List<daneNewsa>> wNewsy = delegate(DataGrid wpfDataGrid, List<daneNewsa> lista)
            {
            wpfDataGrid.ItemsSource = lista;
            };
        #endregion

        #region === metody timerowe, wczytujące tabele ===
        // ================
        // metody wrzucane do timerów, które w wątkach organizują infromację dla tabel
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

        void tabelujWzrostySpadki()
            {
            filtrujWzrostySpadki(daneTabel.tAkcje, ref daneTabel.tAkcjeGPWWzrosty, ref daneTabel.tAkcjeGPWSpadki);
            filtrujWzrostySpadki(daneTabel.tWig, ref daneTabel.tWigWzrosty, ref daneTabel.tWigSpadki);
            filtrujWzrostySpadki(daneTabel.tWig20, ref daneTabel.tWig20Wzrosty, ref daneTabel.tWig20Spadki);
            filtrujWzrostySpadki(daneTabel.tmWig40, ref daneTabel.tMwig40Wzrosty, ref daneTabel.tMwig40Spadki);
            filtrujWzrostySpadki(daneTabel.tsWig80, ref daneTabel.tSwig80Wzrosty, ref daneTabel.tSwig80Spadki);

            this.Dispatcher.Invoke(wWS, akcjeGPWWzrostyGrid, akcjeGPWSpadkiGrid, daneTabel.tAkcjeGPWWzrosty, daneTabel.tAkcjeGPWSpadki);
            this.Dispatcher.Invoke(wWS, wigWzrostyGrid, wigSpadkiGrid,daneTabel.tWigWzrosty, daneTabel.tWigSpadki);
            this.Dispatcher.Invoke(wWS, wig20WzrostyGrid, wig20SpadkiGrid, daneTabel.tWig20Wzrosty, daneTabel.tWig20Spadki);
            this.Dispatcher.Invoke(wWS, mwig40WzrostyGrid, mwig40SpadkiGrid, daneTabel.tMwig40Wzrosty, daneTabel.tMwig40Spadki);
            this.Dispatcher.Invoke(wWS, swig80WzrostyGrid, swig80SpadkiGrid, daneTabel.tSwig80Wzrosty, daneTabel.tSwig80Spadki);
            }

        void tabelujNajaktywniejsze()
            {
            filtrujNajaktywniejsze(daneTabel.tAkcje, ref daneTabel.tAkcjeGPWNajaktwyniejsze);
            filtrujNajaktywniejsze(daneTabel.tWig,ref daneTabel.tWigNajaktywniejsze);
            filtrujNajaktywniejsze(daneTabel.tWig20, ref daneTabel.tWig20Najaktywniejsze);
            filtrujNajaktywniejsze(daneTabel.tmWig40, ref daneTabel.tMwig40Najaktywniejsze);
            filtrujNajaktywniejsze(daneTabel.tsWig80, ref daneTabel.tSwig80Najaktywniejsze);

            this.Dispatcher.Invoke(wN, akcjeGPWNajaktywniejszeGrid, daneTabel.tAkcjeGPWNajaktwyniejsze);
            this.Dispatcher.Invoke(wN, wigNajaktywniejszeGrid, daneTabel.tWigNajaktywniejsze);
            this.Dispatcher.Invoke(wN, wig20NajaktywniejszeGrid, daneTabel.tWig20Najaktywniejsze);
            this.Dispatcher.Invoke(wN, mwig40NajaktywniejszeGrid, daneTabel.tMwig40Najaktywniejsze);
            this.Dispatcher.Invoke(wN, swig80NajaktywniejszeGrid, daneTabel.tSwig80Najaktywniejsze);
            }
        #endregion

        #region ==== odswiezanie dolnych kwadracików ====
        void odswiezDolInfoIndeksy(string item, Label kurs, Label otwarcie, Label odniesienie, Label obrot, Label wolumen, Label zmiana, Label zmianaproc, Label godz, Grid bgGrid, Rectangle rect)
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

            KolorISymbol(oo.Zmiana, null,bgGrid, rect);
            }

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
            var tmptab = new List<daneNewsa>(50);

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
                if (czyOdswierzacAkcje())
                    {
                    //Loger.dodajDoLogaInfo(serwisy.Stooq + typy.Akcje + messages.pobStart);
                    daneTabel.tAkcje = stooqAkcje.generujTabele();

                    //=============
                    // mając tabelę z akcjami można filtrować na indeksy (w tle, żeby główna tab z akcjami się wczytała //
                    Action wczytajTab = delegate()
                        {
                            // odswieżanie składu indeksów //
                            pobierzSkladIndeksow();

                            // wczytywanie tabel do gridów w interfejsie
                            wczytajTabeleIndeksu("wig", ref daneTabel.tWig, wigGrid);
                            wczytajTabeleIndeksu("wig20", ref daneTabel.tWig20, wig20Grid);
                            wczytajTabeleIndeksu("mwig40", ref daneTabel.tmWig40, mwig40Grid);
                            wczytajTabeleIndeksu("swig80", ref daneTabel.tsWig80, swig80Grid);
                            wczytajTabeleIndeksu("wig_banki", ref daneTabel.tWigBanki, wigbankiGrid);
                            wczytajTabeleIndeksu("wig_budow", ref daneTabel.tWigBudow, wigbudowGrid);
                            wczytajTabeleIndeksu("wig_cee", ref daneTabel.tWigCee, wigceeGrid);
                            wczytajTabeleIndeksu("wig_chemia", ref daneTabel.tWigChemia, wigchemiaGrid);
                            wczytajTabeleIndeksu("wig_dewel", ref daneTabel.tWigDewel, wigdewelGrid);
                            wczytajTabeleIndeksu("wig_energ", ref daneTabel.tWigEnerg, wigenergGrid);
                            wczytajTabeleIndeksu("wig_info", ref daneTabel.tWigInfo, wiginfoGrid);
                            wczytajTabeleIndeksu("wig_media", ref daneTabel.tWigMedia, wigmediaGrid);
                            wczytajTabeleIndeksu("wig_paliwa", ref daneTabel.tWigPaliwa, wigpaliwaGrid);
                            wczytajTabeleIndeksu("wig_plus", ref daneTabel.tWigPlus, wigplusGrid);
                            wczytajTabeleIndeksu("wig_poland", ref daneTabel.tWigPoland, wigpolandGrid);
                            wczytajTabeleIndeksu("wig_spozyw", ref daneTabel.tWigSpozyw, wigspozywGrid);
                            wczytajTabeleIndeksu("wig_surowc", ref daneTabel.tWigSurowc, wigsurowcGrid);
                            wczytajTabeleIndeksu("wig_telkom", ref daneTabel.tWigTelkom, wigtelkomGrid);
                            wczytajTabeleIndeksu("wig_ukrain", ref daneTabel.tWigUkrain, wigukrainGrid);
                            wczytajTabeleIndeksu("respect", ref daneTabel.tRespect, respectGrid);
                        };

                    wczytajTab.BeginInvoke(done => { tabelujWzrostySpadki(); tabelujNajaktywniejsze(); }, null);
                    }
                }
            catch { }
            }

        private void generujIndeksyGPW()
            {
            var stooqIndeksyGPW = new IndeksyGPW(serwisy.Stooq, typy.IndeksyGPW);
            try
                {
                if (czyOdswierzacIndeksyGPW())
                    {
                    //Loger.dodajDoLogaInfo(serwisy.Stooq + typy.IndeksyGPW + messages.pobStart);
                    daneTabel.tIndeksyGPW = stooqIndeksyGPW.generujTabele();

                    if (daneTabel.tIndeksyGPW.Count != 0)
                        {
                        // wczytanie dolnych kwadracików
                        this.Dispatcher.Invoke(new Action(() => { odswiezDolInfoIndeksy("WIG",wigkursl,wigotwl,wigodnl,wigobrl,wigwoll,wigzmianal,wigprocentl,wiggodzl,wiggrid,wigRect);}));
                        this.Dispatcher.Invoke(new Action(() => { odswiezDolInfoIndeksy("WIG20", wig20kursl, wig20otwl, wig20odnl, wig20obrl, wig20woll, wig20zmianal, wig20procentl, wig20godzl, wig20grid,wig20Rect); }));
                        this.Dispatcher.Invoke(new Action(() => { odswiezDolInfoIndeksy("MWIG40", mwigkursl, mwigotwl, mwigodnl, mwigobrl, mwigwoll, mwigzmianal, mwigprocentl, mwiggodzl, mwiggrid,mwigRect); }));
                        this.Dispatcher.Invoke(new Action(() => { odswiezDolInfoIndeksy("SWIG80", swigkursl, swigotwl, swigodnl, swigobrl, swigwoll, swigzmianal, swigprocentl, swiggodzl, swiggrid,swigRect); }));
                        }
                    else
                        Loger.dodajDoLogaError(messages.indBoxRefreshError);
                    }
                }
            catch { }
            }

        private void generujIndeksy()
            {
            var stooqIndeksy = new Indeksy(serwisy.Stooq, typy.Indeksy);
            try
                {
                Loger.dodajDoLogaInfo(serwisy.Stooq + typy.Indeksy + messages.pobStart);
                daneTabel.tIndeksy = stooqIndeksy.generujTabele();
                }
            catch { }
            }

        private void generujIndeksyFut()
            {
            var stooqIndeksyFut = new IndeksyFut(serwisy.Stooq, typy.IndeksyFut);
            try
                {
                Loger.dodajDoLogaInfo(serwisy.Stooq + typy.IndeksyFut + messages.pobStart);
                daneTabel.tIndeksyFut = stooqIndeksyFut.generujTabele();
                }
            catch { }
            }

        private void generujTowary()
            {
            var stooqTowary = new Towary(serwisy.Stooq, typy.Towary);
            try
                {
                Loger.dodajDoLogaInfo(serwisy.Stooq + typy.Towary + messages.pobStart);
                daneTabel.tTowary = stooqTowary.generujTabele();
                }
            catch { }
            }

        private void generujWaluty()
            {
            var stooqWaluty = new Waluty(serwisy.Stooq, typy.Waluty);
            try
                {
                Loger.dodajDoLogaInfo(serwisy.Stooq + typy.Waluty + messages.pobStart);
                daneTabel.tWaluty = stooqWaluty.generujTabele();

                if (daneTabel.tWaluty.Count != 0)
                    {
                    // wczytanie dolnych kwadracików
                    this.Dispatcher.Invoke(odswiezDolInfoWaluty, "EURPLN", eurplnkursl, eurplnotwarciel, eurplnodniesieniel, eurplnzmianal, eurplnprocentl, eurplngodzl, eurplngrid);
                    this.Dispatcher.Invoke(odswiezDolInfoWaluty, "USDPLN", usdplnkursl, usdplnotwarciel, usdplnodniesieniel, usdplnzmianal, usdplnprocentl, usdplngodzl, usdplngrid);
                    this.Dispatcher.Invoke(odswiezDolInfoWaluty, "EURUSD", eurusdkursl, eurusdotwarciel, eurusdodniesieniel, eurusdzmianal, eurusdprocentl, eurusdgodzl, eurusdgrid);
                    }
                else
                    Loger.dodajDoLogaError(messages.walutyBoxRefreshError);
                }
            catch { }
            }
        }
    }
