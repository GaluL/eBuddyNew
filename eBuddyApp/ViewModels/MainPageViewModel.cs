using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.WebUI;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using eBuddyApp.Models;
using eBuddyApp.Services.Azure;
using Microsoft.WindowsAzure.MobileServices;

namespace eBuddyApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        internal string WelcomeText
        {
            get
            {
                string privateName = "";

                if (MobileService.Instance.UserData != null)
                {
                    privateName = MobileService.Instance.UserData.PrivateName;
                }

                return string.Format("Welcome back {0}!", privateName);
            }
        }
        internal ObservableCollection<RunItem> FinishedRuns = new ObservableCollection<RunItem>();
        internal ObservableCollection<ScheduledRunItem> UpcomingRuns;

        public MainPageViewModel()
        {
            FinishedRuns.Add(new RunItem() { Date = DateTime.Now, Distance = 222.22, Speed = 97, Time = TimeSpan.FromSeconds(1000)});
            FinishedRuns.Add(new RunItem() { Date = DateTime.Now, Distance = 34.4, Speed = 57, Time = TimeSpan.FromSeconds(2000) });
        }
    }
}

