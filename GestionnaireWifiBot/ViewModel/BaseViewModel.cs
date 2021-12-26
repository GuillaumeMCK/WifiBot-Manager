using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using GestionnaireWifiBot.Commands;
using System.Windows.Input;
using System.Windows;

namespace GestionnaireWifiBot.ViewModel
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }
        public ICommand MoveWindowCommand { get; set; }

        public BaseViewModel()
        {
            CloseWindowCommand    = new BaseCommand(o => ((Window)o).Close());
            MinimizeWindowCommand = new BaseCommand(o => ((Window)o).WindowState = WindowState.Minimized);
            MoveWindowCommand = new BaseCommand(o => {

            }); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
