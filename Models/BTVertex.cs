using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.IO;

namespace oop_project.Models
{
    public class BTVertex : INotifyPropertyChanged
    {
        private bool _selected = false;

        public BTVertex()
        {
            // Empty constructor for JSON deserialization
        }

        public BTVertex(int value)
        {
            Value = value;
            Left = null;
            Right = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BTVertex Left { get; set; }
        public BTVertex Right { get; set; }
        public int Value { get; set; }
        public (int x, int y) Position { get; set; }

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

        public void PrintPretty(string indent, bool last, string pathToFile)
        {
            //https://stackoverflow.com/questions/36311991/c-sharp-display-a-binary-search-tree-in-console
            //modified to write to the file

            StreamWriter writer = new StreamWriter(pathToFile, append: true);

            writer.Write(indent);
            if (last)
            {
                writer.Write("└─");
                indent += "  ";
            }
            else
            {
                writer.Write("├─");
                indent += "| ";
            }
            writer.WriteLine(this.Value);
            writer.Close();

            var children = new List<BTVertex>();
            if (this.Left != null)
                children.Add(this.Left);
            if (this.Right != null)
                children.Add(this.Right);

            for (int i = 0; i < children.Count; i++)
            {
                children[i].PrintPretty(indent, i == children.Count - 1, pathToFile);
            }

        }
    }
}
