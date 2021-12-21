using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib.Math
{
    public class LineF
    {
        private readonly Vector2 _origin;
        private readonly Vector2 _direction;
        private readonly float _distance;

        private readonly Vector2 _end;

        public LineF(Vector2 origin, Vector2 direction, float distance)
        {
            _origin = origin;
            _direction = direction;
            _distance = distance;
            _end = origin + direction * distance;
        }

        public bool IntersectsWith(IShapeF shape)
        {
            if (shape is RectangleF rect)
            {
                if (rect.Contains(_origin) || rect.Contains(_end)) return true;
                
                // Todo check if segment intersects
                return false;
            }
            
            Console.WriteLine($"Error unknown shape detected: {shape.GetType().FullName}");
            return false;
        }
    }
}