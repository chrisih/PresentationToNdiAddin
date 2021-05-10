using System;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace EvKgHuelben.Helpers.Interop
{
  public static class NativeHelpers
  {
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr windowHandle, ref Rect rectangle);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromRect([In] ref Rect rectPointer, uint flags);

    public static IntPtr GetMonitorForWindow(IntPtr window)
    {
      var rect = new Rect();
      GetWindowRect(window, ref rect);
      return MonitorFromRect(ref rect, 0x00000002);
    }
  }
}
