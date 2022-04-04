using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;


namespace oop_project.Models
{
    public class BinaryTree
    {
        public BinaryTree Left { get; set; }
        public BinaryTree Right { get; set; }
        public int Value { get; private set; }

        public BinaryTree(int value)
        {
            Value = value;
        }

        public void Add(int value)
        {
            switch (value.CompareTo(Value))
            {
                case var expression when value.CompareTo(Value) < 0:
                    if (Left == null)
                    {
                        Left = new BinaryTree(value);
                    }
                    else
                    {
                        Left.Add(value);
                    }
                    break;
                case var expression when value.CompareTo(Value) > 0:
                    if (Right == null)
                    {
                        Right = new BinaryTree(value);
                    }
                    else
                    {
                        Right.Add(value);
                    }
                    break;
                case 0:
                    //they are the same picture
                    break;
            }
        }

        public BinaryTree DeleteAndCreate(List<int> selection)
        {
            var values = PreOrder().Except(selection).ToList();
            var newTree = new BinaryTree(values[0]);
            foreach (var value in values)
            {
                if (values.IndexOf(value) == 0) continue;

                newTree.Add(value);
            }

            return newTree;
        }

        
        public List<int> PreOrder()
        {
            List<int> values = new List<int>();

            values.Add(Value);
            PreOrder(Left, values);
            PreOrder(Right, values);

            return values;
        }

        public void PreOrder(BinaryTree subtree, List<int> values)
        {
            if (subtree == null) return;

            values.Add(subtree.Value);
            PreOrder(subtree.Left, values);
            PreOrder(subtree.Right, values);
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
            writer.WriteLine(Value);
            writer.Close();

            var children = new List<BinaryTree>();
            if (this.Left != null)
                children.Add(this.Left);
            if (this.Right != null)
                children.Add(this.Right);

            for (int i = 0; i < children.Count; i++)
            {
                children[i].PrintPretty(indent, i == children.Count - 1, pathToFile);
            }

        }

        public override string ToString()
        {

            return String.Join(", ", PreOrder());
        }

        public Dictionary<int, int> InOrderWithDepth()
        {
            int depth = 0;
            Dictionary<int, int> values = new Dictionary<int, int>();

            InOrderWithDepth(Left, values, ref depth);
            depth--;

            values.Add(Value, depth);

            InOrderWithDepth(Right, values, ref depth);
            //unnecessary?
            depth--;

            return values;
        }

        public void InOrderWithDepth(BinaryTree subtree, Dictionary<int, int> values, ref int depth)
        {
            depth++;
            if (subtree == null)
            {
                return;
            }


            InOrderWithDepth(subtree.Left, values, ref depth);
            depth--;

            values.Add(subtree.Value, depth);
            InOrderWithDepth(subtree.Right, values, ref depth);

            depth--;
        }
    }
}
