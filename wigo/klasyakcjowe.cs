
// copy marazmista 24.04.2012 //


// ================
// wszystkie klasy związane z aktualnymi danymni akcji, indeksów itp. są tutaj //
// ================

using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;

using commonStrings;
using mm_gielda;


namespace klasyakcjowe
    {

    //*********************************************
    // ====== ABSTRACTY DO IMPLEMENTACJI ======= //
    //*********************************************

    // **********************
    // ========= szablony do generowania list kompatybilnych z datagridem //
    // **********************
    #region szablony do generowania list kompatybilnych z datagridem
        abstract class daneNumOgolne
            {
            public string Data { get; set; }
            public string Nazwa { get; set; }
            public string Symbol { get; set; }
            public float Kurs { get; set; }
            public string MaxMin { get; set; }
            public string Zmiana { get; set; }
            public string ZmianaProc { get; set; }
            public float Otwarcie { get; set; }
            public float Odniesienie { get; set; }
            }

        class daneAkcji : daneNumOgolne
            {
            public int Wolumen { get; set; }
            public int Obrot { get; set; }
            public int Transakcje { get; set; }

            public daneAkcji() { }

            public daneAkcji(DateTime data, string nazwa, string symbol, float kurs, float max, float min, float otwarcie, float odniesienie, int wolumen, int obrot, int transakcje)
                {
                this.Data = data.ToString("dd-MM-yyyy HH:mm:ss");  //zmiana na stringa, żeby format był ok, nie hmerykansky
                this.Nazwa = nazwa;
                this.Symbol = symbol;
                this.Kurs = kurs;
                this.MaxMin = max.ToString() + " | " + min.ToString();
                this.Zmiana = (kurs - odniesienie).ToString("0.##");
                this.ZmianaProc = ((Convert.ToSingle(this.Zmiana) / odniesienie) * 100).ToString("0.##") + "%";
                this.Otwarcie = otwarcie;
                this.Odniesienie = odniesienie;
                this.Wolumen = wolumen;
                this.Obrot = obrot;
                this.Transakcje = transakcje;
                }
            }

        class daneNumTabeli : daneNumOgolne
            {
            public daneNumTabeli() { }

            public daneNumTabeli(DateTime data, string nazwa, string symbol, float kurs, float max, float min, float otwarcie, float odniesienie)
                {
                this.Data = data.ToString("dd-MM-yyyy HH:mm:ss");  //zmiana na stringa, żeby format był ok, nie hmerykansky
                this.Nazwa = nazwa;
                this.Symbol = symbol;
                this.Kurs = kurs;
                this.MaxMin = max.ToString() + " | " + min.ToString();
                this.Zmiana = (kurs - odniesienie).ToString("0.##");
                this.ZmianaProc = ((Convert.ToSingle(this.Zmiana) / odniesienie) * 100).ToString("0.##") + "%";
                this.Otwarcie = otwarcie;
                this.Odniesienie = odniesienie;
                }
            }
        
        // waluty mają dokładność do .0000, więc nie może być to samo
        class daneWaluty : daneNumOgolne
            {
            public daneWaluty() { }

            public daneWaluty(DateTime data, string nazwa, string symbol, float kurs, float max, float min, float otwarcie, float odniesienie)
                {
                this.Data = data.ToString("dd-MM-yyyy HH:mm:ss");  //zmiana na stringa, żeby format był ok, nie hmerykansky
                this.Nazwa = nazwa;
                this.Symbol = symbol;
                this.Kurs = kurs;
                this.MaxMin = max.ToString() + " | " + min.ToString();
                this.Zmiana = (kurs - odniesienie).ToString("0.####");
                this.ZmianaProc = ((Convert.ToSingle(this.Zmiana) / odniesienie) * 100).ToString("0.####") + "%";
                this.Otwarcie = otwarcie;
                this.Odniesienie = odniesienie;
                }
            }

    #endregion


    // **********************
    // ========= szablony do bazowych klas (na tabelki) //
    // **********************
    #region Bazowe klasy

        // **********************
        // ======== bazowa klasa dla danych do akcji z procedurkami i zmiennymi dla nich //
        // **********************
        internal abstract class NumAkcje
            {
            protected List<DateTime> tDataGodz = new List<DateTime>();    // t od temp
            protected List<string> tNazwa = new List<string>();
            protected List<string> tSymbol = new List<string>();
            protected List<float> tKurs = new List<float>();
            protected List<float> tMax = new List<float>();
            protected List<float> tMin = new List<float>();
            protected List<string> tZmiana = new List<string>();
            protected List<string> tZmProc = new List<string>();
            protected List<float> tOtwarcie = new List<float>();
            protected List<float> tOdniesienie = new List<float>();

            //dane do loga
            protected string serwis, typ;

            public abstract void Prasuj(string url, string plik);

            public virtual bool Pobierz(string url, string sciezka)
                {
                try
                    {
                    var pobieracz = new WebClient();
                    pobieracz.DownloadFile(url, sciezka);

                    var fi = new FileInfo(sciezka);
                    Loger.dodajDoLogaInfo(this.serwis + this.typ + messages.pobOk + " (" + fi.Length / 1024 + "kB)");
                    Loger.dodajPobrane(fi.Length);
                    return true;
                    }
                catch 
                    {
                    Loger.dodajDoLogaError(this.serwis + this.typ + messages.pobFail);
                    return false; 
                    }
                }
            // =====
            // zbiera nazwy ze stooq //
            protected string zbierzNazwy(string weString, string regexString)
                {
                var praser = new Regex(regexString);   //"href=q/\?s=(\w*)>(\.*[^<]*)"
                var m = praser.Matches(weString)[0];

                return m.ToString().Remove(0, m.ToString().Length - (m.ToString().Length - m.ToString().IndexOf('>')) + 1); ;
                }

            // ======
            // zbiera symbole ze stooq //
            protected string zbierzSymbole(string weString, string regexString)
                {
                var praser = new Regex(regexString);  //">\((\w*)\)<"
                var m = praser.Matches(weString);

                return m[0].ToString()
                        .Remove(m[0].Length - 2)
                        .Remove(0, 2);
                }

            // =====
            // zbiera wartości ze stooq //
            protected float zbierzWartosciFloat(string weString, string regexParam)
                {
                // otw: "_o>(\.*[^<]*)"
                // max: "_h>(\.*[^<]*)"
                // min: "_l>(\.*[^<]*)"
                // kurs: "_c2>(\.*[^<]*)"
                // odn: "_p>(\.*[^<]*)"

                string tms;  //tmp string, żeby było mniej ToString()

                var praser = new Regex("_" + regexParam + @">(\.*[^<]*)");
                var m = praser.Matches(weString);

                if (!(m[0].ToString() == ""))
                    {
                    tms = m[0].ToString();
                    tms = tms
                            .Remove(0, tms.Length - (tms.Length - tms.IndexOf('>')) + 1)
                            .Replace('.', ',');  // zmiana . na , żeby się przekonwertowało

                    return Convert.ToSingle(tms);
                    }
                else
                    return 0.0F;
                }


            // ======
            // zbiera inne wartości ze stooq //
            protected int zbierzWartosciInt(string weString, string regexParam)
                {
                // wolumen: _v2>(\.*[^<]*)
                // obrót :_r2>(\.*[^<]*)
                // transakcje :_n3>(\.*[^<]*)
                // lop : _i3>(\.*[^<]*)

                string s;
                float tmpflo;

                var praser = new Regex("_" + regexParam + @">(\.*[^<]*)");
                var m = praser.Matches(weString);

                // wywalenie niepotrzebnych znaków, do '>'//
                s = m[0].ToString();

                s = s
                    .Remove(0, s.Length - (s.Length - s.IndexOf('>')) + 1)
                    .Replace('.', ',');

                if (!(s == ""))  // jeśli nie puste //
                    {
                    if (s.Contains("k") || s.Contains("m") || s.Contains("g")) // jeśli ma mnożnik //
                        {
                        // konwersja do inta i wywalanie mnożnika z końca (k, m albo g), żeby się skonwertowało//
                        tmpflo = Convert.ToSingle(s.Remove(s.Length - 1));

                        // indentyfikacja mnożnika i przemnożenie //
                        switch (s[s.Length - 1])
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
                        return (int)tmpflo;
                        }
                    else
                        return Convert.ToInt32(s);        // jesli nie było mnożnika //
                    }
                else
                    return 0;       // jeśli wartość była pusta //
                }


            // ======
            // zbiera datę i godzinę ze stooq //
            protected DateTime zbierzDateGodz(string weString, string regexStringData, string regexStringGodz)
                {

                //_t1>(\d*:\d*:\d*)
                //_d2>(\d*-\d*-\d*)

                string ds, gs;  //tmpowe stringi z datą i godziną

                var praserData = new Regex(regexStringData);
                MatchCollection dm = praserData.Matches(weString);
                ds = dm[0].ToString();
                ds = ds.Remove(0, ds.Length - (ds.Length - ds.IndexOf('>')) + 1);

                var praserGodz = new Regex(regexStringGodz);
                MatchCollection gm = praserGodz.Matches(weString);
                gs = gm[0].ToString();
                gs = gs.Remove(0, gs.Length - (gs.Length - gs.IndexOf('>')) + 1);

                return new DateTime(Convert.ToInt32(ds.Substring(0, 4)),
                                    Convert.ToInt32(ds.Substring(5, 2)),
                                    Convert.ToInt32(ds.Substring(8, 2)),
                                    Convert.ToInt32(gs.Substring(0, 2)),
                                    Convert.ToInt32(gs.Substring(3, 2)),
                                    Convert.ToInt32(gs.Substring(6, 2)));
                }
            }


        // **********************
        // ======== bazowa klasa dla danych do tabel z indeksami, wautami i towarami z procedurkami i zmiennymi dla nich //
        // **********************
        internal abstract class NumTabela : NumAkcje   // dziedziczy żeby były te same tempowe tableki
            {
            protected List<daneNumTabeli> numKolekcja;

            // żeby się nie powtarzać nizej
            private float StrToFlo(string weString)
                {
                weString = weString
                                   .Remove(weString.Length - 2)
                                   .Remove(0, 1)
                                   .Replace('.', ',');
                return Convert.ToSingle(weString);
                }

            protected void ZbierzWartosci(string weString)
                {
                // na jednym praserze nie leciało jak trzeba, więc są odddzielne

                var numPraser = new Regex(@">([0-9.]+?)</");
                MatchCollection numCol = numPraser.Matches(weString);

                var nazwaPraser = new Regex(@">([0-9a-zA-z\(\)\s-&'\#/:]+?)</");
                MatchCollection nazwaCol = nazwaPraser.Matches(weString);

                var symbolPraser = new Regex(@">\(([0-9A-Z\^\._]+?)\)</");
                MatchCollection symbolCol = symbolPraser.Matches(weString);

                var dataPraser = new Regex(@">([0-9-:]+?)</");
                MatchCollection dataCol = dataPraser.Matches(weString);

                string tmps;

                try
                    {
                    // dodawanie nazw
                    tmps = nazwaCol[0].ToString();
                    tmps = tmps
                               .Remove(tmps.Length - 2)
                               .Remove(0, 1);
                    tNazwa.Add(tmps);

                    // dodwanie symboli
                    tmps = symbolCol[0].ToString();
                    tmps = tmps
                               .Remove(tmps.Length - 3)
                               .Remove(0, 2);
                    tSymbol.Add(tmps);

                    // czytanie dancyh numerycznych
                    tOtwarcie.Add(StrToFlo(numCol[0].ToString()));
                    tMax.Add(StrToFlo(numCol[1].ToString()));
                    tMin.Add(StrToFlo(numCol[2].ToString()));
                    tKurs.Add(StrToFlo(numCol[3].ToString()));
                    tOdniesienie.Add(StrToFlo(numCol[5].ToString()));

                    //datka i godzinka
                    var data = dataCol[0].ToString();
                    var godz = dataCol[1].ToString();
                    data = data
                                .Remove(data.Length - 2)
                                .Remove(0, 1);
                    godz = godz
                                .Remove(godz.Length - 2)
                                .Remove(0, 1);

                    tDataGodz.Add(new DateTime(Convert.ToInt32(data.Substring(0, 4)),
                                               Convert.ToInt32(data.Substring(5, 2)),
                                               Convert.ToInt32(data.Substring(8, 2)),
                                               Convert.ToInt32(godz.Substring(0, 2)),
                                               Convert.ToInt32(godz.Substring(3, 2)),
                                               Convert.ToInt32(godz.Substring(6, 2))));
                    }
                catch { }

                }

            public override void Prasuj(string url, string plik)
                {
                bool statPob;
                try
                    {
                    statPob = Pobierz(url, staleapki.appdir + staleapki.tmpdir + @"\" + plik);
                    }
                catch { statPob = false; }
                
                if (statPob)
                    {
                    numKolekcja = new List<daneNumTabeli>();

                    try
                        {
                        string[] aStooqIndeksy = File.ReadAllLines(staleapki.appdir + staleapki.tmpdir + @"\" + plik);

                        var tabPraser = new Regex("<table (.+?)</table>");
                        MatchCollection matches = tabPraser.Matches(aStooqIndeksy[0]);

                        foreach (var m in matches)
                            {
                            if (m.ToString().Contains("Poprz. kurs") && !(m.ToString().Contains("c></span")))
                                {
                                ZbierzWartosci(m.ToString());
                                }
                            }

                        for (int i = 0; i < tNazwa.Count; i++)
                            {
                            numKolekcja.Add(new daneNumTabeli(tDataGodz[i],
                                                                tNazwa[i],
                                                                tSymbol[i],
                                                                tKurs[i],
                                                                tMax[i],
                                                                tMin[i],
                                                                tOtwarcie[i],
                                                                tOdniesienie[i]));
                            }
                        Loger.dodajDoLogaInfo(this.serwis + this.typ + messages.prasowanieOk + " (" + numKolekcja.Count + " rekordów)");
                        }
                    catch 
                        { 
                        numKolekcja.Add(new daneNumTabeli(DateTime.Now, "Błąd ładowania danych", "", 0.0F, 0.0F, 0.0F, 0.0F, 0.0F));
                        Loger.dodajDoLogaError(this.serwis + this.typ + messages.prasowanieFail);
                        }
                    }
                }

            //public abstract List<daneNumTabeli> generujTabele();
            }
    #endregion

    //*********************************************
    // ====== I M P L E M E N T A C J E ======= //
    //*********************************************

    #region implementacje
        // ================
        // klasa zajmująca się akcjami //
        // ================
        class Akcje : NumAkcje
            {
            private List<daneAkcji> akcjeKolekcja;

            public Akcje(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }

            public override void Prasuj(string url, string plik)
                {
                bool statPob;
                try
                    {
                    statPob = Pobierz(url, staleapki.appdir + staleapki.tmpdir + @"\" + plik);
                    }
                catch { statPob = false; }

                if (statPob)
                    {

                    akcjeKolekcja = new List<daneAkcji>();
                    try
                        {
                        string[] aStooqAkcje = File.ReadAllLines(staleapki.appdir + staleapki.tmpdir + @"\" + plik);

                        // szukanie tabel z danymi, taki główny regex
                        var tabPraser = new Regex("<table (.+?)</table>");

                        List<int> tObrot = new List<int>();
                        List<int> tWolumen = new List<int>();
                        List<int> tTransakcje = new List<int>();

                        foreach (var m in tabPraser.Matches(aStooqAkcje[0]))
                            {
                            // żeby procek nie musiał cały czas .ToString() w tym kodzie, to użyjemy pamięci.
                            var tmpm = m.ToString();

                            //ok tłumaczenie
                            // Constains "Poprz. kurs" dlatego, że tylko te interesujące nas matche tabelkowe mają wszystkie dane o akcji
                            // po prostu warunek tego, że jesteśmy w dobrej tabeli
                            // !Constains c></span> dlatego, żeby uniknąć tych jakiś 'zombie' kursów, które nawet nie były na stronie
                            // i na których się regex sypał

                            if (tmpm.Contains("Poprz. kurs") && !(tmpm.Contains("c></span>")))
                                {
                                tDataGodz.Add(zbierzDateGodz(tmpm, @"_d2>(\d*-\d*-\d*)", @"_t1>(\d*:\d*:\d*)"));
                                tNazwa.Add(zbierzNazwy(tmpm, @"href=q/\?s=(\w*)>(\.*[^<]*)"));
                                tSymbol.Add(zbierzSymbole(tmpm, @">\((\w*)\)<"));
                                tKurs.Add(zbierzWartosciFloat(tmpm, "c2"));
                                tMax.Add(zbierzWartosciFloat(tmpm, "h"));
                                tMin.Add(zbierzWartosciFloat(tmpm, "l"));
                                tOtwarcie.Add(zbierzWartosciFloat(tmpm, "o"));
                                tOdniesienie.Add(zbierzWartosciFloat(tmpm, "p"));
                                tWolumen.Add(zbierzWartosciInt(tmpm, "v2"));
                                tObrot.Add(zbierzWartosciInt(tmpm, "r2"));
                                tTransakcje.Add(zbierzWartosciInt(tmpm, "n3"));
                                }
                            }

                        for (int i = 0; i < tNazwa.Count; i++)
                            {
                            akcjeKolekcja.Add(new daneAkcji(tDataGodz[i],
                                                            tNazwa[i],
                                                            tSymbol[i],
                                                            tKurs[i],
                                                            tMax[i],
                                                            tMin[i],
                                                            tOtwarcie[i],
                                                            tOdniesienie[i],
                                                            tWolumen[i],
                                                            tObrot[i],
                                                            tTransakcje[i]));
                            }
                        Loger.dodajDoLogaInfo(this.serwis + this.typ + messages.prasowanieOk + " (" + akcjeKolekcja.Count + " rekordów)");
                        }           
                    catch 
                        { 
                        akcjeKolekcja.Add(new daneAkcji(DateTime.Now, "Błąd ładowania danych", "", 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0, 0, 0));
                        Loger.dodajDoLogaError(this.serwis + this.typ + messages.prasowanieFail);
                        }
                    }

                }

            public List<daneAkcji> generujTabele()
                {
                Prasuj(adresy.StooqAkcjeGPW,nazwyPlikow.PStooqAkcje);
                return akcjeKolekcja;
                }
            }

        // ================
        // klasa zajmująca się indeksami GPW //
        // ================
        class IndeksyGPW : NumTabela
            {
            public IndeksyGPW(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }

            public List<daneNumTabeli> generujTabele()
                {
                Prasuj(adresy.StooqIndeksyGPw, nazwyPlikow.PStooqIndeksyGPW);
                return numKolekcja;
                }
            }

        // ================
        // klasa zajmująca się indeksami światowymi //
        // ================
        class Indeksy : NumTabela
            {
            public Indeksy(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }

            public List<daneNumTabeli> generujTabele()
                {
                Prasuj(adresy.StooqIndeksySwiat,nazwyPlikow.PStooqIndeksy);
                return numKolekcja;
                }

            }

        // ================
        // klasa zajmująca się indeksami fut //
        // ================
        class IndeksyFut : NumTabela
            {
            public IndeksyFut(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }

            public List<daneNumTabeli> generujTabele()
                {
                Prasuj(adresy.StooqIndeksyFutures,nazwyPlikow.PStooqIndeksyFut);
                return numKolekcja;
                }

            }
    
        // ================
        // klasa zajmująca się towarami //
        // ================
        class Towary : NumTabela
            {
            public Towary(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }

            public List<daneNumTabeli> generujTabele()
                {
                Prasuj(adresy.StooqTowary,nazwyPlikow.PStooqTowary);
                return numKolekcja;
                }
            }

        // ================
        // klasa zajmująca się walutami //
        // ================
        class Waluty : NumTabela
            {
            public Waluty(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }
       
            public List<daneWaluty> walutyKolekcja;
            
            public override void Prasuj(string url, string plik)
                {
                bool statPob;
                try
                    {
                    statPob = Pobierz(url, staleapki.appdir + staleapki.tmpdir + @"\" + plik);
                    }
                catch { statPob = false; }

                if (statPob)
                    {
                    walutyKolekcja = new List<daneWaluty>();

                    try
                        {
                        string[] aStooqIndeksy = File.ReadAllLines(staleapki.appdir + staleapki.tmpdir + @"\" + plik);

                        var tabPraser = new Regex("<table (.+?)</table>");
                        MatchCollection matches = tabPraser.Matches(aStooqIndeksy[0]);

                        foreach (var m in matches)
                            {
                            if (m.ToString().Contains("Poprz. kurs") && !(m.ToString().Contains("c></span")))
                                {
                                ZbierzWartosci(m.ToString());
                                }
                            }

                        for (int i = 0; i < tNazwa.Count; i++)
                            {
                            walutyKolekcja.Add(new daneWaluty(tDataGodz[i],
                                                              tNazwa[i],
                                                              tSymbol[i],
                                                              tKurs[i],
                                                              tMax[i],
                                                              tMin[i],
                                                              tOtwarcie[i],
                                                              tOdniesienie[i]));
                            }
                        Loger.dodajDoLogaInfo(this.serwis + this.typ + messages.prasowanieOk + " (" + walutyKolekcja.Count + " rekordów)");
                        }
                    catch 
                        { 
                        walutyKolekcja.Add(new daneWaluty(DateTime.Now, "Błąd ładowania danych", "", 0.0F, 0.0F, 0.0F, 0.0F, 0.0F));
                        Loger.dodajDoLogaError(this.serwis + this.typ + messages.prasowanieFail);
                        }
                    }
                }

            public List<daneWaluty> generujTabele()
                {
                Prasuj(adresy.StooqWaluty,nazwyPlikow.PStooqWaluty);
                return walutyKolekcja;
                }
            }
    #endregion
    }