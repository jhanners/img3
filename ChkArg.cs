using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenGL
{
    public class ChkArg
    {
        public static void IsNotNull(
            object value,
            string message,
            params object[] args)
        {
            if (object.ReferenceEquals(null, value))
            {
                throw new ArgumentNullException(string.Format(message, args));
            }
        }

        public static void IsGreaterThan<T>(
            T lhs,
            T rhs,
            string message,
            params object[] args) where T : IComparable
        {
            int compare = lhs.CompareTo(rhs);
            if (compare < 1)
            {
                throw new ArgumentOutOfRangeException(string.Format(message, args));
            }
        }
    }
}
