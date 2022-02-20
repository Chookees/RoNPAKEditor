using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RoNPakEditor.Controller;
using RoNPakEditor.Interfaces;
using RoNPakEditor.Logic;

namespace RoNPakEditor.View
{
    /// <summary>
    /// View for V_Main.xaml
    /// </summary>
    public partial class VMain : Window
    {
        private IMain _mainInterface = new LMain();
        private bool hasShowedFileEditorWarning = false;

        public VMain()
        {
            InitializeComponent();
            InitialiseDataGrid();
            InitializeGlobalCache();

            // Because not working.
            this.FileSave.IsEnabled = false;
        }

        private void InitializeGlobalCache()
        {
            GlobalCache.FolderPathsDictionary.Add("Dev", GlobalCache.PathToDevFolder);
            GlobalCache.FolderPathsDictionary.Add("Config", GlobalCache.PathToDevFolder+"/Config");
            GlobalCache.FolderPathsDictionary.Add("Output", GlobalCache.PathToOutput);

            foreach (string value in GlobalCache.FolderPathsDictionary.Values)
            {
                Directory.CreateDirectory(value);
            }

            GlobalCache.FileToPathDictionary.Add("Config", GlobalCache.FolderPathsDictionary["Config"]+"/config.ini");

            foreach (string value in GlobalCache.FileToPathDictionary.Values)
            {
                using (var stream = File.Create(value))
                {
                    // Wait while file is in use.
                    stream.DisposeAsync();
                }
            }

            StreamWriter initializeConfig =
                new StreamWriter(GlobalCache.FolderPathsDictionary["Config"] + "/config.ini");

            initializeConfig.WriteLine("Version=19022022");
            initializeConfig.Flush();
            initializeConfig.Close();

            GlobalCache.Version = File.ReadAllLines(GlobalCache.FileToPathDictionary["Config"]).First();
        }

        private void FileOpenPAK_OnClick(object sender, RoutedEventArgs e)
        {
            _mainInterface.OpenFile(sender, e);
            _mainInterface.FillListBox(this.FileList);
        }

        private void RepackPAK_OnClick(object sender, RoutedEventArgs e)
        {
            _mainInterface.SaveToPAK();
        }

        private void InitialiseDataGrid()
        {
            this.DataGrid.AutoGenerateColumns = true;
            this.DataGrid.CanUserAddRows = false;
            this.DataGrid.CanUserDeleteRows = false;

            #region Columns

            DataGridTextColumn nameColumn = new DataGridTextColumn();
            DataGridTextColumn valueColumn = new DataGridTextColumn();
            DataGridTextColumn descriptionColumn = new DataGridTextColumn();

            this.DataGrid.Columns.Add(nameColumn);
            this.DataGrid.Columns.Add(valueColumn);
            this.DataGrid.Columns.Add(descriptionColumn);

            nameColumn.Header = "Name";
            nameColumn.Binding = new Binding("Name");
            valueColumn.Header = "Value";
            valueColumn.Binding = new Binding("Value");
            descriptionColumn.Header = "Description";
            descriptionColumn.Binding = new Binding("Description");

            nameColumn.IsReadOnly = true;
            valueColumn.IsReadOnly = false;
            descriptionColumn.IsReadOnly = true;

            #endregion

        }

        private void OptionsLightMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.OptionsLightMode.IsChecked)
            {
                MessageBox.Show("Not supported yet.", "Do you really want to get flashbanged?");
                this.OptionsLightMode.IsChecked = false;
            }
        }

        private void OptionsWriteAll_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.OptionsWriteAll.IsChecked)
            {
                GlobalCache.WriteAll = true;
                if (!hasShowedFileEditorWarning)
                {
                    if (MessageBox.Show(
                        "This editor is only capable of editing .ini files at the moment.\nWith this option every extracted File will be shown in this application.\nWhen trying to edit another file format it is possible that some errors occur, do you accept this?",
                        "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                    {
                        GlobalCache.WriteAll = false;
                        this.OptionsWriteAll.IsChecked = false;
                    }
                }
            }
            else
            {
                GlobalCache.WriteAll = false;
            }
        }

        private void HelpAbout_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "This tool was created to extract and edit .pak files of the game 'Ready or Not'. It's main purpose was to make the general settings easier to edit.\n\nCreated by "+GlobalCache.Author+"\n"+GlobalCache.Version, "About", MessageBoxButton.OK);
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            _mainInterface.UpdateFileList(listBox);
            _mainInterface.FillDataGrid(this.DataGrid,listBox.SelectedItem.ToString());
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _mainInterface.UpdateObservableDataGridRowList(sender);
        }
    }
}
