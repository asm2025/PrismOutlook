using System;
using System.Runtime.InteropServices;
using essentialMix;

namespace PrismOutlook;

internal static class NativeMethods
{
	public const int WM_GETMINMAXINFO = 0x0024;
	public const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

	public const int S_OK = 0;

	[StructLayout(LayoutKind.Sequential)]
	public struct MONITORINFO
	{
		public int cbSize;
		public RECT rcMonitor;
		public RECT rcWork;
		public uint dwFlags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MINMAXINFO
	{
		public POINT ptReserved;
		public POINT ptMaxSize;
		public POINT ptMaxPosition;
		public POINT ptMinTrackSize;
		public POINT ptMaxTrackSize;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DWMCOLORIZATIONPARAMS
	{
		public uint colorizationColor;
		public uint colorizationAfterglow;
		public uint colorizationColorBalance; // Ranging from 0 to 100
		public uint colorizationAfterglowBalance;
		public uint colorizationBlurBalance;
		public uint colorizationGlassReflectionIntensity;
		public uint colorizationOpaqueBlend;
	}

	[DllImport("user32.dll")]
	public static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

	[DllImport("user32.dll")]
	public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

	[DllImport("Dwmapi.dll")]
	public static extern int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool pfEnabled);

	[DllImport("Dwmapi.dll", EntryPoint = "#127")] // Undocumented API
	public static extern int DwmGetColorizationParameters(out DWMCOLORIZATIONPARAMS parameters);
}