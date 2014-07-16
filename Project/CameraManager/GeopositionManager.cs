﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Devices.Geolocation;

namespace Kazyx.WPMMM.CameraManager
{
    class GeopositionManager
    {

        private static GeopositionManager _GeopositionManager = new GeopositionManager();

        
        internal Geoposition LatestPosition { get; set; }
        internal Action<GeopositionEventArgs> GeopositionUpdated;

        private Geolocator geolocator;
        private DispatcherTimer Timer;
        private const int AcquiringInterval = 5; // min.
        private const int MaximumAge = 15; // min.
        private const int Timeout = 20; // sec.

        private bool _Enable;
        internal bool Enable
        {
            get { return _Enable; }
            set
            {
                _Enable = value;
                if (value)
                {
                    UpdateGeoposition(new object(), new EventArgs());
                    Timer.Interval = TimeSpan.FromMinutes(AcquiringInterval);
                    Timer.Tick += new EventHandler(UpdateGeoposition);
                    Timer.Start();
                }
                else
                {
                    LatestPosition = null;
                    Timer.Stop();
                }
            }
        }

        private async void UpdateGeoposition(object sender, EventArgs e)
        {
            await _UpdateGeoposition();
        }

        private async Task _UpdateGeoposition()
        {
            Debug.WriteLine("Starting to acquire geo location");

            LatestPosition = await geolocator.GetGeopositionAsync(
                TimeSpan.FromMinutes(MaximumAge),
                TimeSpan.FromSeconds(Timeout)
                );
            if (GeopositionUpdated != null)
            {
                GeopositionUpdated(new GeopositionEventArgs() { UpdatedPosition = LatestPosition });
            }
        }

        internal async Task<Geoposition> AcquireGeoPosition()
        {
            if (LatestPosition != null)
            {
                return LatestPosition;
            }
            await _UpdateGeoposition();
            return LatestPosition;
        }

        private GeopositionManager()
        {
            geolocator = new Geolocator();
            Timer = new DispatcherTimer();
            geolocator.DesiredAccuracy = PositionAccuracy.Default;
        }

        public static GeopositionManager GetInstance()
        {
            return _GeopositionManager;
        }
    }

    internal class GeopositionEventArgs : EventArgs
    {
        public Geoposition UpdatedPosition { get; set; }
    }
}
