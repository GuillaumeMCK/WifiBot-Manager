using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
