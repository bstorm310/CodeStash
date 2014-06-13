﻿using System;
using Xamarin.Utilities.Core.ViewModels;
using ReactiveUI;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using AtlassianStashSharp.Helpers;
using System.Reactive.Linq;
using CodeStash.Core.ViewModels.Commits;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestChangesViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public long PullRequestId { get; set; }

        public string PullRequestDestination { get; set; }

        public ReactiveList<Change> Changes { get; private set; }

        public IReactiveCommand GoToDiffCommand { get; private set; }


        public PullRequestChangesViewModel(IApplicationService applicationService)
        {
            Changes = new ReactiveList<Change>();

            LoadCommand.RegisterAsyncTask(async _ =>
            {
                Changes.Reset(await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests[PullRequestId].GetAllChanges().ExecuteAsyncAll());
            });
         
            GoToDiffCommand = new ReactiveCommand();
            GoToDiffCommand.OfType<Change>().Subscribe(x =>
            {
                var vm = CreateViewModel<PullRequestDiffViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.PullRequestId = PullRequestId;
                vm.Path = x.Path.ToString;
                vm.Name = x.Path.Name;
                ShowViewModel(vm);
            });
        }
    }
}

