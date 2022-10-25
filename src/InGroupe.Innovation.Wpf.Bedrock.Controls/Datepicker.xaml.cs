using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls
{
    /// <summary>
    /// Interaction logic for Datepicker.xaml
    /// </summary>
    public partial class DatePicker : UserControl 
    {
        private static readonly ResourceDictionary DefaultCalendarResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Controls;component/CalendarDefaultTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        private DateOnly? selectedDate;
        private Calendar? calendar;

        public IValueConverter? SelectedDateDisplayConverter { get; set; }     

        public event EventHandler<DateOnly?> OnSelectedDate;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty SelectedDateDisplayFormatProperty = DependencyProperty.Register(
            nameof(SelectedDateDisplayFormat),
            typeof(string),
            typeof(DatePicker));

        [AllowNull]
        [MaybeNull]
        public string? SelectedDateDisplayFormat
        {
            get => this.GetValue(SelectedDateDisplayFormatProperty) as string;
            set => this.SetValue(SelectedDateDisplayFormatProperty, value);
        }

        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
            nameof(SelectedDate),
            typeof(DateOnly?),
            typeof(DatePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        [AllowNull]
        [MaybeNull]
        public DateOnly? SelectedDate
        {
            get => this.GetValue(SelectedDateProperty) as DateOnly?;
            set
            {
                this.SetValue(SelectedDateProperty, value);

                if (value is null)
                {
                    this.DateTextBox.Text = string.Empty;
                }
            }
        } 

        public static readonly DependencyProperty HideWhenDateSelectedProperty = DependencyProperty.Register(
            nameof(HideWhenDateSelected),
            typeof(bool),
            typeof(DatePicker),
            new PropertyMetadata(true));

        public bool HideWhenDateSelected
        {
            get => (bool)this.GetValue(HideWhenDateSelectedProperty);
            set => this.SetValue(HideWhenDateSelectedProperty, value);
        }

        public static readonly DependencyProperty ShowCancelValidateButtonsProperty = DependencyProperty.Register(
            nameof(ShowCancelValidateButtons),
            typeof(bool),
            typeof(DatePicker),
            new PropertyMetadata(true));

        public bool ShowCancelValidateButtons
        {
            get => (bool)this.GetValue(ShowCancelValidateButtonsProperty);
            set => this.SetValue(ShowCancelValidateButtonsProperty, value);
        }

        public static readonly DependencyProperty CalendarPlacementProperty = DependencyProperty.Register(
            nameof(CalendarPlacement),
            typeof(PlacementMode),
            typeof(DatePicker),
            new FrameworkPropertyMetadata(PlacementMode.Center));

        public PlacementMode CalendarPlacement
        {
            get => (PlacementMode)this.GetValue(CalendarPlacementProperty);
            set => this.SetValue(CalendarPlacementProperty, value);
        }

        public static readonly DependencyProperty CalendarHeightProperty = DependencyProperty.Register(
            nameof(CalendarHeight),
            typeof(double),
            typeof(DatePicker),
            new FrameworkPropertyMetadata(491D));


        public double CalendarHeight
        {
            get => (double)this.GetValue(CalendarHeightProperty);
            set => this.SetValue(CalendarHeightProperty, value);
        }

        public static readonly DependencyProperty CalendarWidthProperty = DependencyProperty.Register(
            nameof(CalendarWidth),
            typeof(double),
            typeof(DatePicker),
            new FrameworkPropertyMetadata(397D));

        public double CalendarWidth
        {
            get => (double)this.GetValue(CalendarWidthProperty);
            set => this.SetValue(CalendarWidthProperty, value);
        }

        public static readonly DependencyProperty DisplayContainerStyleProperty = DependencyProperty.Register(
            nameof(DisplayContainerStyle),
            typeof(Style),
            typeof(DatePicker),
            new FrameworkPropertyMetadata((Style)DefaultCalendarResourceDictionary["DateTimePickerDisplayContainer"]));

        public Style DisplayContainerStyle
        {
            get => (Style)this.GetValue(DisplayContainerStyleProperty);
            set => this.SetValue(DisplayContainerStyleProperty, value);
        }

        public static readonly DependencyProperty DisplayLabelStyleProperty = DependencyProperty.Register(
            nameof(DisplayLabelStyle),
            typeof(Style),
            typeof(DatePicker),
            new FrameworkPropertyMetadata((Style)DefaultCalendarResourceDictionary["DateTimePickerDisplayLabel"]));

        public Style DisplayLabelStyle
        {
            get => (Style)this.GetValue(DisplayLabelStyleProperty);
            set => this.SetValue(DisplayLabelStyleProperty, value);
        }

        public DatePicker() =>this.InitializeComponent();

        public void ClearDisplay() => this.DateTextBox.Text = null;

        private void ShowCalendar()
        {
            if (this.calendar is null)
            {
                this.calendar = new Calendar
                {
                    ShowCancelValidateButtons = this.ShowCancelValidateButtons
                };

                if (this.selectedDate is not null)
                {
                    this.calendar.Date = this.selectedDate.Value;
                }
                else
                {
                    this.calendar.Date = null;
                }

                this.calendar.OnValidated += this.Calendar_OnValidated;
                this.calendar.OnCandelled += this.Calendar_OnCandelled;
                this.calendar.OnDatePicked += this.calendar_OnDatePicked;

                var selectedDateBinding = new Binding(nameof(Calendar.Date))
                {
                    Source = this.calendar,
                    Converter = this.SelectedDateDisplayConverter ?? new DefaultDateDisplayConverter(this.SelectedDateDisplayFormat ?? "dd/MM/yyyy")
                };

                this.DateTextBox.SetBinding(TextBox.TextProperty, selectedDateBinding);


                this.CalendarViewer.Child = this.calendar;
                this.CalendarViewer.PopupAnimation = PopupAnimation.Fade;
                this.CalendarViewer.StaysOpen = true;
                this.CalendarViewer.Placement = this.CalendarPlacement;
                this.CalendarViewer.Height = this.CalendarHeight;
                this.CalendarViewer.Width = this.CalendarWidth;
                this.CalendarViewer.IsOpen = true;
            }
        }

        private void CalendarViewer_Closed(object sender, EventArgs e) => this.HideCalendar();

        private void HideCalendar()
        {
            if (this.calendar is not null)
            {
                this.calendar!.OnValidated -= this.Calendar_OnValidated;
                this.calendar!.OnCandelled -= this.Calendar_OnCandelled;
                this.calendar.OnDatePicked -= this.calendar_OnDatePicked;

                BindingOperations.ClearBinding(this.DateTextBox, Label.ContentProperty);

                this.DisplayDate();

                this.CalendarViewer.IsOpen = false;

                this.CalendarViewer.Child = null;
                this.calendar = null;
            }
        }


        private void DisplayDate()
        {
            if (this.calendar?.Date is not null)
            {
                var date = this.SelectedDateDisplayConverter?.Convert(this.calendar.Date.Value, typeof(string), null, CultureInfo.CurrentUICulture) as string;

                if (date is null && this.SelectedDateDisplayFormat is {  Length: > 0 })
                {
                    date = this.calendar.Date.Value.ToString(this.SelectedDateDisplayFormat);
                }

                if (date is null && this.SelectedDateDisplayFormat is null or { Length: 0 })
                {
                    date = this.calendar.Date.Value.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern);
                }

                this.DateTextBox.Text = date ?? string.Empty;
            }
        }

        private void Calendar_OnCandelled(object? sender, EventArgs e) => this.HideCalendar();

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Event")]
        private void Calendar_OnValidated(object? sender, DateOnly? e)
        {
            this.UpdateSelectedDate(e);

            if (this.HideWhenDateSelected)
            {
                this.HideCalendar();
            }
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Event")]
        private void calendar_OnDatePicked(object? sender, DateOnly? e)
        {
            this.UpdateSelectedDate(e);

            if (this.HideWhenDateSelected)
            {
                this.HideCalendar();
            }
        }

        private void UpdateSelectedDate(DateOnly? date)
        {
            this.OnSelectedDate?.Invoke(this, date);

            this.SelectedDate = date;
            this.selectedDate = date;

            if (date is null)
            {
                this.DateTextBox.Text = string.Empty;
            }
        }

        private void DateTextBox_GotFocus(object sender, RoutedEventArgs e) => this.ShowCalendar();

        private void DateTextBox_LostFocus(object sender, RoutedEventArgs e) => this.HideCalendar();

        private void DateTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => this.HideCalendar();
    }

    internal sealed class DefaultDateDisplayConverter : IValueConverter
    {
        private readonly string format;

        public DefaultDateDisplayConverter(string? format)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                this.format = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
            }
            else
            {
                this.format = format!;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            DateOnly date => date.ToString(this.format, CultureInfo.CurrentUICulture),
            _ => value
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            if (DateOnly.TryParseExact(value as string, this.format, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
