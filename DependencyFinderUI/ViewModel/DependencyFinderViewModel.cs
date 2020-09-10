using DependencyFinderEngine;
using DependencyFinderUI.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static DependencyFinderEngine.CommonUtil;

namespace DependencyFinderUI.ViewModel
{
    class DependencyFinderViewModel
    {
        private FilePathModel _filePathObj;
        public FilePathModel FilePathObj { get => _filePathObj; set => _filePathObj = value; }

        private CommandWrapper _genReport;
        public CommandWrapper GenReport
        {
            get
            {
                if (_genReport == null)
                    _genReport = new CommandWrapper(GenReport_CanExecute, GenReport_Execute);
                return _genReport;
            }
            set
            {
                _genReport = value;
            }
        }

        private CommandWrapper _browseFile;
        public CommandWrapper BrowseFile
        {
            get
            {
                if (_browseFile == null)
                    _browseFile = new CommandWrapper(Browse_CanExecute, Browse_Execute);
                return _browseFile;
            }
            set
            {
                _browseFile = value;
            }
        }

        public DependencyFinderViewModel()
        {
            FilePathObj = new FilePathModel();
            FilePathObj.OutputFilePath = CommonUtil.OutputPath;
        }

        private bool Browse_CanExecute(object param)
        {
            return true;
        }

        private void Browse_Execute(object param)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    FilePathObj.InputFilePath = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error);
            }
        }

        private bool GenReport_CanExecute(object param)
        {
            return true;
        }

        private void GenReport_Execute(object param)
        {
            try
            {
                SolutionParser slnParser = new SolutionParser(param as string);
                slnParser.ParseSolution();
                if (CommonUtil.SlnToCSV(slnParser.Solution))
                    MessageBox.Show("Report Generation Successful", "Status", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Report Generation Failed", "Status", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Log(ex.Message, LogLevel.Error);
            }
        }
    }
}
