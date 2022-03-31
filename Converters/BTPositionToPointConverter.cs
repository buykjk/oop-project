using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace oop_project.Converters
{
    class BTPositionToPointConverter : IValueConverter
    {
        private const int cellWidth = 50;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            (int x, int y) position = ((int x, int y))value;

            double x = (position.x + 1) * cellWidth - cellWidth / 2;
            double y = (position.y + 1) * cellWidth - cellWidth / 2;

            return new Point(x, y);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
