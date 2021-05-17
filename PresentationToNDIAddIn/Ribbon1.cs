using Microsoft.Office.Tools.Ribbon;
using System.Globalization;

namespace PresentationToNDIAddIn
{
  public partial class Ribbon1
  {
    private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
    {
      chkEnableExport.Checked = Properties.Settings.Default.NDIStatic;
      chkEnableExport.Click += ChkEnableExport_Click;

      chkEnableNDI.Checked = Properties.Settings.Default.NDIDynamic;
      chkEnableNDI.Click += ChkEnableNDI_Click;

      fps.Text = Properties.Settings.Default.FPS_Zaehler.ToString(CultureInfo.InvariantCulture);
      fps.TextChanged += Fps_TextChanged;

      fpsd.Text = Properties.Settings.Default.FPS_Nenner.ToString(CultureInfo.InvariantCulture);
      fpsd.TextChanged += Fpsd_TextChanged;
    }

    private void Fpsd_TextChanged(object sender, RibbonControlEventArgs e)
    {
      var reb = e.Control as RibbonEditBox;
      try
      {
        Properties.Settings.Default.FPS_Nenner = int.Parse(reb.Text, NumberStyles.Integer);
        Properties.Settings.Default.Save();
      }
      catch
      {
      }
    }

    private void Fps_TextChanged(object sender, RibbonControlEventArgs e)
    {
      var reb = e.Control as RibbonEditBox;
      try
      {
        Properties.Settings.Default.FPS_Zaehler = int.Parse(reb.Text, NumberStyles.Integer);
        Properties.Settings.Default.Save();
      }
      catch 
      { 
      }
    }

    private void ChkEnableNDI_Click(object sender, RibbonControlEventArgs e)
    {
      var rcb = e.Control as RibbonCheckBox;
      Properties.Settings.Default.NDIDynamic = rcb.Checked;
      Properties.Settings.Default.Save();
    }

    private void ChkEnableExport_Click(object sender, RibbonControlEventArgs e)
    {
      var rcb = e.Control as RibbonCheckBox;
      Properties.Settings.Default.NDIStatic = rcb.Checked;
      Properties.Settings.Default.Save();
    }
  }
}
