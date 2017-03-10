using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
using Microsoft.WindowsAzure.MobileServices;
using Template10.Common;

namespace eBuddy
{
    class RunManager
    {
        private static RunManager _Instance;
        public static RunManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new RunManager();   
                }

                return _Instance;
            }
        }

        private RunItem _RunData;
        internal RunItem RunData
        {
            get { return _RunData; }
            private set { _RunData = value; }
        }

        public bool InRun = false;

        private IList<Geopoint> _Waypoints;

        private ManualResetEvent _DataUpdateSyncEvent;

        internal event EventHandler<MapRoute> OnRouteUpdate; 

        private RunManager()
        {
            _DataUpdateSyncEvent = new ManualResetEvent(true);

            RunData = new RunItem();
            RunData.FacebookId = MobileService.Instance.UserData.FacebookId;
        }

        internal void Start()
        {
            InRun = true;

            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;

            RunData.Date = DateTime.Now;
            _Waypoints = new List<Geopoint>();

            LocationService.Instance.Start();
        }

        internal void Stop()
        {
            InRun = false;

            LocationService.Instance.Stop();
            LocationService.Instance.OnLocationChange -= Instance_OnLocationChange;

            MobileService.Instance.SaveRunData(RunData);
        }

        private async void Instance_OnLocationChange(Geoposition obj)
        {
            _DataUpdateSyncEvent.Reset();

            _Waypoints.Add(obj.ToGeoPoint());

            var route = await MapServiceWrapper.Instance.GetRoute(_Waypoints);

            if (route != null)
            {
                OnRouteUpdate?.Invoke(this, route);

                RunData.Time = DateTime.Now - RunData.Date;
                RunData.Distance = route.LengthInMeters;
                RunData.Speed = (RunData.Distance / 1000) / (RunData.Time.Seconds / 60.0 / 60.0);

                _DataUpdateSyncEvent.Set();
            }
        }
    }
}
