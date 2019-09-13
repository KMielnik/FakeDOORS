using System;
using System.Globalization;
using System.Windows.Data;

namespace FakeDOORS.DatabaseControls.ChapterSelectionControls
{
    public class TupleDisplayChapterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tuple = value as (string chapter, int id)?;

            if (tuple == null)
                return null;
            return tuple.Value.chapter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
