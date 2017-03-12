using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using eBuddyApp.Models;
using eBuddyApp.Services;
using eBuddyApp.Services.Azure;
using eBuddyApp.Services.Location;
using Template10.Utils;

namespace eBuddy
{
    internal class ScoreRunManager
    {
        private const int CHILL_TIME = 20;
        private const int PRE_RUN_TIME = 10;
        private const int WARM_UP_DISTANCE = 1600;
        private const int INTENSE_DISTANCE = 1200;
           
        private static ScoreRunManager _Instance;
        public static ScoreRunManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ScoreRunManager();
                }

                return _Instance;
            }
        }

        private IList<Geopoint> _Waypoints;

        private ManualResetEvent _DataUpdateSyncEvent;

        internal event EventHandler<MapRoute> OnRouteUpdate;

        private ScoreRunItem _RunData;
        internal ScoreRunItem RunData
        {
            get { return _RunData; }
            private set { _RunData = value; }
        }

        private TimeSpan _TimeBeforeIntense;
        private TimeSpan _TimeBeforePreRun;
        private double _DistanceBeforeWarmUp;
        private int _MinHeartrate = int.MaxValue;
        private int _MaxHeartrate = 0;
        private double _DistanceBeforeIntense;
        private List<int> _HeartratePreRunList;

        private ScoreRunManager()
        {
            _HeartratePreRunList = new List<int>();
        }

        public void Start()
        { 
            LocationService.Instance.OnLocationChange += Instance_OnLocationChange;

            RunData.Date = DateTime.Now;
            RunData.RunPhase = ScoreRunItem.ERunPhase.Chill;
            _Waypoints = new List<Geopoint>();

            LocationService.Instance.Start();

            BandService.Instance.OnHeartRateChange += Instance_OnHeartRateChange;
        }

        private void Instance_OnHeartRateChange(object sender, int e)
        {
            if (RunData.RunPhase == ScoreRunItem.ERunPhase.Chill && e < _MinHeartrate)
            {
                _MinHeartrate = e;
            }
            else if (RunData.RunPhase == ScoreRunItem.ERunPhase.Intense && e > _MaxHeartrate)
            {
                _MaxHeartrate = e;
            }
            else if (RunData.RunPhase == ScoreRunItem.ERunPhase.PreRun)
            {
                _HeartratePreRunList.Add(e);
            }
        }

        public void Stop()
        {
            BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
            LocationService.Instance.Stop();
            _RunData.RunPhase = ScoreRunItem.ERunPhase.NotStarted;
        }

        protected async void Instance_OnLocationChange(Geoposition obj)
        {
            _DataUpdateSyncEvent.Reset();

            await UpdateRunStats(obj);

            _DataUpdateSyncEvent.Set();
        }

        private async Task UpdateRunStats(Geoposition obj)
        {
            _Waypoints.Add(obj.ToGeoPoint());

            var route = await MapServiceWrapper.Instance.GetRoute(_Waypoints);

            if (route != null)
            {
                OnRouteUpdate?.Invoke(this, route);

                RunData.Time = DateTime.Now - RunData.Date;
                RunData.Distance = route.LengthInMeters;
                RunData.Speed = (RunData.Distance / 1000) / (RunData.Time.Seconds / 60.0 / 60.0);
            }
        }

        private void UpdateTestPhase()
        {
            switch (RunData.RunPhase)
            {
                case ScoreRunItem.ERunPhase.Chill:
                {
                    if (RunData.Time.Seconds >= CHILL_TIME)
                    {
                        _DistanceBeforeWarmUp = RunData.Distance;
                        RunData.RunPhase++;
                    }

                    break;
                }
                case ScoreRunItem.ERunPhase.WarmUp:
                    {
                        if (RunData.Distance >= _DistanceBeforeWarmUp + WARM_UP_DISTANCE)
                        {
                            _TimeBeforePreRun = RunData.Time;
                            RunData.RunPhase++;
                        }

                        break;
                    }
                case ScoreRunItem.ERunPhase.PreRun:
                    {
                        if (RunData.Time.Seconds >= _TimeBeforePreRun.Seconds + PRE_RUN_TIME)
                        {
                            RunData.RunPhase++;
                            _TimeBeforeIntense = RunData.Time;
                            _DistanceBeforeIntense = RunData.Distance;
                            RunData.Distance = 0;
                            RunData.Time = TimeSpan.Zero;
                        }

                        break;
                    }
                case ScoreRunItem.ERunPhase.Intense:
                    {
                        if (RunData.Distance >= INTENSE_DISTANCE)
                        {

                            BandService.Instance.OnHeartRateChange -= Instance_OnHeartRateChange;
                            RunData.RunPhase++;

                            LocationService.Instance.Stop();

                            CalculateScore();
                        }

                        break;
                    }
            }
        }

        private void CalculateScore()
        {
            double vo2max_hr_based = 15.3 * (_MaxHeartrate / _MinHeartrate);
            double avg_hr_prerun = _HeartratePreRunList.Average();
            double vo2max_measures_based = 132.853 - (0.0769 * 2.204 * MobileService.Instance.UserData.Weight) -
                                           (0.387 * MobileService.Instance.UserData.Weight) +
                                           (6.315 * (MobileService.Instance.UserData.Gender.Value ? 1 : 0) -
                                           (3.2649 * (_TimeBeforeIntense.Seconds - _TimeBeforePreRun.Seconds) / 60.0) -
                                           0.1565 * avg_hr_prerun);

            double vo2max_avg = (vo2max_measures_based + vo2max_hr_based) / 2;
            double mas_vo2max_based = vo2max_avg / 3.5;
            double mas_run_based;

            if (MobileService.Instance.UserData.Weight > 100)
            {
                mas_run_based = INTENSE_DISTANCE / (double)(RunData.Time.Seconds - 29);
            }
            else
            {
                mas_run_based = INTENSE_DISTANCE / (RunData.Time.Seconds - 20.3);
            }

            double mas_avg = (mas_run_based + mas_vo2max_based) / 2;

            double score_by_mas = mas_avg / 6.22 * 100;
            double score_by_vo2max = vo2max_avg / 70.0 * 100;

            double score_avg = (score_by_mas + score_by_vo2max) / 2;

            RunData.Score = score_avg;
        }
    }
}
