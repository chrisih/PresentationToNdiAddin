using System;
using NewTek.NDI;

namespace EvKgHuelben.Helpers.NDI
{
  public abstract class BufferedFrameBase : IDisposable
  {
    protected static int? _nominator;
    protected static int? _denominator;
    
    static BufferedFrameBase()
    {
      _nominator = PresentationToNDIAddIn.Properties.Settings.Default.FPS_Zaehler;
      _denominator = PresentationToNDIAddIn.Properties.Settings.Default.FPS_Nenner;
      PresentationToNDIAddIn.Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
    }

    private static void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(PresentationToNDIAddIn.Properties.Settings.Default.FPS_Zaehler))
        _nominator = PresentationToNDIAddIn.Properties.Settings.Default.FPS_Zaehler;
      else if (e.PropertyName == nameof(PresentationToNDIAddIn.Properties.Settings.Default.FPS_Nenner))
        _denominator = PresentationToNDIAddIn.Properties.Settings.Default.FPS_Nenner;
    }

    /// <inheritdoc />
    public abstract void Dispose();

    public abstract VideoFrame ToVideoFrame();
  }
}