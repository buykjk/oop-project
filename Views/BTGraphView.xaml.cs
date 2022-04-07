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
        private BTGraphViewModel _viewModel;

        // Text filtering
        private string _lastNodeValueText;
        private int _lastNodeValueCaretIndex;

        // Zoom
        private const double _scaleMax = 3;
        private const double _scaleMin = 0.6;
        private const double _zoomSpeed = 0.1;

        // Pan
        private Point _lastMousePos = new Point(0,0);

        // Transforms
        private ScaleTransform _scaleVertices, _scaleEdges;
        private TranslateTransform _translationVertices, _translationEdges;

        public BTGraphView()
        {
            InitializeComponent();

            DataContext = new BTGraphViewModel();
            _viewModel = DataContext as BTGraphViewModel;

            Loaded += onLoaded;
            _viewModel.BTVertices.CollectionChanged += BTVertices_CollectionChanged;
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            // Get graph transforms
            _scaleVertices = (ScaleTransform)((TransformGroup)graphVertices.RenderTransform).Children.First(tr => tr is ScaleTransform);
            _translationVertices = (TranslateTransform)((TransformGroup)graphVertices.RenderTransform).Children.First(tr => tr is TranslateTransform);

            _scaleEdges = (ScaleTransform)((TransformGroup)graphEdges.RenderTransform).Children.First(tr => tr is ScaleTransform);
            _translationEdges = (TranslateTransform)((TransformGroup)graphEdges.RenderTransform).Children.First(tr => tr is TranslateTransform);

            // Input focus
            NodeValue.Focus();
        }

        private void resetUI()
        {
            // NodeValue input
            NodeValue.Clear();

            // Vertices transforms
            _translationVertices.X = 0;
            _translationVertices.Y = 0;

            _scaleVertices.CenterX = 0;
            _scaleVertices.CenterY = 0;
            _scaleVertices.ScaleX = 1;
            _scaleVertices.ScaleY = 1;

            // Edges transforms
            _translationEdges.X = 0;
            _translationEdges.Y = 0;

            _scaleEdges.CenterX = 0;
            _scaleEdges.CenterY = 0;
            _scaleEdges.ScaleX = 1;
            _scaleEdges.ScaleY = 1;

            NodeValue.Focus();
        }

        private void updateDeleteButtonState()
        {
            Delete.IsEnabled = _viewModel.BTVertices.Any(v => v.Selected);
        }

        private void updateDeleteAllButtonState()
        {
            DeleteAll.IsEnabled = (_viewModel.BTVertices.Count != 0);
        }

        private void updateAddButtonState()
        {
            Add.IsEnabled = !(string.IsNullOrWhiteSpace(NodeValue.Text) || NodeValue.Text == "-");
        }

        private void clearSelected()
        {
            foreach (BTVertex vertex in _viewModel.BTVertices)
            {
                vertex.Selected = false;
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
            else if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                clearSelected();
            }

            updateDeleteButtonState();
        }

        private void BTVertices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            updateDeleteAllButtonState();
        }

        private void graphBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePos = e.GetPosition(graphVertices);
            graphBorder.CaptureMouse();

            handleSelection(e);
        }

        private void graphBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            graphBorder.ReleaseMouseCapture();
        }

        /// <summary>
        /// Update last caret index
        /// </summary>
        private void NodeValue_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _lastNodeValueCaretIndex = NodeValue.CaretIndex;
        }

        /// <summary>
        /// NodeValue validation
        /// </summary>
        private void NodeValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Restoration of old text triggers another event, ignore it
            if (NodeValue.Text == _lastNodeValueText)
            {
                return;
            }

            // Do not accept text with whitespaces
            if (NodeValue.Text.Any(c => char.IsWhiteSpace(c)))
            {
                NodeValue.Text = _lastNodeValueText;
                NodeValue.CaretIndex = _lastNodeValueCaretIndex;

                return;
            }

            // Only numbers, may start with '-'
            Regex regex = new Regex(@"^-?[0-9]*$");

            if (regex.IsMatch(NodeValue.Text))
            {
                // Accept new text

                _lastNodeValueText = NodeValue.Text;
                _lastNodeValueCaretIndex = NodeValue.CaretIndex;

                updateAddButtonState();
            }
            else
            {
                // Restore previous text

                NodeValue.Text = _lastNodeValueText;
                NodeValue.CaretIndex = _lastNodeValueCaretIndex;
            }
        }

        /// <summary>
        /// Add node
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (Add.IsEnabled)
            {
                _viewModel.AddNodeCommand.Execute(NodeValue.Text);

                NodeValue.Clear();
                NodeValue.Focus();
            }
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
                _viewModel.DeleteNodesCommand.Execute(null);

                updateDeleteButtonState();
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
                text: "The current tree will be deleted.\n\tAre you sure?",
                caption: "Delete All",
                buttons: MessageBoxButton.OKCancel,
                icon: MessageBoxImage.Warning);

            if (result == MessageBoxResult.OK)
            {
                // Reset data
                _viewModel.ResetTreeCommand.Execute(null);

                // Reset UI
                resetUI();

                updateDeleteButtonState();
            }
        }

        private void ResetView_Click(object sender, RoutedEventArgs e)
        {
            resetUI();
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
            Vector v = e.GetPosition(graphVertices) - _lastMousePos;

            // Apply the new translation
            _translationVertices.X += v.X * _scaleVertices.ScaleX;
            _translationVertices.Y += v.Y * _scaleVertices.ScaleY;

            _translationEdges.X += v.X * _scaleEdges.ScaleX;
            _translationEdges.Y += v.Y * _scaleEdges.ScaleY;

            // Update last mouse position
            _lastMousePos = e.GetPosition(graphVertices);
        }

        /// <summary>
        /// Zoom
        /// </summary>
        private void graphBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double newScale = _scaleVertices.ScaleX;

            // Determine zoom direction, return if not possible to zoom any further
            if (e.Delta > 0)
            {
                if (_scaleVertices.ScaleX == _scaleMax) return;

                newScale = _scaleVertices.ScaleX + _zoomSpeed;
            }
            else if (e.Delta < 0)
            {
                if (_scaleVertices.ScaleX == _scaleMin) return;

                newScale = _scaleVertices.ScaleX - _zoomSpeed;
            }

            // Clamp the zoom
            newScale = Math.Clamp(newScale, _scaleMin, _scaleMax);

            // Old scaling center
            double oldCenterX = _scaleVertices.CenterX;
            double oldCenterY = _scaleVertices.CenterY;

            // Current mouse position
            var position = e.GetPosition(graphVertices);

            // New scale center at mouse position
            _scaleVertices.CenterX = position.X;
            _scaleVertices.CenterY = position.Y;

            _scaleEdges.CenterX = position.X;
            _scaleEdges.CenterY = position.Y;

            // Compensate translations to keep the contents aligned with the mouse position
            _translationVertices.X += (_scaleVertices.CenterX - oldCenterX) * (_scaleVertices.ScaleX - 1);
            _translationVertices.Y += (_scaleVertices.CenterY - oldCenterY) * (_scaleVertices.ScaleY - 1);

            _translationEdges.X += (_scaleEdges.CenterX - oldCenterX) * (_scaleEdges.ScaleX - 1);
            _translationEdges.Y += (_scaleEdges.CenterY - oldCenterY) * (_scaleEdges.ScaleY - 1);

            // Apply the new scale
            _scaleVertices.ScaleX = newScale;
            _scaleVertices.ScaleY = newScale;

            _scaleEdges.ScaleX = newScale;
            _scaleEdges.ScaleY = newScale;
        }
    }
}
