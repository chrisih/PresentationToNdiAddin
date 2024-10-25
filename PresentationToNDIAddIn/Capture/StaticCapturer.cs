using EvKgHuelben.Helpers.NDI;
using Microsoft.Office.Interop.PowerPoint;
using NewTek.NDI;
using System;
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

      _lastIndex = -1;
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
            _currentFrame = new BufferedSlideFrame(Globals.ThisAddIn.Application.ActivePresentation.Slides[_window.View.Slide.SlideIndex]).ToVideoFrame();
            _lastIndex = _window.View.Slide.SlideIndex;
          }

          _sender.Send(_currentFrame);

        }
        catch (ThreadAbortException)
        { break; }
        catch { }

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
  }
}
