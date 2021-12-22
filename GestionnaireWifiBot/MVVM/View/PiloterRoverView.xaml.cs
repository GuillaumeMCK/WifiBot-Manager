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
using GestionnaireWifiBot.MVVM.ViewModel;

namespace GestionnaireWifiBot.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour PiloterRoverView.xaml
    /// </summary>
    public partial class PiloterRoverView : Window
    {
        ControleBoutonViewModel controleBoutonViewModel;
        ControleJoystickViewModel controleJoystickViewModel;
        ControleVocaleViewModel controleVocaleViewModel;

        public PiloterRoverView()
        {
            controleBoutonViewModel = new ControleBoutonViewModel();
            controleJoystickViewModel = new ControleJoystickViewModel();
            controleVocaleViewModel = new ControleVocaleViewModel();

            ControleBoutonViewModel.CommandLoopActivated = true;
            ControleJoystickViewModel.CommandLoopActivated = false;
            ControleVocaleViewModel.CommandLoopActivated = false;

            InitializeComponent();
            DataContext = controleBoutonViewModel;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Menu_Item_GOTO_ButtonMode(object sender, RoutedEventArgs e)
        {
            if (DataContext != controleBoutonViewModel)
            {
                ControleBoutonViewModel.CommandLoopActivated = true;
                ControleJoystickViewModel.CommandLoopActivated = false;
                ControleVocaleViewModel.CommandLoopActivated = false;
                DataContext = controleBoutonViewModel;
            }
        }

        private void Menu_Item_GOTO_JoystickMode(object sender, RoutedEventArgs e)
        {
            if (DataContext != controleJoystickViewModel)
            {
                ControleBoutonViewModel.CommandLoopActivated = false;
                ControleJoystickViewModel.CommandLoopActivated = true;
                ControleVocaleViewModel.CommandLoopActivated = false;
                DataContext = controleJoystickViewModel;
            }
        }

        private void Menu_Item_GOTO_VocaleMode(object sender, RoutedEventArgs e)
        {
            if (DataContext != controleVocaleViewModel)
            {
                ControleBoutonViewModel.CommandLoopActivated = false;
                ControleJoystickViewModel.CommandLoopActivated = false;
                ControleVocaleViewModel.CommandLoopActivated = true;
                DataContext = controleVocaleViewModel;
            }
        }
    }
}
