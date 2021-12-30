using System;

namespace GestionnaireWifiBot.ViewModel
{
    class ControleJoystickViewModel : PiloterRoverViewModel
    {
        //----------------------------------------------
        // ATTRIBUTS
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
                SendValues();
                OnPropertyChanged(nameof(X));
            }
        }
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                SendValues();
                OnPropertyChanged(nameof(Y));
            }
        }

        //----------------------------------------------
        // METHODES
        void SendValues() // Permet d'envoyer les valeurs du joystick au rover
        {
            double _x, _y;
            double V, W;
            int R, L;
            byte RTrame, LTrame;
            //-------------------------------------------
            // Calcule des valeurs Left et Right du rover
            // Invertion des valeurs X Y
            _x = -X;
            _y = -Y;
            // Calcule de L+R
            V = (1 - Math.Abs(_x)) * _y + _y;
            // Calcule de R-L
            W = (1 - Math.Abs(_y)) * _x + _x;
            // Calcule de R sur 40
            R = (int)Math.Round(((V + W) / 2f) * 40);
            // Calcule de L sur 40
            L = (int)Math.Round(((V - W) / 2f) * 40);
            // Definition de la trame selon les valeurs obtenues
            if (R >= 0) RTrame = ValeursAvancer[Math.Abs(R)];
            else RTrame = ValeursReculer[Math.Abs(R)];

            if (L >= 0) LTrame = ValeursAvancer[Math.Abs(L)];
            else LTrame = ValeursReculer[Math.Abs(L)];
            // Definition de la commande du rover
            rover.Command = new byte[] { LTrame, RTrame };
        }
    }
}
