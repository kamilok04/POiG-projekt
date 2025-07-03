using Org.BouncyCastle.Asn1.X509;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public abstract class TwoListBoxesViewModel : ObservableObject
    {
        public ObservableCollection<object> LeftPaneItems = new();
        private object? _leftPaneSelectedItem;

        public ObservableCollection<object> RightPaneItems = new();
        private object? _rightPaneSelectedItem;

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

        private object? SetSelectedItem(ObservableCollection<object> source, object? value)
        => source == LeftPaneItems ? LeftPaneSelectedItem = value : RightPaneSelectedItem = value; 

        public abstract void GetData();

        public TwoListBoxesViewModel()
        {

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
        { foreach (object obj in target) Move(target); }
        
        

    }
}
