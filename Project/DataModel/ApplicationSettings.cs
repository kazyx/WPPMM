using Kazyx.RemoteApi;
using Kazyx.RemoteApi.Camera;
using Kazyx.WPPMM.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Kazyx.WPPMM.DataModel
{

    public class ApplicationSettings : INotifyPropertyChanged
    {

        private static ApplicationSettings sSettings = new ApplicationSettings();
        private CameraManager.CameraManager manager;

        internal List<string> GridTypeSettings = new List<string>()
        {
            FramingGridTypes.Off,
            FramingGridTypes.RuleOfThirds,
            FramingGridTypes.Diagonal,
            FramingGridTypes.Square,
            FramingGridTypes.Crosshairs,
            FramingGridTypes.Fibonacci,
            FramingGridTypes.GoldenRatio,
        };

        internal List<string> GridColorSettings = new List<string>()
        {
            FramingGridColor.White,
            FramingGridColor.Black,
            FramingGridColor.Red,
            FramingGridColor.Green,
            FramingGridColor.Blue,
        };

        internal List<string> FibonacciLineOriginSettings = new List<string>()
        {
            FibonacciLineOrigins.UpperLeft,
            FibonacciLineOrigins.UpperRight,
            FibonacciLineOrigins.BottomLeft,
            FibonacciLineOrigins.BottomRight,
        };

        private ApplicationSettings()
        {
            manager = CameraManager.CameraManager.GetInstance();
            IsPostviewTransferEnabled = Preference.IsPostviewTransferEnabled();
            IsIntervalShootingEnabled = Preference.IsIntervalShootingEnabled();
            IntervalTime = Preference.IntervalTime();
            IsShootButtonDisplayed = Preference.IsShootButtonDisplayed();
            IsHistogramDisplayed = Preference.IsHistogramDisplayed();
            GeotagEnabled = Preference.GeotagEnabled();
            GridType = Preference.FramingGridsType() ?? FramingGridTypes.Off;
            GridColor = Preference.FramingGridsColor() ?? FramingGridColor.White;

        }

        public static ApplicationSettings GetInstance()
        {
            return sSettings;
        }

        private bool _IsPostviewTransferEnabled = true;

        public bool IsPostviewTransferEnabled
        {
            set
            {
                if (_IsPostviewTransferEnabled != value)
                {
                    Preference.SetPostviewTransferEnabled(value);
                    _IsPostviewTransferEnabled = value;
                    OnPropertyChanged("IsPostviewTransferEnabled");
                }
            }
            get
            {
                return _IsPostviewTransferEnabled;
            }
        }

        private bool _IsIntervalShootingEnabled = false;

        public bool IsIntervalShootingEnabled
        {
            set
            {
                if (_IsIntervalShootingEnabled != value)
                {
                    Preference.SetIntervalShootingEnabled(value);
                    _IsIntervalShootingEnabled = value;

                    OnPropertyChanged("IsIntervalShootingEnabled");
                    OnPropertyChanged("IntervalTimeVisibility");

                    // exclusion
                    if (value)
                    {
                        if (manager.cameraStatus.IsAvailable("setSelfTimer") && manager.IntervalManager != null)
                        {
                            SetSelfTimerOff();
                        }
                    }
                }
            }
            get
            {
                return _IsIntervalShootingEnabled;
            }
        }

        private async void SetSelfTimerOff()
        {
            try
            {
                await manager.CameraApi.SetSelfTimerAsync(SelfTimerParam.Off);
            }
            catch (RemoteApiException e)
            {
                Debug.WriteLine("Failed to set selftimer off: " + e.code);
            }
        }

        public Visibility IntervalTimeVisibility
        {
            get { return IsIntervalShootingEnabled ? Visibility.Visible : Visibility.Collapsed; }
        }

        private int _IntervalTime = 10;

        public int IntervalTime
        {
            set
            {
                if (_IntervalTime != value)
                {
                    Preference.SetIntervalTime(value);
                    _IntervalTime = value;
                    // Debug.WriteLine("IntervalTime changed: " + value);
                    OnPropertyChanged("IntervalTime");
                }
            }
            get
            {
                return _IntervalTime;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            // Debug.WriteLine("OnProperty changed: " + name);
            if (PropertyChanged != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(name));
                    }
                    catch (COMException)
                    {
                        Debug.WriteLine("Caught COMException: ApplicationSettings");
                    }
                });
            }
        }

        private bool _IsShootButtonDisplayed = true;
        public bool IsShootButtonDisplayed
        {
            set
            {
                if (_IsShootButtonDisplayed != value)
                {
                    Preference.SetShootButtonDisplayed(value);
                    _IsShootButtonDisplayed = value;
                    OnPropertyChanged("ShootButtonVisibility");
                    Debug.WriteLine("ShootbuttonVisibility updated: " + value.ToString());
                }
            }
            get
            {
                return _IsShootButtonDisplayed;
            }
        }

        private bool _IsHistogramDisplayed = true;
        public bool IsHistogramDisplayed
        {
            set
            {
                if (_IsHistogramDisplayed != value)
                {
                    Preference.SetHistogramDisplayed(value);
                    _IsHistogramDisplayed = value;
                    OnPropertyChanged("HistogramVisibility");
                }
            }
            get { return _IsHistogramDisplayed; }
        }

        private bool _GeotagEnabled = false;
        public bool GeotagEnabled
        {
            set
            {
                if (_GeotagEnabled != value)
                {
                    Preference.SetGeotagEnabled(value);
                    _GeotagEnabled = value;
                    OnPropertyChanged("GeotagEnabled");
                    OnPropertyChanged("GeopositionStatusVisibility");
                }
            }
            get { return _GeotagEnabled; }
        }

        private string _GridType = FramingGridTypes.Off;
        public string GridType
        {
            set
            {
                if (_GridType != value)
                {
                    Debug.WriteLine("GridType updated: " + value);
                    Preference.SetFramingGridsType(value);
                    _GridType = value;
                    OnPropertyChanged("GridType");
                }
            }
            get { return _GridType; }
        }

        public int GridTypeIndex
        {
            set
            {
                GridType = GridTypeSettings[value];
            }
            get
            {
                int i = 0;
                foreach (string type in GridTypeSettings)
                {
                    if (GridType.Equals(type))
                    {
                        return i;
                    }
                    i++;
                }
                return 0;
            }
        }

        private string _GridColor = FramingGridColor.White;
        public string GridColor
        {
            set
            {
                if (_GridColor != value)
                {
                    Preference.SetFramingGridsColor(value);
                    _GridColor = value;
                    OnPropertyChanged("GridColor");
                    OnPropertyChanged("GridColorBrush");
                }
            }
            get { return _GridColor; }
        }

        public SolidColorBrush GridColorBrush
        {
            get
            {
                Color color;
                switch (this.GridColor)
                {
                    case FramingGridColor.White:
                        color = Color.FromArgb(200, 200, 200, 200);
                        break;
                    case FramingGridColor.Black:
                        color = Color.FromArgb(200, 50, 50, 50);
                        break;
                    case FramingGridColor.Red:
                        color = Color.FromArgb(200, 250, 30, 30);
                        break;
                    case FramingGridColor.Green:
                        color = Color.FromArgb(200, 30, 250, 30);
                        break;
                    case FramingGridColor.Blue:
                        color = Color.FromArgb(200, 30, 30, 250);
                        break;
                    default:
                        color = Color.FromArgb(200, 200, 200, 200);
                        break;

                }
                return new SolidColorBrush() { Color = color };
            }
        }

        public int GridColorIndex
        {
            set
            {
                GridColor = GridColorSettings[value];
            }
            get
            {
                int i = 0;
                foreach (string color in GridColorSettings)
                {
                    if (GridColor.Equals(color))
                    {
                        return i;
                    }
                    i++;
                }
                return 0;
            }
        }

        private string _FibonacciLineOrigin = FibonacciLineOrigins.UpperLeft;
        public string FibonacciLineOrigin
        {
            get { return _FibonacciLineOrigin; }
            set
            {
                if (value != _FibonacciLineOrigin)
                {
                    this._FibonacciLineOrigin = value;
                    OnPropertyChanged("FibonacciLineOrigin");
                }
            }
        }

        public int FibonacciOriginIndex
        {
            set
            {
                FibonacciLineOrigin = FibonacciLineOriginSettings[value];
            }
            get
            {
                int i = 0;
                foreach (string f in FibonacciLineOriginSettings)
                {
                    if (FibonacciLineOrigin.Equals(f))
                    {
                        return i;
                    }
                    i++;
                }
                return 0;
            }
        }

        public Visibility ShootButtonVisibility
        {
            get
            {
                if (_IsShootButtonDisplayed && !ShootButtonTemporaryCollapsed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        private bool _ShootButtonTemporaryCollapsed = false;
        public bool ShootButtonTemporaryCollapsed
        {
            get { return _ShootButtonTemporaryCollapsed; }
            set
            {
                if (value != _ShootButtonTemporaryCollapsed)
                {
                    _ShootButtonTemporaryCollapsed = value;
                    OnPropertyChanged("ShootButtonVisibility");
                }
            }
        }

        public Visibility HistogramVisibility
        {
            get
            {
                if (_IsHistogramDisplayed)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility GeopositionStatusVisibility
        {
            get
            {
                if (GeotagEnabled) { return Visibility.Visible; }
                else { return Visibility.Collapsed; }
            }
        }

    }

}
