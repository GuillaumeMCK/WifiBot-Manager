using GestionnaireWifiBot.Commands;
using System;
using System.Windows.Input;

namespace GestionnaireWifiBot.ViewModel
{
    class ControleBoutonViewModel : PiloterRoverViewModel
    {
        
        
        int vitesse;            // definit la vitesse du rover 0 - 40
        int L_Speed_Ratio = 0;  // Ratio des roues gauche
        int R_Speed_Ratio = 0;  // Ratio des roues droite

        
        
        public int SliderValue
        {
            get { return vitesse; }
            set
            {
                vitesse = value;
                SendValues(L_Speed_Ratio, R_Speed_Ratio);
                OnPropertyChanged(nameof(SliderValue));
            }
        }

        
        public ICommand UpPadButtonCommand { get; }
        public ICommand UpRightPadButtonCommand { get; }
        public ICommand UpLeftPadButtonCommand { get; }
        public ICommand RightPadButtonCommand { get; }
        public ICommand LeftPadButtonCommand { get; }
        public ICommand StopPadButtonCommand { get; }
        public ICommand DownPadButtonCommand { get; }
        public ICommand DownRightPadButtonCommand { get; }
        public ICommand DownLeftPadButtonCommand { get; }

        
        
        public ControleBoutonViewModel()
        {
            StopPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = 0, R_Speed_Ratio = 0));

            UpPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = 100, R_Speed_Ratio = 100));
            DownPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = -100, R_Speed_Ratio = -100));
            LeftPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = -50, R_Speed_Ratio = 50));
            RightPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = 50, R_Speed_Ratio = -50));

            UpRightPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = 100, R_Speed_Ratio = 75));
            UpLeftPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = 75, R_Speed_Ratio = 100));
            DownRightPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = -100, R_Speed_Ratio = -75));
            DownLeftPadButtonCommand = new BaseCommand(o => SendValues(L_Speed_Ratio = -75, R_Speed_Ratio = -100));

        }

        
        
        void SendValues(int L_Speed_Ratio, int R_Speed_Ratio)
        {
            int R, L;
            byte RTrame, LTrame;

            
            // Calcule des valeurs Left et Right a partir de la vitesse
            R = (int)Math.Abs(Math.Round(vitesse * (R_Speed_Ratio / 100f)));
            L = (int)Math.Abs(Math.Round(vitesse * (L_Speed_Ratio / 100f)));

            // Definition de la trame selon les valeurs obtenues
            if (R_Speed_Ratio >= 0) RTrame = ValeursAvancer[R];
            else RTrame = ValeursReculer[R];

            if (L_Speed_Ratio >= 0) LTrame = ValeursAvancer[Math.Abs(L)];
            else LTrame = ValeursReculer[L];
            // Definition de la commande du rover
            rover.Command = new byte[] { LTrame, RTrame };
        }
    }
}