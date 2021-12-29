using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Recognition;
using GestionnaireWifiBot.Model;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using GestionnaireWifiBot.Commands;

namespace GestionnaireWifiBot.ViewModel
{
    class ControleVocaleViewModel : PiloterRoverViewModel
    {
        //----------------------------------------------
        // ATTRIBUTS
        Task roverTask;                          // Tache asynchrone pour l'envoie des valeurs au rovers
        public static bool CommandLoopActivated; // Permet de mettre en pause la tache "roverTask" 
                                                 //   lorsque la window n'est pas active
        int L_Speed_Ratio = 0;         // Ratio de rotation des roues gauches
        int R_Speed_Ratio = 0;         // Ratio de rotation des roues droite
        int vitesse = 0;   // Definit la vitesse du rover (0 - 40)
        int selectedItemIndex = 0; // Index de l'element selectionné par défaut
        string mot_reconnu;  
        SpeechRecognitionEngine MoteurReconnaissance;
        Choices MouvementChoisie;
        GrammarBuilder ContraintesReconnaissance;
        Grammar MotsAReconnaitre;

        //----------------------------------------------
        // PROPERTYS
        public string Mot_Reconnu
        {
            get { return mot_reconnu; }
            set
            {
                mot_reconnu = value;
                OnPropertyChanged(nameof(Mot_Reconnu));
            }
        }
        public int ActionVocaleIndexListe
        {
            get { return selectedItemIndex; }
            set
            {
                selectedItemIndex = value;
                OnPropertyChanged(nameof(ActionVocaleIndexListe));
            }
        }

        //----------------------------------------------
        // CONSTRUCTOR
        public ControleVocaleViewModel()
        {
            // On construit le dictionnaire des mots à reconnaitres
            MouvementChoisie = new Choices(
                "Avancer normalement", "Avancer doucement", "Avancer à droite", "Avancer à gauche",
                "Rotation à droite", "Rotation à gauche", "Arrêt",
                "Reculer normalement", "Reculer doucement", "Reculer à droite", "Reculer à gauche"
            );
            // On implante le dictionnaire dans le moteur de reconnaissance en utilisant un GrammarBuilder
            ContraintesReconnaissance = new GrammarBuilder(MouvementChoisie);
            ContraintesReconnaissance.Culture = new System.Globalization.CultureInfo("fr-FR");

            // On instancie une nouvelle grammaire avec celle précédemment construite
            MotsAReconnaitre = new Grammar(ContraintesReconnaissance);

            // Création du moteur de roconnaissance avec les mots a reconnaitre
            MoteurReconnaissance = new SpeechRecognitionEngine();   
            MoteurReconnaissance.LoadGrammar(MotsAReconnaitre);

            // Évennements liés à la reconnaissance vocale
            MoteurReconnaissance.SpeechRecognized += MoteurReconnaissance_SpeechRecognized; //Evennement déclanché lorsqu'un mot est reconnu
            MoteurReconnaissance.SpeechRecognitionRejected += MoteurReconnaissance_SpeechRecognitionRejected;   //Evennement déclanché lorsqu'un mot n'est pas reconnu

            // Definition de l'entrée audio
            try
            {
                MoteurReconnaissance.SetInputToDefaultAudioDevice();        //Capture l'entrée audio par défaut
                MoteurReconnaissance.RecognizeAsync(RecognizeMode.Multiple);
                roverTask = new Task(() => SendValues());
                roverTask.Start();
            }
            catch
            {
                MessageBox.Show("Micro non trouvé ou inaccessible !");
            }
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

        //----------------------------------------------
        // EVENTS
        private void MoteurReconnaissance_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Commande non reconnu ! : " + e.Result.Text);
        }

        private void MoteurReconnaissance_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Mot_Reconnu = e.Result.Text;    // Affichage du Mot dans la View
            switch (e.Result.Text)          // Change les valeur de 
            {
                case "Arrêt":
                    ActionVocaleIndexListe = 0;
                    vitesse = 0;
                    L_Speed_Ratio = 0;
                    R_Speed_Ratio = 0;
                    break;
                case "Avancer normalement":
                    ActionVocaleIndexListe = 1;
                    vitesse = 20;
                    L_Speed_Ratio = 100;
                    R_Speed_Ratio = 100;
                    break;
                case "Avancer doucement":
                    ActionVocaleIndexListe = 2;
                    vitesse = 10;
                    L_Speed_Ratio = 100;
                    R_Speed_Ratio = 100;
                    break;
                case "Avancer à droite":
                    ActionVocaleIndexListe = 3;
                    vitesse = 30;
                    L_Speed_Ratio = 100;
                    R_Speed_Ratio = 75;
                    break;
                case "Avancer à gauche":
                    ActionVocaleIndexListe = 4;
                    vitesse = 30;
                    L_Speed_Ratio = 75;
                    R_Speed_Ratio = 100;
                    break;
                case "Rotation à gauche":
                    ActionVocaleIndexListe = 5;
                    vitesse = 20;
                    L_Speed_Ratio = -50;
                    R_Speed_Ratio = 50;
                    break;
                case "Rotation à droite":
                    ActionVocaleIndexListe = 6;
                    vitesse = 20;
                    L_Speed_Ratio = 50;
                    R_Speed_Ratio = -50;
                    break;
                case "Reculer normalement":
                    ActionVocaleIndexListe = 7;
                    vitesse = 20;
                    L_Speed_Ratio = -100;
                    R_Speed_Ratio = -100;
                    break;
                case "Reculer doucement":
                    ActionVocaleIndexListe = 8;
                    vitesse = 10;
                    L_Speed_Ratio = -100;
                    R_Speed_Ratio = -100;
                    break;
                case "Reculer à droite":
                    ActionVocaleIndexListe = 9;
                    vitesse = 30;
                    L_Speed_Ratio = -100;
                    R_Speed_Ratio = -75;
                    break;
                case "Reculer à gauche":
                    ActionVocaleIndexListe = 10;
                    vitesse = 30;
                    L_Speed_Ratio = -75;
                    R_Speed_Ratio = -100;
                    break;
                default:
                    break;
            }
        }

    }
}
