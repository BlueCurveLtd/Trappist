using System;
using System.Collections;
using System.Collections.Generic;

namespace Trappist.Wpf.Bedrock.Controls.Gestures;

#nullable disable
/// <summary>
/// Option type, represents encapsulation of an optional value (so we don't need to use a nasty nullable... I really wish c# had this type)
/// Thanks to <see href="https://github.com/dtchepak">David Tchepak</see> for implementing the <see href="https://github.com/dtchepak/optiontype/blob/master/Option.cs">Option</see> type for me.
/// </summary>
/// <typeparam name="T">Instance or reference to the wrapped by the option type.</typeparam>
internal struct Option<T> : IEnumerable<T>, IEquatable<Option<T>>
{
    private readonly bool hasValue;
    private readonly T value;

    public static Option<T> Empty() => new Option<T>(false, default(T));

    public static Option<T> Full(T value) => new Option<T>(true, value);

    private Option(bool hasValue, T value)
    {
        this.hasValue = hasValue;
        this.value = value;
    }

    public bool HasValue() => this.hasValue;

    public bool IsEmpty() => !this.hasValue;

    public T ValueOr(T other) => this.Fold(x => x, other);

    public T ValueOrDefault() => this.ValueOr(default(T));

    public TResult Fold<TResult>(Func<T, TResult> ifValue, TResult elseValue)
        => this.FoldLazy(ifValue, () => elseValue);

    public TResult FoldLazy<TResult>(Func<T, TResult> ifValue, Func<TResult> elseValue) => this.hasValue ? ifValue(this.value) : elseValue();

    public Option<TResult> Select<TResult>(Func<T, TResult> f) => this.Fold(x => Option.Full(f(x)), Option.Empty());

    public Option<TResult> Map<TResult>(Func<T, TResult> f) => this.Select(f);

    public Option<T> Where(Func<T, bool> pred) => this.hasValue && pred(this.value) ? this : Option.Empty();

    public Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> f) => this.SelectMany(f, (x, y) => y);

    public Option<TResult> SelectMany<TK, TResult>(Func<T, Option<TK>> f, Func<T, TK, TResult> selector) => this.Fold(val => f(val).Fold(next => Option.Full(selector(val, next)), Option.Empty()), Option.Empty());

    public void Do(Action<T> ifValue) => this.DoElse(ifValue, () => { });

    public void DoElse(Action<T> ifValue, Action elseValue)
    {
        if (this.hasValue) 
        { 
            ifValue(this.value); 
        }
        else 
        {
            elseValue();
        }
    }

    public Option<T> OrElse(Option<T> other) => this.hasValue ? this : other;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static implicit operator Option<T>(Option option) => Empty();

    public IEnumerator<T> GetEnumerator()
    {
        if (this.hasValue)
        {
            yield return this.value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public bool Equals(Option<T> other)
    {
        return this.hasValue.Equals(other.hasValue) && EqualityComparer<T>.Default.Equals(this.value, other.value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is Option && !this.hasValue)
        {
            return true;
        }

        return obj is Option<T> option && this.Equals(option);
    }

    //return (this._hasValue.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(this._value);
    public override int GetHashCode() => unchecked(HashCode.Combine(this.hasValue.GetHashCode(), EqualityComparer<T>.Default.GetHashCode(this.value!)));

    public override string ToString() => this.IsEmpty() ? "Option.Empty" : this.value!.ToString();

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
}

internal class Option : IEquatable<Option>
{
    private static readonly Option empty = new Option();

    private Option() { }

    public static Option<T> Full<T>(T value) => Option<T>.Full(value);

    public static Option Empty() => empty;

    public bool Equals(Option other) => true;

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is Option)
        {
            return true;
        }

        if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(Option<>))
        {
            return obj.Equals(this);
        }

        return false;
    }

    public override int GetHashCode() => 1234;

    public static bool operator ==(Option left, Option right) => Equals(left, right);

    public static bool operator !=(Option left, Option right) => !Equals(left, right);
}
#nullable restore
