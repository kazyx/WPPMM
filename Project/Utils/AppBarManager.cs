using Kazyx.WPPMM.Resources;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;

namespace Kazyx.WPPMM.Utils
{
    public class AppBarManager
    {
        readonly ApplicationBarMenuItem PostViewMenuItem = new ApplicationBarMenuItem(AppResources.Setting_PostViewImageSize);

        readonly ApplicationBarIconButton WifiMenuItem = new ApplicationBarIconButton
        {
            Text = AppResources.WifiSettingLauncherButtonText,
            IconUri = new Uri("/Assets/AppBar/appBar_wifi.png", UriKind.Relative)
        };

        readonly ApplicationBarMenuItem AboutMenuItem = new ApplicationBarMenuItem
        {
            Text = AppResources.About,
        };

#if DEBUG
        readonly ApplicationBarMenuItem LogMenuItem = new ApplicationBarMenuItem
        {
            Text = "Log Viewer",
        };
        readonly ApplicationBarMenuItem ContentsMenuItem = new ApplicationBarMenuItem
        {
            Text = "Short cut to contents",
        };
#endif

        readonly ApplicationBarIconButton ControlPanelItem = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_ControlPanel,
            IconUri = new Uri("/Assets/AppBar/appbar_cameraSetting.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton AppSettingItem = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_AppSetting,
            IconUri = new Uri("/Assets/AppBar/feature.settings.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton TouchAfCancelItem = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_CancelTouchAf,
            IconUri = new Uri("/Assets/AppBar/appBar_cancel.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton CameraRollItem = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_CameraRoll,
            IconUri = new Uri("/Assets/AppBar/appBar_playback.png", UriKind.Relative),
        };

        readonly ApplicationBarIconButton HiddenMenuItem = new ApplicationBarIconButton
        {
            Text = AppResources.Donation,
            IconUri = new Uri("/Assets/AppBar/appBar_Dollar.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton OkItem = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_Exit,
            IconUri = new Uri("/Assets/AppBar/appBar_ok.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton Refresh = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_Refresh,
            IconUri = new Uri("/Assets/AppBar/sync.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton DownloadMultiple = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_Download,
            IconUri = new Uri("/Assets/AppBar/appBar_download.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton DeleteMultiple = new ApplicationBarIconButton
        {
            Text = AppResources.AppBar_Delete,
            IconUri = new Uri("/Assets/AppBar/appBar_delete.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton ShowDetailInfo = new ApplicationBarIconButton
        {
            Text = AppResources.ShowDetailInfo,
            IconUri = new Uri("/Assets/AppBar/appBar_display_info.png", UriKind.Relative)
        };

        readonly ApplicationBarIconButton HideDetailInfo = new ApplicationBarIconButton
        {
            Text = AppResources.HideDetailInfo,
            IconUri = new Uri("/Assets/AppBar/appBar_close_display.png", UriKind.Relative)
        };


        readonly Dictionary<Menu, ApplicationBarMenuItem> MenuItems = new Dictionary<Menu, ApplicationBarMenuItem>();
        readonly Dictionary<IconMenu, ApplicationBarIconButton> IconMenuItems = new Dictionary<IconMenu, ApplicationBarIconButton>();

        readonly SortedSet<Menu> EnabledItems = new SortedSet<Menu>();
        readonly SortedSet<IconMenu> EnabledIconItems = new SortedSet<IconMenu>();

        public AppBarManager()
        {
            IconMenuItems.Add(IconMenu.WiFi, WifiMenuItem);
            MenuItems.Add(Menu.About, AboutMenuItem);
            IconMenuItems.Add(IconMenu.ControlPanel, ControlPanelItem);
            IconMenuItems.Add(IconMenu.ApplicationSetting, AppSettingItem);
            IconMenuItems.Add(IconMenu.TouchAfCancel, TouchAfCancelItem);
            IconMenuItems.Add(IconMenu.CameraRoll, CameraRollItem);
            IconMenuItems.Add(IconMenu.Hidden, HiddenMenuItem);
            IconMenuItems.Add(IconMenu.Ok, OkItem);
            IconMenuItems.Add(IconMenu.Refresh, Refresh);
            IconMenuItems.Add(IconMenu.DeleteMultiple, DeleteMultiple);
            IconMenuItems.Add(IconMenu.DownloadMultiple, DownloadMultiple);
            IconMenuItems.Add(IconMenu.ShowDetailInfo, ShowDetailInfo);
            IconMenuItems.Add(IconMenu.HideDetailInfo, HideDetailInfo);
#if DEBUG
            MenuItems.Add(Menu.Log, LogMenuItem);
            MenuItems.Add(Menu.Contents, ContentsMenuItem);
#endif
        }

        public AppBarManager SetEvent(Menu type, EventHandler handler)
        {
            MenuItems[type].Click += handler;
            return this;
        }

        public AppBarManager SetEvent(IconMenu type, EventHandler handler)
        {
            IconMenuItems[type].Click += handler;
            return this;
        }

        public AppBarManager Clear()
        {
            EnabledItems.Clear();
            EnabledIconItems.Clear();
            return this;
        }

        public bool IsEnabled(Menu type)
        {
            return EnabledItems.Contains(type);
        }

        public AppBarManager Enable(Menu type)
        {
            if (!EnabledItems.Contains(type))
            {
                EnabledItems.Add(type);
            }
            return this;
        }

        public AppBarManager Disable(Menu type)
        {
            if (EnabledItems.Contains(type))
            {
                EnabledItems.Remove(type);
            }
            return this;
        }

        public bool IsEnabled(IconMenu type)
        {
            return EnabledIconItems.Contains(type);
        }

        public AppBarManager Enable(IconMenu type)
        {
            if (!EnabledIconItems.Contains(type))
            {
                EnabledIconItems.Add(type);
            }
            return this;
        }

        public AppBarManager Disable(IconMenu type)
        {
            if (EnabledIconItems.Contains(type))
            {
                EnabledIconItems.Remove(type);
            }
            return this;
        }

        public IApplicationBar CreateNew(double opacity)
        {
            var bar = new ApplicationBar();

            var IconItems = new SortedSet<IconMenu>(EnabledIconItems);

            var Items = new SortedSet<Menu>(EnabledItems);

            if (IconItems.Count == 0)
                if (Items.Count == 0)
                    return bar;
                else
                    bar.Mode = ApplicationBarMode.Minimized;
            else
                bar.Mode = ApplicationBarMode.Default;

            bar.Opacity = opacity;

            foreach (var menu in Items)
            {
                bar.MenuItems.Add(MenuItems[menu]);
            }
            foreach (var menu in EnabledIconItems)
            {
                bar.Buttons.Add(IconMenuItems[menu]);
            }
            return bar;
        }
    }

    public enum Menu
    {
        About,
#if DEBUG
        Log,
        Contents,
#endif
    }

    public enum IconMenu
    {
        WiFi,
        ControlPanel,
        ApplicationSetting,
        TouchAfCancel,
        CameraRoll,
        Hidden,
        Ok,
        Refresh,
        DeleteMultiple,
        DownloadMultiple,
        ShowDetailInfo,
        HideDetailInfo,
    }
}
