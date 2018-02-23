using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;

namespace V_sem___GK___projekt2
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window, INotifyPropertyChanged
    {
        public event EventHandler ColorSelected;
        public event PropertyChangedEventHandler PropertyChanged;
        private int r;
        public int R
        {
            get
            {
                return r;
            }
            set
            {
                r = value;
                NotifyPropertyChanged("R");
            }
        }
        private int g;
        public int G
        {
            get
            {
                return g;
            }
            set
            {
                g = value;
                NotifyPropertyChanged("G");
            }
        }
        private int b;
        public int B
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
                NotifyPropertyChanged("B");
            }
        }


        public ColorPicker()
        {
            InitializeComponent();
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (ColorSelected != null)
                ColorSelected(System.Drawing.Color.FromArgb(R, G, B), null);
            this.Close();
        }

        private void Self_Closed(object sender, EventArgs e)
        {
           
        }
    }

    public class SliderToRgbConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return int.Parse((string)value);
        }
    }
    public class RgbToColorConverter : IMultiValueConverter
    {
        //#region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var r = System.Convert.ToByte(values[0]);
            var g = System.Convert.ToByte(values[1]);
            var b = System.Convert.ToByte(values[2]);

            return System.Windows.Media.Color.FromRgb(r, g, b);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        //#endregion
    }
}
