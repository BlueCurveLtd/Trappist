using System;
using System.Collections;
using System.Collections.Generic;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Gestures
{
#nullable disable
    /// <summary>
    /// Option type, represents encapsulation of an optional value (so we don't need to use a nasty nullable... I really wish c# had this type)
    /// Thanks to <see href="https://github.com/dtchepak">David Tchepak</see> for implementing the <see href="https://github.com/dtchepak/optiontype/blob/master/Option.cs">Option</see> type for me.
    /// </summary>
    /// <typeparam name="T">Instance or reference to the wrapped by the option type.</typeparam>
    internal struct GestureOption<T> : IEnumerable<T>, IEquatable<GestureOption<T>>
    {
        private readonly bool hasValue;
        private readonly T value;

        public static GestureOption<T> Empty() => new GestureOption<T>(false, default(T));

        public static GestureOption<T> Full(T value) => new GestureOption<T>(true, value);

        private GestureOption(bool hasValue, T value)
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

        public GestureOption<TResult> Select<TResult>(Func<T, TResult> f) => this.Fold(x => GestureOption.Full(f(x)), GestureOption.Empty());

        public GestureOption<TResult> Map<TResult>(Func<T, TResult> f) => this.Select(f);

        public GestureOption<T> Where(Func<T, bool> pred) => this.hasValue && pred(this.value) ? this : GestureOption.Empty();

        public GestureOption<TResult> SelectMany<TResult>(Func<T, GestureOption<TResult>> f) => this.SelectMany(f, (x, y) => y);

        public GestureOption<TResult> SelectMany<TK, TResult>(Func<T, GestureOption<TK>> f, Func<T, TK, TResult> selector) => this.Fold(val => f(val).Fold(next => GestureOption.Full(selector(val, next)), GestureOption.Empty()), GestureOption.Empty());

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

        public GestureOption<T> OrElse(GestureOption<T> other) => this.hasValue ? this : other;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static implicit operator GestureOption<T>(GestureOption option) => Empty();

        public IEnumerator<T> GetEnumerator()
        {
            if (this.hasValue)
            {
                yield return this.value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Equals(GestureOption<T> other)
        {
            return this.hasValue.Equals(other.hasValue) && EqualityComparer<T>.Default.Equals(this.value, other.value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is GestureOption && !this.hasValue)
            {
                return true;
            }

            return obj is GestureOption<T> option && this.Equals(option);
        }

        //return (this._hasValue.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(this._value);
        public override int GetHashCode() => unchecked(HashCode.Combine(this.hasValue.GetHashCode(), EqualityComparer<T>.Default.GetHashCode(this.value!)));

        public override string ToString() => this.IsEmpty() ? "Option.Empty" : this.value!.ToString();

        public static bool operator ==(GestureOption<T> left, GestureOption<T> right) => left.Equals(right);

        public static bool operator !=(GestureOption<T> left, GestureOption<T> right) => !left.Equals(right);
    }

    internal class GestureOption : IEquatable<GestureOption>
    {
        private static readonly GestureOption empty = new GestureOption();

        private GestureOption() { }

        public static GestureOption<T> Full<T>(T value) => GestureOption<T>.Full(value);

        public static GestureOption Empty() => empty;

        public bool Equals(GestureOption other) => true;

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

            if (obj is GestureOption)
            {
                return true;
            }

            if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(GestureOption<>))
            {
                return obj.Equals(this);
            }

            return false;
        }

        public override int GetHashCode() => 1234;

        public static bool operator ==(GestureOption left, GestureOption right) => Equals(left, right);

        public static bool operator !=(GestureOption left, GestureOption right) => !Equals(left, right);
    }
#nullable restore
}
