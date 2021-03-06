using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AtlassianStashSharp.Models;
using CodeStash.Core.Services;
using ReactiveUI;
using Xamarin.Utilities.Core.ViewModels;

namespace CodeStash.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : BaseViewModel, ILoadableViewModel
    {
        protected readonly IApplicationService ApplicationService;
        private int _selectedView;

        public string ProjectKey { get; set; }

        public string RepositorySlug { get; set; }

        public ReactiveList<PullRequest> PullRequests { get; private set; }

        public IReactiveCommand<object> GoToPullRequestCommand { get; private set; }

        public IReactiveCommand LoadCommand { get; private set; }

        public int SelectedView
        {
            get { return _selectedView; }
            set { this.RaiseAndSetIfChanged(ref _selectedView, value); }
        }

        public PullRequestsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
            PullRequests = new ReactiveList<PullRequest>();

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => Load());

            this.WhenAnyValue(x => x.SelectedView).Skip(1).Subscribe(_ => LoadCommand.ExecuteIfCan());

            GoToPullRequestCommand = ReactiveCommand.Create();
            GoToPullRequestCommand.OfType<PullRequest>().Subscribe(x =>
            {
                var vm = CreateViewModel<PullRequestViewModel>();
                vm.ProjectKey = ProjectKey;
                vm.RepositorySlug = RepositorySlug;
                vm.PullRequestId = x.Id;
                ShowViewModel(vm);
            });
        }

        private async Task Load()
        {
            string state;
            switch (SelectedView)
            {
                case 1:
                    state = "MERGED";
                    break;
                case 2:
                    state = "DECLINED";
                    break;
                default:
                    state = "OPEN";
                    break;
            }

            var response = await ApplicationService.StashClient.Projects[ProjectKey].Repositories[RepositorySlug].PullRequests.GetAll(state: state).ExecuteAsync();
            PullRequests.Reset(response.Data.Values);
        }
    }
}