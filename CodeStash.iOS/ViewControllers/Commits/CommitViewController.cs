﻿using System;
using System.Linq;
using CodeStash.Core.ViewModels.Commits;
using MonoTouch.Dialog;
using CodeStash.iOS.Views;
using System.Reactive.Linq;
using ReactiveUI;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace CodeStash.iOS.ViewControllers.Commits
{
    public class CommitViewController : ViewModelDialogViewController<CommitViewModel>
    {
        private ImageAndTitleHeaderView _header;

        public override void ViewDidLoad()
        {
            Title = ViewModel.Title;

            base.ViewDidLoad();

            Root = new RootElement(Title) { UnevenRows = true };

            var splitElement1 = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Commit, "-", () => ViewModel.GoToParentCommitCommand.ExecuteIfCan()),
                Button2 = new SplitElement.SplitButton(Images.Branch, "-", () => ViewModel.GoToBranchesCommand.ExecuteIfCan()),
            };
            var splitElement2 = new SplitElement
            {
                Button1 = new SplitElement.SplitButton(Images.Build, "0 Builds", () => ViewModel.GoToBuildStatusCommand.ExecuteIfCan()),
                Button2 = new SplitElement.SplitButton(Images.Comment, "0 Comments", () => ViewModel.GoToCommentsCommand.ExecuteIfCan()),
            };

            Root.Add(new Section() { splitElement1, splitElement2 });

            _header = new ImageAndTitleHeaderView
            {
                EnableSeperator = true,
                SeperatorColor = TableView.SeparatorColor
            };

            _header.Image = Images.LoginUserUnknown;
            _header.Text = ViewModel.RepositorySlug;
            TableView.TableHeaderView = _header;

            ViewModel.WhenAnyValue(x => x.Commit).Where(x => x != null).Subscribe(x =>
            {
                _header.Text = x.Message;
                TableView.TableHeaderView = _header;
                var firstParent = x.Parents.FirstOrDefault();
                if (firstParent != null)
                    splitElement1.Button1.Text = firstParent.DisplayId;
            });

            ViewModel.WhenAnyValue(x => x.BuildStatus).Where(x => x != null && x.Length > 0).Subscribe(x =>
            {
                var first = x.FirstOrDefault();
                if (string.Equals(first.State, "SUCCESSFUL", StringComparison.OrdinalIgnoreCase))
                    splitElement2.Button1.Image = Images.BuildOk;
                else if (string.Equals(first.State, "FAILED", StringComparison.OrdinalIgnoreCase))
                    splitElement2.Button1.Image = Images.Error;
                else
                    splitElement2.Button1.Image = Images.Update;

                splitElement2.Button1.Text = string.Format("{0} Build{1}", x.Length, x.Length == 1 ? string.Empty : "s");
            });

            ViewModel.Branches.Changed.Subscribe(_ =>
            {
                if (ViewModel.Branches.Count > 1)
                    splitElement1.Button2.Text = string.Format("{0} Branches", ViewModel.Branches.Count);
                else
                {
                    var firstBranch = ViewModel.Branches.FirstOrDefault();
                    if (firstBranch != null)
                        splitElement1.Button2.Text = firstBranch.DisplayId;
                }
            });

            ViewModel.Changes.Changed.Subscribe(_ =>
            {
                var sections = new List<Section>();
                sections.Add(new Section() { splitElement1, splitElement2 });
                foreach (var @group in ViewModel.Changes.GroupBy(x => x.Path.Parent))
                {
                    var sec = new Section("/" + @group.Key);
                    foreach (var entry in @group)
                    {
                        var element = new StyledStringElement(entry.Path.Name, () => {});
                        element.Image = Images.File;
                        sec.Add(element);
                    }
                    sections.Add(sec);
                }

                Root.Reset(sections);
            });
        }
    }
}

