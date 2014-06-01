﻿using System;
using System.Reactive.Linq;
using MonoTouch.UIKit;
using CodeStash.Core.ViewModels.Application;
using MonoTouch.Dialog.Utilities;
using System.Drawing;
using ReactiveUI;
using Xamarin.Utilities.ViewControllers;

namespace CodeStash.iOS.ViewControllers.Application
{
    public class StartupViewController : ViewModelViewController<StartupViewModel>, IImageUpdated
    {
        const float ImageSize = 128f;

        private UIImageView _imgView;
        private UILabel _statusLabel;
        private UIActivityIndicatorView _activityView;

        public StartupViewController()
        {
            ManualLoad = true;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            _imgView.Frame = new RectangleF(View.Bounds.Width / 2 - ImageSize / 2, View.Bounds.Height / 2 - ImageSize / 2 - 30f, ImageSize, ImageSize);
            _statusLabel.Frame = new RectangleF(0, _imgView.Frame.Bottom + 10f, View.Bounds.Width, 18f);
            _activityView.Center = new PointF(View.Bounds.Width / 2, _statusLabel.Frame.Bottom + 16f + 16F);

            try
            {
                View.BackgroundColor = Xamarin.Utilities.Images.BackgroundHelper.CreateRepeatingBackground();
            }
            catch (Exception)
            {
                View.BackgroundColor = UIColor.FromRGB(0xeb, 0xeb, 0xeb);
            }
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AutosizesSubviews = true;

            _imgView = new UIImageView();
            _imgView.Layer.CornerRadius = ImageSize / 2;
            _imgView.Layer.MasksToBounds = true;
            Add(_imgView);

            _statusLabel = new UILabel
            {
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.FromName("HelveticaNeue", 13f),
                TextColor = UIColor.FromWhiteAlpha(0.34f, 1f)
            };
            Add(_statusLabel);

            _activityView = new UIActivityIndicatorView
            {
                HidesWhenStopped = true,
                Color = UIColor.FromRGB(0.33f, 0.33f, 0.33f)
            };
            Add(_activityView);


            ViewModel.WhenAnyValue(x => x.Account).Where(x => x != null).Subscribe(x =>
            {
                Uri avatarUri;
                if (Uri.TryCreate(x.AvatarUrl, UriKind.Absolute, out avatarUri))
                    UpdatedImage(avatarUri);

                _statusLabel.Text = "Logging in " + x.Username;
                _activityView.Hidden = false;
                _statusLabel.Hidden = false;
            });

            ViewModel.LoadCommand.IsExecuting.Subscribe(x =>
            {
                if (x)
                    _activityView.StartAnimating();
                else
                    _activityView.StopAnimating();
            });

            ViewModel.BecomeActiveWindowCommand.Subscribe(_ => NavigationController.PopToRootViewController(false));
        }

        public void UpdatedImage(Uri uri)
        {
            if (uri == null)
            {
                AssignUnknownUserImage();
            }
            else
            {
                var img = ImageLoader.DefaultRequestImage(uri, this);
                if (img == null)
                {
                    AssignUnknownUserImage();
                }
                else
                {
                    UIView.Transition(_imgView, 0.50f, UIViewAnimationOptions.TransitionCrossDissolve, () => _imgView.Image = img, null);
                }
            }
        }

        private void AssignUnknownUserImage()
        {
            var img = Images.LoginUserUnknown.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            _imgView.Image = img;
            _imgView.TintColor = UIColor.FromWhiteAlpha(0.34f, 1f);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel.LoadCommand.Execute(null);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Fade);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UIApplication.SharedApplication.SetStatusBarHidden(false, UIStatusBarAnimation.Fade);
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.Default;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
            return UIInterfaceOrientationMask.All;
        }

    }
}

