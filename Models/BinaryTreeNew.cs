using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;


namespace oop_project.Models
{
    public class BinaryTreeNew
    {
        public BTVertex Root { get; set; }

        public BinaryTreeNew()
        {
            Root = null;
        }

        public void Add(int value)
        {
            Root = Add(Root,value);
        }

        public BTVertex Add(BTVertex subtreeRoot, int value)
        {
            //if tree is empty set root vertex
            if (subtreeRoot == null)
            {
                subtreeRoot = new BTVertex(value);
                return subtreeRoot;
            }


            if (value < subtreeRoot.Value)
            {
                //add value to left subtree
                subtreeRoot.Left = Add(subtreeRoot.Left,value);
            }
            else if (value > subtreeRoot.Value)
            {
                //add value to right subtree
                subtreeRoot.Right = Add(subtreeRoot.Right,value);
            }

            return subtreeRoot;
        }

        public BinaryTreeNew DeleteAndCreate(List<int> selection)
        {
            var oldValues = PreOrder();
            var newValues = oldValues.Except(selection).ToList();
            var newTree = new BinaryTreeNew();
            foreach (var value in newValues)
            {
                newTree.Add(value);
            }

            return newTree;
        }

        
        public List<int> PreOrder()
        {
            List<int> values = new List<int>();

            values.Add(Root.Value);
            PreOrder(Root.Left, values);
            PreOrder(Root.Right, values);

            return values;
        }

        public void PreOrder(BTVertex subtree, List<int> values)
        {
            if (subtree == null) return;

            values.Add(subtree.Value);
            PreOrder(subtree.Left, values);
            PreOrder(subtree.Right, values);
        }

        public void Print(string pathToFile)
        {
            //delete file contents before writing
            StreamWriter sw = new StreamWriter(pathToFile, false);
            sw.WriteLine(String.Empty);
            sw.Close();

            Root.PrintPretty("", true, pathToFile);
        }

        public Dictionary<int, int> InOrderWithDepth()
        {
            int depth = 0;
            Dictionary<int, int> values = new Dictionary<int, int>();

            InOrderWithDepth(Root.Left, values, ref depth);
            depth--;

            values.Add(Root.Value, depth);

            InOrderWithDepth(Root.Right, values, ref depth);
            //unnecessary?
            depth--;

            return values;
        }

        public void InOrderWithDepth(BTVertex subtree, Dictionary<int, int> values, ref int depth)
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

        public BTVertex Find(BTVertex root, int value)
        {
            if (root == null || root.Value == value)
            {
                return root;
            }

            if (root.Value < value)
            {
                return Find(root.Right, value);
            }
            else
            {
                return Find(root.Left, value);
            }
        }
    }
}
