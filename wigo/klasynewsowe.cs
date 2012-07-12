
// copy marazmista 04.2012 //


// ================
// wszystkie klasy związane z wiadomościami są tutaj //
// ================

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Generic;

using mm_gielda;
using commonStrings;

namespace klasynewsowe
    {

//*********************************************
// ====== ABSTRACTY DO IMPLEMENTACJI ======= //
//*********************************************


    // ================
    // szablon do generowania listy kompatybilnej z datagridem //
    // ================
    public class daneNewsa
        {
        public string Data {get; set; }
        public string Kategoria {get; set; }
        public string Serwis {get; set; }
        public string Tytul {get; set; }
        public string Link {get; set; }

        public daneNewsa() { }
        public daneNewsa(string data, string kategoria, string serwis, string tytul, string link)
            {
            this.Data = data;
            this.Kategoria = kategoria;
            this.Serwis = serwis;
            this.Tytul = tytul;
            this.Link = link;
            }
        }


    // ================
    // bazowa klasa dla newsów z procedurkami i zmiennymi dla nich //
    // ================
    internal abstract class News
        {
        protected string data, godzina, kategoria, serwis, tytul, link, typ;

        public abstract void Prasuj();
        public abstract List<daneNewsa> generujTabele();

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
        }



//*********************************************
// ====== I M P L E M E N T A C J E ======= //
//*********************************************


    // ================
    // klasa obrabiająca newsy z bankiera, ogólnie serwis bankier (newsy, rynki itp.) //
    // ================
    class newsBankier : News
        {
        private List<daneNewsa> bankierKolekcja;

        // ---- konsztruktor ----//
        public newsBankier(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }

        public override void Prasuj()
            {
            bool statPob;
            try
                {
                statPob = Pobierz(adresy.BankierRynkiNews, staleapki.appDir + staleapki.tmpDir  + nazwyPlikow.PBankierRynki);
                }
            catch { statPob = false; }

           // ---- jesli pobrało plik, to zabieramy sie za prasowanie ----//
            if (statPob)
                {
                
                this.kategoria = kategorie.BankierRynki;

                // ---- lista ze sprasowanymi rzeczami bankiera ----//
                bankierKolekcja = new List<daneNewsa>();

                try
                    {
                    string[] aBankierRynki = File.ReadAllLines(staleapki.appDir + staleapki.tmpDir  + nazwyPlikow.PBankierRynki);

                    foreach (string s in aBankierRynki)
                        {
                        // ---- wyławianie daty ----//
                        if (s.Contains("-") && s.Contains("artSrc"))
                            this.data = s.Substring(s.IndexOf("\">") + 2, 10);

                        // ---- wyławianie godziny ----//
                        if (s.Contains(":") && s.Contains("artSrc"))
                            this.godzina = s.Substring(s.IndexOf(">") + 1, 5);

                        // ---- wyławianie danych artykulu ----//
                        if (s.Contains("articleTitleLink"))
                            {

                            // m o ż n a    k i e d y s    p o p r a w i c //

                            string tmpstring = s.Replace('"', ';');  // były problemy z " jako znakiem, wiec zamienione //

                            Regex rzeczy = new Regex(@"(;[^;]+;)");   // łapanie rzeczy z pomiędzy ';'
                            MatchCollection match = rzeczy.Matches(tmpstring);

                            this.link = match[1].Value;
                            this.tytul = match[2].Value;
                            // ---- clean up linii z ';' na koncu i początku ---- //
                            this.link = this.link
                                                .Remove(this.link.Length - 1)
                                                .Remove(0, 1);

                            this.tytul = this.tytul
                                                 .Remove(this.tytul.Length - 1)
                                                 .Remove(0, 1);
                                                 
                            // ---- na koniec wrzucenie do listy, majac wszystkie dane ---- //
                            bankierKolekcja.Add(new daneNewsa(this.data + " " + this.godzina,
                                                              this.kategoria,
                                                              this.serwis,
                                                              this.tytul,
                                                              "http://www.bankier.pl"+ this.link));
                            }
                        }
                    Loger.dodajDoLogaInfo(this.serwis + this.typ + messages.prasowanieOk + " (" + bankierKolekcja.Count + " rekordów)");
                   }
                catch  { Loger.dodajDoLogaError(this.serwis + this.typ + messages.prasowanieFail); }
                finally  { File.Delete(staleapki.appDir + staleapki.tmpDir + nazwyPlikow.PBankierRynki); }; // żeby nie było bałaganu
              }
            }

        // ---- wyrzucanie listy na zewnątrz ----//
        public override List<daneNewsa> generujTabele()
            {
            Prasuj();
            return bankierKolekcja;
            }
    }

    class komentarzeMoney : News
        {
        private List<daneNewsa> moneyKolekcja;

        public komentarzeMoney(string serwis, string typ) { this.serwis = serwis; this.typ = typ; }

        public override void Prasuj()
            {
            bool statPob;
            try
                {
                statPob = Pobierz(adresy.MoneyKomentarze, staleapki.appDir + staleapki.tmpDir  + nazwyPlikow.PMoneyKomentarze);
                }
            catch { statPob = false; }

            if (statPob)
                {
                try
                    {
                    this.kategoria = kategorie.MoneyKomentarze;

                    moneyKolekcja = new List<daneNewsa>();

                    string[] aKomentarzeMoney = File.ReadAllLines(staleapki.appDir + staleapki.tmpDir  + nazwyPlikow.PMoneyKomentarze);
                    short i = 0;

                    foreach (string s in aKomentarzeMoney)
                        {
                        if (s.Contains("<div class=\"data\"><a href"))
                            {
                            this.data = s.Substring(s.Length - 20, 10);

                            string tmps = aKomentarzeMoney[i + 1].ToString();

                            var reg = new Regex("=\"(.+?)\"");
                            MatchCollection match = reg.Matches(tmps);

                            string tmps2;
                            tmps2 = match[0].ToString();

                            tmps2 = tmps2
                                .Remove(tmps2.Length - 1)
                                .Remove(0, 2);

                            this.link = tmps2;

                            tmps2 = match[1].ToString();

                            tmps2 = tmps2
                                .Remove(tmps2.Length - 1)
                                .Remove(0, 2);

                            this.tytul = tmps2;

                            moneyKolekcja.Add(new daneNewsa(
                                this.data,
                                this.kategoria,
                                this.serwis,
                                this.tytul,
                                this.link));
                            }
                        i++;
                        }
                    Loger.dodajDoLogaInfo(this.serwis + this.typ + messages.prasowanieOk + " (" + moneyKolekcja.Count + " rekordów)");
                    }
                catch { Loger.dodajDoLogaError(this.serwis + this.typ + messages.prasowanieFail); }
                finally { File.Delete(staleapki.appDir + staleapki.tmpDir + nazwyPlikow.PMoneyKomentarze); };
                }
            }

        public override List<daneNewsa> generujTabele()
            {
            Prasuj();
            return moneyKolekcja;
            }
        }
    }