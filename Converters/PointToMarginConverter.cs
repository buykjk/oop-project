using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace oop_project.Converters
{
    public class PointToMarginConverter : IMultiValueConverter
    {
        /// <summary>
        /// TODO summary
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Point position = (Point)values[0];
            double actualWidth = (double)values[1];
            double actualHeight = (double)values[2];

            double dx = actualWidth / 2;
            double dy = actualHeight / 2;

            return new Thickness(position.X - dx, position.Y - dy, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
