using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Pages
{
    /// <summary>
    /// Interaction logic for MasterDetail.xaml
    /// </summary>
    public partial class MasterDetail : Window
    {
        private bool menuExpanded;

        private static readonly ResourceDictionary DefaultResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Themes;component/BasicBedrockTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        private static readonly ResourceDictionary MasterResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("/InGroupe.Innovation.Wpf.Bedrock.Controls;component/Pages/MasterDetailTheme.xaml", UriKind.RelativeOrAbsolute)
        };

        /// <summary>
        /// Indicates if the left site menu is visible
        /// </summary>
        public static readonly DependencyProperty ShowSideBarProperty = DependencyProperty.Register(
            nameof(ShowSideBar),
            typeof(Visibility),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnShowSideBarChanged)));

        /// <summary>
        /// Indicates if the left site menu is visible
        /// </summary>
        public Visibility ShowSideBar
        {
            get => (Visibility)this.GetValue(ShowSideBarProperty);
            set => this.SetValue(ShowSideBarProperty, value);
        }

        public static readonly DependencyProperty ContainerBackgroundProperty = DependencyProperty.Register(
            nameof(ContainerBackground),
            typeof(Brush),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnContainerBackgroundChanged)));


        /// <summary>
        /// Gets or Sets the container background color.
        /// </summary>
        public Brush ContainerBackground
        {
            get => (Brush)this.GetValue(ContainerBackgroundProperty);
            set => this.SetValue(ContainerBackgroundProperty, value);
        }

        public static readonly DependencyProperty MenuBackgroundProperty = DependencyProperty.Register(
            nameof(MenuBackground),
            typeof(Brush),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata((SolidColorBrush)DefaultResourceDictionary["BrushGray200"], new PropertyChangedCallback(OnMenuBackgroundChanged)));

        /// <summary>
        /// Gets or Sets the menu background color.
        /// </summary>
        public Brush MenuBackground
        {
            get => (Brush)this.GetValue(MenuBackgroundProperty);
            set => this.SetValue(MenuBackgroundProperty, value);
        }

        public static readonly DependencyProperty HamburgerColorProperty = DependencyProperty.Register(
            nameof(HamburgerColor),
            typeof(Brush),
            typeof(MasterDetail),
            new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnHambugerColorChanged)));
        
        /// <summary>
        /// Gets or Sets the hamburger background color.
        /// </summary>
        public Brush HamburgerColor
        {
            get => (Brush)this.GetValue(HamburgerColorProperty);
            set => this.SetValue(HamburgerColorProperty, value);
        }

        public static readonly DependencyProperty TopActionBarBackgroundProperty = DependencyProperty.Register(
            nameof(TopActionBarBackground),
            typeof(Brush),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata(Brushes.White, new PropertyChangedCallback(OnTopActionBarBackgroundChanged)));

        /// <summary>
        /// Gets or Sets the top action bar background color.
        /// </summary>
        public Brush TopActionBarBackground
        {
            get => (Brush)this.GetValue(TopActionBarBackgroundProperty);
            set => this.SetValue(TopActionBarBackgroundProperty, value);
        }

        public static readonly DependencyProperty TopActionBarProperty = DependencyProperty.Register(
            nameof(TopActionBar),
            typeof(FrameworkElement),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTopActionBarChanged)));

        /// <summary>
        /// Gets or Sets the content of the top action bar.
        /// </summary>
        public FrameworkElement? TopActionBar
        {
            get => this.GetValue(TopActionBarProperty) as FrameworkElement;
            set => this.SetValue(TopActionBarProperty, value);
        }

        public static readonly DependencyProperty TopActionBarTitleProperty = DependencyProperty.Register(
            nameof(TopActionBarTitle),
            typeof(string),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata("Default application title", new PropertyChangedCallback(OnTopActionBarTitleChanged)));

        /// <summary>
        /// Gets or Sets the title of the window
        /// </summary>
        [AllowNull]
        [MaybeNull]
        public string? TopActionBarTitle
        {
            get => this.GetValue(TopActionBarTitleProperty) as string;
            set => this.SetValue(TopActionBarTitleProperty, value);
        }

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            nameof(Menu),
            typeof(FrameworkElement),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMenuChanged)));

        /// <summary>
        /// Gets or Sets the content of the left menu.
        /// </summary>
        [AllowNull]
        [MaybeNull]
        public FrameworkElement? Menu
        {
            get => this.GetValue(MenuProperty) as FrameworkElement;
            set => this.SetValue(MenuProperty, value);
        }

        public static readonly DependencyProperty MenuExpandStoryBoardProperty = DependencyProperty.Register(
            nameof(MenuExpandStoryBoard),
            typeof(Storyboard),
            typeof(MasterDetail),
            new PropertyMetadata((Storyboard)MasterResourceDictionary["ExpandMenuStoryboard"]));

        /// <summary>
        /// Gets or Sets the story board of the left menu when it expand.
        /// </summary>
        [AllowNull]
        [MaybeNull]
        public Storyboard? MenuExpandStoryBoard
        {
            get => this.GetValue(MenuExpandStoryBoardProperty) as Storyboard;
            set => this.SetValue(MenuExpandStoryBoardProperty, value);
        }


        public static readonly DependencyProperty MenuFoldStoryBoardProperty = DependencyProperty.Register(
            nameof(MenuFoldStoryBoard),
            typeof(Storyboard),
            typeof(MasterDetail),
            new PropertyMetadata((Storyboard)MasterResourceDictionary["FoldMenuStoryboard"]));

        /// <summary>
        /// Gets or Sets the story board of the left menu when it fold.
        /// </summary>
        [AllowNull]
        [MaybeNull]
        public Storyboard? MenuFoldStoryBoard
        {
            get => this.GetValue(MenuFoldStoryBoardProperty) as Storyboard;
            set => this.SetValue(MenuFoldStoryBoardProperty, value);
        }

        public static readonly DependencyProperty MenuWidthProperty = DependencyProperty.Register(
            nameof(MenuWidth),
            typeof(int?),
            typeof(MasterDetail),
            new PropertyMetadata(60));

        /// <summary>
        /// Gets or Sets the width of the left menu.
        /// </summary>
        [AllowNull]
        [MaybeNull]
        public int? MenuWidth
        {
            get => this.GetValue(MenuWidthProperty) as int?;
            set => this.SetValue(MenuWidthProperty, value);
        }


        public static readonly DependencyProperty HamburgerProperty = DependencyProperty.Register(
            nameof(Hamburger),
            typeof(FrameworkElement),
            typeof(MasterDetail),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHamburgerChanged)));

        /// <summary>
        /// Gets or Sets the content of the left menu.
        /// </summary>
        [AllowNull]
        [MaybeNull]
        public FrameworkElement? Hamburger
        {
            get => this.GetValue(HamburgerProperty) as FrameworkElement;
            set => this.SetValue(HamburgerProperty, value);
        }

        public MasterDetail()
        {
            this.InitializeComponent();

            // Because Window is a root element we cannot bind it on custom defined property, so we do first "binding" manually.
            OnShowSideBarChanged(this, new DependencyPropertyChangedEventArgs(ShowSideBarProperty, null, this.ShowSideBar));
            OnMenuChanged(this, new DependencyPropertyChangedEventArgs(MenuProperty, null, this.Menu));
            OnHambugerColorChanged(this, new DependencyPropertyChangedEventArgs(HamburgerColorProperty, null, this.HamburgerColor));
            OnMenuBackgroundChanged(this, new DependencyPropertyChangedEventArgs(MenuBackgroundProperty, null, this.MenuBackground));
            OnContainerBackgroundChanged(this, new DependencyPropertyChangedEventArgs(ContainerBackgroundProperty, null, this.ContainerBackground));
            OnTopActionBarBackgroundChanged(this, new DependencyPropertyChangedEventArgs(TopActionBarBackgroundProperty, null, this.TopActionBarBackground));
            OnTopActionBarTitleChanged(this, new DependencyPropertyChangedEventArgs(TopActionBarTitleProperty, null, this.TopActionBarTitle));
            OnHamburgerChanged(this, new DependencyPropertyChangedEventArgs(HamburgerProperty, null, this.Hamburger));
        }

        private void MoveApplication(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Grid grid && grid.Name.Equals("header", StringComparison.OrdinalIgnoreCase))
            {
                this.DragMove();
            }
        }

        private void CloseApplication(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        internal void FullScreen()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.BringIntoView();
            }
        }

        internal void Minimize()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void FullScreen(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                this.FullScreen();
            }
        }

        private void Minimize(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private static double? size;

        private static void OnShowSideBarChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var page = (MasterDetail)sender;
            var menu = page.sideMenu;

            if (size is null)
            {
                size = menu.Width;
            }

            if ((Visibility)e.NewValue == Visibility.Visible && menu.Width == 0D)
            {
                menu.Width = size.Value;
            }

            if (((Visibility)e.NewValue == Visibility.Hidden || (Visibility)e.NewValue == Visibility.Collapsed) && menu.Width > 0D)
            {
                menu.Width = 0D;
            }
        }

        private static void OnMenuChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FrameworkElement frameworkElement)
            {
                ((MasterDetail)sender).menuContent.Content = frameworkElement;
            }
        }

        private static void OnHamburgerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var masterDetail = (MasterDetail)sender;

            if (e.NewValue is null)
            {
                masterDetail.hamburgerContent.Content = (FrameworkElement)masterDetail.Resources["BedrockDefaultHamburger"];
            }

            if (e.NewValue is FrameworkElement frameworkElement)
            {
                masterDetail.hamburgerContent.Content = frameworkElement;
            }
        }

        private static void OnHambugerColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var masterDetail = (MasterDetail)sender;

            //if (e.NewValue is Brush brush && masterDetail.hamburgerContent.Children[0] is Grid grid && string.Equals(grid.Name, "BedrockDefaultHamburger", StringComparison.OrdinalIgnoreCase))
            //{
            //    foreach (UIElement child in grid.Children)
            //    {
            //        child.SetValue(Rectangle.FillProperty, brush);
            //    }
            //}
        }

        private static void OnMenuBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Brush brush)
            {
                ((MasterDetail)sender).sideMenu.Background = brush;
            }
        }

        private static void OnContainerBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Brush brush)
            {
                ((MasterDetail)sender).mainContainer.Background = brush;
            }
        }

        private static void OnTopActionBarBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Brush brush)
            {
                ((MasterDetail)sender).header.Background = brush;
            }
        }         

        private static void OnTopActionBarTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var masterDetail = (MasterDetail)sender;

            if (masterDetail.actionBarPlaceHolder.Children is { Count: > 1 })
            {
                masterDetail.actionBarPlaceHolder.Children.Clear();
            }

            if (masterDetail.actionBarPlaceHolder.Children is { Count: 0 })
            {
                SetActionBarTextblockTitle(masterDetail, e.NewValue as string);
            }
            else
            {
                if (masterDetail.actionBarPlaceHolder.Children[0] is TextBlock textBlock)
                {
                    textBlock.Text = e.NewValue as string ?? string.Empty;
                }
                else
                {
                    SetActionBarTextblockTitle(masterDetail, e.NewValue as string);
                }
            }
        }

        private static void SetActionBarTextblockTitle(MasterDetail masterDetail, string? title)
        {
            masterDetail.actionBarPlaceHolder.Children.Add(new TextBlock
            {
                Text = title ?? string.Empty,
                Style = (Style)DefaultResourceDictionary["TextH3"],
                VerticalAlignment = VerticalAlignment.Center,
            });
        }

        private static void OnTopActionBarChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FrameworkElement topBar && sender is MasterDetail masterDetail)
            {
                masterDetail.actionBarPlaceHolder.Children.Clear();
                masterDetail.actionBarPlaceHolder.Children.Add(topBar);
            }
        }

        private void Hamburger_StylusDown(object sender, StylusDownEventArgs e) => this.AnimateMenu();

        private void Hamburger_MouseDown(object sender, MouseButtonEventArgs e) => this.AnimateMenu();

        private void Hamburger_TouchDown(object sender, TouchEventArgs e) => this.AnimateMenu();

        private void Hamburger_MouseEnter(object sender, MouseEventArgs e) => Mouse.OverrideCursor = Cursors.Hand;

        private void Hamburger_MouseLeave(object sender, MouseEventArgs e) => Mouse.OverrideCursor = Cursors.Arrow;

        private void AnimateMenu()
        {
            this.menuExpanded = !this.menuExpanded;

            if (this.menuExpanded)
            {
                this.sideMenu.BeginStoryboard(this.MenuExpandStoryBoard ??  (Storyboard)MasterResourceDictionary["ExpandMenuStoryboard"]);
            }
            else
            {
                this.sideMenu.BeginStoryboard(this.MenuFoldStoryBoard ?? (Storyboard)MasterResourceDictionary["FoldMenuStoryboard"]);
            }
        }

        private void mainWindow_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(this.WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0024)
            {
                Native.WmGetMinMaxInfo(hwnd, lParam, (int)this.MinWidth, (int)this.MinHeight);
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
