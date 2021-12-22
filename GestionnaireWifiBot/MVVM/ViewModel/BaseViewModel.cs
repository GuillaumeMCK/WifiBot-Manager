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

namespace GestionnaireWifiBot.MVVM.ViewModel
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public ICommand CloseWindowCommand;

        public BaseViewModel()
        {
            CloseWindowCommand = new BaseCommand(o => CloseWindow(o));
        }

        public void CloseWindow(object o)
        {
            ((Window)o).Close(); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
