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
    class SoloRunViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand StartRun;
        public RelayCommand StopRun;

        #endregion

        #region Properties

        public bool IsScoreRun { get; set; }

        public RunItem RunData
        {
            get { return RunManager.Instance.RunData; }
        }

        private int _Heartrate;
        public int Heartrate { get { return _Heartrate; } private set { Set(ref _Heartrate, value); } }

        private MapRoute _Route;
        public MapRoute Route { get { return _Route; } private set { Set(ref _Route, value); } }

        private Geopoint _CurrentLocation;
        public Geopoint CurrentLocation { get { return _CurrentLocation; } private set { Set(ref _CurrentLocation, value); } }

        #endregion

        public SoloRunViewModel() : base()
        {
            StartRun = new RelayCommand(() => {
                    RunManager.Instance.Start();
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                }, 
                () => { return (!RunManager.Instance.InRun /*&& BandService.Instance.IsConnected*/); });

            StopRun = new RelayCommand(() =>
                {
                    RunManager.Instance.Stop();  
                    StopRun.RaiseCanExecuteChanged();
                    StartRun.RaiseCanExecuteChanged();
                },
                () => { return RunManager.Instance.InRun; });

            RunManager.Instance.OnRouteUpdate += Instance_OnRouteUpdate;
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;
            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;

            CurrentLocation = ExtentionMethods.GetDefaultPoint();
        }

        private void Instance_OnHeartRateChange(object sender, int e)
        {
            Heartrate = e;
        }

        private void Instance_OnLocationChange(Geoposition obj)
        {
            CurrentLocation = obj.ToGeoPoint();
        }

        private void Instance_OnRouteUpdate(object sender, MapRoute e)
        {
            Route = e;
        }
    }
}
