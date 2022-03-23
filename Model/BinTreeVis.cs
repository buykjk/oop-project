using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace oop_project.Model
{
    public class BinTreeVis
    {
        public string GenerateRandomText()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
