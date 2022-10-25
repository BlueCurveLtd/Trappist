using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls
{
    /// <summary>
    /// Logique d'interaction pour ButtonGroup.xaml
    /// </summary>
    public partial class ButtonGroup : UserControl
    {
        //some shadowing shit!!!!
        public new event EventHandler<RoutedEventArgs> LostFocus;
        public new event EventHandler<TouchEventArgs> TouchUp;
        public new event EventHandler<TouchEventArgs> TouchDown;
        public new event EventHandler<RoutedEventArgs> GotFocus;
        public new event EventHandler<KeyboardFocusChangedEventArgs> GotKeyboardFocus;


        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(
            nameof(ButtonStyle),
            typeof(Style),
            typeof(ButtonGroup),
            new PropertyMetadata(ButtonStyleChanged));

        [AllowNull]
        [MaybeNull]
        public Style? ButtonStyle
        {
            get => this.GetValue(ButtonStyleProperty) as Style;
            set => this.SetValue(ButtonStyleProperty, value);
        }
         
        public static readonly DependencyProperty ButtonTextStyleProperty = DependencyProperty.Register(
            nameof(ButtonTextStyle),
            typeof(Style),
            typeof(ButtonGroup),
            new PropertyMetadata(ButtonTextStyleChanged));

        [AllowNull]
        [MaybeNull]
        public Style? ButtonTextStyle
        {
            get => this.GetValue(ButtonTextStyleProperty) as Style;
            set => this.SetValue(ButtonTextStyleProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(ButtonGroup),
            new PropertyMetadata(TextChanged));

        [AllowNull]
        [MaybeNull]
        public string? Text
        {
            get => this.GetValue(TextProperty) as string;
            set => this.SetValue(TextProperty, value);
        }


        public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register(
            nameof(TextBoxStyle),
            typeof(Style),
            typeof(ButtonGroup),
            new PropertyMetadata(TextBoxStyleChanged));

        [AllowNull]
        [MaybeNull]
        public Style? TextBoxStyle
        {
            get => this.GetValue(TextBoxStyleProperty) as Style;
            set => this.SetValue(TextBoxStyleProperty, value);
        }

        public ButtonGroup()
        {
            this.InitializeComponent();

            this.button.Style = (Style)this.Resources["DefaultButtonGroupButton"];
            this.buttonText.Style = (Style)this.Resources["DefaultButtonGroupButtonText"];
        }

        private static void ButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Style style)
            {
                var buttonGroup = (ButtonGroup)d;
                buttonGroup.button.Style = style;
            }
        }

        private static void ButtonTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Style style)
            {
                var buttonGroup = (ButtonGroup)d;
                buttonGroup.buttonText.Style = style;
            }
        }

        private static void TextBoxStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Style style)
            {
                var buttonGroup = (ButtonGroup)d;
                buttonGroup.textBox.Style = style;
            }
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var buttonGroup = (ButtonGroup)d;
            buttonGroup.textBox.Text = e.NewValue as string;
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
           => this.LostFocus?.Invoke(sender, e);

        private void textBox_TouchUp(object sender, TouchEventArgs e)
            => this.TouchUp?.Invoke(sender, e);

        private void textBox_TouchDown(object sender, TouchEventArgs e)
            => this.TouchDown?.Invoke(sender, e);

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
            => this.GotFocus?.Invoke(sender, e);

        private void textBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
            => this.GotKeyboardFocus?.Invoke(sender, e);
    }
}
