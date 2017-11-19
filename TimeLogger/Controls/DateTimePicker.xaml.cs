using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for DateTimePicker.xaml
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        private const int FormatLengthOfLast = 2;
        private enum Direction : int
        {
            Previous = -1,
            Next = 1
        }
        TextBoxUpDownAdorner _upDownButtons;

        public DateTimePicker()
        {
            InitializeComponent();
            CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
            DateDisplay.PreviewMouseUp += DateDisplay_PreviewMouseUp;
            DateDisplay.LostFocus += DateDisplay_LostFocus;
            DateDisplay.PreviewKeyDown += DateTimePicker_PreviewKeyDown;
            DateDisplay.TextChanged += new TextChangedEventHandler(DateDisplay_TextChanged);

            this.Loaded += (s, e) =>
            {
                AdornerLayer adLayer = GetAdornerLayer(DateDisplay);
                if (adLayer != null)
                {
                    adLayer.Add(_upDownButtons = new TextBoxUpDownAdorner(DateDisplay));
                    _upDownButtons.Click += (textBox, direction) => { OnUpDown(direction); };
                }
            };
        }

        static AdornerLayer GetAdornerLayer(FrameworkElement subject)
        {
            AdornerLayer layer = null;
            do
            {
                if ((layer = AdornerLayer.GetAdornerLayer(subject)) != null)
                    break;
            } while ((subject = subject.Parent as FrameworkElement) != null);
            return layer;
        }

        #region "Properties"

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public string DateFormat
        {
            get { return Convert.ToString(GetValue(DateFormatProperty)); }
            set { SetValue(DateFormatProperty, value); }
        }

        public bool ShowCalendarButton
        {
            get { return PopUpCalendarButton.Visibility == Visibility.Visible; }
            set { PopUpCalendarButton.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); }
        }

        public string _inputDateFormat;
        public string InputDateFormat()
        {
            if (_inputDateFormat == null)
            {
                string df = DateFormat;
                if (!df.Contains("MMM"))
                    df = df.Replace("MM", "M");
                if (!df.Contains("ddd"))
                    df = df.Replace("dd", "d");
                // Note: do not replace Replace("tt", "t") because a single "t" will not accept "AM" or "PM".
                _inputDateFormat = df.Replace("hh", "h").Replace("HH", "H").Replace("mm", "m").Replace("ss", "s");
            }
            return _inputDateFormat;
        }

        public DateTime MinimumDate
        {
            get { return Convert.ToDateTime(GetValue(MinimumDateProperty)); }
            set { SetValue(MinimumDateProperty, value); }
        }

        public DateTime MaximumDate
        {
            get { return Convert.ToDateTime(GetValue(MaximumDateProperty)); }
            set { SetValue(MaximumDateProperty, value); }
        }

        #endregion

        #region "Events"

        public event RoutedEventHandler DateChanged
        {
            add { AddHandler(DateChangedEvent, value); }
            remove { RemoveHandler(DateChangedEvent, value); }
        }

        public static readonly RoutedEvent DateChangedEvent = EventManager.RegisterRoutedEvent("DateChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DateTimePicker));

        public event RoutedEventHandler DateFormatChanged
        {
            add { this.AddHandler(DateFormatChangedEvent, value); }
            remove { this.RemoveHandler(DateFormatChangedEvent, value); }
        }

        public static readonly RoutedEvent DateFormatChangedEvent = EventManager.RegisterRoutedEvent("DateFormatChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DateTimePicker));

        #endregion

        #region "DependencyProperties"

        public static readonly DependencyProperty DateFormatProperty = DependencyProperty.Register("DateFormat", typeof(string), typeof(DateTimePicker), new FrameworkPropertyMetadata("yyyy-MM-dd HH:mm", OnDateFormatChanged));

        public static DependencyProperty MaximumDateProperty = DependencyProperty.Register("MaximumDate", typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(Convert.ToDateTime("3000-01-01 00:00"), null, new CoerceValueCallback(CoerceMaxDate)));

        public static DependencyProperty MinimumDateProperty = DependencyProperty.Register("MinimumDate", typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(Convert.ToDateTime("1900-01-01 00:00"), null, new CoerceValueCallback(CoerceMinDate)));

        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate",
            typeof(DateTime), typeof(DateTimePicker),
            new FrameworkPropertyMetadata(DateTime.Now,
                new PropertyChangedCallback(OnSelectedDateChanged),
                new CoerceValueCallback(CoerceDate)));

        /// <summary>true when user is busy editing DateDisplay and the SelectedDate
        /// becomes different from the date shown on the text box.</summary>
        public static readonly DependencyProperty DateTextIsWrongProperty = DependencyProperty.Register("DateTextIsWrong", typeof(bool), typeof(DateTimePicker), new FrameworkPropertyMetadata(false));

        protected bool DateTextIsWrong
        {
            get { return (bool)GetValue(DateTextIsWrongProperty); }
            set { SetValue(DateTextIsWrongProperty, value); }
        }

        #endregion

        #region "EventHandlers"

        bool _forceTextUpdateNow = true;

        private void CalDisplay_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PopUpCalendarButton.IsChecked = false;
            TimeSpan timeOfDay = TimeSpan.Zero;
            timeOfDay = SelectedDate.TimeOfDay;
            SelectedDate = CalDisplay.SelectedDate.Value.Date + timeOfDay;
        }

        private void DateDisplay_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DateDisplay.SelectionLength == 0)
                FocusOnDatePart(DateDisplay.SelectionStart);
        }

        bool IsDateInExpectedFormat()
        {
            return ParseDateText(false) != null;
        }
        DateTime? ParseDateText(bool flexible)
        {
            DateTime selectedDate;

            if (!DateTime.TryParseExact(DateDisplay.Text, InputDateFormat(), null, DateTimeStyles.AllowWhiteSpaces, out selectedDate))
                if (!flexible || !DateTime.TryParse(DateDisplay.Text, out selectedDate))
                    return null;
            return selectedDate;
        }

        void ReformatDateText()
        {
            // Changes DateDisplay.Text to match the current DateFormat
            DateTime? date = ParseDateText(true);
            if (date != null)
            {
                string newText = date.Value.ToString(DateFormat);
                if (DateDisplay.Text != newText)
                    DateDisplay.Text = newText;
            }
        }

        private void DateDisplay_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            DateDisplay.Text = SelectedDate.ToString(DateFormat);
            // When the user selects a field again, then the box loses focus, then
            // the user clicks the same field again, the selection is cleared, 
            // causing the arrows not to appear. To fix, clear selection in advance.
            try
            {
                DateDisplay.SelectionLength = 0;
            }
            catch (NullReferenceException)
            {
                // Occurs during shutdown. Bug in WPF? Ain't documented, that's for sure.
            }
        }
        void DateDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime? date = ParseDateText(true);
            if (date != null)
                SelectedDate = date.Value;
        }

        private void DateTimePicker_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            int selstart = DateDisplay.SelectionStart;

            if (!IsDateInExpectedFormat())
                return;

            switch (e.Key)
            {
                case Key.Up:
                    OnUpDown(+1);
                    break;
                case Key.Down:
                    OnUpDown(-1);
                    break;
                case Key.Left:
                    if (Keyboard.Modifiers != ModifierKeys.None)
                        return;
                    SelectPosition(selstart, Direction.Previous);
                    break;
                case Key.Right:
                    if (Keyboard.Modifiers != ModifierKeys.None)
                        return;
                    SelectPosition(selstart, Direction.Next);
                    break;
                case Key.Tab:
                    var dir = Direction.Next;
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                        dir = Direction.Previous;
                    e.Handled = SelectPosition(selstart, dir);
                    break;
                default:
                    char nextChar = '\0';
                    if (selstart < DateDisplay.Text.Length)
                        nextChar = DateDisplay.Text[selstart];

                    if ((e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.OemQuestion || e.Key == Key.Divide) &&
                        (nextChar == '/' || nextChar == '-') ||
                        e.Key == Key.Space && nextChar == ' ' ||
                        e.Key == Key.OemSemicolon && nextChar == ':')
                        SelectPosition(selstart, Direction.Next);
                    else
                        return;
                    break;
            }
            e.Handled = true;
        }

        private void OnUpDown(int increment)
        {
            int selstart = DateDisplay.SelectionStart;
            _forceTextUpdateNow = true;
            SelectedDate = Increase(selstart, increment);
            FocusOnDatePart(selstart);
        }

        private static object CoerceDate(DependencyObject d, object value)
        {
            DateTimePicker me = (DateTimePicker)d;
            DateTime current = Convert.ToDateTime(value);
            if (current < me.MinimumDate)
                current = me.MinimumDate;
            if (current > me.MaximumDate)
                current = me.MaximumDate;
            return current;
        }

        private static object CoerceMinDate(DependencyObject d, object value)
        {
            DateTimePicker me = (DateTimePicker)d;
            DateTime current = Convert.ToDateTime(value);
            if (current >= me.MaximumDate)
                throw new ArgumentException("MinimumDate can not be equal to, or more than maximum date");

            if (current > me.SelectedDate)
                me.SelectedDate = current;

            return current;
        }

        private static object CoerceMaxDate(DependencyObject d, object value)
        {
            DateTimePicker me = (DateTimePicker)d;
            DateTime current = Convert.ToDateTime(value);
            if (current <= me.MinimumDate)
                throw new ArgumentException("MaximimumDate can not be equal to, or less than MinimumDate");

            if (current < me.SelectedDate)
                me.SelectedDate = current;

            return current;
        }

        public static void OnDateFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var me = (DateTimePicker)obj;
            me._inputDateFormat = null; // will be recomputed on-demand
            me.DateDisplay.Text = me.SelectedDate.ToString(me.DateFormat);
        }

        public static void OnSelectedDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var me = (DateTimePicker)obj;

            var date = (DateTime)args.NewValue;
            me.CalDisplay.SelectedDate = date;
            me.CalDisplay.DisplayDate = date;
            if (me.DateDisplay.IsFocused && !me._forceTextUpdateNow)
            {
                DateTime? oldDate = me.ParseDateText(true);
                if (oldDate != null)
                    me.DateTextIsWrong = date != oldDate.Value;
            }
            else
            {
                me.DateTextIsWrong = false;
                me._forceTextUpdateNow = false;
                me.DateDisplay.Text = date.ToString(me.DateFormat);
            }
        }

        #endregion

        // Selects next or previous date value, depending on the incrementor value  
        // Alternatively moves focus to previous control or the calender button
        private bool SelectPosition(int selstart, Direction direction)
        {
            selstart = CalcPosition(selstart, direction);
            if (selstart > -1)
            {
                return FocusOnDatePart(selstart);
            }
            else
                return false;
        }

        static char At(string s, int index)
        {
            if ((uint)index < (uint)s.Length)
                return s[index];
            return '\0';
        }

        // Gets location of next/previous date field, depending on the incrementor value.
        // Returns -1 if there is no next/previous field.
        private int CalcPosition(int selStart, Direction direction)
        {
            string df = DateFormat;
            if (selStart >= df.Length)
                selStart = df.Length - 1;
            char startChar = df[selStart];
            int i = selStart;

            for (;;)
            {
                i += (int)direction;
                if ((uint)i >= (uint)df.Length)
                    return -1;
                if (df[i] == startChar)
                    continue;
                if (char.IsLetter(df[i]))
                    break;
                startChar = '\0'; // to handle cases like "yyyy-MM-dd (ddd)" correctly
            }

            if (direction < 0)
                // move to the beginning of the field
                while (i > 0 && df[i - 1] == df[i])
                    i--;

            return i;
        }

        private bool FocusOnDatePart(int selStart)
        {
            ReformatDateText();

            // Find beginning of field to select
            string df = DateFormat;
            if (selStart > df.Length - 1)
                selStart = df.Length - 1;
            char firstchar = df[selStart];
            while (!char.IsLetter(firstchar) && selStart + 1 < df.Length)
            {
                selStart++;
                firstchar = df[selStart];
            }
            while (selStart > 0 && df[selStart - 1] == firstchar)
                selStart--;

            int selLength = 1;
            while (selStart + selLength < df.Length && df[selStart + selLength] == firstchar)
                selLength++;

            // don't select AM/PM: we have no interface to change it.
            if (firstchar == 't')
                return false;

            DateDisplay.Focus();
            DateDisplay.Select(selStart, selLength);
            return true;
        }

        private DateTime Increase(int selstart, int value)
        {
            DateTime retval = (ParseDateText(false) ?? SelectedDate);

            try
            {
                switch (DateFormat.Substring(selstart, 1))
                {
                    case "h":
                    case "H":
                        retval = retval.AddHours(value);
                        break;
                    case "y":
                        retval = retval.AddYears(value);
                        break;
                    case "M":
                        retval = retval.AddMonths(value);
                        break;
                    case "m":
                        retval = retval.AddMinutes(value);
                        break;
                    case "d":
                        retval = retval.AddDays(value);
                        break;
                    case "s":
                        retval = retval.AddSeconds(value);
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                //Catch dates with year over 9999 etc, dont throw
            }

            return retval;
        }
    }


    // Adorners must subclass the abstract base class Adorner. 
    public class TextBoxUpDownAdorner : Adorner
    {
        StreamGeometry _triangle = new StreamGeometry();
        bool _shown;
        double _x, _top, _bottom;
        public Pen Outline = new Pen(new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)), 5);
        public Brush Fill = Brushes.Black;

        public TextBoxUpDownAdorner(TextBox adornedBox) : base(adornedBox)
        {
            _triangle = new StreamGeometry();
            _triangle.FillRule = FillRule.Nonzero;
            using (StreamGeometryContext c = _triangle.Open())
            {
                c.BeginFigure(new Point(-10, 0), true /* filled */, true /* closed */);
                c.LineTo(new Point(10, 0), true, false);
                c.LineTo(new Point(0, 15), true, false);
            }
            _triangle.Freeze();

            MouseDown += (s, e) => {
                if (Click != null)
                {
                    bool up = e.GetPosition(AdornedElement).Y < (_top + _bottom) / 2;
                    Click((TextBox)AdornedElement, up ? 1 : -1);
                }
            };

            adornedBox.LostFocus += RelevantEventOccurred;
            adornedBox.SelectionChanged += RelevantEventOccurred;
        }

        void RelevantEventOccurred(object sender, RoutedEventArgs e)
        {
            // In OnRender, GetRectFromCharacterIndex may return Infinity values,
            // so measure the location of the selection here instead.
            var box = AdornedElement as TextBox;
            if (box.IsFocused)
            {
                int start = box.SelectionStart, len = box.SelectionLength;
                if (_shown = len > 0)
                {
                    var rect1 = box.GetRectFromCharacterIndex(start);
                    var rect2 = box.GetRectFromCharacterIndex(start + len);
                    _top = rect1.Top - 2;
                    _bottom = rect1.Bottom + 2;
                    _x = (rect1.Left + rect2.Left) / 2;
                }
            }
            else
                _shown = false;

            InvalidateVisual();
        }

        public event Action<TextBox, int> Click;

        // A common way to implement an adorner's rendering behavior is to override the OnRender 
        // method, which is called by the layout system as part of a rendering pass. 
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_shown)
            {
                drawingContext.PushTransform(new TranslateTransform(_x, _top));
                drawingContext.PushTransform(new ScaleTransform(1, -1));
                drawingContext.DrawGeometry(Fill, Outline, _triangle);
                drawingContext.Pop();
                drawingContext.Pop();
                drawingContext.PushTransform(new TranslateTransform(_x, _bottom));
                drawingContext.DrawGeometry(Fill, Outline, _triangle);
                drawingContext.Pop();
            }
        }
    }

    public class BoolInverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        { return this; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { return !(bool)value; }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { return !(bool)value; }
    }
}