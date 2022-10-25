//using System;

//namespace InGroupe.Innovation.Wpf.Bedrock.Bus
//{
//    /// <summary>
//    /// Defines the navigation bus.
//    /// </summary>
//    [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_001")]
//    public interface INavigationEventBus
//    {
//        /// <summary>
//        /// Subscribes to the bus.
//        /// </summary>
//        /// <param name="navigate">The navigation data.</param>
//        /// <returns>The <see cref="SubscriptionToken"/> associated with this subscription.</returns>
//        [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_002")]
//        SubscriptionToken Subscribe(Action<(Type ViewType, Uri ViewUri)> navigate);
//        /// <summary>
//        /// Unsubscribes using the specified subscriber token.
//        /// </summary>
//        /// <param name="subscriberToken">The subscriber token.</param>
//        [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_003")]
//        void Unsubscribe(SubscriptionToken subscriberToken);
//    }  
//}
