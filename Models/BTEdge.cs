using System;
using System.Collections.Generic;
using System.Text;

namespace oop_project.Models
{
    public class BTEdge
    {
        public BTEdge((int, int) start, (int, int) end)
        {
            Start = start;
            End = end;
        }

        public (int x, int y) Start { get; }
        public (int x, int y) End { get; }
    }
}
