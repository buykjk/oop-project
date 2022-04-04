using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace oop_project.Views
{
    /// <summary>
    /// Interaction logic for BinTreeVisView.xaml
    /// </summary>
    public partial class BTGraphView : UserControl
    {

        public BTGraphView()
        {
            InitializeComponent();
        }

        private void NubmersOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
