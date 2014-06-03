﻿using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeStash.iOS.ViewControllers.Application;
using System.Reactive.Concurrency;
using ReactiveUI;
using Xamarin.Utilities.Core.Services;
using System;
using System.Reactive;
using System.Threading;
using CodeStash.Core.Messages;

namespace CodeStash.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        public override UIWindow Window { get; set; }

        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Theme();

            RxApp.DefaultExceptionHandler = Observer.Create((Exception e) =>
            {
                IoC.Resolve<IAlertDialogService>().Alert("Error", e.Message);
                Console.WriteLine("Exception occured: " + e.Message + " at " + e.StackTrace);
            });

            // Load the IoC
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Xamarin.Utilities.Core.Services.IDefaultValueService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Xamarin.Utilities.Services.DefaultValueService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Core.Services.IApplicationService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(GetType().Assembly);

            var viewModelViewService = IoC.Resolve<IViewModelViewService>();
            viewModelViewService.RegisterViewModels(typeof(Xamarin.Utilities.Services.DefaultValueService).Assembly);
            viewModelViewService.RegisterViewModels(GetType().Assembly);

            var startupViewController = new StartupViewController { ViewModel = IoC.Resolve<CodeStash.Core.ViewModels.Application.StartupViewModel>() };
            startupViewController.ViewModel.View = startupViewController;

            var mainNavigationController = new UINavigationController(startupViewController) { NavigationBarHidden = true };
            MessageBus.Current.Listen<LogoutMessage>().Subscribe(_ => mainNavigationController.PopToRootViewController(false));

            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = mainNavigationController;
            Window.MakeKeyAndVisible();
            return true;
        }

        private void Theme()
        {
            //UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

            UISwitch.Appearance.OnTintColor = UIColor.FromRGB(45,80,148);

            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(45,80,148);
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(18f) });
            UINavigationBar.Appearance.BackIndicatorImage = Images.BackButton;
            UINavigationBar.Appearance.BackIndicatorTransitionMaskImage = Images.BackButton;

            //CodeFramework.iOS.Utils.Hud.BackgroundTint = UIColor.FromRGBA(228, 228, 228, 128);

            //UISegmentedControl.Appearance.TintColor = UIColor.FromRGB(45,80,148);

            UISegmentedControl.AppearanceWhenContainedIn(typeof(UINavigationController)).TintColor = UIColor.White;

            UITableViewHeaderFooterView.Appearance.TintColor = UIColor.FromRGB(228, 228, 228);
            UILabel.AppearanceWhenContainedIn(typeof(UITableViewHeaderFooterView)).TextColor = UIColor.FromRGB(136, 136, 136);
            UILabel.AppearanceWhenContainedIn(typeof(UITableViewHeaderFooterView)).Font = UIFont.SystemFontOfSize(13f);

            //UIToolbar.Appearance.BarTintColor = UIColor.FromRGB(245, 245, 245);

            UIBarButtonItem.AppearanceWhenContainedIn(typeof(UISearchBar)).SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = UIColor.White,
            }, UIControlState.Normal);

//            CodeFramework.iOS.Views.StartupView.TextColor = UIColor.FromWhiteAlpha(0.9f, 1.0f);
//            CodeFramework.iOS.Views.StartupView.SpinnerColor = UIColor.FromWhiteAlpha(0.85f, 1.0f);
//            CodeFramework.iOS.Views.StartupView.StatusBarStyle = UIStatusBarStyle.LightContent;
        }
    }
}

