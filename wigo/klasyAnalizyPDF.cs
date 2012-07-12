
// copy marazmista 9.07.2012

// ================
// klasy do obsługi analiz z pdfów //
// ================

using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;

using mm_gielda;
using commonStrings;

namespace klasyAnalizyPDF
{
    // szablon dla grida //
    class daneAnalizy
    {
        public string Tytul { get; set; }
        public string Link { get; set; }

        public daneAnalizy(string tytul, string link)
        {
            this.Tytul = tytul;
            this.Link = link;
        }
    }

    // metodki przydające się w pobieraniu i ogarnianiu tych list z analizami //
    abstract class mainAnaliza
    {
        protected List<daneAnalizy> analizyLista = new List<daneAnalizy>(15);

        protected string serwis;

        public virtual void Pobierz(string url, string sciezka)
        {
            try
            {
                var pobieracz = new WebClient();
                pobieracz.DownloadFile(url, sciezka);

                var fi = new FileInfo(sciezka);
                Loger.dodajDoLogaInfo(this.serwis + messages.pobListaBI + " (" + fi.Length / 1024 + "kB)");
                Loger.dodajPobrane(fi.Length);
            }
            catch { Loger.dodajDoLogaError(this.serwis + messages.pobListeBIFail); }
        }
    }

    class analizyInvestors : mainAnaliza
    {
        public analizyInvestors() { this.serwis = serwisy.Investors; }

        void prasujListe(string biUrl, string krUrl)
        {
            string tmpFilePathBI = staleapki.appDir + staleapki.tmpDir + "tmpfileBI.html";
            string tmpFilePathKR = staleapki.appDir + staleapki.tmpDir + "tmpfileKR.html";

            // tempowe listy, żeby w dwóch wątkach poleciec //
            List<daneAnalizy> tmpBI = new List<daneAnalizy>(20);
            List<daneAnalizy> tmpKR = new List<daneAnalizy>(13);

            // action dla biuletynów tygodniowych //
            Action bi = delegate()
            {
                try
                {
                    Pobierz(biUrl, tmpFilePathBI);
                    string[] aInvestorsHTML = File.ReadAllLines(tmpFilePathBI);

                    foreach (var s in aInvestorsHTML)
                    {
                        if (s.Contains("<td class=\"PDF\"><a href="))
                        {
                            var regex = new Regex("\"(.+?)\"");
                            MatchCollection matches = regex.Matches(s);

                            string tmpLink = matches[1].ToString();
                            tmpLink = tmpLink
                                            .Remove(tmpLink.Length - 1)
                                            .Remove(0, 1);

                            var tmpTytul = matches[3].ToString().Split(',');
                            var tmpTytul2 = tmpTytul[tmpTytul.Length - 1];

                            tmpBI.Add(new daneAnalizy(tmpTytul2, "http://investors.pl" + tmpLink));
                        }
                    }
                }
                catch { Loger.dodajDoLogaError(serwis + messages.pobListeBIFail); }
            };

            // action dla miesięcznych komentarzy rynkowych //
            Action kr = delegate()
            {
                try
                {
                    Pobierz(krUrl, tmpFilePathKR);
                    string[] aInvestorsHTML = File.ReadAllLines(tmpFilePathKR);

                    foreach (var s in aInvestorsHTML)
                    {
                        if (s.Contains("<td class=\"PDF\"><a href="))
                        {
                            var regex = new Regex("\"(.+?)\"");
                            MatchCollection matches = regex.Matches(s);

                            string tmpLink = matches[1].ToString();
                            tmpLink = tmpLink
                                            .Remove(tmpLink.Length - 1)
                                            .Remove(0, 1);

                            var tmpTytul = matches[3].ToString().Split(',');
                            var tmpTytul2 = tmpTytul[tmpTytul.Length - 1];

                            tmpKR.Add(new daneAnalizy("Komentarz rynkowy" + tmpTytul2, "http://investors.pl" + tmpLink));
                        }
                    }
                }
                catch { Loger.dodajDoLogaError(serwis + messages.pobListeKRFail); }
            };

            // tablica tasków
            var t = new[] {
                Task.Factory.StartNew(bi),
                Task.Factory.StartNew(kr)
            };
            // odpalenie obu i poczekanie, żeby potem wrzucić do wspólnej listy dla datagrida //
            Task.WaitAll(t);

            analizyLista.AddRange(tmpKR);
            analizyLista.AddRange(tmpBI);

            // wywalenie jesli są
            if (File.Exists(tmpFilePathBI)) File.Delete(tmpFilePathBI);
            if (File.Exists(tmpFilePathKR)) File.Delete(tmpFilePathKR);
        } 

        public List<daneAnalizy> generujTabele()
            {
                prasujListe(adresy.InvestorsBI,adresy.InvestorsKR);
                return analizyLista;
            }
    }

    class newsletterGPW : mainAnaliza
    {
        public newsletterGPW() { this.serwis = serwisy.GPW; }

        void prasujListe(string newsletterURL)
            {
            string tmpFilePath = staleapki.appDir + staleapki.tmpDir + "tmpFileNewsGPW.html";

            try
            {
                Pobierz(newsletterURL, tmpFilePath);
                string[] newsletterHTML = File.ReadAllLines(tmpFilePath);

                foreach (var s in newsletterHTML)
                {
                    if (s.Contains("<a class=\"pdf\" href=\"")) //szukanie linii, bo tam jest i linik, i tytul
                    {
                        var regexLink = new Regex("\"(.+?)\"");
                        var regexTytul = new Regex(">(.+?)<");

                        string tmpL = regexLink.Matches(s)[1].ToString();
                        tmpL = tmpL
                            .Remove(tmpL.Length - 1)
                            .Remove(0, 1);

                        string tmpT = regexTytul.Matches(s)[0].ToString();
                        tmpT = tmpT
                            .Remove(tmpT.Length - 1)
                            .Remove(0,1);

                        analizyLista.Add(new daneAnalizy("Newsletter " + tmpT,tmpL));
                    }
                }
            }
            catch { Loger.dodajDoLogaError(serwis + messages.pobGPWNewsletterFail); }
            finally { if (File.Exists(tmpFilePath)) File.Delete(tmpFilePath);};
            }
    
        public List<daneAnalizy> generujTabele()
            {
            prasujListe(adresy.GPWNewsletter);
            return analizyLista;
            }
    }

    class pekaoAnalizy : mainAnaliza
    {
        public pekaoAnalizy() { this.serwis = serwisy.Pekao; }

        void prasujListe(byte dzien, byte miesiac, ushort rok)
        {
            string tmpFilePath = staleapki.appDir + staleapki.tmpDir + "tmpPekao.html";

            try
            {
                // P R O B L E M //
                Pobierz("http://www.analizy.pekao.com.pl/?p=aPdf&m=" + miesiac + "&r=" + rok + "&d=" + dzien, tmpFilePath);

                string[] pekaoHTML = File.ReadAllLines(tmpFilePath);

                var regexTytul = new Regex(">(.+?)<");

            }
            catch { };
        }

    }
}