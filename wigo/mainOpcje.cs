
// copy marazmista 6.07.2012

// ================
// Obsługa opcji
// ================

using System;
using System.Xml.Linq;
using System.Windows;

namespace mm_gielda
{
    static class config
    {
        public static bool czyPobieracIndeksyGPW = true;
        public static bool czyPobieracAkcje = true;
        public static bool czyPobieracIndeksySw = true;
        public static bool czyPobieracIndeksyFut = true;
        public static bool czyPobieracWaluty = true;
        public static bool czyPobieracTowary = true;

        public static bool czyPobieracNewsyBankier = true;
        public static bool czyPobieracKomentarzeMoney = true;

        public static byte gpwInterwal = 2;
        public static byte swiatInterwal = 2;
        public static byte newsyInterwal = 5;
    }

    public partial class MainWindow
    {
        public readonly string configFilePath = staleapki.appdir + @"\config.xml";

        private void zapiszConfig_Click(object sender, RoutedEventArgs e)
        {
            // sprawdzanie poprawności danych w interwałach
            try
            {
                if (Convert.ToByte(T_GPWinterwal.Text) == 0)
                    throw new Exception();
                if (Convert.ToByte(T_swiatInterwal.Text) == 0)
                    throw new Exception();
                if (Convert.ToByte(T_newsyInterwal.Text) == 0)
                    throw new Exception();

                zapiszOpcje();
            }
            catch { MessageBox.Show("Błędne interwały"); }
        }

        void zapiszOpcje()
        {
            var configFile = new XElement("Opcje",
                new XElement("GPW",
                    new XAttribute("czyPobieracIndeksyGPW", CB_pobierajIndeksyGPW.IsChecked),
                    new XAttribute("czyPobieracAkcjeGPW", CB_pobierajAkcjeGPW.IsChecked),
                    new XAttribute("GPWinterwal", T_GPWinterwal.Text)),
                new XElement("Swiat",
                    new XAttribute("czyPobieacIndeksySwiat", CB_pobierajIndeksySwiat.IsChecked),
                    new XAttribute("czyPobieracIndeksyFut", CB_pobierajIndeksyFut.IsChecked),
                    new XAttribute("czyPobieracWaluty", CB_pobierajWaluty.IsChecked),
                    new XAttribute("czyPobieracTowary", CB_pobierjaTowary.IsChecked),
                    new XAttribute("SwiatInterwal", T_swiatInterwal.Text)),
                new XElement("Newsy",
                    new XAttribute("czyPobieracNewsyBankier", CB_pobierajNewsyBankier.IsChecked),
                    new XAttribute("czyPobieracKomMoney", CB_pobierajKomMoney.IsChecked),
                    new XAttribute("NewsyInterwal", T_newsyInterwal.Text)));

            configFile.Save(configFilePath);         
        }

        void wczytajOpcje()
        {
            var configFile = XElement.Load(configFilePath);

            var gpwElem = configFile.Element("GPW");
            CB_pobierajIndeksyGPW.IsChecked = config.czyPobieracIndeksyGPW = (bool)gpwElem.Attribute("czyPobieracIndeksyGPW");
            CB_pobierajAkcjeGPW.IsChecked = config.czyPobieracAkcje = (bool)gpwElem.Attribute("czyPobieracAkcjeGPW");
            config.gpwInterwal = Convert.ToByte((int)gpwElem.Attribute("GPWinterwal"));
            T_GPWinterwal.Text = config.gpwInterwal.ToString();

            var swElem = configFile.Element("Swiat");
            CB_pobierajIndeksySwiat.IsChecked = config.czyPobieracIndeksySw = (bool)swElem.Attribute("czyPobieacIndeksySwiat");
            CB_pobierajIndeksyFut.IsChecked = config.czyPobieracIndeksyFut = (bool)swElem.Attribute("czyPobieracIndeksyFut");
            CB_pobierajWaluty.IsChecked = config.czyPobieracWaluty = (bool)swElem.Attribute("czyPobieracWaluty");
            CB_pobierjaTowary.IsChecked = config.czyPobieracTowary = (bool)swElem.Attribute("czyPobieracTowary");
            config.swiatInterwal = Convert.ToByte((int)swElem.Attribute("SwiatInterwal"));
            T_swiatInterwal.Text = config.swiatInterwal.ToString();

            var newsyElem = configFile.Element("Newsy");
            CB_pobierajNewsyBankier.IsChecked = config.czyPobieracNewsyBankier = (bool)newsyElem.Attribute("czyPobieracNewsyBankier");
            CB_pobierajKomMoney.IsChecked = config.czyPobieracKomentarzeMoney = (bool)newsyElem.Attribute("czyPobieracKomMoney");
            config.newsyInterwal = Convert.ToByte((int)newsyElem.Attribute("NewsyInterwal"));
            T_newsyInterwal.Text = config.newsyInterwal.ToString();
        }
    }
}
