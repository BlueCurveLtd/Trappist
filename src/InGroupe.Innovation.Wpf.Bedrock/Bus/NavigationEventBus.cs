//using System;
//using System.Collections.Generic;

//namespace InGroupe.Innovation.Wpf.Bedrock.Bus
//{
//    [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_005")]
//    internal sealed class NavigationEventBus : INavigationEventBus
//    {
//        private Type? currentView = null;
//        private readonly List<Subscriber> subscribers = new();
//        private static readonly Lazy<NavigationEventBus> LazyInitializer = new Lazy<NavigationEventBus>(() => new NavigationEventBus());

//        private NavigationEventBus()
//        {
//        }

//        public static NavigationEventBus Instance => LazyInitializer.Value;

//        [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_006")]
//        internal void Publish<TView>() where TView : class
//        {
//            this.currentView = typeof(TView);
//            this.Notify();
//        }

//        /// <summary>Subscribes to the bus.</summary>
//        /// <param name="navigate">The navigation data.</param>
//        /// <returns>The <see cref="T:InGroupe.Innovation.Wpf.Bedrock.SubscribtionToken" /> associated with this subscription.</returns>

//        [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_002")]
//        public SubscriptionToken Subscribe(Action<(Type ViewType, Uri ViewUri)> navigate)
//        {
//            var subscriber = new Subscriber(navigate, default);
//            this.subscribers.Add(subscriber);
//            return subscriber.ToSubscriberToken();
//        }

//        /// <summary>Unsubscribe using the specified subscriber token.</summary>
//        /// <param name="subscriberToken">The subscriber token.</param>
//        [Obsolete("This api is obsolete use the generic messenger api instead.", DiagnosticId = "NBUS_003")]
//        public void Unsubscribe(SubscriptionToken subscriberToken)
//            => this.subscribers.Remove(new Subscriber(subscriberToken.Id));

//        private void Notify()
//        {
//            foreach (var subscriber in this.subscribers.ToArray())
//            {
//                subscriber.Navigate?.Invoke((this.currentView!, NavigationItemContainer.GetView(this.currentView!).Uri));

//                subscriber.NavigateFiltered?.Invoke(NavigationItemContainer.GetView(this.currentView!).Uri);
//            }
//        }

//        private sealed class Subscriber
//        {
//            public Action<(Type ViewType, Uri ViewUri)>? Navigate { get; } = default;
//            public Action<Uri>? NavigateFiltered { get; } = default;

//            private readonly Guid id;

//            public Subscriber(Action<(Type ViewType, Uri ViewUri)>? navigate, Action<Uri>? navigateFiltered)
//            {
//                this.Navigate = navigate;
//                this.NavigateFiltered = navigateFiltered;
//                this.id = Guid.NewGuid();
//            }

//            public Subscriber(Guid id) => this.id = id;

//            public override bool Equals(object? obj) => obj switch
//            {
//                Subscriber subscriber => unchecked(subscriber.GetHashCode() == this.GetHashCode()),
//                _ => false
//            };

//            public override int GetHashCode() => unchecked(this.id.GetHashCode());

//            public SubscriptionToken ToSubscriberToken() => new(this.id);
//        }
//    }
//}
