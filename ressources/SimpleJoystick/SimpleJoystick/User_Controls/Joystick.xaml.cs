using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleJoystick.User_Controls
{
    /// <summary>
    /// Logique d'interaction pour Joystick.xaml
    /// </summary>
    public partial class Joystick : UserControl, INotifyPropertyChanged
    {
        private double rayonDuJoystick;
        private double rayonDuKnob;
        private double XJoystickValue;
        private double YJoystickValue;

        public double X {
            get { return XJoystickValue; }
            set {
                XJoystickValue = value;
                OnPropertyChanged(nameof(X));
            }
        }
        public double Y
        {
            get { return YJoystickValue; }
            set
            {
                YJoystickValue = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        public Joystick()
        {
            InitializeComponent();

            // Recuperation des rayons du joystick
            rayonDuJoystick = joystick.Height * 0.5;
            // Recuperation du rayon du Handler
            rayonDuKnob = knob.Width * 0.5;

            XJoystickValue = 0;
            YJoystickValue = 0;

            UpdateKnobPosition();
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            //Make coords related to the center
            // Recuperation des coordonnées de la souris dans le joystick
            Vector positionVtJoystick = e.GetPosition(joystick) - new Point(rayonDuJoystick, rayonDuJoystick);

            //Normalize coords
            positionVtJoystick /= rayonDuJoystick;

            //Limit R [0; 1]
            if (positionVtJoystick.Length > 1.0)
                positionVtJoystick.Normalize();

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                X = positionVtJoystick.X;
                Y = positionVtJoystick.Y;

                UpdateKnobPosition();
            }
        }

        void UpdateKnobPosition()
        {
            // Repositionnement du Handler
            Canvas.SetLeft(knob, Canvas.GetLeft(joystick) + (XJoystickValue * rayonDuJoystick) + (rayonDuJoystick - rayonDuKnob));
            Canvas.SetTop(knob, Canvas.GetTop(joystick) + (YJoystickValue * rayonDuJoystick) + (rayonDuJoystick - rayonDuKnob));
        }

        void ResetKnobPosition(object sender, MouseEventArgs e)
        {
            X = 0f;
            Y = 0f;
            Canvas.SetLeft(knob, Canvas.GetLeft(joystick) + rayonDuJoystick - rayonDuKnob);
            Canvas.SetTop(knob, Canvas.GetTop(joystick) + rayonDuJoystick - rayonDuKnob);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

