using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
