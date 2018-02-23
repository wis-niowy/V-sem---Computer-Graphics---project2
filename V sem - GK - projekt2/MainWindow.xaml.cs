using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace V_sem___GK___projekt2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public System.Windows.Forms.Timer AppTimer;
        private static int bitmapWidth;
        private static int bitmapHeight;
        public double minRandomPolygonSpeed;
        public double MinRandomPolygonSpeed
        {
            get
            {
                return minRandomPolygonSpeed;
            }
            set
            {
                if (value < MaxRandomPolygonSpeed)
                {
                    minRandomPolygonSpeed = value > 0 ? value : 0.0;
                    NotifyPropertyChanged("MinRandomPolygonSpeed");
                }
            }
        }
        public double maxRandomPolygonSpeed;
        public double MaxRandomPolygonSpeed
        {
            get
            {
                return maxRandomPolygonSpeed;
            }
            set
            {
                if (value > MinRandomPolygonSpeed)
                {
                    maxRandomPolygonSpeed = value > 0 ? value : 0.0;
                    NotifyPropertyChanged("MaxRandomPolygonSpeed");
                }
            }
        }
        public double xLightCoord;
        public double XLightCoord
        {
            get
            {
                return xLightCoord;
            }
            set
            {
                xLightCoord = value;
                CurrentBitmap = new Bitmap(BottomBitmap);
                Graphics g = Graphics.FromImage(CurrentBitmap);
                g.DrawString("X", new Font("Arial", 8), System.Drawing.Brushes.Black, (float)xLightCoord, (float)yLightCoord);
                BottomImage.Source = loadBitmap(CurrentBitmap);
                NotifyPropertyChanged("XLightCoord");
            }
        }
        public double yLightCoord;
        public double YLightCoord
        {
            get
            {
                return yLightCoord;
            }
            set
            {
                yLightCoord = value;
                CurrentBitmap = new Bitmap(BottomBitmap);
                Graphics g = Graphics.FromImage(CurrentBitmap);
                g.DrawString("X", new Font("Arial", 8), System.Drawing.Brushes.Black, (float)xLightCoord, (float)yLightCoord);
                BottomImage.Source = loadBitmap(CurrentBitmap);
                NotifyPropertyChanged("YLightCoord");
            }
        }
        public double lightHeight;
        public double LightHeight
        {
            get
            {
                return lightHeight;
            }
            set
            {
                if (value > 0)
                {
                    lightHeight = value;
                    NotifyPropertyChanged("LightHeight");
                }
                
            }
        }
        public System.Drawing.Color LightColor { get; set; }
        public bool IsDrawingInProgress { get; set; }
        public Figure currentlyEditedFigure { get; set; }
        public Figure candidateToMark { get; set; }
        public List<Figure> MarkedFigures { get; set; }
        public List<Figure> DrawnFigureList { get; set; }
        public List<Figure> RandomFigureList { get; set; }
        public System.Drawing.Color[,] RandomFiguresFillTexture { get; set; }
        public System.Drawing.Color[,] RandomFiguresBumpMap { get; set; }
        public List<Figure> IntersectionFigureList { get; set; }
        private Bitmap bottomBitmap; // on this bitmap drawn figures are kept
        public Bitmap BottomBitmap
        {
            get
            {
                return bottomBitmap;
            }
            set
            {
                bottomBitmap = value;
                NotifyPropertyChanged("BottomBitmap");
            }
        }
        private Bitmap currentBitmap; // on this bitmap we draw curentlyEditedFigure and RandomFigureList
        public Bitmap CurrentBitmap
        {
            get
            {
                return currentBitmap;
            }
            set
            {
                currentBitmap = value;
                NotifyPropertyChanged("CurrentBitmap");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            AppTimer = new System.Windows.Forms.Timer();
            LightColor = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            AppTimer.Tick += TimerEventProcessor;
            AppTimer.Interval = 100;
            DrawnFigureList = new List<Figure>();
            RandomFigureList = new List<Figure>();
            IntersectionFigureList = new List<Figure>();
            MarkedFigures = new List<Figure>();
            this.KeyDown += MainWindow_KeyDown;
        }

        

        private void CanvasWithImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickCoords = new Point(e.GetPosition(BottomImage).X, e.GetPosition(BottomImage).Y);
            bool figureClicked = false;
            foreach (var fig in DrawnFigureList)
            {
                if (fig.WasClicked(clickCoords))
                {
                    figureClicked = true;
                    candidateToMark = fig;
                    break;
                }
            }
            if (!figureClicked)
            {
                if (IsDrawingInProgress)
                {
                    Polygon figure = currentlyEditedFigure as Polygon;
                    Point lastPointDrawn = figure.getSegmentByIdx(figure.getSegmentsNumber() - 1).pointB;
                    Point firstPoint = figure.getSegmentByIdx(0).pointA;
                    figure.addSegment(new Segment(lastPointDrawn, clickCoords));
                    UpdateCurrentBitmap();
                }
                else
                {
                    IsDrawingInProgress = true;
                    currentlyEditedFigure = new Polygon();
                    (currentlyEditedFigure as Polygon).addSegment(new Segment(clickCoords, clickCoords));
                    UpdateCurrentBitmap();
                    //bool test, test2;
                    //if (DrawnFigureList.Count > 0)
                    //{
                    //    test = Polygon.IsPointInsidePolygon(DrawnFigureList[0] as Polygon, clickCoords);
                    //    test2 = (DrawnFigureList[0] as Polygon).IsClockWise;
                    //    int a = 0;
                    //}
                }
            }
        }

        private void CanvasWithImage_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point clickCoords = new Point(e.GetPosition(BottomImage).X, e.GetPosition(BottomImage).Y);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsDrawingInProgress)
                {
                    Polygon figure = currentlyEditedFigure as Polygon;
                    figure.getSegmentByIdx(figure.getSegmentsNumber() - 1).SetPoint("B", clickCoords); // move current segment
                    UpdateCurrentBitmap();
                }
            }
            
        }

        private void CanvasWithImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point clickCoords = new Point(e.GetPosition(BottomImage).X, e.GetPosition(BottomImage).Y);
            bool figureClicked = false;
            foreach (var fig in DrawnFigureList)
            {
                if (fig.WasClicked(clickCoords))
                {
                    figureClicked = true;
                    if (candidateToMark == fig)
                    {
                        MarkedFigures.Add(fig);
                        fig.MarkUnmark();
                        RefreshAndDrawFigures();
                    }
                    candidateToMark = null;
                    break;
                }
            }
            if (!figureClicked)
            {
                if (IsDrawingInProgress)
                {
                    Polygon figure = currentlyEditedFigure as Polygon;
                    Segment currentSegment = figure.getSegmentByIdx(figure.getSegmentsNumber() - 1);
                    Point lastPointDrawn = currentSegment.pointB;
                    Point firstPoint = figure.getSegmentByIdx(0).pointA;
                    if (firstPoint.WasClicked(lastPointDrawn))
                    {
                        currentSegment.SetPoint("B", firstPoint);
                        //RefreshAndDrawFigures(currentlyEditedFigure);
                        DrawnFigureList.Add(currentlyEditedFigure);
                        foreach (var seg in figure.segmentList)
                        {
                            seg.pointA.Tag.adherentSegments.Add(seg);
                            seg.pointB.Tag.adherentSegments.Add(seg);
                        }
                        figure.UpdateFigureColor();
                        AddFiguresToBottomBitmap(currentlyEditedFigure);
                        currentlyEditedFigure = null;
                        IsDrawingInProgress = false;
                    }
                    else
                    {
                        currentSegment.SetPoint("B", clickCoords);
                        UpdateCurrentBitmap();
                    }
                }
            }
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.D:
                    foreach (var fig in MarkedFigures)
                    {
                        DrawnFigureList.Remove(fig);
                    }
                    MarkedFigures = new List<Figure>();
                    RefreshAndDrawFigures();
                    break;
                case Key.M:
                    Polygon newRandom = Quickhull.CreateConvexPolygon(0, bottomBitmap.Width, 0, bottomBitmap.Height);
                    newRandom.FillColor = System.Drawing.Color.FromArgb(100, 255, 0, 0);
                    newRandom.FillTexture = RandomFiguresFillTexture;
                    newRandom.BumpMap = RandomFiguresBumpMap;
                    newRandom = new RandomPolygon(newRandom);
                    (newRandom as RandomPolygon).Speed = ReturnRandomSpeed();
                    RandomFigureList.Add(newRandom);
                    DrawRandomFigures();
                    break;
                case Key.K:
                    Polygon[] temp = WeilerAtherton.PerformAlgorithm(DrawnFigureList[0] as Polygon, DrawnFigureList[1] as Polygon);
                    DrawnFigureList = new List<Figure>();
                    DrawnFigureList.AddRange(temp);
                    RefreshAndDrawFigures();
                    break;
                case Key.Y:
                    foreach(var fig in DrawnFigureList)
                    {
                        fig.Fill(CurrentBitmap, LightColor, new Vector3D(XLightCoord, YLightCoord, LightHeight));
                        fig.Draw(CurrentBitmap);
                        BottomImage.Source = loadBitmap(CurrentBitmap);
                    }
                    break;
            }
        }

        private void FillIntersectionFigures()
        {
            foreach (var figure in IntersectionFigureList)
            {
                figure.Fill(CurrentBitmap, LightColor, new Vector3D(XLightCoord, YLightCoord, LightHeight));
            }
            BottomImage.Source = loadBitmap(CurrentBitmap);
        }
        private void DrawRandomFigures()
        {
            CurrentBitmap = new Bitmap(BottomBitmap);

            foreach (var el in RandomFigureList)
            {
                el.Fill(CurrentBitmap, LightColor, new Vector3D(XLightCoord, YLightCoord, LightHeight));
                el.Draw(CurrentBitmap);
            }
            Graphics g = Graphics.FromImage(CurrentBitmap);
            g.DrawString("X", new Font("Arial", 8), System.Drawing.Brushes.Black, (float)XLightCoord, (float)YLightCoord);
            
            BottomImage.Source = loadBitmap(CurrentBitmap);
        }
        private void DrawAllFigures()
        {
            //Graphics g = Graphics.FromImage(BottomBitmap);
            //foreach (var fig in DrawnFigureList)
            //{
            //    Polygon polygon = fig as Polygon;
            //    for (int i = 0; i < polygon.segmentList.Count; ++i)
            //    {
            //        g.DrawString(i.ToString(), new Font("Arial", 8), System.Drawing.Brushes.Black, (float)polygon.segmentList[i].pointA.X, (float)polygon.segmentList[i].pointA.Y);
            //    }
            //}
            foreach (var el in DrawnFigureList)
            {
                el.Draw(BottomBitmap);
            }
        }
        public void RefreshAndDrawFigures(params Figure[] figures)
        {
            BottomBitmap = new Bitmap(bitmapWidth, bitmapHeight);
            DrawAllFigures();
            foreach (var fig in figures)
            {
                fig.Draw(BottomBitmap);
            }
            Graphics g = Graphics.FromImage(CurrentBitmap);
            g.DrawString("X", new Font("Arial", 8), System.Drawing.Brushes.Black, (float)XLightCoord, (float)YLightCoord);

            BottomImage.Source = loadBitmap(BottomBitmap);
            
        }
        public void UpdateCurrentBitmap()
        {
            DrawRandomFigures();
            currentlyEditedFigure.Draw(CurrentBitmap);
            BottomImage.Source = loadBitmap(CurrentBitmap);
        }
        public void AddFiguresToBottomBitmap(params Figure[] figures)
        {
            BottomBitmap = new Bitmap(BottomBitmap);
            foreach (var fig in figures)
            {
                fig.Draw(BottomBitmap);
            }
            CurrentBitmap = new Bitmap(BottomBitmap);
            Graphics g = Graphics.FromImage(CurrentBitmap);
            g.DrawString("X", new Font("Arial", 8), System.Drawing.Brushes.Black, (float)XLightCoord, (float)YLightCoord);
            BottomImage.Source = loadBitmap(CurrentBitmap);
        }
        public double ReturnRandomSpeed()
        {
            Random r = new Random();
            return r.Next((int)MinRandomPolygonSpeed, (int)MaxRandomPolygonSpeed + 1);
        }
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }

        private void CanvasWithImage_Loaded(object sender, RoutedEventArgs e)
        {
            bitmapWidth = (int)CanvasWithImage.ActualWidth;
            bitmapHeight = (int)CanvasWithImage.ActualHeight;
            this.BottomBitmap = new Bitmap(bitmapWidth, bitmapHeight);
            this.CurrentBitmap = new Bitmap(bitmapWidth, bitmapHeight);
            AppTimer.Start();
            MinRandomPolygonSpeed = 5;
            MaxRandomPolygonSpeed = 10;
        }

        private void CanvasWithImage_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CanvasWithImage_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void SetPolygonTextureButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter += "Image files (*.jpg, *.bmp, *.png) | *.jpg; *.bmp; *.png | All files (*.*) | *.*";
            while (true)
            {
                bool? dr = ofd.ShowDialog();
                if (dr.HasValue && dr.Value)
                {
                    string filename = ofd.FileName;
                    string ext = System.IO.Path.GetExtension(filename);
                    if (ext != ".jpg" && ext != ".bmp" && ext != ".png")
                    {
                        System.Windows.MessageBox.Show("Wrong file extension!\n You need to upload graphics.", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    foreach (var fig in MarkedFigures)
                    {
                        Polygon polygon = fig as Polygon;
                        polygon.FillTexture = WriteBitmapToArray(new Bitmap(filename));
                    }
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        private void ClearPolygonTextureButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var fig in MarkedFigures)
            {
                Polygon polygon = fig as Polygon;
                polygon.FillTexture = null;
            }
        }

        private void SetPolygonColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            foreach (var fig in MarkedFigures)
            {
                Polygon polygon = fig as Polygon;
                polygon.FillColor = cd.Color;
            }
        }

        private void SetPolygonBumpmapButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter += "Image files (*.jpg, *.bmp, *.png) | *.jpg; *.bmp; *.png | All files (*.*) | *.*";
            while (true)
            {
                bool? dr = ofd.ShowDialog();
                if (dr.HasValue && dr.Value)
                {
                    string filename = ofd.FileName;
                    string ext = System.IO.Path.GetExtension(filename);
                    if (ext != ".jpg" && ext != ".bmp" && ext != ".png")
                    {
                        System.Windows.MessageBox.Show("Wrong file extension!\n You need to upload graphics.", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    foreach (var fig in MarkedFigures)
                    {
                        Polygon polygon = fig as Polygon;
                        polygon.BumpMap = WriteBitmapToArray(new Bitmap(filename));
                    }
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        private void ClearPolygonBumpmapButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var fig in MarkedFigures)
            {
                Polygon polygon = fig as Polygon;
                polygon.BumpMap = null;
            }
        }

        private void SetRandomPolygonTextureButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter += "Image files (*.jpg, *.bmp, *.png) | *.jpg; *.bmp; *.png | All files (*.*) | *.*";
            while (true)
            {
                bool? dr = ofd.ShowDialog();
                if (dr.HasValue && dr.Value)
                {
                    string filename = ofd.FileName;
                    string ext = System.IO.Path.GetExtension(filename);
                    if (ext != ".jpg" && ext != ".bmp" && ext != ".png")
                    {
                        System.Windows.MessageBox.Show("Wrong file extension!\n You need to upload graphics.", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    RandomFiguresFillTexture = WriteBitmapToArray(new Bitmap(filename));
                    foreach (var figure in RandomFigureList)
                    {
                        Polygon polygon = figure as Polygon;
                        polygon.FillTexture = RandomFiguresFillTexture;
                    }
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        private void SetRandomPolygonBumpmapButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter += "Image files (*.jpg, *.bmp, *.png) | *.jpg; *.bmp; *.png | All files (*.*) | *.*";
            while (true)
            {
                bool? dr = ofd.ShowDialog();
                if (dr.HasValue && dr.Value)
                {
                    string filename = ofd.FileName;
                    string ext = System.IO.Path.GetExtension(filename);
                    if (ext != ".jpg" && ext != ".bmp" && ext != ".png")
                    {
                        System.Windows.MessageBox.Show("Wrong file extension!\n You need to upload graphics.", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }
                    RandomFiguresBumpMap = WriteBitmapToArray(new Bitmap(filename));
                    foreach (var figure in RandomFigureList)
                    {
                        Polygon polygon = figure as Polygon;
                        polygon.BumpMap = RandomFiguresBumpMap;
                    }
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        private void ClearRandomPolygonTextureButton_Click(object sender, RoutedEventArgs e)
        {
            RandomFiguresFillTexture = null;
            foreach (var fig in RandomFigureList)
            {
                Polygon polygon = fig as Polygon;
                polygon.FillTexture = null;
            }
        }

        private void ClearRandomPolygonBumpmapButton_Click(object sender, RoutedEventArgs e)
        {
            RandomFiguresBumpMap = null;
            foreach (var fig in RandomFigureList)
            {
                Polygon polygon = fig as Polygon;
                polygon.BumpMap = null;
            }
        }

        private void SetRandomPolygonColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            foreach (var fig in RandomFigureList)
            {
                Polygon polygon = fig as Polygon;
                polygon.FillColor = cd.Color;
            }
        }

        private void SetLightColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker cp = new ColorPicker();
            cp.ColorSelected += SetLightColor;
            cp.Show();
        }

        private void SetLightColor(object sender, EventArgs e)
        {
            LightColor = (System.Drawing.Color)sender;
        }

        private Polygon[] FindAllIntersectionPolygons()
        {
            List<Polygon> returnList = new List<Polygon>();
            foreach (var subject in DrawnFigureList)
            {
                foreach (var clipping in RandomFigureList)
                {
                    returnList.AddRange(WeilerAtherton.PerformAlgorithm(subject as Polygon, clipping as Polygon));
                }
            }
            return returnList.ToArray();
        }

        private System.Drawing.Color[,] WriteBitmapToArray(Bitmap bitmap)
        {
            System.Drawing.Color[,] returnArray = new System.Drawing.Color[bitmap.Width, bitmap.Height];
            for (int i = 0; i < bitmap.Width; ++i)
                for (int j = 0; j < bitmap.Height; ++j)
                {
                    returnArray[i, j] = bitmap.GetPixel(i, j);
                }
            return returnArray;
        }

        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric(e);
        }

        private void CheckIsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result) || e.Text == "."))
            {
                e.Handled = true;
            }
        }

        private void TimerEventProcessor(object myObject, EventArgs myEventArgs)
        {
            if (RandomFigureList.Count == 0)
                return;
            List<Figure> ListToRemove = new List<Figure>();
            foreach (var figure in RandomFigureList)
            {
                bool toRemove = true;
                Polygon polygon = figure as Polygon;
                double speed = (polygon as RandomPolygon).Speed;
                (polygon as RandomPolygon).WayGone += speed;
                figure.MoveFigure(new Point(-speed, 0));
                foreach (var segment in polygon.segmentList)
                {
                    if (segment.pointA.X >= 0)
                    {
                        toRemove = false;
                        break;
                    }
                }
                if (toRemove)
                {
                    ListToRemove.Add(figure);
                }
            }
            IntersectionFigureList.AddRange(FindAllIntersectionPolygons());
            foreach (var fig in ListToRemove)
            {
                RandomFigureList.Remove(fig);
            }
            DrawRandomFigures();
            FillIntersectionFigures();
            IntersectionFigureList = new List<Figure>();
        }
    }

    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return double.Parse((string)value);
        }
    }
}
