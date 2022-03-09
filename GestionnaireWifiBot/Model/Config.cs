using System;
using System.Collections.Generic;

namespace GestionnaireWifiBot.Model
{
    [Serializable]
    class Config
    {
        public string NomDuRover { get; set; }
        public string AdresseIP { get; set; }
        public int PortTCP { get; set; }
    }

    [Serializable]
    class ConfigsBackup
    {
        public List<Config> ConfigsList { get; set; }
    }
}
