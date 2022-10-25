using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls
{
    /// <summary>
    /// Interaction logic for DateTimePicker.xaml
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        private Calendar? calendar;
        private HourPicker? hourPicker;
        private Button? closeButton;
        private Button? validateButton;

        private static readonly ResourceDictionary DefaultResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Themes;component/BasicBedrockTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        private static readonly ResourceDictionary DefaultCalendarResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Controls;component/CalendarDefaultTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        public IValueConverter? SelectedDateTimeDisplayConverter { get; set; }

        public event EventHandler<DateTime?> OnSelectedDateTime;
        public event EventHandler? OnCandelled;

        public static readonly DependencyProperty SelectedDateDisplayFormatProperty = DependencyProperty.Register(
            nameof(SelectedDateDisplayFormat),
            typeof(string),
            typeof(DateTimePicker));

        [AllowNull]
        [MaybeNull]
        public string? SelectedDateDisplayFormat
        {
            get => this.GetValue(SelectedDateDisplayFormatProperty) as string;
            set => this.SetValue(SelectedDateDisplayFormatProperty, value);
        }


        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
            nameof(DateAndTime),
            typeof(DateTime?),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        [AllowNull]
        [MaybeNull]
        public DateTime? DateAndTime
        {
            get => this.GetValue(SelectedDateProperty) as DateTime?;
            set => this.SetValue(SelectedDateProperty, value);
        }

        public static readonly DependencyProperty CalendarPlacementProperty = DependencyProperty.Register(
            nameof(CalendarPlacement),
            typeof(PlacementMode),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(PlacementMode.Center));

        public PlacementMode CalendarPlacement
        {
            get => (PlacementMode)this.GetValue(CalendarPlacementProperty);
            set => this.SetValue(CalendarPlacementProperty, value);
        }


        public static readonly DependencyProperty CalendarHeightProperty = DependencyProperty.Register(
            nameof(CalendarHeight),
            typeof(double),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(491D));

        public double CalendarHeight
        {
            get => (double)this.GetValue(CalendarHeightProperty);
            set => this.SetValue(CalendarHeightProperty, value);
        }

        public static readonly DependencyProperty CalendarWidthProperty = DependencyProperty.Register(
            nameof(CalendarWidthProperty),
            typeof(double),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(397D));

        public double CalendarWidth
        {
            get => (double)this.GetValue(CalendarWidthProperty);
            set => this.SetValue(CalendarWidthProperty, value);
        }

        public static readonly DependencyProperty HourWidthProperty = DependencyProperty.Register(
            nameof(HourWidth),
            typeof(double),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(300D));

        public double HourWidth
        {
            get => (double)this.GetValue(HourWidthProperty);
            set => this.SetValue(HourWidthProperty, value);
        }

        public static readonly DependencyProperty CancelButtonSyleProperty = DependencyProperty.Register(
            nameof(CancelButtonSyle),
            typeof(Style),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(DefaultResourceDictionary["ButtonClose"]));

        [AllowNull]
        [MaybeNull]
        public Style? CancelButtonSyle
        {
            get => this.GetValue(CancelButtonSyleProperty) as Style;
            set => this.SetValue(CancelButtonSyleProperty, value);
        }

        public static readonly DependencyProperty ValidateButtonSyleProperty = DependencyProperty.Register(
            nameof(ValidateButtonSyle),
            typeof(Style),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(DefaultResourceDictionary["ButtonValidate"]));


        [AllowNull]
        [MaybeNull]
        public Style? ValidateButtonSyle
        {
            get => this.GetValue(ValidateButtonSyleProperty) as Style;
            set => this.SetValue(ValidateButtonSyleProperty, value);
        }

        public static readonly DependencyProperty ButtonsZoneHeightroperty = DependencyProperty.Register(
            nameof(ButtonsZoneHeight),
            typeof(double),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata(60D));

        public double ButtonsZoneHeight
        {
            get => (double)this.GetValue(ButtonsZoneHeightroperty);
            set => this.SetValue(ButtonsZoneHeightroperty, value);
        }

        public static readonly DependencyProperty DisplayContainerStyleProperty = DependencyProperty.Register(
            nameof(DisplayContainerStyle),
            typeof(Style),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata((Style)DefaultCalendarResourceDictionary["DateTimePickerDisplayContainer"]));

        public Style DisplayContainerStyle
        {
            get => (Style)this.GetValue(DisplayContainerStyleProperty);
            set => this.SetValue(DisplayContainerStyleProperty, value);
        }

        public static readonly DependencyProperty DisplayLabelStyleProperty = DependencyProperty.Register(
            nameof(DisplayLabelStyle),
            typeof(Style),
            typeof(DateTimePicker),
            new FrameworkPropertyMetadata((Style)DefaultCalendarResourceDictionary["DateTimePickerDisplayLabel"]));

        public Style DisplayLabelStyle
        {
            get => (Style)this.GetValue(DisplayLabelStyleProperty);
            set => this.SetValue(DisplayLabelStyleProperty, value);
        }


        public DateTimePicker() => this.InitializeComponent();

        private void BedrockDateTimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DateAndTime is not null)
            {
                this.DisplayDateTime(this.DateAndTime);
            }
        }

        private void CalendarHourViewer_Closed(object sender, EventArgs e) => this.HideCalendar();

        

        private void AddActionButtons(Grid grid)
        {
            var container = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };

            this.closeButton = new Button
            {
                Style = this.CancelButtonSyle
            };

            this.closeButton.Click += this.CloseButton_Click;
           

            this.validateButton = new Button
            {
                Style = this.ValidateButtonSyle
            };

            this.validateButton.Click += this.ValidateButton_Click;

            Grid.SetColumn(this.closeButton, 0);
            Grid.SetColumn(this.validateButton, 1);

            _ = container.Children.Add(this.closeButton);
            _ = container.Children.Add(this.validateButton);

            Grid.SetColumnSpan(container, 2);
            Grid.SetRow(container, 1);

            _ = grid.Children.Add(container);
        }

        private DateTime? GetSeletedDateTime()
        {
            DateTime? date = null;

            if (this.calendar is { Date: not null })
            {
                if (this.hourPicker is { SelectedHour: not null })
                {
                    date = new DateTime(
                        this.calendar.Date.Value.Year, 
                        this.calendar.Date.Value.Month, 
                        this.calendar.Date.Value.Day, 
                        this.hourPicker.SelectedHour!.Value.Hour,
                        this.hourPicker.SelectedHour!.Value.Minute, 0);
                        
                       // this.calendar.Date.Value.AddHours(this.hourPicker.SelectedHour!.Value.Hour).AddMinutes(this.hourPicker.SelectedHour!.Value.Minute);
                }
                else
                {
                    date = new DateTime(this.calendar.Date.Value.Year, this.calendar.Date.Value.Month, this.calendar.Date.Value.Day);
                }
            }

            return date;
        }

        private void DisplayDateTime()
            => this.DisplayDateTime(this.GetSeletedDateTime());

        private void DisplayDateTime(DateTime? dateTime)
        {
            if (dateTime is not null)
            {
                var date = this.SelectedDateTimeDisplayConverter?.Convert(dateTime, typeof(string), null, CultureInfo.CurrentUICulture) as string;

                if (date is null && this.SelectedDateDisplayFormat is { Length: > 0 })
                {
                    date = dateTime.Value.ToString(this.SelectedDateDisplayFormat);
                }

                if (date is null && this.SelectedDateDisplayFormat is null or { Length: 0 })
                {
                    date = dateTime.Value.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern);
                }

                this.DateTimeTextBox.Text = date;
            }
            else
            {
                this.DateTimeTextBox.Text = string.Empty;
            }
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDateTime = this.GetSeletedDateTime();

            this.OnSelectedDateTime?.Invoke(this, selectedDateTime);

            //if (selectedDateTime is not null)
            //{
            //    this.DateAndTime = selectedDateTime;
            //}

            this.HideCalendar();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnCandelled?.Invoke(this, EventArgs.Empty);
            this.HideCalendar();
        }

        private void ShowCalendar()
        {
            if (this.calendar is null && this.hourPicker is null)
            {
                this.calendar = new Calendar
                {
                    ShowCancelValidateButtons = false
                };

                this.hourPicker = new HourPicker
                {
                    ShowCancelValidateButtons = false,
                    ShowBackwardForwardButtons = true
                };

                if (this.DateAndTime is not null)
                {
                    this.calendar.Date = DateOnly.FromDateTime(this.DateAndTime.Value);
                    this.hourPicker.SelectedHour = TimeOnly.FromDateTime(this.DateAndTime.Value);
                }

                var container = new Grid
                {
                    ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.CalendarWidth) },
                    new ColumnDefinition { Width = new GridLength(this.HourWidth) }
                },
                    RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(this.CalendarHeight) },
                    new RowDefinition { Height = new GridLength(this.ButtonsZoneHeight) }
                },
                    Background = this.calendar.Background
                };

                Grid.SetColumn(this.calendar!, 0);
                Grid.SetColumn(this.hourPicker!, 1);

                Grid.SetRow(this.calendar!, 0);
                Grid.SetRow(this.hourPicker!, 0);

                _ = container.Children.Add(this.calendar!);
                _ = container.Children.Add(this.hourPicker!);

                this.AddActionButtons(container);


                var selectedDateBinding = new Binding(nameof(Calendar.Date))
                {
                    Source = this.calendar,
                    Converter = this.SelectedDateTimeDisplayConverter
                };

                _ = this.DateTimeTextBox.SetBinding(TextBox.TextProperty, selectedDateBinding);

                this.CalendarHourViewer.Child = container;
                this.CalendarHourViewer.PopupAnimation = PopupAnimation.Fade;
                this.CalendarHourViewer.Placement = this.CalendarPlacement;
                this.CalendarHourViewer.StaysOpen = true;
                this.CalendarHourViewer.IsOpen = true;
            }
        }

        private void HideCalendar()
        {
            if (this.calendar is not null && this.hourPicker is not null)
            {
                BindingOperations.ClearBinding(this.DateTimeTextBox, TextBox.TextProperty);

                this.DisplayDateTime();
                this.CalendarHourViewer.IsOpen = false;

                if (this.closeButton is not null)
                {
                    this.closeButton.Click -= this.CloseButton_Click;
                }

                if (this.validateButton is not null)
                {
                    this.validateButton.Click -= this.ValidateButton_Click;
                }

                this.CalendarHourViewer.Child = null;
                this.closeButton = null;
                this.validateButton = null;
                this.calendar = null;
                this.hourPicker = null;
            }
        }

        private void Grid_TouchUp(object sender, System.Windows.Input.TouchEventArgs e) => this.ShowCalendar();

        private void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) => this.ShowCalendar();

        private void DateTimeTextBox_GotFocus(object sender, RoutedEventArgs e) => this.ShowCalendar();

        private void DateTimeTextBox_LostFocus(object sender, RoutedEventArgs e) => this.HideCalendar();

        private void DateTimeTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => this.HideCalendar();
    }
}
