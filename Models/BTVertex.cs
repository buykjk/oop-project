using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace oop_project.Models
{
    public class BTVertex : INotifyPropertyChanged
    {
        private bool _selected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public BTVertex(int value, (int x, int y) position)
        {
            Value = value;
            Position = position;
        }

        public int Value { get; }
        public (int x, int y) Position { get; }

        // For selection-handling in View
        // TODO does it break the MVVM pattern?
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }
    }
}
