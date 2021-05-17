using EvKgHuelben.Helpers.NDI;
using Microsoft.Office.Interop.PowerPoint;
using NewTek.NDI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace PresentationToNDIAddIn
{
  public class StaticCapturer : IDisposable
  {
    private Thread _ndiSender;
    private Sender _sender;
    private SlideShowWindow _window;
    private int _lastIndex;
    private VideoFrame _currentFrame;

    public StaticCapturer()
    {
      _lastIndex = -1;
      Globals.ThisAddIn.Application.PresentationOpen += Application_PresentationOpen;
      Globals.ThisAddIn.Application.SlideShowBegin += Application_SlideShowBegin;
      Globals.ThisAddIn.Application.SlideShowEnd += Application_SlideShowEnd;
    }

    /// <summary>
    /// Create NDI Stuff
    /// </summary>
    /// <param name="Pres"></param>
    private void Application_PresentationOpen(Presentation Pres)
    {
      _sender = new Sender(Environment.MachineName + " - Static (" + Pres.Name + ")");
    }

    private void Application_SlideShowBegin(SlideShowWindow Wn)
    {
      if(Properties.Settings.Default.NDIStatic)
      {
        _window = Wn;
        _ndiSender = new Thread(SendNdi) { Priority = ThreadPriority.Normal, Name = "StaticNdiSenderThread", IsBackground = true };
        _ndiSender.Start();
      }
    }

    private void Application_SlideShowEnd(Presentation Pres)
    {
      if (Properties.Settings.Default.NDIStatic)
      {
        try {
          _ndiSender?.Abort();
        }
        catch { }
      }

      _window = null;
    }

    private void SendNdi()
    {
      while(true)
      {
        try
        {
          if (_window.View.Slide.SlideIndex != _lastIndex)
          {
            _currentFrame?.Dispose();
            _currentFrame = new BufferedFrame(Globals.ThisAddIn.Application.ActivePresentation.Slides[_window.View.Slide.SlideIndex]).ToVideoFrame();
          }

          _sender.Send(_currentFrame);

        }
        catch (ThreadAbortException)
        { break; }

        Thread.Sleep(200);
      }
    }

    public void Dispose()
    {
      try
      {
        _sender.Dispose();
      }
      catch { }

      try
      {
        _ndiSender?.Abort();
      }
      catch { }

      Globals.ThisAddIn.Application.PresentationOpen -= Application_PresentationOpen;
      Globals.ThisAddIn.Application.SlideShowBegin -= Application_SlideShowBegin;
      Globals.ThisAddIn.Application.SlideShowEnd -= Application_SlideShowEnd;
    }

    private Bitmap ToImage(Slide s)
    {
      var setup = (s.Parent as Presentation).PageSetup;
      var image = new Bitmap((int)setup.SlideWidth, (int)setup.SlideHeight);

      using (var g = Graphics.FromImage(image))
      {
        g.Clear(Color.Transparent);
        g.CompositingMode = CompositingMode.SourceOver;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        foreach (Shape shape in s.Shapes)
        {
          var tmpfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
          shape.Export(tmpfile, PpShapeFormat.ppShapeFormatPNG, 0, 0, PpExportMode.ppClipRelativeToSlide);
          using (var i = Image.FromFile(tmpfile))
          {
            var rect = new Rectangle((int)shape.Left, (int)shape.Top, (int)shape.Width + 16, (int)shape.Height + 4);
            using (var wrapMode = new ImageAttributes())
            {
              wrapMode.SetWrapMode(WrapMode.TileFlipXY);
              g.DrawImage(i, rect, 16, 4, i.Width, i.Height, GraphicsUnit.Pixel, wrapMode);
            }
          }
          try
          {
            File.Delete(tmpfile);
          }
          catch { }
        }
        g.Flush();
      }

      return image;
    }
  }
}
