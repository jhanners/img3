using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace img3
{
    public class MathHelpers
    {
        public static void Test()
        {
            MathHelpers.Fraction(1.1f);
            MathHelpers.Fraction(-1.1f);
        }

        public static float Fraction(float value)
        {
            float result;
            if (value < 0)
            {
                value = -value;
            }
            result = value - (float)Math.Floor(value);
            return result;
        }
    }
}
