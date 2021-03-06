﻿using System;
using System.Reactive;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using Xamarin.Utilities.Core.Services;
using CodeFramework.iOS.Views.Application;
using CodeFramework.Core.Messages;
using CodeFramework.Core.ViewModels.Application;

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
            // Load the IoC
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Xamarin.Utilities.Core.Services.IDefaultValueService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Xamarin.Utilities.Services.DefaultValueService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(CodeFramework.Core.Services.IAccountsService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(CodeFramework.iOS.Theme).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(typeof(Core.Services.IApplicationService).Assembly);
            IoC.RegisterAssemblyServicesAsSingletons(GetType().Assembly);

            var viewModelViewService = IoC.Resolve<IViewModelViewService>();
            viewModelViewService.RegisterViewModels(typeof(Xamarin.Utilities.Services.DefaultValueService).Assembly);
            viewModelViewService.RegisterViewModels(typeof(CodeFramework.iOS.Theme).Assembly);
            viewModelViewService.RegisterViewModels(GetType().Assembly);

            // Stamp the date this was installed
            var stampedData = this.StampInstallDate("CodeStash", "codestash_install_date");
            Console.WriteLine("Install Stamp = " + stampedData);

            // Need it to startup!
            CodeStash.Core.Bootstrap.Init();

            Theme();

            var startupViewController = new StartupView { ViewModel = IoC.Resolve<StartupViewModel>() };
            startupViewController.ViewModel.View = startupViewController;

            var mainNavigationController = new UINavigationController(startupViewController) { NavigationBarHidden = true };
            MessageBus.Current.Listen<LogoutMessage>().Subscribe(_ => 
            {
                mainNavigationController.PopToRootViewController(false);
                mainNavigationController.DismissViewController(true, null);
            });

            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = mainNavigationController;
            Window.MakeKeyAndVisible();
            return true;
        }

        private void Theme()
        {
            var primaryColor = UIColor.FromRGB(45, 80, 148);

            UIGraphics.BeginImageContext(new System.Drawing.SizeF(1, 64f));
            primaryColor.SetFill();
            UIGraphics.RectFill(new System.Drawing.RectangleF(0, 0, 1, 64));
            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            UIToolbar.Appearance.BackgroundColor = UIColor.White;

            UITabBar.Appearance.TintColor = primaryColor;

            Xamarin.Utilities.ViewControllers.ViewModelPrettyDialogViewController.RefreshIndicatorColor = UIColor.White;

            UIApplication.SharedApplication.SetStatusBarHidden(false, UIStatusBarAnimation.Fade);

            //UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

            UISwitch.Appearance.OnTintColor = primaryColor;

            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.BarTintColor = primaryColor;
            UINavigationBar.Appearance.BackgroundColor = primaryColor;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(18f) });
            UINavigationBar.Appearance.SetBackgroundImage(img, UIBarPosition.Any, UIBarMetrics.Default);
            UINavigationBar.Appearance.BackIndicatorImage = CodeFramework.iOS.Images.BackButton;
            UINavigationBar.Appearance.BackIndicatorTransitionMaskImage = CodeFramework.iOS.Images.BackButton;

            UIImageView.AppearanceWhenContainedIn(typeof(UITableViewCell)).TintColor = UIColor.FromRGB(45, 80, 148);

            Xamarin.Utilities.DialogElements.SplitButtonElement.TextColor = UIColor.FromRGB(45, 80, 148);

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

