using NewTek.NDI;
using SharpDX;
using SharpDX.DXGI;
using System;
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
      var buf = new byte[_stream.Length];
      _stream.Read(buf, 0, (int)_stream.Length);

      _bufferPtr = Marshal.AllocHGlobal(buf.Length);
      Marshal.Copy(buf, 0, _bufferPtr, buf.Length);

      var ret = new VideoFrame(_bufferPtr, _originalSize.Width, _originalSize.Height, Stride, FourCC, _originalSize.Width/_originalSize.Height, 30000, 1000, frame_format_type_e.frame_format_type_progressive);
      return ret;
    }
  }
}
