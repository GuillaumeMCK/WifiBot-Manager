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
        public static bool CommandLoopActivated;
        int L = 0;       // Ratio de rotation des roues gauches
        int R = 0;       // Ratio de rotation des roues droite
        int vitesse = 0; // definit la vitesse du rover 0 - 40
        string mot_reconnu;
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
            get { return actionVocaleIndexListe; }
            set
            {
                actionVocaleIndexListe = value;
                OnPropertyChanged(nameof(ActionVocaleIndexListe));
            }
        }
        int actionVocaleIndexListe = 0;
        bool microIsFound = false;
        Task roverTask;
        SpeechRecognitionEngine MoteurReconnaissance;
        Choices MouvementChoisie;
        GrammarBuilder ContraintesReconnaissance;
        Grammar MotsAReconnaitre;
        
        public ControleVocaleViewModel()
        {
            //On construit le dictionnaire des mots à reconnaitre, ceux qui ne figurent pas dans cette liste ne seront pas reconnus
            MouvementChoisie = new Choices(
                "Avancer normalement", "Avancer doucement", "Avancer à droite", "Avancer à gauche",
                "Rotation à droite", "Rotation à gauche", "Arrêt",
                "Reculer normalement", "Reculer doucement", "Reculer à droite", "Reculer à gauche"
            );
            //On implante le dictionnaire dans le moteur de reconnaissance en utilisant un GrammarBuilder
            ContraintesReconnaissance = new GrammarBuilder(MouvementChoisie);
            ContraintesReconnaissance.Culture = new System.Globalization.CultureInfo("fr-FR");

            MotsAReconnaitre = new Grammar(ContraintesReconnaissance);
            MoteurReconnaissance = new SpeechRecognitionEngine();   //Création d'un objet reconnaissance vocale
            MoteurReconnaissance.LoadGrammar(MotsAReconnaitre);

            //évennements liés à la reconnaissance vocale
            MoteurReconnaissance.SpeechRecognized += MoteurReconnaissance_SpeechRecognized; //Evennement déclanché lorsqu'un mot est reconnu
            MoteurReconnaissance.SpeechRecognitionRejected += MoteurReconnaissance_SpeechRecognitionRejected;   //Evennement déclanché lorsqu'un mot n'est pas reconnu

            try
            {
                MoteurReconnaissance.SetInputToDefaultAudioDevice();    //Capture l'entrée audio par défaut
                microIsFound = true;
            }
            catch
            {
                MessageBox.Show("Micro non trouvé ou inaccessible !");
            }

            if(microIsFound)
            {
                MoteurReconnaissance.RecognizeAsync(RecognizeMode.Multiple);

                roverTask = new Task(() => SendVocalVals2Rover());

                roverTask.Start();
            }
        }

        private void MoteurReconnaissance_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Mot_Reconnu = e.Result.Text;
            switch (e.Result.Text)  //exécute la commande en fonction de mots définits
            {
                case "Arrêt":
                    ActionVocaleIndexListe = 0;
                    vitesse = 0;
                    L = 0;
                    R = 0;
                    break;
                case "Avancer normalement":
                    ActionVocaleIndexListe = 1;
                    vitesse = 20;
                    L = 100;
                    R = 100;
                    break;
                case "Avancer doucement":
                    ActionVocaleIndexListe = 2;
                    vitesse = 10;
                    L = 100;
                    R = 100;
                    break;
                case "Avancer à droite":
                    ActionVocaleIndexListe = 3;
                    vitesse = 30;
                    L = 100;
                    R = 75;
                    break;
                case "Avancer à gauche":
                    ActionVocaleIndexListe = 4;
                    vitesse = 30;
                    L = 75;
                    R = 100;
                    break;
                case "Rotation à gauche":
                    ActionVocaleIndexListe = 5;
                    vitesse = 20;
                    L = -50;
                    R = 50;
                    break;
                case "Rotation à droite":
                    ActionVocaleIndexListe = 6;
                    vitesse = 20;
                    L = 50;
                    R = -50;
                    break;
                case "Reculer normalement":
                    ActionVocaleIndexListe = 7;
                    vitesse = 20;
                    L = -100;
                    R = -100;
                    break;
                case "Reculer doucement":
                    ActionVocaleIndexListe = 8;
                    vitesse = 10;
                    L = -100;
                    R = -100;
                    break;
                case "Reculer à droite":
                    ActionVocaleIndexListe = 9;
                    vitesse = 30;
                    L = -100;
                    R = -75;
                    break;
                case "Reculer à gauche":
                    ActionVocaleIndexListe = 10;
                    vitesse = 30;
                    L = -75;
                    R = -100;
                    break;
                default:
                    break;
            }
        }

        private void MoteurReconnaissance_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //MessageBox.Show("Commande non reconnu !");
        }

        void SendVocalVals2Rover()
        {
            byte RTrame;
            byte LTrame;
            int normR;
            int normL;
            while (rover.ConnectionState != false)
            {
                while (!CommandLoopActivated && rover.ConnectionState == true)
                {
                    Console.WriteLine("Waiting Voice Control Loop...");
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

                //Console.WriteLine(L + " : " + normL + "  " + R + " : " + normR);

                rover.Commander(new byte[] { LTrame, RTrame });
                Thread.Sleep(200);

            };
            MoteurReconnaissance.Dispose();
            roverTask.Wait();
            roverTask.Dispose();
        }
    }
}
