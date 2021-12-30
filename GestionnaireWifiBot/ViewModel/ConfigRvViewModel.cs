using GestionnaireWifiBot.Commands;
using GestionnaireWifiBot.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace GestionnaireWifiBot.ViewModel
{
    class ConfigRvViewModel : BaseViewModel
    {
        //---------------------------------
        // ATTRIBUTS
        public static ObservableCollection<Config> listeRvConfig;
        // Rover avec des parametres valides ( Valeurs par défaut )
        string rvConfig_Nom = "Rover";
        string rvConfig_IP  = "127.0.0.1";
        int rvConfig_Port   = 15020;
        // Rover Selectionne
        public static Config rvConfigSelectionne;

        //----------------------------------------------
        // PROPERTYS
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
        public string RvConfig_Nom
        {
            get { return rvConfig_Nom; }
            set { rvConfig_Nom = value; }
        }
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
        public Config RvConfigSelectionne
        {
            get { return rvConfigSelectionne; }
            set
            {
                rvConfigSelectionne = value;
                OnPropertyChanged(nameof(RvConfigSelectionne));
            }
        }
        public static bool operation_canceled { get; set; }

        // COMMANDS
        public ICommand AddConfigCommand { get; set; }
        public ICommand DeleteConfigCommand { get; set; }
        public ICommand SetSelectConfigCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ConfigRvViewModel()
        {
            listeRvConfig = new ObservableCollection<Config>(HomeViewModel.listeRvConfig);
            rvConfig = new Config(); 
            operation_canceled = false;

            AddConfigCommand       = new BaseCommand(o => AddConfig());
            DeleteConfigCommand    = new BaseCommand(o => DeleteConfig());
            SetSelectConfigCommand = new BaseCommand(o => SetCurrentRvConfig());
            CancelCommand          = new BaseCommand(o => { operation_canceled = true; ((Window)o).Close();});
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
                operation_canceled = false;
            }
            else
            {
                MessageBox.Show("Aucun rover n'est selectionné.",
                                "Opération impossible !",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                operation_canceled = true;
            }
        }

        private void AddConfig()
        {
            if (!ListeRvConfig.Any(item => item.NomDuRover == rvConfig_Nom &&
                                           item.AdresseIP == rvConfig_IP &&
                                           item.PortTCP == rvConfig_Port))
            {
                ListeRvConfig.Add(new Config() { NomDuRover = rvConfig_Nom, AdresseIP = rvConfig_IP, PortTCP=rvConfig_Port });
                operation_canceled = false;
            }
            else
            {
                MessageBox.Show("Cette configuration existe deja !",
                                "Opération impossible !",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }
    }
}