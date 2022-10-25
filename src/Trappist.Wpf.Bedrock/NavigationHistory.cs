//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;

//using Trappist.Wpf.Bedrock.Abstractions;

//namespace Trappist.Wpf.Bedrock;

//internal static class NavigationHistory
//{
//    private static readonly Stack<Type> Stack = new();

//    [Obsolete("The api has been deprecated.", DiagnosticId = "HST_DEP_002")]
//    [return: MaybeNull]
//    public static Type? Peek()
//    {
//        if (Stack.TryPeek(out var view))
//        {
//            return view;
//        }

//        return default;
//    }

//    [Obsolete("The api has been deprecated.", DiagnosticId = "HST_DEP_003")]
//    [return: MaybeNull]
//    public static Type? Pop()
//    {
//        if (Stack.TryPop(out var view))
//        {
//            return view;
//        }

//        return default;
//    }

//    [Obsolete("The api has been deprecated.", DiagnosticId = "HST_DEP_004")]
//    public static void Push<TView>()
//    {
//        if (!Stack.Contains(typeof(TView)) && !typeof(IModal).IsAssignableFrom(typeof(TView)))
//        {
//            Stack.Push(typeof(TView));                
//        }
//    }

//    [Obsolete("The api has been deprecated.", DiagnosticId = "HST_DEP_005")]
//    public static void Clear() => Stack.Clear();
//}
