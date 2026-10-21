using System;
using UnityEngine;

namespace OpenWaters
{
    public enum DisplayMode { Fullscreen, Borderless, Windowed }

    // Display preferences are separate from run state. The browser owns its fullscreen gesture.
    public static class DisplaySettingsController
    {
        public static bool Supported=>!Application.isEditor &&
            (Application.platform==RuntimePlatform.WindowsPlayer || Application.platform==RuntimePlatform.LinuxPlayer || Application.platform==RuntimePlatform.OSXPlayer);

        public static FullScreenMode ScreenMode(DisplayMode mode)
        {
            switch(mode)
            {
                case DisplayMode.Borderless:return FullScreenMode.FullScreenWindow;
                case DisplayMode.Windowed:return FullScreenMode.Windowed;
                default:return FullScreenMode.ExclusiveFullScreen;
            }
        }

        public static bool Apply(DemoSettings settings)
        {
            if(settings==null)throw new ArgumentNullException(nameof(settings));
            if(!Supported)return false;
            // currentResolution describes the display, rather than a stale saved window size.
            var display=Screen.currentResolution;
            int width=display.width>0?display.width:Math.Max(Screen.width,1280);
            int height=display.height>0?display.height:Math.Max(Screen.height,720);
            if(settings.displayMode==DisplayMode.Windowed)
            {
                // Keep a reachable desktop border for recovery even on smaller displays.
                width=Math.Min(1280,Math.Max(320,Mathf.FloorToInt(width*.9f)));
                height=Math.Min(720,Math.Max(240,Mathf.FloorToInt(height*.9f)));
            }
            Screen.SetResolution(width,height,ScreenMode(settings.displayMode));
            return true;
        }

        public static bool SetMode(DemoSettings settings,DisplayMode mode,bool persist=true)
        {
            if(settings==null)throw new ArgumentNullException(nameof(settings));
            settings.displayMode=Enum.IsDefined(typeof(DisplayMode),mode)?mode:DisplayMode.Fullscreen;
            if(persist)settings.Save();
            return Apply(settings);
        }
    }
}
