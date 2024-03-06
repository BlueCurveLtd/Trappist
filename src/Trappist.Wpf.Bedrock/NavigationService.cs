using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Trappist.Wpf.Bedrock.Abstractions;

using Microsoft.Extensions.DependencyInjection;

using Trappist.Wpf.Bedrock.Navigation;
using Trappist.Wpf.Bedrock.Messaging;

namespace Trappist.Wpf.Bedrock;

public sealed class NavigationService : INavigation
{
    private readonly NavigationServiceConfiguration navigationServiceConfiguration;
    private readonly IMessenger messenger;
    private readonly IServiceProvider serviceProvider;
    private readonly INavigationObserver? navigationObserver;
    private NavigationWindow? navigationWindow;
    private Frame? navigationFrame;
    private bool modalContainerShown;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="navigationServiceConfiguration">Configuration.</param>
    /// <param name="messenger">The navigation message bus.</param>
    /// <param name="serviceProvider">The service provider.</param>
    internal NavigationService(NavigationServiceConfiguration navigationServiceConfiguration, IMessenger messenger, IServiceProvider serviceProvider)
    {
        this.navigationServiceConfiguration = navigationServiceConfiguration;
        this.messenger = messenger;
        this.serviceProvider = serviceProvider;
        this.navigationObserver = serviceProvider.GetService<INavigationObserver>();
    }

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="viewType">The type of the view.</param>
    /// <param name="canRefreshIfSameView"><c>True</c> to allow refresh of the current view if we re-navigated to itself.</param>
    /// <exception cref="ArgumentNullException">The view type cannot be null. <paramref name="viewType"/></exception>
    public void Navigate([DisallowNull] Type viewType, bool canRefreshIfSameView = false)
        => this.GetType().GetMethod(nameof(this.NavigateInternal), BindingFlags.NonPublic | BindingFlags.Instance)
             ?.MakeGenericMethod(viewType)
             ?.Invoke(this, [canRefreshIfSameView]);

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <typeparam name="TView">The view.</typeparam>
    /// <param name="canRefreshIfSameView"><c>True</c> to allow refresh of the current view if we re-navigated to itself.</param>
    public void Navigate<TView>(bool canRefreshIfSameView = false) where TView : notnull, FrameworkElement
        => this.NavigateInternal<TView>(canRefreshIfSameView);

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="alias">The view alias.</param>
    /// <exception cref="ArgumentException">The view alias cannot be null, empty or white spaces., <paramref name="alias"/></exception>
    /// <param name="canRefreshIfSameView"><c>True</c> to allow refresh of the current view if we re-navigated to itself.</param>
    public void Navigate([DisallowNull] string alias, bool canRefreshIfSameView = false)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentException("The view alias cannot be null, empty or white spaces.", nameof(alias));
        }

        var viewMetaData = NavigationItemContainer.GetView(alias);

        _ = (this.GetType().GetMethod(nameof(this.NavigateInternal), BindingFlags.NonPublic | BindingFlags.Instance)
             ?.MakeGenericMethod(viewMetaData!.View!)
             ?.Invoke(this, new object[] { canRefreshIfSameView }));
    }

    private void NavigateInternal<TView>(bool canRefreshIfSameView) where TView : notnull, FrameworkElement
    {
        if (this.modalContainerShown)
        {
            this.HideModalContainer();
        }

        if (CurrentNavigationState.CurrentView is not null && typeof(TView).Equals(CurrentNavigationState.CurrentView.GetType()))
        {
            if (canRefreshIfSameView)
            {
                this.RefreshCurrentView<TView>();
            }
            else
            {
                this.messenger.Send<TView>();
            }
            return;
        }

        if (!CanNavigate())
        {
            return;
        }

        if (CurrentModalNavigationState.CurrentView is not null)
        {
            ModalViewModelBinder.NavigateFromPreviousView();
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                var viewMetaData = NavigationItemContainer.GetView<TView>();

                if (!viewMetaData.IsModal)
                {
                    //NavigationHistory.Push<TView>();

                    this.HideModalContainer();

                    if (this.navigationServiceConfiguration.NavigationModel == NavigationModels.NavigationWindow)
                    {
                        (this.navigationWindow ??= (NavigationWindow)this.serviceProvider.GetService<Window>()!)!.Source = viewMetaData.Uri;
                    }

                    if (this.navigationServiceConfiguration.NavigationModel == NavigationModels.Frame)
                    {
                        (this.navigationFrame ??= (Frame)this.serviceProvider.GetService<Window>()!.FindName("Bedrock_NavigationFrame")).Navigate(viewMetaData.Uri);
                    }

                    this.Observe<TView>(viewMetaData.Uri, viewMetaData.Alias);
                }
                else
                {
                    this.ShowModalContainer(viewMetaData);
                }
            }
            finally
            {
                this.messenger.Send<TView>();
            }
        }, System.Windows.Threading.DispatcherPriority.Render);
    }

    private void RefreshCurrentView<TView>() where TView : notnull, FrameworkElement
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                var viewMetaData = NavigationItemContainer.GetView<TView>();

                if (this.navigationServiceConfiguration.NavigationModel == NavigationModels.NavigationWindow)
                {
                    (this.navigationWindow ??= (NavigationWindow)this.serviceProvider.GetService<Window>()!)!.Refresh();
                }

                if (this.navigationServiceConfiguration.NavigationModel == NavigationModels.Frame)
                {
                    (this.navigationFrame ??= (Frame)this.serviceProvider.GetService<Window>()!.FindName("Bedrock_NavigationFrame")).Refresh();
                }

                this.Observe<TView>(viewMetaData.Uri, viewMetaData.Alias);
            }
            finally
            {
                this.messenger.Send<TView>();
            }
        }, System.Windows.Threading.DispatcherPriority.Render);
    }

    private static bool CanNavigate()
    {
        if (CurrentNavigationState.CurrentViewModel is IConfirmNavigation vmConfirmNavigation)
        {
            return vmConfirmNavigation.CanNavigate(NavigationParameters.Instance);
        }

        if (CurrentNavigationState.CurrentView is IConfirmNavigation viewConfirmNavigation)
        {
            return viewConfirmNavigation.CanNavigate(NavigationParameters.Instance);
        }

        return true;
    }

    private void Observe<TView>(Uri uri, string? alias) where TView : notnull, FrameworkElement
        => this.navigationObserver?.OnNext<TView>(uri, alias);

    private void ShowModalContainer(NavigationItem navigationItem)
    {
        if (!this.modalContainerShown)
        {
            this.modalContainerShown = true;
            this.serviceProvider.GetService<IModalNavigationProvider>()!.Navigate(navigationItem.Uri, navigationItem.IsFullScreenModal, this.serviceProvider.GetService<Window>()!);
        }
    }

    private void HideModalContainer()
    {
        if (this.modalContainerShown)
        {
            this.modalContainerShown = false;
            this.serviceProvider.GetService<IModalNavigationProvider>()!.Close();
        }
    }
}
