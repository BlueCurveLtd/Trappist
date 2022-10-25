//using System;
//using System.Diagnostics;

//namespace Trappist.Wpf.Bedrock.Bus
//{
//    /// <summary>
//    /// Represent a subscription. This class cannot be inherited.
//    /// </summary>
//    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
//    [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_004")]
//    public sealed class SubscriptionToken
//    {
//        /// <summary>
//        /// Gets the identifier.
//        /// </summary>
//        /// <value>The identifier.</value>
//        internal Guid Id { get; }


//        /// <summary>
//        /// Initializes a new instance of the <see cref="SubscriptionToken"/> class.
//        /// </summary>
//        /// <param name="id">The identifier.</param>
//        internal SubscriptionToken(Guid id) => this.Id = id;

//        /// <summary>
//        /// Returns a hash code for this instance.
//        /// </summary>
//        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
//        public override int GetHashCode() => unchecked(this.Id.GetHashCode());

//        /// <summary>
//        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
//        /// </summary>
//        /// <param name="obj">The object to compare with the current object.</param>
//        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
//        public override bool Equals(object? obj) => obj switch
//        {
//            SubscriptionToken subscriberToken => unchecked(subscriberToken.GetHashCode() == this.GetHashCode()),
//            _ => false
//        };

//        private string GetDebuggerDisplay() => this.Id.ToString();
//    }
//}
