using Projekt.Miscellaneous;
using System.Data;
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

        private string? _previousCellValue;
        public string? PreviousCellValue
        {
            get => _previousCellValue;
            set => _previousCellValue = value;
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


        private ICommand? _tableUndoCommand;
        public ICommand TableUndoCommand
        {
            get => _tableUndoCommand ??= new RelayCommand(
                param => GetDataAsync().Wait());
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


                if (
                    oldRow != null &&
                    oldColumn != null &&
                   oldKey != null &&
                    oldRow[oldColumn].ToString() != oldValue &&
                    oldRow[RowKey].ToString() == oldKey

                    )
                    CreateTransactionCommand(oldColumn, oldRow[RowKey].ToString(), oldValue, oldRow[oldColumn].ToString());


            }
        }




        public abstract void CreateTransactionCommand(string? columnName, string? RowKey, string? oldValue, string? newValue);


        public abstract Task GetDataAsync();

        public abstract bool ConfirmExit();

        public abstract ICommand TableSaveCommand { get; }
        public abstract ICommand TableDeleteCommand { get; }
        public abstract ICommand TableCancelCommand { get; }
        public abstract ICommand TableCreateCommand { get; }

    }




}
