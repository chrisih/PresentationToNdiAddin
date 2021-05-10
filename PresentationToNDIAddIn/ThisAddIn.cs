using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using EvKgHuelben.Base;

namespace PresentationToNDIAddIn
{
  public partial class ThisAddIn
  {
    private PresentationCapturer _capture;

    private void ThisAddIn_Startup(object sender, System.EventArgs e)
    {
      Application.SlideShowBegin += Application_SlideShowBegin;
      Application.SlideShowOnNext += Application_SlideShowOnNext;
      Application.SlideShowEnd += Application_SlideShowEnd;
    }

    private void Application_SlideShowOnNext(PowerPoint.SlideShowWindow Wn)
    {
    }

    private void Application_SlideShowEnd(PowerPoint.Presentation Pres)
    {
      _capture?.Dispose();
    }

    private void Application_SlideShowBegin(PowerPoint.SlideShowWindow Wn)
    {
      _capture = new PresentationCapturer(Wn);
      _capture.StartCapture();
    }

    private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
    {
      Application.SlideShowBegin -= Application_SlideShowBegin;
      Application.SlideShowEnd -= Application_SlideShowEnd;
    }

    #region Von VSTO generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InternalStartup()
    {
      this.Startup += new System.EventHandler(ThisAddIn_Startup);
      this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
    }

    #endregion
  }
}
