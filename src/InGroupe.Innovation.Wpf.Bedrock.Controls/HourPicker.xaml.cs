using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls
{
    /// <summary>
    /// Interaction logic for HourPicker.xaml
    /// </summary>
    public partial class HourPicker : UserControl
    {
        private int? hour;
        private int? minute;

        private static readonly ResourceDictionary DefaultResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Themes;component/BasicBedrockTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        private static readonly ResourceDictionary DefaultHourPickerResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Controls;component/HourPickerDefaultTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        public event EventHandler<TimeOnly?>? OnSelected;
        public event EventHandler? OnCandelled;


        public static readonly DependencyProperty SelectedHourProperty = DependencyProperty.Register(
            nameof(SelectedHour),
            typeof(TimeOnly?),
            typeof(HourPicker));

        [AllowNull]
        [MaybeNull]
        public TimeOnly? SelectedHour
        {
            get => this.GetValue(SelectedHourProperty) as TimeOnly?;
            set => this.SetValue(SelectedHourProperty, value);
        }

        public static readonly DependencyProperty HourItemStyleProperty = DependencyProperty.Register(
            nameof(HourItemStyle),
            typeof(Style),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(DefaultHourPickerResourceDictionary["DefaultCell"], new PropertyChangedCallback(HourItemStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? HourItemStyle
        {
            get => this.GetValue(HourItemStyleProperty) as Style;
            set => this.SetValue(HourItemStyleProperty, value);
        }

        public static readonly DependencyProperty MinuteItemStyleProperty = DependencyProperty.Register(
            nameof(MinuteItemStyle),
            typeof(Style),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(DefaultHourPickerResourceDictionary["DefaultCell"], new PropertyChangedCallback(MinuteItemStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? MinuteItemStyle
        {
            get => this.GetValue(MinuteItemStyleProperty) as Style; 
            set => this.SetValue(MinuteItemStyleProperty, value);
        }

        public static readonly DependencyProperty BackwardForwardStyleProperty = DependencyProperty.Register(
            nameof(BackwardForwardStyle),
            typeof(Style),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(BackwardForwardStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? BackwardForwardStyle
        {
            get => this.GetValue(BackwardForwardStyleProperty) as Style;
            set => this.SetValue(BackwardForwardStyleProperty, value);
        }

        public static readonly DependencyProperty CancelButtonStyleProperty = DependencyProperty.Register(
            nameof(CancelButtonStyle),
            typeof(Style),
            typeof(HourPicker),
            new FrameworkPropertyMetadata((Style)DefaultResourceDictionary["ButtonDanger"], new PropertyChangedCallback(CancelButtonStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? CancelButtonStyle
        {
            get => this.GetValue(CancelButtonStyleProperty) as Style;
            set => this.SetValue(CancelButtonStyleProperty, value);
        }

        public static readonly DependencyProperty ValidateButtonStyleProperty = DependencyProperty.Register(
            nameof(ValidateButtonStyle),
            typeof(Style),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(DefaultResourceDictionary["ButtonSuccess"], new PropertyChangedCallback(ValidateButtonStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? ValidateButtonStyle
        {
            get => this.GetValue(ValidateButtonStyleProperty) as Style;
            set => this.SetValue(ValidateButtonStyleProperty, value);
        }

        public static readonly DependencyProperty ShowBackwardForwardButtonsProperty = DependencyProperty.Register(
            nameof(ShowBackwardForwardButtons),
            typeof(bool),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ShowBackwardForwardButtonsChanged)));

        public bool ShowBackwardForwardButtons
        {
            get => (bool)this.GetValue(ShowBackwardForwardButtonsProperty);
            set => this.SetValue(ShowBackwardForwardButtonsProperty, value);
        }

        public static readonly DependencyProperty CellTextStyleProperty = DependencyProperty.Register(
            nameof(CellTextStyle),
            typeof(Style),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(DefaultHourPickerResourceDictionary["CellText"], new PropertyChangedCallback(CellTextStyleChanged)));

        [AllowNull]
        [MaybeNull]
        public Style? CellTextStyle
        {
            get => this.GetValue(CellTextStyleProperty) as Style;
            set => this.SetValue(CellTextStyleProperty, value);
        }

        public static readonly DependencyProperty SelectedCellStyleProperty = DependencyProperty.Register(
            nameof(SelectedCellStyle),
            typeof(Style),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(DefaultHourPickerResourceDictionary["SelectedCell"]));

        [AllowNull]
        [MaybeNull]
        public Style? SelectedCellStyle
        {
            get => this.GetValue(SelectedCellStyleProperty) as Style;
            set => this.SetValue(SelectedCellStyleProperty, value);
        }

        public static readonly DependencyProperty ShowCancelValidateButtonsProperty = DependencyProperty.Register(
            nameof(ShowCancelValidateButtons),
            typeof(bool),
            typeof(HourPicker),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(ShowCancelValidateButtonsChanged)));
         
        public bool ShowCancelValidateButtons
        {
            get => (bool)this.GetValue(ShowCancelValidateButtonsProperty);
            set => this.SetValue(ShowCancelValidateButtonsProperty, value);
        }

        public HourPicker() => this.InitializeComponent();

        private static void ShowCancelValidateButtonsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;

            hourPicker.CancelButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            hourPicker.ValidateButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void HourItemStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;

            foreach (var child in hourPicker.hours.Children)
            {
                ((Grid)child).Style = e.NewValue as Style;
            }
        }

        private static void MinuteItemStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;
             
            foreach (var child in hourPicker.minutes.Children)
            {
                ((Grid)child).Style = e.NewValue as Style;
            }
        }

        private static void BackwardForwardStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;

            ((TextBlock)hourPicker.FindName("BackwardHour")).Style = e.NewValue as Style;
            ((TextBlock)hourPicker.FindName("BackwardMinute")).Style = e.NewValue as Style;
            ((TextBlock)hourPicker.FindName("ForwardHour")).Style = e.NewValue as Style;
            ((TextBlock)hourPicker.FindName("ForwardMinute")).Style = e.NewValue as Style;
        }

        private static void CancelButtonStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;

            ((Button)hourPicker.FindName("CancelButton")).Style = e.NewValue as Style;
        }

        private static void ValidateButtonStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;

            ((Button)hourPicker.FindName("ValidateButton")).Style = e.NewValue as Style;
        }

        private static void ShowBackwardForwardButtonsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;
            var visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;

            ((TextBlock)hourPicker.FindName("BackwardHour")).Visibility = visibility;
            ((TextBlock)hourPicker.FindName("BackwardMinute")).Visibility = visibility;
            ((TextBlock)hourPicker.FindName("ForwardHour")).Visibility = visibility;
            ((TextBlock)hourPicker.FindName("ForwardMinute")).Visibility = visibility;
        }      
        
        private static void CellTextStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var hourPicker = (HourPicker)sender;
            
            foreach(var child in hourPicker.minutes.Children)
            {
                ((TextBlock)((Grid)child).Children[0]).Style = e.NewValue as Style;
            }

            foreach (var child in hourPicker.hours.Children)
            {
                ((TextBlock)((Grid)child).Children[0]).Style = e.NewValue as Style;
            }
        }

        private void PopulateHours()
        {
            this.hours.Children.Clear();

            var date = DateTime.Now.Date;

            for (int i = 0; i < 24; i++)
            {
                var hour = date.Date.AddHours(i);

                var grid = new Grid
                {
                    Style = this.HourItemStyle,
                    Name = $"Hour{i}",
                    Children =
                    {
                        new TextBlock
                        {
                            Text = hour.ToString("HH"),
                            DataContext = hour.Hour,
                            Style = this.CellTextStyle,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
                };

                grid.MouseUp += this.HourMouseUp;
                grid.TouchUp += this.HourTouchup;

                this.hours.Children.Add(grid);
            }
        }

        private void PopulateMinutes()
        {
            this.minutes.Children.Clear();

            for (var i = 0; i < 60; i++)
            {
                var grid = new Grid
                {
                    Name = $"Minute{i}",
                    Style = this.MinuteItemStyle,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = i.ToString("00"),
                            DataContext = i,
                            Style = this.CellTextStyle,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
                };

                grid.MouseUp += this.MinuteMouseUp;
                grid.TouchUp += this.MinuteTouchup;

                this.minutes.Children.Add(grid);
            }
        }

        private void HourTouchup(object? sender, TouchEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdateHour((int)((TextBlock)grid.Children[0]).DataContext);
                this.UpdateSelectedCellStyle(grid);
                this.UpdateHourSelectedCell(grid);
            }
        }

        private void HourMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdateHour((int)((TextBlock)grid.Children[0]).DataContext);
                this.UpdateSelectedCellStyle(grid);
                this.UpdateHourSelectedCell(grid);
            }
        }

        private void ResetHourCellsStyle()
        {
            foreach(Grid child in this.hours.Children)
            {
                child.Style = this.HourItemStyle;
            }
        }

        private void ResetMinuteCellsStyle()
        {
            foreach (Grid child in this.minutes.Children)
            {
                child.Style = this.MinuteItemStyle;
            }
        }

        private void ScrollToSpecificHour(int hour)
        {
            this.ResetHourCellsStyle();
            var child = (Grid)this.hours.Children[hour];
            this.UpdateSelectedCellStyle(child);

            var currentOffset = this.ScrollviewerHour.VerticalOffset;
            var itemHeight = GetChildrenMaxHeight(this.hours);

            var childPosition = (hour == 0 ? 1 : hour) * itemHeight;

            if (currentOffset > childPosition)
            {
                this.ScrollviewerHour.ScrollToVerticalOffset(-childPosition);
            }

            if (currentOffset < childPosition)
            {
                this.ScrollviewerHour.ScrollToVerticalOffset(childPosition);
            }
        }

        private void ScollToSpecificMinute(int minute)
        {
            this.ResetMinuteCellsStyle();
            var child = (Grid)this.minutes.Children[minute];
            this.UpdateSelectedCellStyle(child);

            var currentOffset = this.ScrollviewerMinute.VerticalOffset;
            var itemHeight = GetChildrenMaxHeight(this.minutes);

            var childPosition = (minute == 0 ? 1 : minute) * itemHeight;

            if (currentOffset > childPosition)
            {
                this.ScrollviewerMinute.ScrollToVerticalOffset(-childPosition);
            }

            if (currentOffset < childPosition)
            {
                this.ScrollviewerMinute.ScrollToVerticalOffset(childPosition);
            }
        }

        private void MinuteTouchup(object? sender, TouchEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdateMinute((int)((TextBlock)grid.Children[0]).DataContext);
                this.UpdateSelectedCellStyle(grid);
                this.UpdateMinuteSelectedCell(grid);
            }
        }

        private void MinuteMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid grid)
            {
                this.UpdateMinute((int)((TextBlock)grid.Children[0]).DataContext);
                this.UpdateSelectedCellStyle(grid);
                this.UpdateMinuteSelectedCell(grid);
            }
        }

        private void UpdateHour(int hour)
        {
            this.hour = hour;

            if (this.SelectedHour is null)
            {
                this.SelectedHour = new TimeOnly(hour, 0);
            }
            else
            {
                var minute = this.SelectedHour.Value;
                this.SelectedHour = new TimeOnly(hour, minute.Minute);
            }
        }

        private void UpdateMinute(int minute)
        {
            this.minute = minute;

            if (this.SelectedHour is null)
            {
                this.SelectedHour = new TimeOnly(0, minute);
            }
            else
            {
                var hour = this.SelectedHour.Value;                
                this.SelectedHour = new TimeOnly(hour.Hour, minute);
            }
        }

        private void UpdateSelectedCellStyle(Grid grid)
            => grid.Style = this.SelectedCellStyle;

        private void UpdateHourSelectedCell(Grid grid)
        {
            var value = (int)((TextBlock)grid.Children[0]).DataContext;

            foreach (var child in this.hours.Children)
            {
                if (child is Grid childGrid && (int)((TextBlock)childGrid.Children[0]).DataContext != value)
                {
                    childGrid.Style = this.HourItemStyle;
                }
            }
        }

        private void UpdateMinuteSelectedCell(Grid grid)
        {
            var value = (int)((TextBlock)grid.Children[0]).DataContext;

            foreach (var child in this.minutes.Children)
            {
                if (child is Grid childGrid && (int)((TextBlock)childGrid.Children[0]).DataContext != value)
                {
                    childGrid.Style = this.HourItemStyle;
                }
            }
        }

        private void CleanHourChildren()
        {
            foreach (var child in this.hours.Children)
            {
                if (child is Grid grid)
                {
                    grid.MouseUp -= this.HourMouseUp;
                    grid.TouchUp -= this.HourTouchup;
                }
            }
        }

        private void CleanMinuteChildren()
        {
            foreach (var child in this.minutes.Children)
            {
                if (child is Grid grid)
                {
                    grid.MouseUp -= this.MinuteMouseUp;
                    grid.TouchUp -= this.MinuteTouchup;
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.CleanHourChildren();
            
            this.CleanMinuteChildren();

            if (this.OnCandelled is not null)
            {
                foreach (EventHandler eventHandler in this.OnCandelled!.GetInvocationList())
                {
                    this.OnCandelled -= eventHandler;
                }
            }

            if (this.OnSelected is not null)
            {
                foreach (EventHandler<TimeOnly?> eventHandler in this.OnSelected!.GetInvocationList())
                {
                    this.OnSelected -= eventHandler;
                }
            }
        }        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.PopulateHours();
            this.PopulateMinutes();
            this.ScrollviewerHour.ScrollToTop();
            this.ScrollviewerMinute.ScrollToTop();

            if (this.SelectedHour is { Hour: > 0, Minute: > 0 })
            {
                this.ScrollToSpecificHour(this.SelectedHour.Value.Hour);
                this.ScollToSpecificMinute(this.SelectedHour.Value.Minute);
            }
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
            => this.OnCandelled?.Invoke(this, EventArgs.Empty);

        private void ValidateClicked(object sender, RoutedEventArgs e)
            => this.OnSelected?.Invoke(this, 
                this.hour != null ?
                new TimeOnly(this.hour.Value, this.minute ?? 0) : null);




        private void BackwardHour_MouseUp(object sender, MouseButtonEventArgs e) => ScrollUp(this.ScrollviewerHour, GetChildrenMaxHeight(this.hours));

        private void BackwardHour_TouchUp(object sender, TouchEventArgs e) => ScrollUp(this.ScrollviewerHour, GetChildrenMaxHeight(this.hours));

        private void ForwardHour_MouseUp(object sender, MouseButtonEventArgs e) => ScrollDown(this.ScrollviewerHour, GetChildrenMaxHeight(this.hours));

        private void ForwardHour_TouchUp(object sender, TouchEventArgs e) => ScrollDown(this.ScrollviewerHour, GetChildrenMaxHeight(this.hours));




        private void BackwardMinute_MouseUp(object sender, MouseButtonEventArgs e) => ScrollUp(this.ScrollviewerMinute, GetChildrenMaxHeight(this.minutes));

        private void BackwardMinute_TouchUp(object sender, TouchEventArgs e) => ScrollUp(this.ScrollviewerMinute, GetChildrenMaxHeight(this.minutes));

        private void ForwardMinute_MouseUp(object sender, MouseButtonEventArgs e) => ScrollDown(this.ScrollviewerMinute, GetChildrenMaxHeight(this.minutes));

        private void ForwardMinute_TouchUp(object sender, TouchEventArgs e) => ScrollDown(this.ScrollviewerMinute, GetChildrenMaxHeight(this.minutes));




        private static void ScrollUp(ScrollViewer scrollViewer, double elemenHeight) => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + elemenHeight);

        private static void ScrollDown(ScrollViewer scrollViewer, double elemenHeight)  => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - elemenHeight);

        private static double GetChildrenMaxHeight(StackPanel stackPanel)
        {
            var height = 0D;

            foreach(var child in stackPanel.Children)
            {
                if (child is Grid grid)
                {
                    if (grid.Height > height)
                    {
                        height = grid.Height + grid.Margin.Bottom + grid.Margin.Top;
                    }
                }
            }

            return height;
        }
    }
}
