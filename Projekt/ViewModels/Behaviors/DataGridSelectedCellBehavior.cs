using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Projekt.ViewModels.Behaviors
{
    public class DataGridSelectedCellBehavior : Behavior<DataGrid>
    {



        public static readonly DependencyProperty SelectedCellProperty =
            DependencyProperty.Register(nameof(SelectedCell), typeof(DataGridCellInfo), typeof(DataGridSelectedCellBehavior),
                new FrameworkPropertyMetadata(default(DataGridCellInfo), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DataGridCellInfo SelectedCell
        {
            get => (DataGridCellInfo)GetValue(SelectedCellProperty);
            set =>
                SetValue(SelectedCellProperty, value);
             
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedCellsChanged += OnSelectedCellsChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectedCellsChanged -= OnSelectedCellsChanged;
        }

        private void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (AssociatedObject.SelectedCells.Count > 0)
                SelectedCell = AssociatedObject.SelectedCells[0];
            else
                SelectedCell = default;
        }
    }
}
