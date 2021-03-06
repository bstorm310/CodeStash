﻿using MonoTouch.SlideoutNavigation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using ReactiveUI;
using CodeStash.Core.ViewModels.Application;
using CodeStash.Core.ViewModels.Projects;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class MainViewController : SimpleSlideoutNavigationController, IViewFor<MainViewModel>
    {
        public MainViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            MenuViewController = new UIViewController();
            MenuViewController.View.BackgroundColor = UIColor.White;

            var c = new CustomMenuNavigationController((UIViewController)this.CreateView<ProjectsViewModel>(), this);
            MenuViewController.AddChildViewController(c);
            c.View.Frame = new RectangleF(0, 0, MenuViewController.View.Bounds.Width, MenuViewController.View.Bounds.Height - 44f);
            c.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            MenuViewController.View.Add(c.View);

            var toolbar = new MenuToolbar(); 
            toolbar.AccountsButton.TouchUpInside += (s, e) => ViewModel.GoToAccountsCommand.ExecuteIfCan();
            toolbar.SettingsButton.TouchUpInside += (s, e) => ViewModel.GoToSettingsCommand.ExecuteIfCan();
            toolbar.Frame = new RectangleF(0, MenuViewController.View.Bounds.Height - 44f, MenuViewController.View.Bounds.Width, 44f);
            toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
            MenuViewController.View.Add(toolbar);
        }

        private class CustomMenuNavigationController : UINavigationController
        {
            private readonly SlideoutNavigationController _slideoutNavigationController;

            /// <summary>
            /// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MenuNavigationController"/> class.
            /// </summary>
            /// <param name="rootViewController">Root view controller.</param>
            /// <param name="slideoutNavigationController">Slideout navigation controller.</param>
            public CustomMenuNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
                : base(rootViewController)
            {
                _slideoutNavigationController = slideoutNavigationController;
            }

            public override void PresentViewController(UIViewController viewControllerToPresent, bool animated, NSAction completionHandler)
            {
                var ctrl = new MainNavigationController(viewControllerToPresent, _slideoutNavigationController);
                _slideoutNavigationController.SetMainViewController(ctrl, animated);
            }
        }

        private class MenuToolbar : UIToolbar
        {
            public readonly UIButton AccountsButton;
            public readonly UIButton SettingsButton;

            public MenuToolbar()
            {
                AccountsButton = new UIButton(UIButtonType.Custom);
                AccountsButton.Layer.CornerRadius = 4f;
                AccountsButton.Layer.MasksToBounds = true;
                AccountsButton.BackgroundColor = UIColor.FromWhiteAlpha(0, 0.125f);
                AccountsButton.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
                AccountsButton.Font = UIFont.FromName("HelveticaNeue", 14f);
                AccountsButton.SetTitle("Accounts", UIControlState.Normal);
                Add(AccountsButton);

                SettingsButton = new UIButton(UIButtonType.Custom);
                SettingsButton.Layer.CornerRadius = 4f;
                SettingsButton.Layer.MasksToBounds = true;
                SettingsButton.BackgroundColor = UIColor.FromWhiteAlpha(0, 0.125f);
                SettingsButton.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);
                SettingsButton.Font = UIFont.FromName("HelveticaNeue", 14f);
                SettingsButton.SetTitle("Settings", UIControlState.Normal);
                Add(SettingsButton);
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                AccountsButton.Frame = new RectangleF(10, 5, Bounds.Width / 2 - 20f, Bounds.Height - 10f);
                SettingsButton.Frame = new RectangleF(AccountsButton.Frame.Right + 10f, 5, Bounds.Width - AccountsButton.Frame.Right - 20f, Bounds.Height - 10);
            }

        }

        /// <summary>
        /// A custom navigation controller specifically for iOS6 that locks the orientations to what the StartupControler's is.
        /// </summary>
        protected class CustomNavigationController : UINavigationController
        {
            readonly UIViewController _parent;
            public CustomNavigationController(UIViewController parent, UIViewController root) : base(root) 
            { 
                _parent = parent;
            }

            public override bool ShouldAutorotate()
            {
                return _parent.ShouldAutorotate();
            }

            public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
            {
                return _parent.GetSupportedInterfaceOrientations();
            }
        }
    }
}

