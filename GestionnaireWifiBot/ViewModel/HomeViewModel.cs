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
        public static Config currentRvConfig;
        public Config CurrentRvConfig {
            get { return currentRvConfig; }
            set {
                currentRvConfig = value;
                OnPropertyChanged(nameof(CurrentRvConfig));
            }
        }
        bool isBlur;
        public bool IsBlur
        {
            get { return isBlur; }
            set
            {
                isBlur = value;
                OnPropertyChanged(nameof(IsBlur));
            }
        }
        public static ObservableCollection<Config> listeRvConfig { get; set; }

        // VIEWS
        AddConfigView addConfigView;
        DelConfigView delConfigView;
        SelConfigView selConfigView;
        PiloterRoverView piloterRoverView;

        //----------------------------------------------
        // COMMANDS
        public ICommand OpenAddConfigViewCommand { get; set; }
        public ICommand OpenDelConfigViewCommand { get; set; }
        public ICommand OpenSelConfigViewCommand { get; set; }
        public ICommand OpenPiloteRoverViewCommand { get; set; }

        //----------------------------------------------
        // CONSTRUCTOR
        public HomeViewModel()
        {
            listeRvConfig = new ObservableCollection<Config>(GetBackupRoverList());

            CloseWindowCommand = new BaseCommand(o => {
                SaveRoverConfigs(new List<Config>(listeRvConfig));
                ((Window)o).Close();
            });
            OpenAddConfigViewCommand = new BaseCommand(o => OpenAddConfigView());
            OpenDelConfigViewCommand = new BaseCommand(o => OpenDelConfigView());
            OpenSelConfigViewCommand = new BaseCommand(o => OpenSelConfigView());
            OpenPiloteRoverViewCommand = new BaseCommand(o => { IsBlur = true; OpenPiloteRoverView(); IsBlur = false; });
        }

        //----------------------------------------------
        // METHODE
        /// GESTION DES ROVERS
        private void OpenPiloteRoverView()
        {
            if (currentRvConfig!=null)
            {
                piloterRoverView = new PiloterRoverView();
                try // Mhhh...
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

        private void OpenAddConfigView()
        {
            addConfigView = new AddConfigView(); 
            addConfigView.ShowDialog();
            if (ConfigRvViewModel.annuler ==  false)
                listeRvConfig = ConfigRvViewModel.listeRvConfig;
        }

        private void OpenSelConfigView()
        {
            if (listeRvConfig.Count > 0)
            {
                selConfigView = new SelConfigView();
                selConfigView.ShowDialog();
                if (ConfigRvViewModel.annuler == false && ConfigRvViewModel.rvConfigSelectionne != null)
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
                if (!ConfigRvViewModel.annuler)
                    listeRvConfig = ConfigRvViewModel.listeRvConfig;
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
        private void SaveRoverConfigs(List<Config> _configs)
        {
            BackupConfigs backupRoversConfigs = new BackupConfigs();

            backupRoversConfigs.ConfigsList = _configs;

            FileStream fichier = new FileStream("save.bin", FileMode.Create);
            
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fichier, backupRoversConfigs);
            fichier.Close();
        }
        private List<Config> GetBackupRoverList()
        {
            BackupConfigs backupConfigList = new BackupConfigs() ;
            try
            {
                FileStream fichier = new FileStream("save.bin", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                backupConfigList = (BackupConfigs)bf.Deserialize(fichier);
                fichier.Close();
            }
            catch
            {
                return new List<Config>();
            }
            return backupConfigList.ConfigsList;
        }
    }
}
