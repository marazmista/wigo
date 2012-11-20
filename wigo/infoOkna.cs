
// copy marazmista 16.07.2012 //

// ================
// okienka z informacjami, wykresami
// ================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace mm_gielda
    {
    class wykresOkno: Window
        {
        public BitmapImage imgPath;
        Image aa = new Image();

        public wykresOkno(BitmapImage wykres)
            {
            this.imgPath = wykres;
            this.Height = 580;
            this.Width = 810;
            this.Content = aa;
            this.Top = Application.Current.MainWindow.Top / 2;
            this.Left = Application.Current.MainWindow.Left / 2;
            }

        public void wczytajWyk()
            {
            aa.Source = imgPath;
            aa.Width = 800;
            aa.Height = 550;
            aa.Stretch = System.Windows.Media.Stretch.UniformToFill;
            }

        }
    }
