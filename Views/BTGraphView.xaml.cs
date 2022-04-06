using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Linq;
using oop_project.Models;
using oop_project.ViewModels;
using oop_project.Helpers;

namespace oop_project.Views
{
    /// <summary>
    /// Interaction logic for BinTreeVisView.xaml
    /// </summary>
    public partial class BTGraphView : UserControl
    {
        private BTGraphViewModel viewModel;

        // Zoom
        private double scaleMax = 3;
        private double scaleMin = 0.6;
        private double zoomSpeed = 0.1;

        // Pan
        private Point last = new Point(0,0);

        // Transforms
        private ScaleTransform scaleVertices, scaleEdges;
        private TranslateTransform translationVertices, translationEdges;

        public BTGraphView()
        {
            InitializeComponent();

            DataContext = new BTGraphViewModel();
            viewModel = DataContext as BTGraphViewModel;

            viewModel.BTVertices.CollectionChanged += BTVertices_CollectionChanged;

            Loaded += onLoaded;
        }

        private void BTVertices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            setDeleteAllButtonState();
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            // Get graph transforms
            scaleVertices = (ScaleTransform)((TransformGroup)graphVertices.RenderTransform).Children.First(tr => tr is ScaleTransform);
            translationVertices = (TranslateTransform)((TransformGroup)graphVertices.RenderTransform).Children.First(tr => tr is TranslateTransform);

            scaleEdges = (ScaleTransform)((TransformGroup)graphEdges.RenderTransform).Children.First(tr => tr is ScaleTransform);
            translationEdges = (TranslateTransform)((TransformGroup)graphEdges.RenderTransform).Children.First(tr => tr is TranslateTransform);

            // Input focus
            NodeValue.Focus();
        }

        private void setDeleteButtonState()
        {
            Delete.IsEnabled = viewModel.BTVertices.Any(v => v.Selected);
        }

        private void setDeleteAllButtonState()
        {
            DeleteAll.IsEnabled = (viewModel.BTVertices.Count != 0);
        }

        private void NumbersOnly(object sender, TextCompositionEventArgs e)
        {
            if (NodeValue.Text == "")
            {
                Regex regex = new Regex(@"^-?\d*$");
                e.Handled = !regex.IsMatch(e.Text);
            }
            else
            {
                Regex regex = new Regex(@"^\d+$");
                e.Handled = !regex.IsMatch(e.Text);
            }
        }

        private void handleSelection(MouseButtonEventArgs e)
        {
            HitTestResult hit = VisualTreeHelper.HitTest(graphVertices, e.GetPosition(graphVertices));

            if (hit != null)
            {
                var hitbox = hit.VisualHit as Shape;
                var vertex = hitbox?.DataContext as BTVertex;

                if (vertex != null)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        // Single-select
                        clearSelected();

                        vertex.Selected = true;
                    }
                    else
                    {
                        // Multi-select
                        if (vertex.Selected)
                        {
                            vertex.Selected = false;
                        }
                        else
                        {
                            vertex.Selected = true;
                        }
                    }
                }
            }
            else if(!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                clearSelected();
            }

            setDeleteButtonState();
        }

        private void clearSelected()
        {
            foreach (BTVertex vertex in viewModel.BTVertices)
            {
                vertex.Selected = false;
            }
        }

        private void graphBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            last = e.GetPosition(graphVertices);
            graphBorder.CaptureMouse();

            handleSelection(e);
        }

        private void graphBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            graphBorder.ReleaseMouseCapture();
        }

        /// <summary>
        /// Add node
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddNodeCommand.Execute(NodeValue.Text);

            NodeValue.Clear();
            NodeValue.Focus();
        }

        /// <summary>
        /// Disable Add button if TextBox is empty
        /// </summary>
        private void NodeValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            Add.IsEnabled = !string.IsNullOrWhiteSpace(NodeValue.Text);
        }

        /// <summary>
        /// Delete nodes
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxEx.Show(
                owner: Application.Current.MainWindow,
                text: "Selected nodes will be deleted.\n\tProceed?",
                caption: "Delete Selected",
                buttons: MessageBoxButton.YesNo,
                icon: MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Delete selected
                viewModel.DeleteNodesCommand.Execute(null);

                setDeleteButtonState();
                NodeValue.Focus();
            }
        }

        /// <summary>
        /// Reset UI
        /// </summary>
        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxEx.Show(
                owner: Application.Current.MainWindow,
                text: "The current tree will be deleted.\nDo you wish to continue?",
                caption: "Delete All",
                buttons: MessageBoxButton.OKCancel,
                icon: MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                // Reset data
                viewModel.ResetTreeCommand.Execute(null);

                // Reset UI
                resetUI();

                setDeleteButtonState();
            }
        }
        private void resetUI()
        {
            // NodeValue input
            NodeValue.Clear();

            // Vertices transforms
            translationVertices.X = 0;
            translationVertices.Y = 0;

            scaleVertices.CenterX = 0;
            scaleVertices.CenterY = 0;
            scaleVertices.ScaleX = 1;
            scaleVertices.ScaleY = 1;

            // Edges transforms
            translationEdges.X = 0;
            translationEdges.Y = 0;

            scaleEdges.CenterX = 0;
            scaleEdges.CenterY = 0;
            scaleEdges.ScaleX = 1;
            scaleEdges.ScaleY = 1;

            NodeValue.Focus();
        }

        private void ResetView_Click(object sender, RoutedEventArgs e)
        {
            resetUI();
        }

        private void NodeValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            {
                if (e.Key == Key.Space)
                    e.Handled = true;
            }
        }

        /// <summary>
        /// Panning
        /// </summary>
        private void graphBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (!graphBorder.IsMouseCaptured)
                return;

            // TODO ??? position variable instead of e.GetPosition() does not work ???
            //var position = e.GetPosition(graphVertices);

            // Panning vector
            Vector v = e.GetPosition(graphVertices) - last;

            // Apply the new translation
            translationVertices.X += v.X * scaleVertices.ScaleX;
            translationVertices.Y += v.Y * scaleVertices.ScaleY;

            translationEdges.X += v.X * scaleEdges.ScaleX;
            translationEdges.Y += v.Y * scaleEdges.ScaleY;

            // Update last mouse position
            last = e.GetPosition(graphVertices);
        }

        /// <summary>
        /// Zoom
        /// </summary>
        private void graphBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double newScale = scaleVertices.ScaleX;

            // Determine zoom direction, return if not possible to zoom any further
            if (e.Delta > 0)
            {
                if (scaleVertices.ScaleX == scaleMax) return;

                newScale = scaleVertices.ScaleX + zoomSpeed;
            }
            else if (e.Delta < 0)
            {
                if (scaleVertices.ScaleX == scaleMin) return;

                newScale = scaleVertices.ScaleX - zoomSpeed;
            }

            // Clamp the zoom
            newScale = Math.Clamp(newScale, scaleMin, scaleMax);

            // Old scaling center
            double oldCenterX = scaleVertices.CenterX;
            double oldCenterY = scaleVertices.CenterY;

            // Current mouse position
            var position = e.GetPosition(graphVertices);

            // New scale center at mouse position
            scaleVertices.CenterX = position.X;
            scaleVertices.CenterY = position.Y;

            scaleEdges.CenterX = position.X;
            scaleEdges.CenterY = position.Y;

            // Compensate translations to keep the contents aligned with the mouse position
            translationVertices.X += (scaleVertices.CenterX - oldCenterX) * (scaleVertices.ScaleX - 1);
            translationVertices.Y += (scaleVertices.CenterY - oldCenterY) * (scaleVertices.ScaleY - 1);

            translationEdges.X += (scaleEdges.CenterX - oldCenterX) * (scaleEdges.ScaleX - 1);
            translationEdges.Y += (scaleEdges.CenterY - oldCenterY) * (scaleEdges.ScaleY - 1);

            // Apply the new scale
            scaleVertices.ScaleX = newScale;
            scaleVertices.ScaleY = newScale;

            scaleEdges.ScaleX = newScale;
            scaleEdges.ScaleY = newScale;
        }
    }
}
