﻿
// copy marazmista 04.2012 //


// ================
// ok, to tu jest spis wszystkich stringów przewalających się w kodzie, 
// żeby zapobiec hardcode
// ================

namespace commonStrings
    {
    struct messages
        {
        public const string pobFail = " Nie udało się pobrać danych";
        public const string prasowanieFail = " Nie udało się odczytać danych z pliku";

        public const string pobOk = " Pobrano plik z danymi";
        public const string prasowanieOk = " Odczytano dane z pliku";
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