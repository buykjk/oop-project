using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace oop_project.Models
{
    public class BinaryTree
    {
        private BinaryTree Left { get; set; }
        private BinaryTree Right { get; set; }
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

        public override string ToString()
        {

            return String.Join(", ", PreOrder());
        }
    }
}
