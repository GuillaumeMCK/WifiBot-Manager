using GestionnaireWifiBot.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GestionnaireWifiBot.MVVM.ViewModel
{
    class ControleJoystickViewModel : PiloterRoverViewModel
    {
        Task roverTask;
        public static bool CommandLoopActivated;

        double x;
        double y;
        
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public ControleJoystickViewModel()
        {
            roverTask = new Task(() => SendJstkVals2Rover());

            if (rover == null || rover.ConnectionState == false)
            {
                rover = new Rover(rvConfig.AdresseIP, rvConfig.PortTCP);
                rover.Connection();
            }

            if (rover.ConnectionState == true)
            {
                roverTask.Start();
            }
            else
            {
                MessageBox.Show("Une erreur est survenue lors de la connexion au rover.",
                                "Erreur !",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }


        void SendJstkVals2Rover()
        {
            double x_, y_;
            double V;
            double W;
            double R, L;
            int normR, normL;
            byte RTrame, LTrame;

            while (rover.ConnectionState != false)
            {
                while (!CommandLoopActivated && rover.ConnectionState == true)
                {
                    Console.WriteLine("Waiting Joystick Control Loop...");
                    Thread.Sleep(1000);
                }

                x_ = -X * 100;
                y_ = -Y * 100;
                // Calcule de L+R
                V = (100 - Math.Abs(x_)) * (y_ / 100) + y_;
                // Calcule de R-L
                W = (100 - Math.Abs(y_)) * (x_ / 100) + x_;
                // Calcule de R
                R = (V + W) / 2;
                // Calcule de L
                L = (V - W) / 2;

                // Normalisation des valeurs
                normR = (int)Math.Abs(Math.Round(R / 2));
                if (normR > 40) normR = 40;
                normL = (int)Math.Abs(Math.Round(L / 2));
                if (normL > 40) normL = 40;

                // Envoi de la trame selon les valeurs obtenues
                if (R >= 0)
                    RTrame = ValeursAvancer[normR];
                else
                    RTrame = ValeursReculer[normR];
                if (L >= 0)
                    LTrame = ValeursAvancer[normL];
                else
                    LTrame = ValeursReculer[normL];

                rover.Commander(new byte[] { LTrame, RTrame });
                Thread.Sleep(200);
            };
            roverTask.Wait();
            roverTask.Dispose();
        }
    }
}
