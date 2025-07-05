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
    public  class TwoListBoxesViewModel : ObservableObject
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

        private object? GetSelectedItem(ObservableCollection<object> source)
            => source == LeftPaneItems ? LeftPaneSelectedItem : RightPaneSelectedItem;

        public object? SetSelectedItem(ObservableCollection<object> source, object? value)
        => source == LeftPaneItems ? LeftPaneSelectedItem = value : RightPaneSelectedItem = value;

        private ICommand? _moveCommand;
        public ICommand MoveCommand
        {
            get => _moveCommand ??= new RelayCommand(
                target => Move((dynamic?) target));
        }

        private ICommand? _moveAllCommand;
        public ICommand MoveAllCommand
        {
            get => _moveAllCommand ??= new RelayCommand(
                target => MoveAll((dynamic?)target));
        }

        public Func<Task> GetData;

        public TwoListBoxesViewModel()
        {

        }

        public TwoListBoxesViewModel(string LeftHeader, string RightHeader)
        {
            LeftPaneHeader = LeftHeader;
            RightPaneHeader = RightHeader;
        }

         void Move( ObservableCollection<object> target)
        {
             var source = target == LeftPaneItems ? RightPaneItems : LeftPaneItems;

             object? movedObject = GetSelectedItem(source);
            if (movedObject == null) return;
            source.Remove(movedObject);
            target.Add(movedObject);
            
            

        }

        void MoveAll(ObservableCollection<object> target)
        {
            var source = target == LeftPaneItems ? RightPaneItems : LeftPaneItems;

            foreach (var item in source.ToList()) 
            {
                target.Add(item);
            }

          
            source.Clear();
        }



    }
}
