using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using NewTek;
using NewTek.NDI;
using SharpDX;
using SharpDX.DXGI;

namespace EvKgHuelben.Helpers.NDI
{
  public class BufferedVideoFrame : BufferedFrameBase
  {
    private readonly SizeInt32 _originalSize;
    private readonly Format _format;
    private readonly DataStream _stream;

    private IntPtr _bufferPtr;

    public BufferedVideoFrame(DataStream stream, SizeInt32 size, Format format)
    {
      _stream = stream;
      _originalSize = size;
      _format = format;
    }

    public override void Dispose()
    {
      Marshal.FreeHGlobal(_bufferPtr);
    }

    private NDIlib.FourCC_type_e FourCC
    {
      get
      {
        switch(_format)
        {
          case Format.R8G8B8A8_UNorm:
            return NDIlib.FourCC_type_e.FourCC_type_RGBA;
          case Format.B8G8R8A8_UNorm:
            return NDIlib.FourCC_type_e.FourCC_type_BGRA;
          case Format.B8G8R8X8_UNorm:
            return NDIlib.FourCC_type_e.FourCC_type_BGRX;
          default:
            return NDIlib.FourCC_type_e.FourCC_type_BGRA;
        }
      }
    }

    private int Stride => (int)(_stream.Length / _format.ComputeScanlineCount(_originalSize.Height));

    public override VideoFrame ToVideoFrame()
    {
      var buf = new byte[_stream.Length];
      _stream.Read(buf, 0, (int)_stream.Length);

      _bufferPtr = Marshal.AllocHGlobal(buf.Length);
      Marshal.Copy(buf, 0, _bufferPtr, buf.Length);

      var ret = new VideoFrame(_bufferPtr, _originalSize.Width, _originalSize.Height, Stride, FourCC, _originalSize.Width/_originalSize.Height, _nominator.Value, _denominator.Value, NDIlib.frame_format_type_e.frame_format_type_progressive);
      return ret;
    }
  }
}