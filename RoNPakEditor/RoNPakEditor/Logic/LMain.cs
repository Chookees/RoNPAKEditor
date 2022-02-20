using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using RoNPakEditor.Controller;
using RoNPakEditor.Helper;
using RoNPakEditor.Interfaces;

namespace RoNPakEditor.Logic
{
    class LMain : IMain, INotifyPropertyChanged
    {
        private IPAKManager _pakManager = new LPAKManager();

        private ObservableCollection<DataGridRowStruct> dataGridRowStructs =
            new ObservableCollection<DataGridRowStruct>();

        private DataGrid DataGrid = new DataGrid();

        private ListBox fileList = new ListBox();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
        {
            if (selectorExpression == null)
                throw new ArgumentNullException("selectorExpression");
            MemberExpression body = selectorExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("The body must be a member expression");
            OnPropertyChanged(body.Member.Name);
        }

        public void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Pak Files(*.pak) | *.pak";
            fileDialog.AddExtension = false;
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            if (fileDialog.ShowDialog().Value)
            {
                GlobalCache.PathToPAK = fileDialog.FileName;
                _pakManager.ExtractPAK();
            }
        }

        // Saving only works for the AILevelData.ini at the moment.
        // Not Working
        public void SaveToPAK()
        {
            if (fileList.SelectedItem.ToString() == "AILevelData.ini")
            {
                StreamWriter writer = new StreamWriter(GlobalCache.FileToPathDictionary[fileList.SelectedItem.ToString()], false);

                writer.WriteLine("[Global]");
                foreach (DataGridRowStruct dataGridRowStruct in dataGridRowStructs)
                {
                    writer.WriteLine(dataGridRowStruct.Name + " = " + dataGridRowStruct.Value + " ; " + dataGridRowStruct.Description);
                }

                writer.Flush();
                writer.Close();

                StreamWriter appendWriter = new StreamWriter(GlobalCache.FileToPathDictionary[fileList.SelectedItem.ToString()], true);


                foreach (string line in File.ReadAllLines("Resources/Origins/AILevelData.txt"))
                {
                    appendWriter.WriteLine(line);
                }

                appendWriter.Flush();
                appendWriter.Close();

                _pakManager.RepackPAK();
            }
            else
            {
                MessageBox.Show(fileList.SelectedItem.ToString() +
                                " is currently not supported for repacking. Only AILevelData.ini is supported.", "Not supported file", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void FillListBox(ListBox fileList)
        {
            bool hasFoundOne = false;
            bool isSuccessfully = false;
            foreach (string file in Directory.GetFiles(GlobalCache.PathToOutput, "*", SearchOption.AllDirectories))
            {
                if (GlobalCache.WriteAll)
                {
                    string[] fileNameSplit = file.Split("\\");
                    fileList.Items.Add(fileNameSplit[^1]);
                    GlobalCache.FileToPathDictionary.Add(fileNameSplit[^1], file);
                    hasFoundOne = true;
                }
                else if (file.EndsWith(".ini"))
                {
                    string[] fileNameSplit = file.Split("\\");
                    fileList.Items.Add(fileNameSplit[^1]);
                    GlobalCache.FileToPathDictionary.Add(fileNameSplit[^1], file);
                    hasFoundOne = true;
                }

                isSuccessfully = true;
            }

            if (!hasFoundOne && isSuccessfully)
            {
                MessageBox.Show("Successfully extracted to " + GlobalCache.PathToOutput +
                                ".\nNone of the extracted files is editable by this Editor yet.", "Extraction complete!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (!isSuccessfully)
            {
                MessageBox.Show(
                    "An error occurred while extracting the PAK File.\nPlease make sure the selected File is valid.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void FillDataGrid(DataGrid dataGrid, string selectedItem)
        {
            this.dataGridRowStructs.Clear();
            this.DataGrid = dataGrid;
            List<List<string>> nameList = new List<List<string>>();

            // Only supports .ini files at the moment since there are a lot more like .uasset files.
            if ("ini" == GlobalCache.FileToPathDictionary[selectedItem].Split('.')[1])
            {
                foreach (string lineInFile in File.ReadAllLines(GlobalCache.FileToPathDictionary[selectedItem]))
                {
                    if (!lineInFile.Contains("[") && lineInFile != "")
                    {
                        string[] splittedLines = lineInFile.Split(' ');

                        string name = "";
                        string value = "";
                        if (splittedLines[0].Contains("="))
                        {
                            string[] splittedLine = splittedLines[0].Split("=");
                            name = splittedLine[0];

                            if (splittedLine.Length > 2)
                            {
                                foreach (string s in splittedLine)
                                {
                                    value += s + " ";
                                }
                            }
                            else
                            {
                                value = splittedLine[1];
                            }
                        }
                        else
                        {
                            name = splittedLines[0];
                            value = splittedLines[2];
                        }


                        if (value.Contains(";"))
                        {
                            value = value.Split(';')[0];
                        }

                        string comment = "";

                        if (splittedLines.Length > 3)
                        {
                            for (int i = 4; i < splittedLines.Length; i++)
                            {
                                comment += splittedLines[i] + " ";
                            }
                        }

                        if (splittedLines[0] == "")
                        {
                            continue;
                        }

                        nameList.Add(new List<string> { name, value, splittedLines.Length > 3 ? comment : null });
                    }

                    // Break when level specific settings appear
                    if (lineInFile.Contains("[RoN_Hotel"))
                    {
                        break;
                    }
                }

                foreach (List<string> list in nameList)
                {
                    dataGridRowStructs.Add(new DataGridRowStruct { Name = list[0], Value = list[1], Description = list[2] });
                }

                dataGrid.ItemsSource = dataGridRowStructs;
            }
        }

        public void UpdateFileList(ListBox newFileList)
        {
            if (this.fileList != newFileList)
            {
                this.fileList = newFileList;
            }
        }

        public void UpdateObservableDataGridRowList(object sender)
        {
            switch (sender)
            {
                case System.Windows.Controls.DataGrid dataGrid:
                    OnPropertyChanged(() => dataGrid.CurrentItem);
                    switch (dataGrid.CurrentItem)
                    {
                        case DataGridRowStruct dataGridRowStruct:
                            DataGridRowStruct toBeEdited =
                                dataGridRowStructs.Single(x => x.Name == dataGridRowStruct.Name);

                            toBeEdited.Value = dataGridRowStruct.Value;
                            break;

                        default:
                            throw new ValueUnavailableException();
                    }
                    break;

                default:
                    throw new ValueUnavailableException();
            }
        }
    }
}
