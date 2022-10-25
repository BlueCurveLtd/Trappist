using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Trappist.Wpf.Bedrock;

/// <summary>
/// Bindable view model locator
/// </summary>
public static class ViewModelLocator
{
    /// <summary>
    /// The automatic wire view model property
    /// </summary>
    public static readonly DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, AutoWireViewModelChanged));

    public static bool GetAutoWireViewModel([DisallowNull] DependencyObject obj)
        => (bool)obj.GetValue(AutoWireViewModelProperty);

    public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        => obj.SetValue(AutoWireViewModelProperty, value);

    private static void AutoWireViewModelChanged([DisallowNull] DependencyObject d, [DisallowNull] DependencyPropertyChangedEventArgs e)
    {
        if (!DesignerProperties.GetIsInDesignMode(d) && (bool)e.NewValue)
        {
            ViewModelLocatorProvider.ResolveViewModel(d, ViewModelBinder.Bind);
        }
    }
}
