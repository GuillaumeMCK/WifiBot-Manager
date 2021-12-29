using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionnaireWifiBot.Model;
using GestionnaireWifiBot.View;
using Microsoft.Win32;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GestionnaireWifiBot.Commands;

namespace GestionnaireWifiBot.ViewModel
{
    class HomeViewModel : BaseViewModel
    {
        //----------------------------------------------
        // ATTRIBUTS
        bool isBlur;
        static ConfigsBackup configsBackup;
        public static Config currentRvConfig;

        // VIEWS
        AddConfigView addConfigView;
        DelConfigView delConfigView;
        SelConfigView selConfigView;
        PiloterRoverView piloterRoverView;

        //----------------------------------------------
        // PROPERTY
        public static List<Config> listeRvConfig
        {
            get
            {
                return configsBackup.ConfigsList;
            }
            set
            {
                configsBackup.ConfigsList = value;
            }
        }
        public Config CurrentRvConfig
        {
            get { return currentRvConfig; }
            set
            {
                currentRvConfig = value;
                OnPropertyChanged(nameof(CurrentRvConfig));
            }
        }
        public bool IsBlur
        {
            get { return isBlur; }
            set
            {
                isBlur = value;
                OnPropertyChanged(nameof(IsBlur));
            }
        }
        // COMMANDS
        public ICommand OpenAddConfigViewCommand { get; set; }
        public ICommand OpenDelConfigViewCommand { get; set; }
        public ICommand OpenSelConfigViewCommand { get; set; }
        public ICommand OpenPiloteRoverViewCommand { get; set; }

        //----------------------------------------------
        // CONSTRUCTOR
        public HomeViewModel()
        {
            GetBackup(out configsBackup);
            listeRvConfig = configsBackup.ConfigsList;

            CloseWindowCommand = new BaseCommand(o => {
                Backup(configsBackup);
                ((Window)o).Close();
            });
            OpenAddConfigViewCommand = new BaseCommand(o => OpenAddConfigView());
            OpenDelConfigViewCommand = new BaseCommand(o => OpenDelConfigView());
            OpenSelConfigViewCommand = new BaseCommand(o => OpenSelConfigView());
            OpenPiloteRoverViewCommand = new BaseCommand(o => { IsBlur = true; OpenPiloteRoverView(); IsBlur = false; });
        }

        //----------------------------------------------
        // METHODE
        private void OpenPiloteRoverView()
        {
            if (currentRvConfig!=null)
            {
                piloterRoverView = new PiloterRoverView();
                try // ...Permet d'eviter lorsque le fenetre est avortée dans le contructeur -> rover inaccessible
                {
                    piloterRoverView.ShowDialog();
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Aucun rover selectionné.",
                                "Opération impossible !",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }
        //new ObservableCollection<Config>
        private void OpenAddConfigView()
        {
            addConfigView = new AddConfigView(); 
            addConfigView.ShowDialog();
            if (ConfigRvViewModel.operation_canceled ==  false)
                listeRvConfig = ConfigRvViewModel.listeRvConfig.ToList<Config>();
        }

        private void OpenSelConfigView()
        {
            if (listeRvConfig.Count > 0)
            {
                selConfigView = new SelConfigView();
                selConfigView.ShowDialog();
                if (ConfigRvViewModel.operation_canceled == false && ConfigRvViewModel.rvConfigSelectionne != null)
                    CurrentRvConfig = new Config
                    {
                        NomDuRover = ConfigRvViewModel.rvConfigSelectionne.NomDuRover,
                        AdresseIP = ConfigRvViewModel.rvConfigSelectionne.AdresseIP,
                        PortTCP = ConfigRvViewModel.rvConfigSelectionne.PortTCP
                    };
            }
            else
            {
                MessageBox.Show("La liste de rover est vide.",
                                "Opération impossible !",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void OpenDelConfigView()
        {
            if (listeRvConfig.Count > 0)
            {
                delConfigView = new DelConfigView();
                delConfigView.ShowDialog();
                if (!ConfigRvViewModel.operation_canceled)
                    listeRvConfig = ConfigRvViewModel.listeRvConfig.ToList<Config>();
            }
            else
            {
                MessageBox.Show("La liste de rover est vide.",
                                "Opération impossible !",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        /// SERIALISATION
        private void Backup(ConfigsBackup configs)
        {
            FileStream fichier = new FileStream("save.bin", FileMode.Create);
            
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, configs);
            fichier.Close();
        }

        private void GetBackup(out ConfigsBackup configs)
        {
            configs = new ConfigsBackup();
            configs.ConfigsList = new List<Config>();
            try
            {
                FileStream fichier = new FileStream("save.bin", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                configs = (ConfigsBackup)bf.Deserialize(fichier);
                fichier.Close();
            }
            catch
            {
                Console.WriteLine("Aucun fichier de serialisation trouvé");
            }
        }
    }
}