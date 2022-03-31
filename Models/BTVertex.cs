using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace oop_project.Models
{
    public class BTVertex
    {
        public BTVertex(int value, (int x, int y) position)
        {
            Value = value;
            Position = position;
        }

        public int Value { get; }
        public (int x, int y) Position { get; }
    }
}
