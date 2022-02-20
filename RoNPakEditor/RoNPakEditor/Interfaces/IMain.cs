using System.Windows;
using System.Windows.Controls;
using RoNPakEditor.Helper;

namespace RoNPakEditor.Interfaces
{
    interface IMain
    {
        void OpenFile(object sender, RoutedEventArgs e);

        void SaveToPAK();

        void FillListBox(ListBox fileList);

        void FillDataGrid(DataGrid dataGrid, string selectedItem);

        void UpdateFileList(ListBox newFileList);

        void UpdateObservableDataGridRowList(object sender);
    }
}
