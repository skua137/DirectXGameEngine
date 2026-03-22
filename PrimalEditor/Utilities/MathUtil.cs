using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PrimalEditor.Utilities
{

    public static class ID
    {
        public static int INVALID_ID = -1;
        public static bool IsValid(int id) => id != INVALID_ID;
    }

    public static class MathUtil
    {
        public static float Epsilon => 0.00001f;

        public static bool IsTheSameAs(this float a, float b)
        {
            return Math.Abs(a - b) < Epsilon;
        }

        public static bool IsTheSameAs(this float? a, float? b)
        {
            if (!a.HasValue || !b.HasValue) return false;
            return Math.Abs(a.Value - b.Value) < Epsilon;
        }
    }

    class DelayEventTimerArgs : EventArgs
    {
        public bool RepeatEvent { get; set; }
        public object Data { get; set; }

        public DelayEventTimerArgs(object data)
        {
            Data = data;
        }
    }

    class DelayEventTimer
    {
        private readonly DispatcherTimer timer;
        private readonly TimeSpan delay;
        private DateTime lastEventTime = DateTime.Now;
        private object data;
        public event EventHandler<DelayEventTimerArgs> Triggered;

        public void Trigger(object data = null)
        {
            this.data = data;
            lastEventTime = DateTime.Now;
            timer.IsEnabled = true;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if ((DateTime.Now - lastEventTime) < delay)
            {
                return;
            }
            var eventArgs = new DelayEventTimerArgs(data);
            Triggered.Invoke(this, eventArgs);
            timer.IsEnabled = eventArgs.RepeatEvent;    
        }

        public DelayEventTimer(TimeSpan delay, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            this.delay = delay;
            timer = new DispatcherTimer(priority)
            {
                Interval = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 0.5)
            };
            timer.Tick += Timer_Tick;
        }

    }
}
