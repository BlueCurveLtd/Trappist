//using System;
//using System.Collections.Generic;

//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;

//namespace Trappist.Wpf.Bedrock.Controls.Routing
//{
//    internal sealed class BreadcrumbRouting
//    {
//        private IBreadcrumbRouting? breadcrumbRouting;

//        private BreadcrumbRouting()
//        {

//        }

//        internal static readonly BreadcrumbRouting Instance = new();

//        internal IBreadcrumbRouting Configure(IBreadcrumbRouting breadcrumbRouting) => this.breadcrumbRouting = breadcrumbRouting;


//        internal IBreadcrumbRouting? GetRouter() => this.breadcrumbRouting;
//    }

//    public interface IBreadcrumbRouting
//    {
//        BreadcrumbRoutingResult GetRouting();
//    }
     

//    public sealed record BreadcrumbRoutingResult(Type? PreviousViewType, string? PreviousViewAlias, bool PreviousViewIsModal, IReadOnlyDictionary<string, object?> NavigationParameters);

//    public static class BreadcrumbExtensions
//    {
//        public static IServiceCollection AddBreadcrumbRouting(this IServiceCollection serviceCollection, Func<IServiceProvider, IBreadcrumbRouting> configure)
//        {
//            serviceCollection.TryAddSingleton(serviceProvider =>  configure(serviceProvider));

//            return serviceCollection;
//        }
//    }
//}
