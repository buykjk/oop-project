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
using Microsoft.Win32;

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

        public ICommand ImportFromJsonCommand
        {
            get { return new DelegateCommand(ImportFromJson); }
        }

        public ICommand ExportToTxtCommand
        {
            get { return new DelegateCommand(ExportToTxt); }
        }

        public ICommand ExportToJsonCommand
        {
            get { return new DelegateCommand(ExportToJson); }
        }

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

            AddVerticesToList();
        }

        private void DeleteNodes()
        {
            // TODO delete node from tree
        }

        private void ResetTree()
        {
            //delete everything
            Tree = null;
            NodeName = "";
            BTVertices.Clear();
            BTEdges.Clear();
        }

        private void ExportToTxt()
        {
            //settings for file dialog
            string path = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = path;

            //if file is selected do stuff, else do nothing
            if (dialog.ShowDialog() == true)
            {
                Tree.PrintPretty("", true, dialog.FileName);
            }
        }

        private void ExportToJson()
        {
            //settings for file dialog
            string path = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            dialog.InitialDirectory = path;

            //if file is selected do stuff, else do nothing
            if (dialog.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(Tree);

                StreamWriter writer = new StreamWriter(dialog.FileName);
                writer.WriteLine(json);
                writer.Close();
            }
        }

        private void ImportFromJson()
        {
            //settings for file dialog
            string path = Environment.ExpandEnvironmentVariables("%USERPROFILE%");
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            dialog.InitialDirectory = path;

            //TODO what if can't import? -> wrong json format

            //if file is selected do stuff, else do nothing
            if (dialog.ShowDialog() == true)
            {
                StreamReader reader = new StreamReader(dialog.FileName);
                string json = reader.ReadToEnd();
                reader.Close();
            
                if (Tree != null) Tree = null;

                Tree = JsonConvert.DeserializeObject<BinaryTree>(json);

                AddVerticesToList();
            }
        }

        private void AddVerticesToList()
        {
            //clear current tree graph
            BTVertices.Clear();

            var nodesWithDepth = Tree.InOrderWithDepth();
            int x = 0;

            //load vertices from tree and print graph
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
