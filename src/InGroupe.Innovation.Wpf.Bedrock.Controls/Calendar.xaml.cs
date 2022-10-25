using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
        private const int NumberOfCells = 42;

        private readonly Dictionary<DayOfWeek, int> daysWeekPositions = new()
        {
            [DayOfWeek.Monday] = 1,
            [DayOfWeek.Tuesday] = 2,
            [DayOfWeek.Wednesday] = 3,
            [DayOfWeek.Thursday] = 4,
            [DayOfWeek.Friday] = 5,
            [DayOfWeek.Saturday] = 6,
            [DayOfWeek.Sunday] = 7,
        };

        private CalendarMode calendarMode = CalendarMode.Days;

        private enum CalendarMode
        {
            Days = 0,
            Month = 2,
            Year = 3
        }

        private static readonly ResourceDictionary DefaultResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Themes;component/BasicBedrockTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        private static readonly ResourceDictionary DefaultCalendarResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Controls;component/CalendarDefaultTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            nameof(Date),
            typeof(DateOnly?),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DateOnly.FromDateTime(DateTime.Today), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        [AllowNull]
        [MaybeNull]
        public DateOnly? Date
        {
            get => this.GetValue(DateProperty) as DateOnly?;
            set => this.SetValue(DateProperty, value);
        }

        public static readonly DependencyProperty CurrentDateCellStyleProperty = DependencyProperty.Register(
            nameof(CurrentDateCellStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["CurrentDayCell"]));//,new PropertyChangedCallback(CurrentDateCellStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? CurrentDateCellStyle
        {
            get => this.GetValue(CurrentDateCellStyleProperty) as Style;
            set => this.SetValue(CurrentDateCellStyleProperty, value);
        }

        public static readonly DependencyProperty DayCellStyleProperty = DependencyProperty.Register(
            nameof(DayCellStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["DefaultCell"], new PropertyChangedCallback(DayCellStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? DayCellStyle
        {
            get => this.GetValue(DayCellStyleProperty) as Style;
            set => this.SetValue(DayCellStyleProperty, value);
        }

        public static readonly DependencyProperty DayCellSelectedStyleProperty = DependencyProperty.Register(
            nameof(DayCellSelectedStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["SelectedCell"]));

        [AllowNull]
        [MaybeNull]
        public Style? DayCellSelectedStyle
        {
            get => this.GetValue(DayCellSelectedStyleProperty) as Style;
            set => this.SetValue(DayCellSelectedStyleProperty, value);
        }

        public static readonly DependencyProperty DayStyleProperty = DependencyProperty.Register(
            nameof(DayStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(DayStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? DayStyle
        {
            get => this.GetValue(DayStyleProperty) as Style;
            set => this.SetValue(DayStyleProperty, value);
        }

        public static readonly DependencyProperty CurrentMonthHeaderStyleProperty = DependencyProperty.Register(
            nameof(CurrentMonthHeaderStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["CurrentMonthOrYear"], new PropertyChangedCallback(CurrentMonthHeaderStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? CurrentMonthHeaderStyle
        {
            get => this.GetValue(CurrentMonthHeaderStyleProperty) as Style;
            set => this.SetValue(CurrentMonthHeaderStyleProperty, value);
        }

        public static readonly DependencyProperty CurrentMonthCellStyleProperty = DependencyProperty.Register(
            nameof(CurrentMonthCellStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["CurrentDayCell"]));//, new PropertyChangedCallback(CurrentMonthCellStyleChanged)));


        [AllowNull]
        [MaybeNull]
        public Style? CurrentMonthCellStyle
        {
            get => this.GetValue(CurrentMonthCellStyleProperty) as Style;
            set => this.SetValue(CurrentMonthCellStyleProperty, value);
        }

        public static readonly DependencyProperty CurrentYearCellStyleProperty = DependencyProperty.Register(
            nameof(CurrentYearCellStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["CurrentDayCell"]));//, new PropertyChangedCallback(CurrentMonthCellStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? CurrentYearCellStyle
        {
            get => this.GetValue(CurrentYearCellStyleProperty) as Style;
            set => this.SetValue(CurrentYearCellStyleProperty, value);
        }

        public static readonly DependencyProperty BackwardForwardStyleProperty = DependencyProperty.Register(
            nameof(BackwardForwardStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["BackwardForwardButtons"], new PropertyChangedCallback(BackwardForwardStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? BackwardForwardStyle
        {
            get => this.GetValue(BackwardForwardStyleProperty) as Style;
            set => this.SetValue(BackwardForwardStyleProperty, value);
        }

        public static readonly DependencyProperty MonthCellStyleProperty = DependencyProperty.Register(
            nameof(MonthCellStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["DefaultCell"], new PropertyChangedCallback(MonthCellStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? MonthCellStyle
        {
            get => this.GetValue(MonthCellStyleProperty) as Style;
            set => this.SetValue(MonthCellStyleProperty, value);
        }

        public static readonly DependencyProperty YearCellStyleProperty = DependencyProperty.Register(
            nameof(YearCellStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["DefaultCell"], new PropertyChangedCallback(YearCellStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? YearCellStyle
        {
            get => this.GetValue(YearCellStyleProperty) as Style;
            set => this.SetValue(YearCellStyleProperty, value);
        }

        public static readonly DependencyProperty CellTextStyleProperty = DependencyProperty.Register(
            nameof(CellTextStyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultCalendarResourceDictionary["CellText"],new PropertyChangedCallback(CellTextStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? CellTextStyle
        {
            get => this.GetValue(CellTextStyleProperty) as Style;
            set => this.SetValue(CellTextStyleProperty, value);
        }

        public static readonly DependencyProperty CancelButtonSyleProperty = DependencyProperty.Register(
            nameof(CancelButtonSyle),
            typeof(Style),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultResourceDictionary["ButtonDanger"], new PropertyChangedCallback(CancelButtonSyleChanged)));

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
            typeof(Calendar),
            new FrameworkPropertyMetadata(DefaultResourceDictionary["ButtonSuccess"], new PropertyChangedCallback(ValidateButtonSyleChanged)));


        [AllowNull]
        [MaybeNull]
        public Style? ValidateButtonSyle
        {
            get => this.GetValue(ValidateButtonSyleProperty) as Style;
            set => this.SetValue(ValidateButtonSyleProperty, value);
        }

        public static readonly DependencyProperty ShowCancelValidateButtonsProperty = DependencyProperty.Register(
            nameof(ShowCancelValidateButtons),
            typeof(bool),
            typeof(Calendar),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(ShowCancelValidateButtonsChanged)));

        public bool ShowCancelValidateButtons
        {
            get => (bool)this.GetValue(ShowCancelValidateButtonsProperty);
            set => this.SetValue(ShowCancelValidateButtonsProperty, value);
        }

        public event EventHandler<DateOnly?>? OnValidated;
        public event EventHandler? OnCandelled;
        public event EventHandler<DateOnly?> OnDatePicked;

        public Calendar()
        {
            this.InitializeComponent();

            //OnDateChanged(this, new DependencyPropertyChangedEventArgs(DateProperty, null, DateOnly.Today));
            CurrentMonthHeaderStyleChanged(this, new DependencyPropertyChangedEventArgs(CurrentMonthHeaderStyleProperty, null, DefaultCalendarResourceDictionary["CurrentMonthOrYear"]));
            BackwardForwardStyleChanged(this, new DependencyPropertyChangedEventArgs(BackwardForwardStyleProperty, null, DefaultCalendarResourceDictionary["BackwardForwardButtons"]));
            //CurrentDateCellStyleChanged(this, new DependencyPropertyChangedEventArgs(CurrentDateCellStyleProperty, null, DefaultCalendarResourceDictionary["CurrentDayCell"]));


            CancelButtonSyleChanged(this, new DependencyPropertyChangedEventArgs(CancelButtonSyleProperty, null, DefaultResourceDictionary["ButtonDanger"]));
            ValidateButtonSyleChanged(this, new DependencyPropertyChangedEventArgs(CancelButtonSyleProperty, null, DefaultResourceDictionary["ButtonSuccess"]));
            CellTextStyleChanged(this, new DependencyPropertyChangedEventArgs(CancelButtonSyleProperty, null, DefaultCalendarResourceDictionary["CellText"]));
        }

        private static void ShowCancelValidateButtonsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            calendar.CancelButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            calendar.ValidateButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        //private static void CurrentDateCellStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var calendar = (Calendar)sender;

        //    if (calendar.calendarMode == CalendarMode.Days)
        //    {
        //        OnDateChanged(calendar, new DependencyPropertyChangedEventArgs(DateProperty, null, calendar.SelectedDate ?? calendar.Date));
        //    }
        //}

        private static void CancelButtonSyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            calendar.CancelButton.Style = e.NewValue as Style;
        }

        private static void ValidateButtonSyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            calendar.ValidateButton.Style = e.NewValue as Style;
        }

        private static void DayCellStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            foreach (var child in ((Grid)calendar.FindName("daysGrid")).Children)
            {
                if (child is Grid grid and { Children: { Count: > 0 } })
                {
                    ((Grid)grid.Children[0]).Style = e.NewValue as Style;
                }
            }
        }

        private static void DayStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            foreach (var child in ((Grid)calendar.FindName("daysGrid")).Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.Style = e.NewValue as Style;
                }
            }
        }

        private static void CurrentMonthHeaderStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            ((TextBlock)calendar.FindName("currentMonthAndYear")).Style = e.NewValue as Style;
            ((TextBlock)calendar.FindName("currentMonth")).Style = e.NewValue as Style;
        }

        private static void BackwardForwardStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            ((TextBlock)calendar.FindName("DayBackward")).Style = e.NewValue as Style;
            ((TextBlock)calendar.FindName("DayForward")).Style = e.NewValue as Style;     
            ((TextBlock)calendar.FindName("YearBackward")).Style = e.NewValue as Style;
            ((TextBlock)calendar.FindName("YearForward")).Style = e.NewValue as Style;
        }

        private static void MonthCellStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            foreach (var child in ((Grid)calendar.FindName("months")).Children)
            {
                if (child is Grid grid and { Children: { Count: > 0 } })
                {
                    ((Grid)grid.Children[0]).Style = e.NewValue as Style;
                }
            }
        }

        private static void YearCellStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            foreach (var child in ((Grid)calendar.FindName("years")).Children)
            {
                if (child is Grid grid and { Children: { Count: > 0 } })
                {
                    ((Grid)grid.Children[0]).Style = e.NewValue as Style;
                }
            }
        }

        private static void CellTextStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var calendar = (Calendar)sender;

            if (calendar.calendarMode == CalendarMode.Days)
            {
                foreach (var child in ((Grid)calendar.FindName("days")).Children)
                {
                    if (child is Grid grid and { Children: { Count: > 0 } } && grid.Children[0] is TextBlock textBlock)
                    {
                        textBlock.Style = e.NewValue as Style;
                    }
                }
            }

            if (calendar.calendarMode == CalendarMode.Month)
            {
                foreach (var child in ((Grid)calendar.FindName("months")).Children)
                {
                    if (child is Grid grid and { Children: { Count: > 0 } } && grid.Children[0] is TextBlock textBlock)
                    {
                        textBlock.Style = e.NewValue as Style;
                    }
                }
            }

            if (calendar.calendarMode == CalendarMode.Year)
            {
                foreach (var child in ((Grid)calendar.FindName("years")).Children)
                {
                    if (child is Grid grid and { Children: { Count: > 0 } } && grid.Children[0] is TextBlock textBlock)
                    {
                        textBlock.Style = e.NewValue as Style;
                    }
                }
            }
        }

        private (int Row, int Column)? todayPosition = null;

        private bool IsSelectedDate(DateOnly date) => this.Date switch
        {
            DateOnly selected => selected.Year == date.Year && selected.Month == date.Month && selected.Day == date.Day,
            _ => false
        };

        private void PopulateDays(DateOnly date)
        {
            this.ClearDateChildren();

            this.FillDaysAndHour(date);

            this.todayPosition = null;

            int rowIndex = 1;

            foreach (var row in BatchBy(this.GetDates(date.Year, date.Month), 7))
            {
                for (var i = 0; i < row.Count; i++)
                {
                    var isToday = row[i] == DateOnly.FromDateTime(DateTime.Today);

                    var grid = new Grid
                    {
                        Style = isToday ? this.CurrentDateCellStyle : (this.IsSelectedDate(row[i]) ? this.DayCellSelectedStyle : this.DayCellStyle),
                        Children =
                        {
                            new TextBlock
                            {
                                Text = row[i].Day.ToString(),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Style = this.CellTextStyle,
                                DataContext = row[i]
                            }
                        }
                    };

                    grid.MouseUp += this.DateMouseUp;
                    grid.TouchUp += this.DateTouchUp;

                    Grid.SetColumn(grid, i);
                    Grid.SetRow(grid, rowIndex);

                    if (isToday)
                    {
                        this.todayPosition = (rowIndex, i);
                    }

                    this.daysGrid.Children.Add(grid);
                }
                rowIndex++;
            }
        }

        private void PopulateMonths()
        {
            var months = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            int rowId = 0;

            foreach (var items in BatchBy(months, 4))
            {
                int columnId = 0;

                foreach (var month in items)
                {
                    var grid = new Grid
                    {
                        Style = month == DateTime.Today.Month ? this.CurrentMonthCellStyle : this.MonthCellStyle,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(month),
                                DataContext = month,
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Style = this.CellTextStyle
                            }
                        }
                    };

                    grid.MouseUp += this.MonthMouseUp;
                    grid.TouchUp += this.MonthTouchUp;

                    Grid.SetRow(grid, rowId);
                    Grid.SetColumn(grid, columnId);
                    columnId++;

                    this.months.Children.Add(grid);
                }

                rowId++;
            }
        }

        private void PopulateYears(int year)
        {
            var t = year - this.years.RowDefinitions.Count * this.years.ColumnDefinitions.Count;

            var batches = BatchBy(
                Enumerable.Range(
                    year - this.years.RowDefinitions.Count * this.years.ColumnDefinitions.Count,
                    this.years.RowDefinitions.Count * this.years.ColumnDefinitions.Count), 7);

            this.years.Children.Clear();

            var row = 0;
            foreach (var batch in batches)
            {
                for (var column = 0; column < batch.Count; column++)
                {
                    var grid = new Grid
                    {
                        Style = batch[column] == DateTime.Today.Year ? this.CurrentYearCellStyle : this.YearCellStyle,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = batch[column].ToString(),
                                Style = this.CellTextStyle,
                                DataContext = batch[column],
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            }
                        }
                    };

                    grid.MouseUp += this.YearMouseUp;
                    grid.TouchUp += this.YearTouchUp;

                    Grid.SetRow(grid, row);
                    Grid.SetColumn(grid, column);

                    this.years.Children.Add(grid);
                }
                row++;
            }
        }

        private void YearTouchUp(object? sender, TouchEventArgs e)
        {
            if (sender is Grid grid)
            {
                var year = (int)((TextBlock)grid.Children[0]).DataContext;

                this.UpdateDateYear(year);

                this.SwitchToMonthMode();
            }
        }

        private void YearMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid)
            {
                var year = (int)((TextBlock)grid.Children[0]).DataContext;

                this.UpdateDateYear(year);

                this.SwitchToMonthMode();
            }
        }

        private void MonthTouchUp(object? sender, TouchEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdateDateMonth((int)((TextBlock)grid.Children[0]).DataContext);

                this.SwitchToDayMode();
            }
        }

        private void MonthMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdateDateMonth((int)((TextBlock)grid.Children[0]).DataContext);

                this.SwitchToDayMode();
                this.PopulateDays(this.Date!.Value);
            }
        }

        private void ClearDateChildren()
        {
            foreach (var child in this.daysGrid.Children)
            {
                if (child is Grid grid)
                {
                    grid.MouseUp -= this.DateMouseUp;
                    grid.TouchUp -= this.DateTouchUp;
                }
            }

            // We ignore the first seven columns.
            this.daysGrid.Children.RemoveRange(7, NumberOfCells);
        }

        private void CleanMonthChildren()
        {
            foreach (var child in this.months.Children)
            {
                if (child is Grid grid)
                {
                    grid.MouseUp -= this.MonthMouseUp;
                    grid.TouchUp -= this.MonthTouchUp;
                }
            }
        }

        private void CleanYearChildren()
        {
            foreach (var child in this.months.Children)
            {
                if (child is Grid grid)
                {
                    grid.MouseUp -= this.YearMouseUp;
                    grid.TouchUp -= this.YearTouchUp;
                }
            }
        }

        private void DateTouchUp(object? sender, TouchEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdatePickedDate(grid);
                this.UpdateSelectedDateCell(grid);
            }
        }

        private void DateMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdatePickedDate(grid);
                this.UpdateSelectedDateCell(grid);
            }
        }

        private void UpdatePickedDate(Grid grid)
        {
            this.Date = ((TextBlock)grid.Children[0]).DataContext as DateOnly?;
            this.OnDatePicked?.Invoke(this, this.Date);
        }

        private void UpdateSelectedDateCell(Grid grid)
        {
            grid.Style = this.DayCellSelectedStyle;

            var column = Grid.GetColumn(grid);
            var row = Grid.GetRow(grid);

            foreach(var child in ((Grid)this.FindName("daysGrid")).Children)
            {
                if (child is Grid childGrid)
                {
                    var childColumn = Grid.GetColumn(childGrid);
                    var childRow = Grid.GetRow(childGrid);

                    if (childColumn == column && row == childRow)
                    {
                        continue;
                    }

                    if (this.todayPosition?.Row == childRow && this.todayPosition?.Column == childColumn)
                    {
                        continue;
                    }

                    childGrid.Style = this.DayCellStyle;
                }
            }
        }

        private List<DateOnly> GetDates(int year, int month)
        {
            var dates = new List<DateOnly>();

            var numberOfDaysInTheMonth = CultureInfo.CurrentUICulture.Calendar.GetDaysInMonth(year, month);

            var firstDay = new DateOnly(year, month, 1);

            if (firstDay.DayOfWeek == DayOfWeek.Monday)
            {
                var remainderDays = NumberOfCells - numberOfDaysInTheMonth;

                dates.AddRange(
                    CreateDateRange(new DateOnly(year, month, numberOfDaysInTheMonth).AddDays(1), firstDay));

                dates.AddRange(
                    CreateDateRange(
                        new DateOnly(year, month, numberOfDaysInTheMonth).AddDays(remainderDays + 1),
                        new DateOnly(year, month, numberOfDaysInTheMonth).AddDays(1)
                        ));
            }
            else
            {
                var daysBefore = this.daysWeekPositions[firstDay.DayOfWeek] - 1;

                dates.AddRange(
                    CreateDateRange(
                        firstDay,
                        firstDay.AddDays(-daysBefore)

                        ));

                dates.AddRange(
                    CreateDateRange(new DateOnly(year, month, numberOfDaysInTheMonth).AddDays(1), firstDay));

                var remainderDays = NumberOfCells - dates.Count;

                dates.AddRange(
                    CreateDateRange(
                        new DateOnly(year, month, numberOfDaysInTheMonth).AddDays(remainderDays + 1),
                        new DateOnly(year, month, numberOfDaysInTheMonth).AddDays(1)
                        ));
            }

            return dates;
        }

        private static string FormatDay(string day) => $"{char.ToUpper(day[0], CultureInfo.CurrentUICulture)}{char.ToLower(day[1], CultureInfo.CurrentUICulture)}";

        private void FillDaysAndHour(DateOnly date)
        {
            this.first.Text = FormatDay(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Monday));
            this.second.Text = FormatDay(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Tuesday));
            this.third.Text = FormatDay(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Wednesday));
            this.forth.Text = FormatDay(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Thursday));
            this.fifth.Text = FormatDay(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Friday));
            this.sixth.Text = FormatDay(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Saturday));
            this.seventh.Text = FormatDay(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Sunday));

            this.currentMonthAndYear.Text = date.ToString("MMMM yyyy");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ClearDateChildren();
            this.CleanMonthChildren();
            this.CleanYearChildren();

            if (this.OnCandelled != null)
            {
                foreach (EventHandler eventHandler in this.OnCandelled!.GetInvocationList())
                {
                    this.OnCandelled -= eventHandler;
                }
            }

            if (this.OnValidated != null)
            {
                foreach (EventHandler<DateOnly?> eventHandler in this.OnValidated!.GetInvocationList())
                {
                    this.OnValidated -= eventHandler;
                }
            }
        }


        private void DaysBackward_MouseUp(object sender, MouseButtonEventArgs e) => this.MoveDaysBackward();

        private void DaysBackward_TouchUp(object sender, TouchEventArgs e) => this.MoveDaysBackward();

        private void DaysForward_MouseUp(object sender, MouseButtonEventArgs e) => this.MoveDaysForward();

        private void DaysForward_TouchUp(object sender, TouchEventArgs e) => this.MoveDaysForward();



        private void YearBackward_MouseUp(object sender, MouseButtonEventArgs e) => this.MoveYearsBackward();

        private void YearBackward_TouchUp(object sender, TouchEventArgs e) => this.MoveYearsBackward();

        private void YearForward_MouseUp(object sender, MouseButtonEventArgs e) => this.MoveYearsForward();

        private void YearForward_TouchUp(object sender, TouchEventArgs e) => this.MoveYearsForward();




        private void MoveDaysBackward()
        {
            this.Date ??= DateOnly.FromDateTime(DateTime.Today);
            this.Date = this.Date!.Value.AddMonths(-1);

            this.PopulateDays(this.Date!.Value);
        }

        private void MoveDaysForward()
        {
            this.Date ??= DateOnly.FromDateTime(DateTime.Today);
            this.Date = this.Date!.Value.AddMonths(1);

            this.PopulateDays(this.Date!.Value);
        }

        private void MoveYearsBackward()
        {
            this.years.Children.Clear();

            this.Date = this.Date!.Value.AddYears(-(this.years.RowDefinitions.Count * this.years.ColumnDefinitions.Count));

            this.PopulateYears(this.Date!.Value.Year);
        }

        private void MoveYearsForward()
        {
            this.years.Children.Clear();

            this.Date = this.Date!.Value.AddYears(this.years.RowDefinitions.Count * this.years.ColumnDefinitions.Count);

            this.PopulateYears(this.Date!.Value.Year);
        }

        private void Cancelled_Click(object sender, RoutedEventArgs e) => this.OnCandelled?.Invoke(this, EventArgs.Empty);

        private void Validate_Click(object sender, RoutedEventArgs e) => this.OnValidated?.Invoke(this, this.Date!);

        private void UpdateDateYear(int year)
        {
            if (this.Date == null)
            {
                this.Date = new DateOnly(year, 1, 1);
            }
            else
            {
                this.Date = new DateOnly(year, this.Date!.Value.Month, 1);
            }
        }

        private void UpdateDateMonth(int month)
        {
            if (this.Date == null)
            {
                this.Date = new DateOnly(DateTime.Today.Year, month, 1);
            }
            else
            {
                this.Date = new DateOnly(this.Date!.Value.Year, month, 1);
            }
        }

        private void SwitchToYearMode_TouchUp(object sender, TouchEventArgs e)
            => this.SwitchToYearMode();

        private void SwitchToYearMode_MouseUp(object sender, MouseButtonEventArgs e)
            => this.SwitchToYearMode();

        private void SwitchToMonthMode_TouchUp(object sender, TouchEventArgs e)
            => this.SwitchToMonthMode();

        private void SwitchToMonthMode_MouseUp(object sender, MouseButtonEventArgs e)
            => this.SwitchToMonthMode();

        private void SwitchToYearMode()
        {
            this.calendarMode = CalendarMode.Year;

            this.currentMonthAndYear.Visibility = Visibility.Collapsed;
            this.daysBackwardForward.Visibility = Visibility.Collapsed;
            this.yearsBackwardForward.Visibility = Visibility.Visible;
            this.currentMonth.Visibility = Visibility.Collapsed;

            this.days.Visibility = Visibility.Collapsed;
            this.years.Visibility = Visibility.Visible;
            this.months.Visibility = Visibility.Collapsed;

            this.PopulateYears(this.Date?.Year ?? DateOnly.FromDateTime(DateTime.Today).Year);
        }

        private void SwitchToMonthMode()
        {
            this.calendarMode = CalendarMode.Month;

            this.currentMonthAndYear.Visibility = Visibility.Collapsed;
            this.daysBackwardForward.Visibility = Visibility.Collapsed;
            this.yearsBackwardForward.Visibility = Visibility.Collapsed;

            this.days.Visibility = Visibility.Collapsed;
            this.years.Visibility = Visibility.Collapsed;
            this.months.Visibility = Visibility.Visible;
            this.currentMonth.Visibility = Visibility.Visible;

            if (this.Date == null)
            {
                this.currentMonth.Text = DateTime.Today.ToString("MMMM");
            }
            else
            {
                this.currentMonth.Text = this.Date!.Value.ToString("MMMM");
            }
        }

        private void SwitchToDayMode()
        {
            this.calendarMode = CalendarMode.Days;

            this.currentMonthAndYear.Visibility = Visibility.Visible;
            this.daysBackwardForward.Visibility = Visibility.Visible;
            this.days.Visibility = Visibility.Visible;

            this.currentMonth.Visibility = Visibility.Collapsed;
            this.yearsBackwardForward.Visibility = Visibility.Collapsed;
            this.years.Visibility = Visibility.Collapsed;
            this.months.Visibility = Visibility.Collapsed;


            if (this.Date == null)
            {
                this.currentMonthAndYear.Text = DateTime.Today.ToString("MMMM yyyy");
            }
            else
            {
                this.currentMonthAndYear.Text = this.Date!.Value.ToString("MMMM yyyy");
            }
        }

        private static IEnumerable<List<T>> BatchBy<T>(IEnumerable<T> enumerable, int batchSize)
        {
            using var enumerator = enumerable.GetEnumerator();

            List<T>? list = null;
            while (enumerator.MoveNext())
            {
                if (list == null)
                {
                    list = new List<T> { enumerator.Current };
                }
                else if (list.Count < batchSize)
                {
                    list.Add(enumerator.Current);
                }
                else
                {
                    yield return list;
                    list = new List<T> { enumerator.Current };
                }
            }

            if (list?.Count > 0)
            {
                yield return list;
            }
        }

        private static IEnumerable<DateOnly> CreateDateRange(DateOnly endDate, DateOnly startDate)
        {
            if (endDate < startDate)
            {
                throw new ArgumentException("endDate must be greater than or equal to startDate");
            }

            while (startDate < endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.PopulateMonths();
            this.SwitchToDayMode();

            if (this.Date is not null)
            {
                this.PopulateDays(this.Date.Value);
            }
            else
            {
                this.PopulateDays(DateOnly.FromDateTime(DateTime.Today));
            }
        }
    }
}
