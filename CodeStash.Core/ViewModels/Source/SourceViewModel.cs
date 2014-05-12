﻿using System;
using System.Threading.Tasks;
using CodeStash.Core.Services;
using System.Reactive.Linq;
using ReactiveUI;
using AtlassianStashSharp.Models;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.Source
{
    public class SourceViewModel : LoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private int _selectedView;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public ReactiveList<Tag> Tags { get; private set; }

        public ReactiveList<Branch> Branches { get; private set; }

        public IReactiveCommand GoToSourceCommand { get; private set; }

        public int SelectedView
        {
            get { return _selectedView; }
            set { this.RaiseAndSetIfChanged(ref _selectedView, value); }
        }

        public SourceViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            GoToSourceCommand = new ReactiveCommand();
            Tags = new ReactiveList<Tag>();
            Branches = new ReactiveList<Branch>();

            this.WhenAnyValue(x => x.SelectedView).Skip(1).Subscribe(_ => LoadCommand.Execute(null));
        }

        protected override async Task Load()
        {
            if (SelectedView == 0)
            {
                var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Branches.GetAll().ExecuteAsync();
                Branches.Reset(response.Data.Values);
            }
            else
            {
                var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].Tags.GetAll().ExecuteAsync();
                Tags.Reset(response.Data.Values);
            }
        }
    }
}

