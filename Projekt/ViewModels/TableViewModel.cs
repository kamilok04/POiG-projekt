using Projekt.Miscellaneous;
using System.Data;
using System.Data.OleDb;
using System.IO.Packaging;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public abstract partial class TableViewModel : ObservableObject
    {
        private DataGridCellInfo _selectedCell;
        private string? _rowKey;

        public string? RowKey
        {
            get { return _rowKey; }
            set { _rowKey = value; }
        }


      

        public DataGridCellInfo SelectedCell
        {
            get => _selectedCell;
            set
            {

                        HandleNewCellSelection(value);
                OnPropertyChanged(nameof(SelectedCell));

            }
        }
        private string? _selectedCellValue;

        public string? SelectedCellValue
        {
            //get => 
            get => _selectedCellValue;
            set
            {

                _selectedCellValue = value;
                OnPropertyChanged(nameof(SelectedCellValue));
            }
        }

        private string? _selectedColumnName;
        public string? SelectedColumnName
        {

            get => _selectedColumnName;
            set
            {
                _selectedColumnName = value;
                OnPropertyChanged(nameof(SelectedColumnName));
            }
        }


        private DataRowView? _selectedRow;
        public DataRowView? SelectedRow
        {
            //  get => 
            get => _selectedRow;
            set
            {
                _selectedRow = value;
                OnPropertyChanged(nameof(SelectedRow));
            }
        }

        private string? _selectedRowKey;

        public string? SelectedRowKey
        { //get =>
            get => _selectedRowKey;
            set
            {
                _selectedRowKey = value;
                OnPropertyChanged(nameof(SelectedRowKey));
            }
        }

     

        private ICommand? _tableCancelCommand;
        public ICommand TableCancelCommand
        {
            get => _tableCancelCommand ??= new RelayCommand(
                async param => await GetDataAsync());
        }

        private ICommand? _clearSelectedCellCommand;
        public ICommand? ClearSelectedCellCommand
        {
            get => _clearSelectedCellCommand ??= new RelayCommand(
                param =>  SelectedRow[SelectedColumnName] = DBNull.Value);
        }

        private void HandleNewCellSelection(DataGridCellInfo value)
        {


            if (_selectedCell != value)
            {
                string? oldKey = SelectedRowKey;
                string? oldColumn = SelectedColumnName;
                 DataRowView? oldRow = SelectedRow;
                string? oldValue = SelectedCellValue;

                _selectedCell = value;
                SelectedColumnName = SelectedCell.Column == null ? "" : (string?)SelectedCell.Column.Header;
                SelectedRow = SelectedCell.Item as DataRowView;
                SelectedRowKey = SelectedRow?[RowKey].ToString();
                SelectedCellValue = (SelectedColumnName == null || SelectedRow == null) ? null : SelectedRow[SelectedColumnName].ToString();

                if (oldRow == null || String.IsNullOrEmpty(oldKey) || String.IsNullOrEmpty(oldColumn))
                    return;



                    if (oldRow[oldColumn]?.ToString() != oldValue && CompareRows(oldRow, oldRow, oldColumn))


                    CreateTransactionCommand(oldColumn, oldRow[RowKey].ToString(), oldValue, oldRow[oldColumn].ToString());


            }
        }

        private bool CompareRows(DataRowView? a, DataRowView? b, string? columnToExclude)
        {
            if (a == null || b == null)
                return false;
            if (columnToExclude == null) return a == b;
            foreach (DataColumn column in a.DataView.Table.Columns)
            {
                string columnName = column.ColumnName;
                if (columnName == columnToExclude)
                    continue;
                if (!a[columnName].Equals(b[columnName])) 
                    return false;
            }
            return true;
        }




        public abstract void CreateTransactionCommand(string? columnName, string? RowKey, string? oldValue, string? newValue);


        public abstract Task GetDataAsync();

        public abstract bool ConfirmExit();

        public abstract ICommand TableSaveCommand { get; }

        //public abstract ICommand TableDeleteCommand { get; }
        // tymaczasowo: Usuwanie odbędzie się przez usunięcie komórki zawierającej PK
 
        public abstract ICommand TableCreateCommand { get; }

    }




}
