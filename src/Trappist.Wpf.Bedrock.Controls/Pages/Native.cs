using System;
using System.Runtime.InteropServices;

namespace Trappist.Wpf.Bedrock.Controls.Pages;

internal static class Native
{

    [DllImport("user32")]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

    [DllImport("user32")]
    internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    public static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam, int minWidth, int minHeight)
    {
        var minMaxInfo = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO))!;

        var monitor = MonitorFromWindow(hwnd, 0x00000002); // 0x00000002 : Adjust the maximized size and position to fit the work area of the correct monitor

        if (monitor != IntPtr.Zero)
        {
            var monitorInfo = new Native.MONITORINFO();

            GetMonitorInfo(monitor, monitorInfo);

            var rcWorkArea = monitorInfo.rcWork;
            var rcMonitorArea = monitorInfo.rcMonitor;

            minMaxInfo.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
            minMaxInfo.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
            minMaxInfo.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
            minMaxInfo.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            minMaxInfo.ptMinTrackSize.x = minWidth;
            minMaxInfo.ptMinTrackSize.y = minHeight;
        }

        Marshal.StructureToPtr(minMaxInfo, lParam, true);
    }


    /// <summary>
    /// POINT aka POINTAPI
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// x coordinate of point.
        /// </summary>
        public int x;
        /// <summary>
        /// y coordinate of point.
        /// </summary>
        public int y;

        /// <summary>
        /// Construct a point of coordinates (x,y).
        /// </summary>
        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };

    /// <summary>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        /// <summary>
        /// </summary>            
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

        /// <summary>
        /// </summary>            
        public RECT rcMonitor = new();

        /// <summary>
        /// </summary>            
        public RECT rcWork = new();

        /// <summary>
        /// </summary>            
        public int dwFlags = 0;
    }


    /// <summary> Win32 </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct RECT
    {
        /// <summary> Win32 </summary>
        public int left;
        /// <summary> Win32 </summary>
        public int top;
        /// <summary> Win32 </summary>
        public int right;
        /// <summary> Win32 </summary>
        public int bottom;

        /// <summary> Win32 </summary>
        public static readonly RECT Empty = new();

        /// <summary> Win32 </summary>
        public int Width => Math.Abs(this.right - this.left);// Abs needed for BIDI OS

        /// <summary> Win32 </summary>
        public int Height => this.bottom - this.top;

        /// <summary> Win32 </summary>
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }


        /// <summary> Win32 </summary>
        public RECT(RECT rcSrc)
        {
            this.left = rcSrc.left;
            this.top = rcSrc.top;
            this.right = rcSrc.right;
            this.bottom = rcSrc.bottom;
        }

        /// <summary> Win32 </summary>
        public bool IsEmpty
        {
            get
            {
                // BUGBUG : On Bidi OS (hebrew arabic) left > right
                return this.left >= this.right || this.top >= this.bottom;
            }
        }

        /// <summary> Return a user friendly representation of this struct </summary>
        public override string ToString()
        {
            if (this == RECT.Empty) 
            {
                return "RECT {Empty}"; 
            }

            return "RECT { left : " + this.left + " / top : " + this.top + " / right : " + this.right + " / bottom : " + this.bottom + " }";
        }

        /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
        public override bool Equals(object? obj) => obj switch
        {
            RECT rect => this == rect,
            _ => false
        };

        /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
        public override int GetHashCode() 
            => HashCode.Combine(this.left.GetHashCode(), this.top.GetHashCode(), this.right.GetHashCode(), this.bottom.GetHashCode());


        /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
        public static bool operator ==(RECT rect1, RECT rect2)
            => (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);

        /// <summary> Determine if 2 RECT are different(deep compare)</summary>
        public static bool operator !=(RECT rect1, RECT rect2)
            => !(rect1 == rect2);
    }
}
