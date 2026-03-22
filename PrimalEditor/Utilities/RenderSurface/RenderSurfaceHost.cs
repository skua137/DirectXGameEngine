using PrimalEditor.DLLWrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;

namespace PrimalEditor.Utilities
{
    public class RenderSurfaceHost : HwndHost
    {
        private readonly int width = 800;
        private readonly int height = 600;
        private IntPtr _renderWindowHandle = IntPtr.Zero;
        private DelayEventTimer resizeTimer;

        public int SurfaceId { get; private set; } = ID.INVALID_ID;

        public void Resize()
        {
            resizeTimer.Trigger();
           
        }

        public RenderSurfaceHost(double width, double height)
        {
            this.width = (int)width;
            this.height = (int)height;
            resizeTimer = new DelayEventTimer(TimeSpan.FromMilliseconds(250.0));
            resizeTimer.Triggered += Resize;
        }

        private void Resize(object? sender, DelayEventTimerArgs e)
        {
            e.RepeatEvent = (Mouse.LeftButton == MouseButtonState.Pressed);
            if (!e.RepeatEvent)
            {
                EngineAPI.ResizeRenderSurface(SurfaceId);
                Logger.Log(MessageType.Info, "Resized");
            }
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            SurfaceId = EngineAPI.CreateRenderSurface(hwndParent.Handle, width, height);
            Debug.Assert(ID.IsValid(SurfaceId));
            _renderWindowHandle = EngineAPI.GetWindowHandle(SurfaceId);
            Debug.Assert(_renderWindowHandle != IntPtr.Zero);

            return new HandleRef(this, _renderWindowHandle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            EngineAPI.RemoveRenderSurface(SurfaceId);
            SurfaceId = ID.INVALID_ID;
            _renderWindowHandle = IntPtr.Zero;
        }
    }
}
