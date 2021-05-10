using EvKgHuelben.Helpers.WindowsRuntime;
using System;

namespace EvKgHuelben.Base
{
  public class PresentationCapturer : CaptureHelperBase
  {
    private float _xOrig;
    private float _yOrig;

    public PresentationCapturer(Microsoft.Office.Interop.PowerPoint.SlideShowWindow window)
    {
      _xOrig = window.Presentation.SlideMaster.Width;
      _yOrig = window.Presentation.SlideMaster.Height;

      var hwnd = new IntPtr(window.Application.HWND);
      var item = CaptureExtensions.CreateItemForWindow(hwnd);

      Initialize(item);
    }

    private int HeaderHeight => 35;
    private int FooterHeight => 30;

    private float xFact => _lastSize.Width / _xOrig;

    private float yFact => (_lastSize.Height - HeaderHeight - FooterHeight) / _yOrig;

    private float Factor => Math.Min(xFact, yFact);

    private float DesiredWidth => _xOrig * Factor;

    private float DesiredHeight => _yOrig * Factor;

    protected override int CropLeft => (int)(_lastSize.Width - DesiredWidth) / 2;

    protected override int CropRight => (int)(_lastSize.Width - DesiredWidth) / 2;

    protected override int CropBottom => (int)(_lastSize.Height - DesiredHeight) / 2;

    protected override int CropTop => (int)(_lastSize.Height - DesiredHeight) / 2;
  }
}
