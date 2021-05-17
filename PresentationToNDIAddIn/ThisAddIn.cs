using EvKgHuelben.Base;
using Microsoft.Office.Tools.Ribbon;

namespace PresentationToNDIAddIn
{
  public partial class ThisAddIn
  {
    private PresentationCapturer _dc;
    private StaticCapturer _sc;

    protected override IRibbonExtension[] CreateRibbonObjects()
    {
      return new IRibbonExtension[] { new Ribbon1() };
    }

    private void ThisAddIn_Startup(object sender, System.EventArgs e)
    {
      _dc = new PresentationCapturer();
      _sc = new StaticCapturer();
    }


    private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
    {
      _dc.Dispose();
      _sc.Dispose();
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
