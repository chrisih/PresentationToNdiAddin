//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using EvKgHuelben.Helpers.DirectX;
using EvKgHuelben.Helpers.NDI;
using NewTek.NDI;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Concurrent;
using System.Threading;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

namespace EvKgHuelben.Base
{
  public abstract class CaptureHelperBase : IDisposable
  {
    private GraphicsCaptureItem _item;
    private Direct3D11CaptureFramePool _framePool;
    private GraphicsCaptureSession _session;
    private IDirect3DDevice _device;
    private SharpDX.Direct3D11.Device _d3dDevice;
    private Sender _sender;
    private SwapChain1 _swapChain;
    private Factory2 _factory;
    private Thread _ndiSender;
    private bool _disposing = false;
    private BlockingCollection<BufferedFrame> _buf = new BlockingCollection<BufferedFrame>();
    protected SizeInt32 _lastSize;

    protected CaptureHelperBase()
    {
      PixelFormat = DirectXPixelFormat.B8G8R8A8UIntNormalized;
    }

    public DirectXPixelFormat PixelFormat { get; set; }

    private Format SharpDxFormat
    {
      get
      {
        switch (PixelFormat)
        {
          default:
            return Format.B8G8R8A8_UNorm;
        }
      }
    }

    protected void Initialize(GraphicsCaptureItem item)
    {
      _item = item;
      _device = Direct3D11Helper.CreateDevice();
      _d3dDevice = Direct3D11Helper.CreateSharpDXDevice(_device);
      _framePool = Direct3D11CaptureFramePool.Create(_device, PixelFormat, 2, _item.Size);
      _framePool.FrameArrived += OnFrameArrived;
      _session = _framePool.CreateCaptureSession(_item);
      _session.IsCursorCaptureEnabled = true;
      _factory = new Factory2();
      
      var description = new SwapChainDescription1 { Width = _item.Size.Width, Height = _item.Size.Height, Format = SharpDxFormat, 
        Stereo = false, SampleDescription = new SampleDescription { Count = 1, Quality = 0 },
        Usage = Usage.RenderTargetOutput, BufferCount = 2, Scaling = Scaling.Stretch, SwapEffect = SwapEffect.FlipSequential, 
        AlphaMode = AlphaMode.Premultiplied, Flags = SwapChainFlags.None
      };

      _swapChain = new SwapChain1(_factory, _d3dDevice, ref description);

      // NDI stuff
      _sender = new Sender(_item.DisplayName);    
      _ndiSender = new Thread(SendNdi) { Priority = ThreadPriority.Normal, Name = "NdiSenderThread", IsBackground = true };
    }

    private void SendNdi()
    {
      try
      {
        while (!_disposing)
        {
          try
          {
            BufferedFrame frame;
            if (_buf.TryTake(out frame, 250))
            {
              // this drops frames if the UI is rendernig ahead of the specified NDI frame rate
              while (_buf.Count > 1)
              {
                frame.Dispose();
                frame = _buf.Take();
              }

              using(frame)
                using (var f = frame.ToVideoFrame())
                  _sender.Send(f);
            }
          }
          catch
          {
          }
        }
      }
      catch (ThreadAbortException) { }
    }

    ~CaptureHelperBase()
    {
      _device?.Dispose();
      _device = null;
      _sender?.Dispose();
      _sender = null;
      _session?.Dispose();
      _session = null;
      _framePool?.Dispose();
      _framePool = null;
      _d3dDevice?.Dispose();
      _d3dDevice = null;
      _factory?.Dispose();
      _factory = null;
      _ndiSender?.Abort();
      _ndiSender = null;
    }

    public void Dispose()
    {
      _disposing = true;
      _device?.Dispose();
      _sender?.Dispose();
      _session?.Dispose();
      _framePool?.Dispose();
      _d3dDevice?.Dispose();
      _factory?.Dispose();
      _ndiSender?.Abort();
    }

    public virtual void StartCapture()
    {
      _session.StartCapture();
      _ndiSender.Start();
    }

    protected int StreamWidth => _item.Size.Width - CropLeft - CropRight;
    protected int StreamHeight => _item.Size.Height - CropTop - CropBottom;

    protected virtual int CropLeft { get { return 0; } }

    protected virtual int CropRight { get { return 0; } }

    protected virtual int CropTop { get { return 0; } }

    protected virtual int CropBottom { get { return 0; } }

    protected ResourceRegion ROI => new ResourceRegion { Left = CropLeft, Right = _item.Size.Width - CropRight, Top = CropTop, Bottom = _item.Size.Height - CropBottom, Front = 0, Back = 1 };

    private void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
    {
      using (var frame = sender.TryGetNextFrame())
      {
        var needsReset = false;
        var recreateDevice = false;

        if ((frame.ContentSize.Width != _lastSize.Width) || (frame.ContentSize.Height != _lastSize.Height))
        {
          needsReset = true;
          _lastSize = frame.ContentSize;
        }

        try
        {
          using (var backBuffer = _swapChain.GetBackBuffer<Texture2D>(0))
          {
            using (var bitmap = Direct3D11Helper.CreateSharpDXTexture2D(frame.Surface))
            {
              // copy current surface to backbuffer
              _d3dDevice.ImmediateContext.CopyResource(bitmap, backBuffer);

              // Create buffer for the resized copy
              var width = StreamWidth;
              var height = StreamHeight;

              using (var copy = new Texture2D(_d3dDevice, new Texture2DDescription { Width = width, Height = height, MipLevels = 1, ArraySize = 1, Format = bitmap.Description.Format,
                Usage = ResourceUsage.Staging, SampleDescription = new SampleDescription(1, 0), BindFlags = BindFlags.None, CpuAccessFlags = CpuAccessFlags.Read, OptionFlags = ResourceOptionFlags.None}))
              {
                // Copy region from captured bitmap to stream bitmap
                _d3dDevice.ImmediateContext.CopySubresourceRegion(backBuffer, 0, ROI, copy, 0);

                // access the copied data in a stream
                _d3dDevice.ImmediateContext.MapSubresource(copy, 0, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None, out DataStream stream);
                _buf.Add(new BufferedFrame(stream, new SizeInt32 { Width = width, Height = height }, bitmap.Description.Format));
                _d3dDevice.ImmediateContext.UnmapSubresource(copy, 0);
              }
            }
          }
        }
        catch (Exception ex)
        {
          needsReset = true;
          recreateDevice = true;
        }

        if (needsReset)
        {
          _swapChain.ResizeBuffers(_swapChain.Description1.BufferCount, _lastSize.Width, _lastSize.Height, _swapChain.Description1.Format, _swapChain.Description1.Flags);
          ResetFramePool(_lastSize, recreateDevice);
        }       
      }
    }

    protected virtual void ResetFramePool(SizeInt32 size, bool recreateDevice)
    {
      do
      {
        try
        {
          if (recreateDevice)
          {
            _device = Direct3D11Helper.CreateDevice();
          }

          _framePool.Recreate(_device, PixelFormat, 2, size);
        }
        catch
        {
          _device = null;
          recreateDevice = true;
        }
      } while (_device == null);
    }
  }
}
