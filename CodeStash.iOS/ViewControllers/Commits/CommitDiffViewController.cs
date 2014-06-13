﻿using System;
using Xamarin.Utilities.ViewControllers;
using CodeStash.Core.ViewModels.Commits;
using ReactiveUI;
using MonoTouch.Foundation;
using System.Reactive.Linq;
using Xamarin.Utilities.Core.Services;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitDiffViewController : WebViewController, IViewFor<CommitDiffViewModel>
    {
        public CommitDiffViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CommitDiffViewModel)value; }
        }

        public override void ViewDidLoad()
        {
            Title = ViewModel.Name;

            base.ViewDidLoad();

            ViewModel.WhenAnyValue(x => x.Diff).Where(x => x != null).Subscribe(x =>
            {
                if (x.Diffs == null || x.Diffs.Count == 0)
                {
                    IoC.Resolve<IAlertDialogService>().Alert("Unable to View", "This file cannot be rendered.");
                }
                else
                {
                    var template = new DiffRazorView { Model = x };
                    var page = template.GenerateString ();
                    Web.LoadHtmlString(page, NSBundle.MainBundle.BundleUrl);
                }
            });

            ViewModel.LoadCommand.ExecuteIfCan();
        }
    }
}

