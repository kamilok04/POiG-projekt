using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;

namespace Projekt.ViewModels
{
    public partial class TableViewModel : ObservableObject
    {
        private DataGridCellInfo _selectedCell;
        public DataGridCellInfo SelectedCell
        {
            get => _selectedCell;
            set
            {
                if (_selectedCell != value)
                {
                    _selectedCell = value;
                    OnPropertyChanged(nameof(SelectedCell));
                    OnPropertyChanged(nameof(SelectedCellValue));
                }
            }
        }

        public object SelectedCellValue
        {
            get
            {
                if (SelectedCell.Item == null || SelectedCell.Column == null)
                    return null;

                var binding = (SelectedCell.Column as DataGridBoundColumn)?.Binding as Binding;
                if (binding == null)
                    return null;

                var propertyName = binding.Path.Path;
                var property = SelectedCell.Item.GetType().GetProperty(propertyName);
                if (property == null)
                    return null;

                return property.GetValue(SelectedCell.Item);
            }
        }

    }
}
