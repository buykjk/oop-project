using oop_project.ViewModels.Commands;
using oop_project.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using System.IO;

namespace oop_project.ViewModels
{
    public class BTGraphViewModel : BaseViewModel
    {
        //private string _someText = "TextPlaceholder";

        public ObservableCollection<BTVertex> BTVertices { get; } = new ObservableCollection<BTVertex>();
        public ObservableCollection<BTEdge> BTEdges { get; } = new ObservableCollection<BTEdge>();
        private string _nodeName;
        public string NodeName
        {
            get => _nodeName;
            set
            {
                if (!string.Equals(_nodeName, value))
                {
                    _nodeName = value;
                    //???
                    RaisePropertyChangedEvent("NodeName");
                }
            }
        }
        private BinaryTree Tree { get; set; } = null;

        //public string SomeText
        //{
        //    get { return _someText; }
        //    set
        //    {
        //        _someText = value;
        //        RaisePropertyChangedEvent("SomeText");
        //    }
        //}

        public ICommand AddNodeCommand
        {
            get { return new RelayCommand<string>(param => AddNode(param)); }
        }

        public ICommand DeleteNodesCommand
        {
            get { return new DelegateCommand(DeleteNodes); }
        }

        public ICommand ResetTreeCommand
        {
            get { return new DelegateCommand(ResetTree); }
        }

        //private void AddTestNodes()
        //{
        //    BTVertex v10 = new BTVertex(0, (1, 0));
        //    BTVertex v01 = new BTVertex(-28, (0, 1));
        //    BTVertex v21 = new BTVertex(135, (2, 1));

        //    BTVertices.Add(v10);
        //    BTVertices.Add(v01);
        //    BTVertices.Add(v21);

        //    BTEdges.Add(new BTEdge((v10.Position.x, v10.Position.y), (v01.Position.x, v01.Position.y)));
        //    BTEdges.Add(new BTEdge((v10.Position.x, v10.Position.y), (v21.Position.x, v21.Position.y)));
        //}

        private void AddNode(string value)
        {
            //TODO disable button if string is empty?
            if (value.Length == 0) return;
            
            //parse value from TextBox to Int
            //create new tree or add it to existing tree
            if (Tree == null)
            {
                Tree = new BinaryTree(Int32.Parse(value));
            }
            else
            {
                Tree.Add(Int32.Parse(value));
            }

            // TODO add multiple nodes to tree and draw them

            //BTVertex v0 = new BTVertex(Tree.Value, (1, 0));
            //BTVertex v1 = new BTVertex(Tree.Value, (0, 1));
            //BTVertex v2 = new BTVertex(Tree.Value, (2, 1));
            //BTVertices.Add(v0);
            //BTVertices.Add(v1);
            //BTVertices.Add(v2);
            BTVertices.Clear();
            AddVerticesToList();
            // testing placeholder
            //AddTestNodes();
        }

        private void DeleteNodes()
        {
            // TODO delete node from tree
        }

        private void ResetTree()
        {
            // TODO delete all tree nodes

            // testing placeholder
            Tree = null;
            NodeName = "";
            BTVertices.Clear();
            BTEdges.Clear();
        }

        private void SaveToFile(string pathToFile)
        {
            Tree.PrintPretty("", true, pathToFile);
        }

        private void ExportToJson(string pathToFile)
        {
            string json = JsonConvert.SerializeObject(Tree);

            StreamWriter writer = new StreamWriter(pathToFile);
            writer.WriteLine(json);
            writer.Close();
        }

        private void ImportFromJson(string pathToFile)
        {
            StreamReader reader = new StreamReader(pathToFile);
            string json = reader.ReadToEnd();
            reader.Close();

            if (Tree != null) Tree = null;

            Tree = JsonConvert.DeserializeObject<BinaryTree>(json);
        }

        private void AddVerticesToList()
        {
            //BTVertex v2 = new BTVertex(Tree.Value, (2, 1));
            //BTVertices.Add(v10);

            var nodesWithDepth = Tree.InOrderWithDepth();
            int x = 0;

            foreach (var node in nodesWithDepth)
            {
                int value = node.Key;
                int depth = node.Value;

                BTVertices.Add(new BTVertex(value, (x, depth)));
               
                x++;
            }
        }
    }
}
