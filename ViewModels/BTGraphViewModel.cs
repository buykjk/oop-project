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

namespace oop_project.ViewModels
{
    public class BTGraphViewModel : BaseViewModel
    {
        //private string _someText = "TextPlaceholder";

        public ObservableCollection<BTVertex> BTVertices { get; } = new ObservableCollection<BTVertex>();
        public ObservableCollection<BTEdge> BTEdges { get; } = new ObservableCollection<BTEdge>();

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
            get { return new DelegateCommand(AddNode); }
        }

        public ICommand DeleteNodeCommand
        {
            get { return new DelegateCommand(DeleteNode); }
        }

        public ICommand ResetTreeCommand
        {
            get { return new DelegateCommand(ResetTree); }
        }

        private void AddTestNodes()
        {
            BTVertex v10 = new BTVertex(0, (1, 0));
            BTVertex v01 = new BTVertex(-28, (0, 1));
            BTVertex v21 = new BTVertex(135, (2, 1));

            BTVertices.Add(v10);
            BTVertices.Add(v01);
            BTVertices.Add(v21);

            BTEdges.Add(new BTEdge((v10.Position.x, v10.Position.y), (v01.Position.x, v01.Position.y)));
            BTEdges.Add(new BTEdge((v10.Position.x, v10.Position.y), (v21.Position.x, v21.Position.y)));
        }

        private void AddNode()
        {
            // TODO add node to tree

            // testing placeholder
            AddTestNodes();
        }

        private void DeleteNode()
        {
            // TODO delete node from tree
        }

        private void ResetTree()
        {
            // TODO delete all tree nodes

            // testing placeholder
            BTVertices.Clear();
            BTEdges.Clear();
        }
    }
}
