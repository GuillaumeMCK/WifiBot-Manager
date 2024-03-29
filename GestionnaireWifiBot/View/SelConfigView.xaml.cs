﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GestionnaireWifiBot.ViewModel;

namespace GestionnaireWifiBot.View
{
    /// <summary>
    /// Logique d'interaction pour DelConfigView.xaml
    /// </summary>
    public partial class SelConfigView : Window
    {

        public SelConfigView()
        {
            InitializeComponent();
            DataContext = new ConfigRvViewModel();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
