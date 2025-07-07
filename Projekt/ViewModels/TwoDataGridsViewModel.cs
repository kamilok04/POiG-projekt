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
    public abstract class TwoDataGridsViewModel : ObservableObject
    {

        public string LeftPaneHeader { get; set; } = "Left Pane";
        public string RightPaneHeader { get; set; } = "Right Pane";

        private ObservableCollection<object> _leftPaneItems = new();
        private object? _leftPaneSelectedItem;

        private ObservableCollection<object> _rightPaneItems = new();
        private object? _rightPaneSelectedItem;


        public ObservableCollection<object> LeftPaneItems
        {
            get => _leftPaneItems;
            set
            {
                _leftPaneItems = value;
                OnPropertyChanged(nameof(LeftPaneItems));
            }
        }
        public ObservableCollection<object> RightPaneItems
        {
            get => _rightPaneItems;
            set
            {
                _rightPaneItems = value;
                OnPropertyChanged(nameof(RightPaneItems));
            }
        }
        public object? LeftPaneSelectedItem
        {
            get => _leftPaneSelectedItem;
            set
            {
                _leftPaneSelectedItem = value;
                OnPropertyChanged(nameof(LeftPaneSelectedItem));
            }
        }

        public object? RightPaneSelectedItem
        {
            get => _rightPaneSelectedItem;
            set
            {
                _rightPaneSelectedItem = value;
                OnPropertyChanged(nameof(RightPaneSelectedItem));
            }
        }

        protected object? GetSelectedItem(ObservableCollection<object> source)
            => source == LeftPaneItems ? LeftPaneSelectedItem : RightPaneSelectedItem;

        protected object? SetSelectedItem(ObservableCollection<object> source, object? value)
        => source == LeftPaneItems ? LeftPaneSelectedItem = value : RightPaneSelectedItem = value;

        private ICommand? _moveCommand;
        public ICommand MoveCommand
        {
            get => _moveCommand ??= new RelayCommand(
                target => Move((dynamic?)target),
                pred => CanMove());
        }

        private ICommand? _moveAllCommand;
        public ICommand MoveAllCommand
        {
            get => _moveAllCommand ??= new RelayCommand(
                target => MoveAll((dynamic?)target),
                pred => CanMove());
        }

        public Func<Task> GetData;

        public TwoDataGridsViewModel()
        {

        }

        public TwoDataGridsViewModel(string LeftHeader, string RightHeader)
        {
            LeftPaneHeader = LeftHeader;
            RightPaneHeader = RightHeader;
        }

        // void Move( ObservableCollection<object> target)
        //{
        //     var source = target == LeftPaneItems ? RightPaneItems : LeftPaneItems;

        //     object? movedObject = GetSelectedItem(source);
        //    if (movedObject == null) return;

        //    source.Remove(movedObject);
        //    if(target == RightPaneItems) 
        //        target.Add(movedObject);


        //}

        public abstract void Move(ObservableCollection<object> target);
        public abstract void MoveAll(ObservableCollection<object> target);
        public abstract bool CanMove();

        //void MoveAll(ObservableCollection<object> target)
        //{
        //    var source = target == LeftPaneItems ? RightPaneItems : LeftPaneItems;

        //    if (target == RightPaneItems)
        //    {
        //        foreach (var item in source.ToList())
        //        {
        //            target.Add(item);
        //        }

        //    }
        //    source.Clear();
        //}



    }
}
