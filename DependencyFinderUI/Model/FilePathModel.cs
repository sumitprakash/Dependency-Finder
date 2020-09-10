using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyFinderUI.Model
{
    class FilePathModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            try
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string inputFilePath;
        private string outputFilePath;

        public string InputFilePath { 
            get => inputFilePath; 
            set { inputFilePath = value; OnPropertyChanged("InputFilePath"); } 
        }
        public string OutputFilePath { 
            get => outputFilePath; 
            set { outputFilePath = value; OnPropertyChanged("OutputFilePath"); } 
        }

        public FilePathModel()
        {
            InputFilePath = OutputFilePath = string.Empty;
        }
    }
}
