using Microsoft.Office.Interop.PowerPoint;
using NewTek.NDI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace EvKgHuelben.Helpers.NDI
{
  public class BufferedSlideFrame : BufferedFrameBase
  {
    private readonly Slide _slide;

    public BufferedSlideFrame(Slide slide)
    {
      _slide = slide;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
    }

    /// <inheritdoc />
    public override VideoFrame ToVideoFrame()
    {
      var setup = (_slide.Parent as Presentation).PageSetup;
      var width = (int)setup.SlideWidth;
      var height = (int)setup.SlideHeight;
      var aspectRatio = width / height;
      var videoFrame = new VideoFrame(width, height, aspectRatio, _nominator.Value, _denominator.Value);

      var tmpfile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
      _slide.Shapes.Range().Export(tmpfile, PpShapeFormat.ppShapeFormatPNG, width, height);

      using (var image = new Bitmap(videoFrame.Width, videoFrame.Height, videoFrame.Stride, PixelFormat.Format32bppPArgb, videoFrame.BufferPtr))
      {
        using (var g = Graphics.FromImage(image))
        {
          g.Clear(Color.Transparent);
          g.CompositingMode = CompositingMode.SourceOver;
          g.CompositingQuality = CompositingQuality.HighQuality;
          g.InterpolationMode = InterpolationMode.HighQualityBicubic;
          g.SmoothingMode = SmoothingMode.HighQuality;
          g.PixelOffsetMode = PixelOffsetMode.HighQuality;

          var rect = new Rectangle(0, 0, width, height);
          using (var wrapMode = new ImageAttributes())
          {
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            var i = Image.FromFile(tmpfile);
            g.DrawImage(i, rect, 0, 0, i.Width, i.Height, GraphicsUnit.Pixel, wrapMode);
          }
          g.Flush();
        }
      }

      try
      {
        File.Delete(tmpfile);
      }
      catch { }

      return videoFrame;
    }
  }
}
