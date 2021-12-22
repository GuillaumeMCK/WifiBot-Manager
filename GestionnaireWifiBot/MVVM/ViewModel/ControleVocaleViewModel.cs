using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using GestionnaireWifiBot.MVVM.Model;
using System.Windows;
using System.Threading;

namespace GestionnaireWifiBot.MVVM.ViewModel
{
    class ControleVocaleViewModel : PiloterRoverViewModel
    {
        public static bool CommandLoopActivated;
        int L = 0;       // Ratio de rotation des roues gauches
        int R = 0;       // Ratio de rotation des roues droite
        int vitesse = 0; // definit la vitesse du rover 0 - 40
        Task roverTask;

        SpeechRecognitionEngine MoteurReconnaissance;
        Choices MouvementChoisie;
        GrammarBuilder ContraintesReconnaissance;
        Grammar MotsAReconnaitre;

        string mot_reconnu;
        public string Mot_Reconnu
        {
            get { return mot_reconnu; }
            set
            {
                mot_reconnu = value;
                OnPropertyChanged(Mot_Reconnu);
            }
        }


        public ControleVocaleViewModel()
        {
            MoteurReconnaissance = new SpeechRecognitionEngine();   //Création d'un objet reconnaissance vocale
            try
            {
                MoteurReconnaissance.SetInputToDefaultAudioDevice();    //Capture l'entrée audio par défaut

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //On construit le dictionnaire des mots à reconnaitre, ceux qui ne figurent pas dans cette liste ne seront pas reconnus
            MouvementChoisie = new Choices(new string[] {
                "Avance Rapidement", "Avance Normalement", "Avance Lentement", "Recule Rapidement",
                "Recule Normalement", "Recule Lentement", "Tourne à droite", "Tourne à gauche", "Stop"
            });

            //On implante le dictionnaire dans le moteur de reconnaissance en utilisant un GrammarBuilder
            ContraintesReconnaissance = new GrammarBuilder(MouvementChoisie);
            MotsAReconnaitre = new Grammar(ContraintesReconnaissance);
            MoteurReconnaissance.LoadGrammarAsync(MotsAReconnaitre);

            //évennements liés à la reconnaissance vocale
            MoteurReconnaissance.SpeechRecognized += MoteurReconnaissance_SpeechRecognized; //Evennement déclanché lorsqu'un mot est reconnu
            MoteurReconnaissance.SpeechRecognitionRejected += MoteurReconnaissance_SpeechRecognitionRejected;   //Evennement déclanché lorsqu'un mot n'est pas reconnu

            roverTask = new Task(() => SendVocalVals2Rover());

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

        private void SendVocalVals2Rover()
        {

        }

        private void MoteurReconnaissance_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)  //exécute la commande en fonction de mots définits
            {
                //case "Avance Rapidement":
                //    listBoxVocal.SelectedIndex = 0;
                //    vitesse = 40;
                //    break;

                //case "Avance Normalement":
                //    listBoxVocal.SelectedIndex = 1;

                //    break;                         

                //case "Avance Lentement":
                //    listBoxVocal.SelectedIndex = 2;

                //    break;

                //case "Recule Rapidement":
                //    listBoxVocal.SelectedIndex = 3;

                //    break;

                //case "Recule Normalement":
                //    listBoxVocal.SelectedIndex = 4;

                //    break;

                //case "Recule Lentement":
                //    listBoxVocal.SelectedIndex = 5;

                //    break;

                //case "Tourne à gauche":
                //    listBoxVocal.SelectedIndex = 6;

                //    break;

                //case "Tourne à droite":
                //    listBoxVocal.SelectedIndex = 7;

                //    break;

                //case "Stop":
                //    listBoxVocal.SelectedIndex = 8;
                //    break;
            }

        }

        private void MoteurReconnaissance_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            MessageBox.Show("Mot non reconnu !");
        }

        void SendCommand2Rover()
        {
            byte RTrame;
            byte LTrame;
            int normR;
            int normL;
            while (rover.ConnectionState != false)
            {
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

                Console.WriteLine(L + " : " + normL + "  " + R + " : " + normR);

                rover.Commander(new byte[] { LTrame, RTrame });
                Thread.Sleep(200);

                while (!CommandLoopActivated && rover.ConnectionState == true)
                {
                    Console.WriteLine("Waiting Voice Control Loop...");
                    Thread.Sleep(1000);
                }

            };
            roverTask.Wait();
            roverTask.Dispose();
        }

    }
}
