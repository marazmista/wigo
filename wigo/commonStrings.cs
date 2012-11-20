
// copy marazmista 04.2012 //


// ================
// ok, to tu jest spis wszystkich stringów przewalających się w kodzie, 
// żeby zapobiec hardcode
// ================

using System.Collections.Generic;

namespace commonStrings
    {
    struct messages
        {
        // ogólne //
        public const string pobStart = " Rozpoczęto pobieranie...";

        public const string pobFail = " Nie udało się pobrać danych";
        public const string prasowanieFail = " Nie udało się odczytać najnowszych danych z pliku";

        public const string pobOk = " Pobrano plik z danymi";
        public const string prasowanieOk = " Odczytano dane z pliku";

        public const string xmlOutOK = " Weksportowano do XML";
        public const string xmlOutFail = " Eksport do XML nieudany";
        public const string xmlOk = " Wczytano wartości z pliku XML";
        public const string xmlFail = " Brak poprzednich dobrych danych w pliku XML (pierwsze uruchomienie, weekend?)";

        // odświeżanie składu indeksów //
        public const string indOdswStart = "**** Rozpoczęcie aktualizacji składu indeksów GPW ****";
        public const string indOdswKoniec = "**** Zakończono aktualizację składu indeksów GPW *****";
        public const string indAktual = "Aktualizacja składu indeksu ";
        public const string indOdswFail = " Nie udało się odświeżyć zawartości indeksów GPW (jesteś połączony z internetem?)";
        public const string indLoadFail = " Nie udało sie odczytać składu indeksu";

        // dolne kwadraciki error wczytywania //
        public const string indBoxRefreshError = "Błąd wczytywania informacji o indeksach GPW do dolnych pól (pusta tabela?)";
        public const string walutyBoxRefreshError = "Błąd wczytywania informacji o walutach do dolnych pól (pusta tabela?)";

        public const string pobListaBI = " Pobrano listę biuletynów tygodniowych";
        public const string pobListeBIFail = " Nie udało sie pobrać listy biuletynów tygodniowych";
        public const string pobListeKR = " Pobrano listę komentarzy rynkowych";
        public const string pobListeKRFail = "Nie udało sie pobrać listy komentarzy rynkowych";

        public const string pobGPWNewsletterFail = " Nie udało się pobrać listy newsletterów GPW";

        public const string docFail = " Nie udało się pobrać dokumentu";
        }


    struct typy
        {
        public const string Wiadomosci = " Wiadomości";
        public const string Komentarze = " Komentarze";

        public const string Akcje = " Akcje";
        public const string AkcjeNC = " Akcje NewConnect";
        public const string Indeksy = " Indeksy";
        public const string IndeksyGPW = " Indeksy GPW";
        public const string IndeksyFut = " Indeksy Fut.";
        public const string Towary = " Towary";
        public const string Waluty = " Waluty";
        }

    struct serwisy
        {
        public const string Bankier = "Bankier.pl";
        public const string Money = "Money.pl";
        public const string Stooq = "Stooq.pl";

        public const string Investors = "Investors.pl";
        public const string Pekao = "Pekao.com.pl";
        public const string GPW = "GPW.pl";
        }

    struct nazwyPlikow
        {
        public const string PBankierRynki = "bankierrynki.html";
        public const string PBankierAkcje = "bankierakcje.html";
        public const string PMoneyKomentarze = "moneykomentarze.html";

        public const string PStooqAkcje = "stooqakcje.html";
        public const string PStooqAkcjeNC = "stooqnc.html";
        public const string PStooqIndeksy = "stooqindeksy.html";
        public const string PStooqIndeksyGPW = "stooqindeksygpw.html";
        public const string PStooqIndeksyFut = "stooqindeksyfut.html";
        public const string PStooqWaluty = "stooqwaluty.html";
        public const string PStooqTowary = "stooqtowary.html";

        public const string xmlStooqAkcje = "stooqakcje.xml";
        public const string xmlStooqAkcjeNC = "stooqakcjenc.xml";
        public const string xmlStooqIndeksy = "stooqindeksy.xml";
        public const string xmlStooqIndeksyGPW = "stooqindeksygpw.xml";
        public const string xmlStooqIndeksyFut = "stooqfut.xml";
        public const string xmlStooqWaluty = "stooqwaluty.xml";
        public const string xmlStooqTowary = "stooqtowary.xml";
        }

    struct adresy
        {
        // ---- używane w klasach newsowych ----//
        public const string BankierRynkiNews = "http://www.bankier.pl/inwestowanie/gielda/news.html?type_id=1&display=2";
        public const string BankierAkcjeNews = "http://www.bankier.pl/inwestowanie/gielda/news.html?type_id=1&category=21&order=priority&priority=2-20";
        public const string MoneyKomentarze = "http://www.money.pl/gielda/komentarze/";


        // ---- używane w klasach akcjowych ----//
        public const string StooqAkcjeGPW = "http://stooq.pl/notowania/index.html?kat=g2&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqAkcjeNC = "http://stooq.pl/notowania/index.html?&kat=n1&show=3&chart=1&typ=l&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqIndeksySwiat = "http://stooq.pl/notowania/index.html?kat=i1&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqIndeksyGPw = "http://stooq.pl/notowania/index.html?kat=g1&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqTowary = "http://stooq.pl/notowania/index.html?kat=t1&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqIndeksyFutures = "http://stooq.pl/notowania/index.html?kat=i6&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqWaluty = "http://stooq.pl/notowania/index.html?kat=w1&show=3&wtr=1&chart_=1&typ_=l&sort=";

        public const string StooqMWykres = "http://stooq.pl/c/?p&s=";
        public const string StooqSkladIndeks = "http://stooq.pl/q/i/?s=";
        public const string StooqProfilSpolki = "http://stooq.pl/q/p/?s=";
        public const string StooqAt = "http://stooq.pl/q/a/?s=";

        public const string InvestorsBI = "http://investors.pl/dokumenty/biuletyny-tygodniowe.html";
        public const string InvestorsKR = "http://investors.pl/dokumenty/komentarze-rynkowe.html";

        public const string GPWNewsletter = "http://www.gpw.pl/newsletter";
        }

    struct kategorie
        {
        public const string BankierRynki = "Wiadomości Rynki";
        public const string BankierAkcje = "Wiadomości Akcje";
        public const string MoneyKomentarze = "Komentarze Money";
        }
    }