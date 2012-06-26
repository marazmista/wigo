
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
        public const string pobFail = " Nie udało się pobrać danych";
        public const string prasowanieFail = " Nie udało się odczytać najnowszych danych z pliku";

        public const string pobOk = " Pobrano plik z danymi";
        public const string prasowanieOk = " Odczytano dane z pliku";

        public const string xmlOutOK = " Weksportowano do XML";
        public const string xmlOutFail = " Eksport do XML nieudany";
        public const string xmlOk = " Wczytano wartośći z pliku XML";
        public const string xmlFail = " Brak poprzednich dobrych danych w pliku XML (pierwsze uruchomienie, weekend?)";

        // odświeżanie składu indeksów //
        public const string indOdswStart = "**** Rozpoczęcie aktualizacji składu indeksów GPW ****";
        public const string indOdswKoniec = "**** Zakończono aktualizację składu indeksów GPW *****";
        public const string indAktual = "Aktualizacja składu indeksu ";
        public const string indOdswFail = " Nie udało się odświeżyć zawartości indeksów GPW (jesteś połączony z internetem?)";
        public const string indLoadFail = " Nie udało sie odczytać składu indeksu";
        }


    struct typy
        {
        public const string Wiadomosci = " Wiadomości";
        public const string Komentarze = " Komentarze";
        public const string Akcje = " Akcje";
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
        }

    struct nazwyPlikow
        {
        public const string PBankierRynki = "bankierrynki.html";
        public const string PBankierAkcje = "bankierakcje.html";
        public const string PMoneyKomentarze = "moneykomentarze.html";

        public const string PStooqAkcje = "stooqakcje.html";
        public const string PStooqIndeksy = "stooqindeksy.html";
        public const string PStooqIndeksyGPW = "stooqindeksygpw.html";
        public const string PStooqIndeksyFut = "stooqindeksyfut.html";
        public const string PStooqWaluty = "stooqwaluty.html";
        public const string PStooqTowary = "stooqtowary.html";

        public const string xmlStooqAkcje = "stooqakcje.xml";
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
        public const string StooqIndeksySwiat = "http://stooq.pl/notowania/index.html?kat=i1&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqIndeksyGPw = "http://stooq.pl/notowania/index.html?kat=g1&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqTowary = "http://stooq.pl/notowania/index.html?kat=t1&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqIndeksyFutures = "http://stooq.pl/notowania/index.html?kat=i6&show=3&wtr=1&chart_=1&typ_=l&sort=";
        public const string StooqWaluty = "http://stooq.pl/notowania/index.html?kat=w1&show=3&wtr=1&chart_=1&typ_=l&sort=";
        }

    struct kategorie
        {
        public const string BankierRynki = "Wiadomości Rynki";
        public const string BankierAkcje = "Wiadomości Akcje";
        public const string MoneyKomentarze = "Komentarze Money";
        }
    }