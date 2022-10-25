using System;
using System.Windows.Markup;

namespace Trappist.Wpf.Bedrock.Navigation.Xaml;

[MarkupExtensionReturnType(typeof(NavigationInfo))]
public sealed class NavigationInfo : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider) => NavigationParameters.Instance.AsDictionary();
}
