using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class IndexableProperty<T>
    {
        private readonly T[] _values;
        public readonly int Length;

        public static implicit operator T[](IndexableProperty<T> t) => t.Values;
        public static explicit operator IndexableProperty<T>(T[] t) => new(t);

        private IndexableProperty() {
            _values = Array.Empty<T>();
            Length = 0;
        }
        public IndexableProperty(int initialSize)
        {
            _values = new T[initialSize];
            Length = initialSize;
        }
        public IndexableProperty(T[] values)
        {
            _values = values;
            Length = values.Length;
        }

        public T this[int i]
        {
            get
            {
                return _values[i];
            }
            set { _values[i] = value; }
        }
        public T[] Values
        {
            get { return _values; }
        }
    }
}