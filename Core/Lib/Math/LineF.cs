using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib.Math
{
    public class LineF
    {
        public readonly Vector2 Origin;
        public readonly Vector2 Direction;
        public readonly float Distance;

        public readonly Vector2 End;

        public LineF(Vector2 origin, Vector2 direction, float distance)
        {
            Origin = origin;
            Direction = direction;
            Distance = distance;
            End = origin + direction * distance;
        }

        public bool IntersectsWith(IShapeF shape)
        {
            if (shape is RectangleF rect)
            {
                if (rect.Contains(Origin) || rect.Contains(End)) return true;
                
                // Todo check if segment intersects
                return false;
            }
            
            Console.WriteLine($"Error unknown shape detected: {shape.GetType().FullName}");
            return false;
        }
    }
}