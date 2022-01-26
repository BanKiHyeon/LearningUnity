using UnityEngine;

namespace Util
{
    public class MaxstUtil : MonoBehaviour
    {
        public static void Swap<T>(ref T x, ref T y)
        {
            /*
            T t = y;
            y = x;
            x = t;
            */
            (y, x) = (x, y);
        }
    }
}