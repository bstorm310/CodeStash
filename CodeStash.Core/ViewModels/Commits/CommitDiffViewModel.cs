﻿using System;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;
using CodeStash.Core.Services;
using System.Threading.Tasks;
using System.Text;
using AtlassianStashSharp.Models;

namespace CodeStash.Core.ViewModels.Commits
{
    public class CommitDiffViewModel : LoadableViewModel
    {
        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public string Node { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }
  
        private Diff _diff;
        public Diff Diff
        {
            get { return _diff; }
            private set { this.RaiseAndSetIfChanged(ref _diff, value); }
        }

        public CommitDiffViewModel(IApplicationService applicationService)
        {
            LoadCommand.RegisterAsyncTask(async _ =>
            {
                Diff = (await applicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Commits[Node].GetDiff(Path).ExecuteAsync()).Data;
            });
        }
    }
}

