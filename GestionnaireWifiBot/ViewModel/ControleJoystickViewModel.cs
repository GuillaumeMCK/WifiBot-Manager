using GestionnaireWifiBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GestionnaireWifiBot.ViewModel
{
    class ControleJoystickViewModel : PiloterRoverViewModel
    {
        //----------------------------------------------
        // ATTRIBUTS
        Task roverTask;                          // Tache asynchrone pour l'envoie des valeurs au rovers
        public static bool CommandLoopActivated; // Permet de mettre en pause la tache "roverTask" 
        double x;   // Valeur de l'abscisse du joystick
        double y;   // Valeur de l'ordonnée du joystick

        //----------------------------------------------
        // PROPERTYS
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

        //----------------------------------------------
        // CONSTRUCTOR
        public ControleJoystickViewModel()
        {
            roverTask = new Task(() => SendValues());
            roverTask.Start();
        }

        //----------------------------------------------
        // METHODES
        void SendValues() // Permet d'envoyer les valeurs du joystick au rover
        {
            double x_, y_;
            double V, W;
            int R, L;
            byte RTrame, LTrame;

            // Arret de la boucle lorsque le ConnectionState du rover est sur false
            while (rover.ConnectionState != false)
            {
                // Pause de la boucle quand la window n'est pas active
                while (!CommandLoopActivated && rover.ConnectionState == true)
                    Thread.Sleep(1000);
                //-------------------------------------------
                // Calcule des valeurs Left et Right du rover
                // Invertion des valeurs X Y
                x_ = -X;
                y_ = -Y;
                // Calcule de L+R
                V = (1 - Math.Abs(x_)) * y_ + y_;
                // Calcule de R-L
                W = (1 - Math.Abs(y_)) * x_ + x_;
                // Calcule de R sur 40
                R = (int)Math.Round(((V + W) / 2f) * 40);
                // Calcule de L sur 40
                L = (int)Math.Round(((V - W) / 2f) * 40);
                // Definition de la trame selon les valeurs obtenues
                if (R >= 0) RTrame = ValeursAvancer[Math.Abs(R)];
                else RTrame = ValeursReculer[Math.Abs(R)];

                if (L >= 0) LTrame = ValeursAvancer[Math.Abs(L)];
                else LTrame = ValeursReculer[Math.Abs(L)];
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
