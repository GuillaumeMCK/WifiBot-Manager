using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GestionnaireWifiBot.Commands;
using GestionnaireWifiBot.Model;

namespace GestionnaireWifiBot.ViewModel
{
    class ControleBoutonViewModel : PiloterRoverViewModel
    {
        public static bool CommandLoopActivated;
        Task roverTask;
        int L = 0;
        int R = 0;
        int vitesse;
        public int SliderValue
        {
            get { return vitesse; }
            set
            {
                vitesse = value;
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
            StopPadButtonCommand = new BaseCommand(o => { L = 0; R = 0; });

            UpPadButtonCommand = new BaseCommand(o => { L = 100; R = 100; });
            DownPadButtonCommand = new BaseCommand(o => { L = -100; R = -100; });
            LeftPadButtonCommand = new BaseCommand(o => { L = -50; R = 50; });
            RightPadButtonCommand = new BaseCommand(o => { L = 50; R = -50; });

            UpRightPadButtonCommand = new BaseCommand(o => { L = 100; R = 75; });
            UpLeftPadButtonCommand = new BaseCommand(o => { L = 75; R = 100; });
            DownRightPadButtonCommand = new BaseCommand(o => { L = -100; R = -75; });
            DownLeftPadButtonCommand = new BaseCommand(o => { L = -75; R = -100; });

            roverTask = new Task(() => SendCommand2Rover());

            roverTask.Start();
        }

        void SendCommand2Rover()
        {
            byte RTrame;
            byte LTrame;
            int normR;
            int normL;
            while (rover.ConnectionState != false)
            {
                while (!CommandLoopActivated && rover.ConnectionState == true)
                {
                    Console.WriteLine("Waiting Button Control Loop...");
                    Thread.Sleep(1000);
                }

                normR = (int)Math.Abs(Math.Round(vitesse * (R / 100.0)));
                normL = (int)Math.Abs(Math.Round(vitesse * (L / 100.0)));

                if (R >= 0)
                    RTrame = ValeursAvancer[normR];
                else
                    RTrame = ValeursReculer[normR];
                if (L >= 0)
                    LTrame = ValeursAvancer[normL];
                else
                    LTrame = ValeursReculer[normL];

                //Console.WriteLine( L + " : " + normL + "  "+ R +" : " + normR);

                rover.Commander(new byte[] { LTrame, RTrame });
                Thread.Sleep(200);
            };
            roverTask.Wait();
            roverTask.Dispose();
        }
    }
}