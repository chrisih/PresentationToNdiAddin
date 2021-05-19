
namespace PresentationToNDIAddIn
{
  partial class Ribbon1 : Microsoft.Office.Tools.Ribbon.RibbonBase
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public Ribbon1()
        : base(Globals.Factory.GetRibbonFactory())
    {
      InitializeComponent();
    }

    /// <summary> 
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">"true", wenn verwaltete Ressourcen gelöscht werden sollen, andernfalls "false".</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Komponenten-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.tabNDI = this.Factory.CreateRibbonTab();
      this.grpNDI = this.Factory.CreateRibbonGroup();
      this.chkEnableNDI = this.Factory.CreateRibbonCheckBox();
      this.chkEnableExport = this.Factory.CreateRibbonCheckBox();
      this.group1 = this.Factory.CreateRibbonGroup();
      this.fps = this.Factory.CreateRibbonEditBox();
      this.fpsd = this.Factory.CreateRibbonEditBox();
      this.group2 = this.Factory.CreateRibbonGroup();
      this.chkHw = this.Factory.CreateRibbonCheckBox();
      this.chkMouse = this.Factory.CreateRibbonCheckBox();
      this.tabNDI.SuspendLayout();
      this.grpNDI.SuspendLayout();
      this.group1.SuspendLayout();
      this.group2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabNDI
      // 
      this.tabNDI.Groups.Add(this.grpNDI);
      this.tabNDI.Groups.Add(this.group1);
      this.tabNDI.Groups.Add(this.group2);
      this.tabNDI.Label = "NDI";
      this.tabNDI.Name = "tabNDI";
      // 
      // grpNDI
      // 
      this.grpNDI.Items.Add(this.chkEnableNDI);
      this.grpNDI.Items.Add(this.chkEnableExport);
      this.grpNDI.Label = "NDI Export";
      this.grpNDI.Name = "grpNDI";
      // 
      // chkEnableNDI
      // 
      this.chkEnableNDI.Checked = true;
      this.chkEnableNDI.Label = "NDI Video (ohne Transparenz)";
      this.chkEnableNDI.Name = "chkEnableNDI";
      // 
      // chkEnableExport
      // 
      this.chkEnableExport.Label = "NDI Folien (mit Transparenz)";
      this.chkEnableExport.Name = "chkEnableExport";
      // 
      // group1
      // 
      this.group1.Items.Add(this.fps);
      this.group1.Items.Add(this.fpsd);
      this.group1.Label = "NDI Stream Settings";
      this.group1.Name = "group1";
      // 
      // fps
      // 
      this.fps.Label = "FPS Zähler";
      this.fps.Name = "fps";
      this.fps.Text = "30000";
      // 
      // fpsd
      // 
      this.fpsd.Label = "FPS Nenner";
      this.fpsd.Name = "fpsd";
      this.fpsd.Text = "1000";
      // 
      // group2
      // 
      this.group2.Items.Add(this.chkHw);
      this.group2.Items.Add(this.chkMouse);
      this.group2.Label = "DirectX Settings";
      this.group2.Name = "group2";
      // 
      // chkHw
      // 
      this.chkHw.Label = "Hardwarebeschleunigung";
      this.chkHw.Name = "chkHw";
      // 
      // chkMouse
      // 
      this.chkMouse.Label = "Maus ausblenden";
      this.chkMouse.Name = "chkMouse";
      // 
      // Ribbon1
      // 
      this.Name = "Ribbon1";
      this.RibbonType = "Microsoft.PowerPoint.Presentation";
      this.Tabs.Add(this.tabNDI);
      this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
      this.tabNDI.ResumeLayout(false);
      this.tabNDI.PerformLayout();
      this.grpNDI.ResumeLayout(false);
      this.grpNDI.PerformLayout();
      this.group1.ResumeLayout(false);
      this.group1.PerformLayout();
      this.group2.ResumeLayout(false);
      this.group2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    internal Microsoft.Office.Tools.Ribbon.RibbonTab tabNDI;
    internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpNDI;
    internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chkEnableNDI;
    internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chkEnableExport;
    internal Microsoft.Office.Tools.Ribbon.RibbonEditBox fps;
    internal Microsoft.Office.Tools.Ribbon.RibbonEditBox fpsd;
    internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
    internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
    internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chkHw;
    internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chkMouse;
  }

  partial class ThisRibbonCollection
  {
    internal Ribbon1 Ribbon1
    {
      get { return this.GetRibbon<Ribbon1>(); }
    }
  }
}
