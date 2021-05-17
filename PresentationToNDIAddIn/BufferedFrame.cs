using Microsoft.Office.Interop.PowerPoint;
using NewTek.NDI;
using SharpDX;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics;
using static NewTek.NDIlib;

namespace EvKgHuelben.Helpers.NDI
{
  public class BufferedFrame : IDisposable
  {
    private SizeInt32 _originalSize;
    private IntPtr _bufferPtr;
    private Format _format;
    private DataStream _stream;
    private static int? _nominator;
    private static int? _denominator;
    private Slide _s;

    static BufferedFrame()
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

    public BufferedFrame(Slide s)
    {
      _s = s;
    }

    public BufferedFrame(DataStream s, SizeInt32 size, Format f)
    {
      _stream = s;
      _originalSize = size;
      _format = f;
    }

    public void Dispose()
    {
      Marshal.FreeHGlobal(_bufferPtr);
    }

    private FourCC_type_e FourCC
    {
      get
      {
        switch(_format)
        {
          case Format.R8G8B8A8_UNorm:
            return FourCC_type_e.FourCC_type_RGBA;
          case Format.B8G8R8A8_UNorm:
            return FourCC_type_e.FourCC_type_BGRA;
          case Format.B8G8R8X8_UNorm:
            return FourCC_type_e.FourCC_type_BGRX;
          default:
            return FourCC_type_e.FourCC_type_BGRA;
        }
      }
    }

    public int Stride => (int)(_stream.Length / _format.ComputeScanlineCount(_originalSize.Height));

    public VideoFrame ToVideoFrame()
    {
      if(_s != null)
      {
        var setup = (_s.Parent as Presentation).PageSetup;
        var ret1 = new VideoFrame((int)setup.SlideWidth, (int)setup.SlideHeight, (int)setup.SlideWidth / (int)setup.SlideHeight, _nominator.Value, _denominator.Value, frame_format_type_e.frame_format_type_progressive);

        using (Bitmap image = new Bitmap(ret1.Width, ret1.Height, ret1.Stride, PixelFormat.Format32bppPArgb, ret1.BufferPtr))
        {
          using (var g = Graphics.FromImage(image))
          {
            g.Clear(Color.Transparent);
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            foreach (Shape shape in _s.Shapes)
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
        }

        return ret1;
      }

      var buf = new byte[_stream.Length];
      _stream.Read(buf, 0, (int)_stream.Length);

      _bufferPtr = Marshal.AllocHGlobal(buf.Length);
      Marshal.Copy(buf, 0, _bufferPtr, buf.Length);

      var ret = new VideoFrame(_bufferPtr, _originalSize.Width, _originalSize.Height, Stride, FourCC, _originalSize.Width/_originalSize.Height, _nominator.Value, _denominator.Value, frame_format_type_e.frame_format_type_progressive);
      return ret;
    }
  }
}
