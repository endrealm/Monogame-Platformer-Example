using System.Linq;
using MonoGame.Extended;

namespace Core.Lib
{
    public static class Utils
    {
        public static bool IsInside(this RectangleF instance, RectangleF other)
        {
            return other.GetCorners().All(vector2 => instance.Contains(vector2));
        }
    }
}