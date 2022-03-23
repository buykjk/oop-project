using oop_project.Commands;
using oop_project.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace oop_project.ViewModel
{
    public class BinTreeVisViewModel : BaseViewModel
    {
        private readonly BinTreeVis _binTreeVis = new BinTreeVis();
        private string _someText;

        public string SomeText
        {
            get { return _someText; }
            set
            {
                _someText = value;
                RaisePropertyChangedEvent("SomeText");
            }
        }

        public ICommand GenerateRandomTextCommand
        {
            get { return new DelegateCommand(GenerateRandomText); }
        }

        private void GenerateRandomText()
        {
            SomeText = _binTreeVis.GenerateRandomText();
        }
    }
}
