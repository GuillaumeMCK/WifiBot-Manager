using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GestionnaireWifiBot.Commands;
using GestionnaireWifiBot.MVVM.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows;

namespace GestionnaireWifiBot.MVVM.ViewModel
{
    class ConfigRvViewModel : BaseViewModel
    {
        //---------------------------------
        // ATTRIBUTS

        public static bool annuler { get; set; } 
        public static ObservableCollection<Config> listeRvConfig;
        public ObservableCollection<Config> ListeRvConfig
        {
            get { return listeRvConfig; }
            set
            {
                listeRvConfig = value;
                OnPropertyChanged(nameof(ListeRvConfig));
            }
        }

        // Caracterise un rover avec des parametres valides
        public Config rvConfig { get; set; }
        public string RvConfig_Nom
        {
            get { return rvConfig.NomDuRover; }
            set { rvConfig.NomDuRover = value; }
        }
        public string RvConfig_IP
        {
            get { return rvConfig.AdresseIP; }
            set
            {
                IPAddress IP;

                if (IPAddress.TryParse(value, out IP))
                    rvConfig.AdresseIP = IP.MapToIPv4().ToString();
                else
                    Console.WriteLine("IP Invalide");

            }
        }
        public int RvConfig_Port
        {
            get { return rvConfig.PortTCP; }
            set
            {
                if (15015 <= value && value <= 15025)
                    rvConfig.PortTCP = value;
                else
                    Console.WriteLine("Port Invalide");
            }
        }

        // Rover Selectionne
        public static Config rvConfigSelectionne;
        public Config RvConfigSelectionne
        {
            get { return rvConfigSelectionne; }
            set
            {
                rvConfigSelectionne = value;
                OnPropertyChanged(nameof(RvConfigSelectionne));
            }
        }

        // Commandes Routée
        public ICommand AddConfigCommand { get; }
        public ICommand DeleteConfigCommand { get; }
        public ICommand SetSelectConfigCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectExitCommand { get; }

        public ConfigRvViewModel()
        {
            listeRvConfig = new ObservableCollection<Config>(HomeViewModel.listeRvConfig);
            rvConfig = new Config()
            {
                NomDuRover = "Rover",
                AdresseIP = "127.0.0.1",
                PortTCP = 15020
            };
            annuler = false;

            AddConfigCommand       = new BaseCommand(o => AddConfig());
            DeleteConfigCommand    = new BaseCommand(o => DeleteConfig());
            SetSelectConfigCommand = new BaseCommand(o => SetCurrentRvConfig());
            CancelCommand          = new BaseCommand(o => { annuler = true; ((Window)o).Close();});
            SelectExitCommand      = new BaseCommand(o => { annuler = true; });
        }

        private void DeleteConfig()
        {
            if (RvConfigSelectionne != null)
            {
                var Result = MessageBox.Show("Voulez-vous vraiment supprimer la configuration : " + RvConfigSelectionne.NomDuRover,
                                             "Confirmation de suppression",
                                              MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (Result == MessageBoxResult.Yes)
                    ListeRvConfig.Remove(RvConfigSelectionne);
            }
        }

        private void SetCurrentRvConfig()
        {
            if (RvConfigSelectionne != null)
            {
                rvConfig = RvConfigSelectionne;
                Console.WriteLine("-- debug --\n" + rvConfig.NomDuRover + "\n" + rvConfig.AdresseIP + "\n" + rvConfig.PortTCP);
            }
            else
            {
                MessageBox.Show("Aucun rover n'est selectionné.",
                                "Opération impossible !",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                annuler = true;
            }

        }
        

        private void AddConfig()
        {
            Console.WriteLine("-- debug --\n" + rvConfig.NomDuRover + "\n" + rvConfig.AdresseIP + "\n" + rvConfig.PortTCP);

            if (!ListeRvConfig.Any(item => item.NomDuRover == rvConfig.NomDuRover &&
                                           item.AdresseIP == rvConfig.AdresseIP &&
                                           item.PortTCP == rvConfig.PortTCP))
                ListeRvConfig.Add(rvConfig);
            else
                Console.WriteLine("Cette configuration existe deja");
        }
    }
}
