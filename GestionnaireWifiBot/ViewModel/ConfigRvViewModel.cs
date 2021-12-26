using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GestionnaireWifiBot.Commands;
using GestionnaireWifiBot.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows;

namespace GestionnaireWifiBot.ViewModel
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
        public Config rvConfig { get; set; }

        // Rover avec des parametres valides
        string rvConfig_Nom = "Rover";
        public string RvConfig_Nom
        {
            get { return rvConfig_Nom; }
            set { rvConfig_Nom = value; }
        }
        string rvConfig_IP = "127.0.0.1";
        public string RvConfig_IP
        {
            get { return rvConfig_IP; }
            set
            {
                IPAddress IP;
                if (IPAddress.TryParse(value, out IP))
                    rvConfig_IP = IP.MapToIPv4().ToString();
                else
                    Console.WriteLine("IP Invalide");
            }
        }
        int rvConfig_Port = 15020;
        public int RvConfig_Port
        {
            get { return rvConfig_Port; }
            set
            {
                if (15015 <= value && value <= 15025)
                    rvConfig_Port = value;
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

        public ICommand AddConfigCommand { get; set; }
        public ICommand DeleteConfigCommand { get; set; }
        public ICommand SetSelectConfigCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ConfigRvViewModel()
        {
            listeRvConfig = new ObservableCollection<Config>(HomeViewModel.listeRvConfig);
            rvConfig = new Config(); // { NomDuRover = "Rover", AdresseIP = "127.0.0.1", PortTCP = 15020 };
            annuler = false;

            AddConfigCommand       = new BaseCommand(o => AddConfig());
            DeleteConfigCommand    = new BaseCommand(o => DeleteConfig());
            SetSelectConfigCommand = new BaseCommand(o => SetCurrentRvConfig());
            CancelCommand          = new BaseCommand(o => { annuler = true; ((Window)o).Close();});
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
                annuler = false;
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
            if (!ListeRvConfig.Any(item => item.NomDuRover == rvConfig_Nom &&
                                           item.AdresseIP == rvConfig_IP &&
                                           item.PortTCP == rvConfig_Port))
            {
                ListeRvConfig.Add(new Config() { NomDuRover = rvConfig_Nom, AdresseIP = rvConfig_IP, PortTCP=rvConfig_Port });
                annuler = false;
            }
            else
            {
                MessageBox.Show("Cette configuration existe deja !",
                    "Opération impossible !",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                annuler = true;
            }
        }
    }
}