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
using System.Linq;
using oop_project.Models;

namespace oop_project.Views
{
    /// <summary>
    /// Interaction logic for BinTreeVisView.xaml
    /// </summary>
    public partial class BTGraphView : UserControl
    {
        // Zoom
        private double scaleMax = 2;
        private double scaleMin = 0.5;
        private double zoomSpeed = 0.1;

        // Pan
        private Point last = new Point(0,0);

        // Transforms
        private ScaleTransform scaleVertices, scaleEdges;
        private TranslateTransform translationVertices, translationEdges;

        public BTGraphView()
        {
            InitializeComponent();

            Loaded += onLoaded;
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            // Get graph transforms

            scaleVertices = (ScaleTransform)((TransformGroup)graphVertices.RenderTransform).Children.First(tr => tr is ScaleTransform);
            translationVertices = (TranslateTransform)((TransformGroup)graphVertices.RenderTransform).Children.First(tr => tr is TranslateTransform);

            scaleEdges = (ScaleTransform)((TransformGroup)graphEdges.RenderTransform).Children.First(tr => tr is ScaleTransform);
            translationEdges = (TranslateTransform)((TransformGroup)graphEdges.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        private void NumbersOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]-+");
            e.Handled = regex.IsMatch(e.Text);
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
        }

        private void clearSelected()
        {
            foreach (BTVertex vertex in graphVertices.Items)
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
