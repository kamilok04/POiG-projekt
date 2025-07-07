using Org.BouncyCastle.Asn1.X509;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    /// <summary>
    /// Abstrakcyjny ViewModel do obsługi dwóch powiązanych DataGridów z funkcjonalnością przenoszenia elementów między nimi.
    /// </summary>
    public abstract class TwoDataGridsViewModel : ObservableObject
    {
        /// <summary>
        /// Nagłówek lewego panelu.
        /// </summary>
        public string LeftPaneHeader { get; set; } = "Left Pane";

        /// <summary>
        /// Nagłówek prawego panelu.
        /// </summary>
        public string RightPaneHeader { get; set; } = "Right Pane";

        private ObservableCollection<object> _leftPaneItems = new();
        private object? _leftPaneSelectedItem;

        private ObservableCollection<object> _rightPaneItems = new();
        private object? _rightPaneSelectedItem;

        /// <summary>
        /// Elementy w lewym panelu.
        /// </summary>
        public ObservableCollection<object> LeftPaneItems
        {
            get => _leftPaneItems;
            set
            {
                _leftPaneItems = value;
                OnPropertyChanged(nameof(LeftPaneItems));
            }
        }

        /// <summary>
        /// Elementy w prawym panelu.
        /// </summary>
        public ObservableCollection<object> RightPaneItems
        {
            get => _rightPaneItems;
            set
            {
                _rightPaneItems = value;
                OnPropertyChanged(nameof(RightPaneItems));
            }
        }

        /// <summary>
        /// Zaznaczony element w lewym panelu.
        /// </summary>
        public object? LeftPaneSelectedItem
        {
            get => _leftPaneSelectedItem;
            set
            {
                _leftPaneSelectedItem = value;
                OnPropertyChanged(nameof(LeftPaneSelectedItem));
            }
        }

        /// <summary>
        /// Zaznaczony element w prawym panelu.
        /// </summary>
        public object? RightPaneSelectedItem
        {
            get => _rightPaneSelectedItem;
            set
            {
                _rightPaneSelectedItem = value;
                OnPropertyChanged(nameof(RightPaneSelectedItem));
            }
        }

        /// <summary>
        /// Zwraca zaznaczony element z podanej kolekcji.
        /// </summary>
        /// <param name="source">Kolekcja źródłowa.</param>
        /// <returns>Zaznaczony element.</returns>
        protected object? GetSelectedItem(ObservableCollection<object> source)
            => source == LeftPaneItems ? LeftPaneSelectedItem : RightPaneSelectedItem;

        /// <summary>
        /// Ustawia zaznaczony element w podanej kolekcji.
        /// </summary>
        /// <param name="source">Kolekcja źródłowa.</param>
        /// <param name="value">Element do zaznaczenia.</param>
        /// <returns>Nowo ustawiony zaznaczony element.</returns>
        protected object? SetSelectedItem(ObservableCollection<object> source, object? value)
            => source == LeftPaneItems ? LeftPaneSelectedItem = value : RightPaneSelectedItem = value;

        private ICommand? _moveCommand;
        /// <summary>
        /// Komenda do przenoszenia pojedynczego elementu.
        /// </summary>
        public ICommand MoveCommand
        {
            get => _moveCommand ??= new RelayCommand(
                target => Move((dynamic?)target),
                pred => CanMove());
        }

        private ICommand? _moveAllCommand;
        /// <summary>
        /// Komenda do przenoszenia wszystkich elementów.
        /// </summary>
        public ICommand MoveAllCommand
        {
            get => _moveAllCommand ??= new RelayCommand(
                target => MoveAll((dynamic?)target),
                pred => CanMove());
        }

        /// <summary>
        /// Funkcja do pobierania danych (do implementacji w pochodnych).
        /// </summary>
        public Func<Task> GetData;

        /// <summary>
        /// Konstruktor domyślny.
        /// </summary>
        public TwoDataGridsViewModel()
        {
        }

        /// <summary>
        /// Konstruktor z nagłówkami paneli.
        /// </summary>
        /// <param name="LeftHeader">Nagłówek lewego panelu.</param>
        /// <param name="RightHeader">Nagłówek prawego panelu.</param>
        public TwoDataGridsViewModel(string LeftHeader, string RightHeader)
        {
            LeftPaneHeader = LeftHeader;
            RightPaneHeader = RightHeader;
        }

        /// <summary>
        /// Przenosi zaznaczony element do wybranego panelu.
        /// </summary>
        /// <param name="target">Panel docelowy.</param>
        public abstract void Move(ObservableCollection<object> target);

        /// <summary>
        /// Przenosi wszystkie elementy do wybranego panelu.
        /// </summary>
        /// <param name="target">Panel docelowy.</param>
        public abstract void MoveAll(ObservableCollection<object> target);

        /// <summary>
        /// Sprawdza, czy przenoszenie elementów jest możliwe.
        /// </summary>
        /// <returns>True, jeśli przenoszenie jest możliwe.</returns>
        public abstract bool CanMove();
    }
}
