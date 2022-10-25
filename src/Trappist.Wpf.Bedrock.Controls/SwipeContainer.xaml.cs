using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Trappist.Wpf.Bedrock.Controls;

/// <summary>
/// Interaction logic for SwiperContainer.xaml
/// </summary>
public partial class SwipeContainer : UserControl
{

    protected Point? swipeStart;

    public static readonly DependencyProperty PagesProperty = DependencyProperty.Register(
        nameof(Pages),
        typeof(string),
        typeof(DateTimePicker),
        new PropertyMetadata(new List<FrameworkElement>()));


    public List<FrameworkElement> Pages
    {
        get => (List<FrameworkElement>)this.GetValue(PagesProperty);
        set => this.SetValue(PagesProperty, value);
    }

    public SwipeContainer() => this.InitializeComponent();

    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) => this.swipeStart = e.GetPosition(this);

    private void UserControl_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var Swipe = e.GetPosition(this);

            //Swipe Left
            if (this.swipeStart is not null && Swipe.X > (this.swipeStart!.Value.X + 200))
            {
                // OR Use Your Logic to switch between pages.
                //MainFrame.Source = new Uri("LeftPage.xaml", UriKind.RelativeOrAbsolute);
            }

            //Swipe Right
            if (this.swipeStart is not null && Swipe.X < (this.swipeStart!.Value.X - 200))
            {
                // OR Use Your Logic to switch between pages.
                //MainFrame.Source = new Uri("RightPage.xaml", UriKind.RelativeOrAbsolute);
            }
        }
        e.Handled = true;
    }
}
