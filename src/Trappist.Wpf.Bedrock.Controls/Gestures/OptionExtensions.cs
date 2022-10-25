using System;
using System.Collections.Generic;
using System.Linq;

namespace Trappist.Wpf.Bedrock.Controls.Gestures;

internal static class OptionExtensions
{
    public static Option<T> ToOption<T>(this T instance)
        => instance is null ? Option.Empty() : Option.Full(instance);

    public static Option<T> FirstOrEmpty<T>(this IEnumerable<T> items)
        => FirstOrEmpty(items, x => true);

    public static Option<T> FirstOrEmpty<T>(this IEnumerable<T> items, Func<T, bool> pred)
    {
        var filtered = items.Where(pred);
        return filtered.Any() ? Option.Full(filtered.First()) : Option.Empty();
    }

    public static Option<T> Flatten<T>(this Option<Option<T>> option)
        => option.SelectMany(x => x);

    public static Option<T> ToOption<T>(this T? instance) where T : struct
        => instance?.ToOption() ?? Option.Empty();

    public static Option<T> AsOption<T>(this object instance) where T : class
        => (instance as T).ToOption();
}
#nullable restore
