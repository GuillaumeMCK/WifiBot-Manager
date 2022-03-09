using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionnaireWifiBot.User_Controls.Base
{
    /// <summary>
    /// Logique d'interaction pour Joystick.xaml
    /// </summary>
    public partial class Joystick : UserControl
    {
        private double rayonDuJoystick;
        private double rayonDuKnob;

        public double Xvalue
        {
            get { return (double)GetValue(XvalueProperty); }
            set { SetValue(XvalueProperty, value);}
        }
        public double Yvalue
        {
            get { return (double)GetValue(YvalueProperty); }
            set { SetValue(YvalueProperty, value);}
        }
        // Using a DependencyProperty as the backing store for Xvalue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XvalueProperty =
            DependencyProperty.Register("Xvalue", typeof(double), typeof(Joystick), new PropertyMetadata(0.0));
        // Using a DependencyProperty as the backing store for Yvalue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YvalueProperty =
            DependencyProperty.Register("Yvalue", typeof(double), typeof(Joystick), new PropertyMetadata(0.0));
        
        public Joystick()
        {
            InitializeComponent();

            // Recuperation des rayons du joystick
            rayonDuJoystick = joystick.Height * 0.5;
            // Recuperation du rayon du knob
            rayonDuKnob = knob.Width * 0.5;

            Xvalue = 0.0;
            Yvalue = 0.0;

            UpdateKnobPosition();
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            //Make coords related to the center
            // Recuperation des coordonnées de la souris dans le joystick
            Vector positionVectorJoystick = e.GetPosition(joystick) - new Point(rayonDuJoystick, rayonDuJoystick);

            //Normalize coords
            positionVectorJoystick /= rayonDuJoystick;

            //Limite du rayon [0; 1]
            if (positionVectorJoystick.Length > 1.0)
                positionVectorJoystick.Normalize();

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Xvalue = positionVectorJoystick.X;
                Yvalue = positionVectorJoystick.Y;

                UpdateKnobPosition();
            }
        }

        void UpdateKnobPosition()
        {
            // Repositionnement du knob (view)
            Canvas.SetLeft(knob, Canvas.GetLeft(joystick) + (Xvalue * rayonDuJoystick) + (rayonDuJoystick - rayonDuKnob));
            Canvas.SetTop(knob, Canvas.GetTop(joystick) + (Yvalue * rayonDuJoystick) + (rayonDuJoystick - rayonDuKnob));
        }

        void ResetKnobPosition(object sender, MouseEventArgs e)
        {
            Xvalue = 0.0;
            Yvalue = 0.0;
            Canvas.SetLeft(knob, Canvas.GetLeft(joystick) + rayonDuJoystick - rayonDuKnob);
            Canvas.SetTop(knob, Canvas.GetTop(joystick) + rayonDuJoystick - rayonDuKnob);
        }

    }
}
