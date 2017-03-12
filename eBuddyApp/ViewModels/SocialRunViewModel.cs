using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using eBuddy;
using eBuddyApp.Models;
using eBuddyApp.Services.Location;
using GalaSoft.MvvmLight.Command;
using Template10.Mvvm;

namespace eBuddyApp.ViewModels
{
    class SocialRunViewModel : SoloRunViewModel
    {

        private MapRoute _BuddyRoute;
        public MapRoute BuddyRoute { get { return _BuddyRoute; } private set { Set(ref _BuddyRoute, value); } }

        private Geopoint _BuddyLocation;
        public Geopoint BuddyLocation { get { return _BuddyLocation; } private set { Set(ref _BuddyLocation, value); } }

        public override RunItem RunData {
            get { return BuddyRunManager.Instance.RunData; }
        }

        public SocialRunViewModel()
        {
            StartRun = new RelayCommand(() => { BuddyRunManager.Instance.Start(); },
                () => { return (!BuddyRunManager.Instance.InRun /*&& BandService.Instance.IsConnected*/); });

            StopRun = new RelayCommand(() => BuddyRunManager.Instance.Stop(),
                () => { return BuddyRunManager.Instance.InRun; });

            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;

            CurrentLocation = ExtentionMethods.GetDefaultPoint();
            BuddyRunManager.Instance.OnBuddyRouteUpdate += Instance_OnBuddyRouteUpdate;
  
        }

        private void Instance_OnBuddyRouteUpdate(MapRoute e)
        {
            BuddyRoute = e;
        }
    }





}
