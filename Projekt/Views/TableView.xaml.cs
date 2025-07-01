using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt.Views
{
    /// <summary>
    /// Interaction logic for UserTableView.xaml
    /// </summary>
    public partial class TableView : UserControl
    {
        public TableView()
        {
            InitializeComponent();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
            DataGrid dg = (DataGrid)sender;

            

            // Get the edited cell and its binding
            var editedElement = e.EditingElement as FrameworkElement;
            
            if (editedElement != null)
            {
                var binding = editedElement.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    // Update the binding source with the new value
                    binding.UpdateSource();
                }
            }

            // Optionally, set the DataGrid's SelectedCell to the edited cell
            if (dg.SelectedCells.Count > 0)
            {
                dg.SelectedCells.Clear();
            }
            dg.SelectedCells.Add(new DataGridCellInfo(e.Row.Item, e.Column));
        }

    
    }
}
