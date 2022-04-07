using oop_project.ViewModels.Commands;
using oop_project.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Win32;

namespace oop_project.ViewModels
{
    public class BTGraphViewModel : BaseViewModel
    {
        private BinaryTreeNew _tree { get; set; } = new BinaryTreeNew();

        public ObservableCollection<BTVertex> BTVertices { get; } = new ObservableCollection<BTVertex>();
        public ObservableCollection<BTEdge> BTEdges { get; } = new ObservableCollection<BTEdge>();

        public ICommand AddNodeCommand => new RelayCommand<string>(param => AddNode(param));

        public ICommand DeleteNodesCommand => new DelegateCommand(DeleteNodes);

        public ICommand ResetTreeCommand => new DelegateCommand(ResetTree);

        public ICommand ImportFromJsonCommand => new DelegateCommand(ImportFromJson);

        public ICommand ExportToTxtCommand => new DelegateCommand(ExportToTxt);

        public ICommand ExportToJsonCommand => new DelegateCommand(ExportToJson);

        // TODO all command logic should be in the model?

        private void DrawTree()
        {
            //clear current tree graph
            BTVertices.Clear();
            BTEdges.Clear();

            var nodesWithDepth = _tree.InOrderWithDepth();
            int x = 0;

            //load vertices from tree and print graph
            foreach (var node in nodesWithDepth)
            {
                int value = node.Key;
                int depth = node.Value;

                var vertex = _tree.Find(_tree.Root, value);

                //set position to vertices in tree
                vertex.Position = (x, depth);

                BTVertices.Add(vertex);

                x++;
            }

            foreach (var vertex in BTVertices)
            {
                if (vertex.Left == null && vertex.Right == null) continue;

                if (vertex.Left != null)
                {
                    BTEdges.Add(new BTEdge(vertex.Position, vertex.Left.Position));
                }
                if (vertex.Right != null)
                {
                    BTEdges.Add(new BTEdge(vertex.Position, vertex.Right.Position));
                }

            }
        }

        private void AddNode(string value)
        {   
            _tree.Add(Int32.Parse(value));
            
            DrawTree();
        }

        private void DeleteNodes()
        {
            List<int> selection = new List<int>();

            foreach (var vertex in BTVertices)
            {
                if (vertex.Selected)
                {
                    selection.Add(vertex.Value);
                }
            }
            if (selection.Count == BTVertices.Count)
            { 
                ResetTree();
            }
            else
            {
                _tree = _tree.DeleteAndCreate(selection);
                DrawTree();
            }
        }

        private void ResetTree()
        {
            //delete everything
            _tree = new BinaryTreeNew();
            BTVertices.Clear();
            BTEdges.Clear();
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

                if (_tree != null) _tree = null;

                _tree = JsonConvert.DeserializeObject<BinaryTreeNew>(json);

                DrawTree();
            }
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
                _tree.Print(dialog.FileName);
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
                string json = JsonConvert.SerializeObject(_tree);

                StreamWriter writer = new StreamWriter(dialog.FileName);
                writer.WriteLine(json);
                writer.Close();
            }
        }
    }
}
