using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GestionnaireWifiBot.Commands;

namespace GestionnaireWifiBot.ViewModel
{
    class ControleBoutonViewModel : PiloterRoverViewModel
    {
        //----------------------------------------------
        // ATTRIBUTS
        Task roverTask;                          // Tache asynchrone pour l'envoie des valeurs au rovers
        public static bool CommandLoopActivated; // Permet de mettre en pause la tache "roverTask" 
        int vitesse;            // definit la vitesse du rover 0 - 40
        int L_Speed_Ratio = 0;  // Ratio des roues gauche
        int R_Speed_Ratio = 0;  // Ratio des roues droite

        //----------------------------------------------
        // PROPERTYS
        public int SliderValue
        {
            get { return vitesse; }
            set
            {
                vitesse = value;
                OnPropertyChanged(nameof(SliderValue));
            }
        }

        // COMMANDS
        public ICommand UpPadButtonCommand { get; }
        public ICommand UpRightPadButtonCommand { get; }
        public ICommand UpLeftPadButtonCommand { get; }
        public ICommand RightPadButtonCommand { get; }
        public ICommand LeftPadButtonCommand { get; }
        public ICommand StopPadButtonCommand { get; }
        public ICommand DownPadButtonCommand { get; }
        public ICommand DownRightPadButtonCommand { get; }
        public ICommand DownLeftPadButtonCommand { get; }

        //----------------------------------------------
        // CONSTRUCTOR
        public ControleBoutonViewModel()
        {
            StopPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = 0; R_Speed_Ratio = 0; });

            UpPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = 100; R_Speed_Ratio = 100; });
            DownPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = -100; R_Speed_Ratio = -100; });
            LeftPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = -50; R_Speed_Ratio = 50; });
            RightPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = 50; R_Speed_Ratio = -50; });

            UpRightPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = 100; R_Speed_Ratio = 75; });
            UpLeftPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = 75; R_Speed_Ratio = 100; });
            DownRightPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = -100; R_Speed_Ratio = -75; });
            DownLeftPadButtonCommand = new BaseCommand(o => { L_Speed_Ratio = -75; R_Speed_Ratio = -100; });

            roverTask = new Task(() => SendValues());

            roverTask.Start();
        }

        //----------------------------------------------
        // METHODES
        void SendValues()
        {
            int R, L;
            byte RTrame, LTrame;

            // Arret de la boucle lorsque le ConnectionState du rover est sur false
            while (rover.ConnectionState != false)
            {
                // Pause de la boucle quand la window n'est pas active
                while (!CommandLoopActivated && rover.ConnectionState == true)
                    Thread.Sleep(1000);
                //-------------------------------------------
                // Calcule des valeurs Left et Right a partir de la vitesse
                R = (int)Math.Abs(Math.Round(vitesse * (R_Speed_Ratio / 100f)));
                L = (int)Math.Abs(Math.Round(vitesse * (L_Speed_Ratio / 100f)));

                // Definition de la trame selon les valeurs obtenues
                if (R_Speed_Ratio >= 0) RTrame = ValeursAvancer[R];
                else RTrame = ValeursReculer[R];

                if (L_Speed_Ratio >= 0) LTrame = ValeursAvancer[Math.Abs(L)];
                else LTrame = ValeursReculer[L];
                // Envoi de la trame au rover
                rover.Commander(new byte[] { LTrame, RTrame });
                // Temporisation
                Thread.Sleep(200);
            };
            
            roverTask.Wait();
            roverTask.Dispose();
        }
    }
}