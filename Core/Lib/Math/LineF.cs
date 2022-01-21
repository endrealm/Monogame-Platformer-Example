using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib.Math
{
    public class LineF
    {
        public readonly Vector2 Origin;
        public readonly Vector2 Direction;
        public readonly float Distance;
        private Vector2? _end ;

        public Vector2 End => GetEnd();

        private Vector2 GetEnd()
        {
            return _end ??= Origin + Direction * Distance;
        }

        public LineF(Vector2 origin, Vector2 direction, float distance)
        {
            Origin = origin;
            Direction = direction;
            Distance = distance;
        }
        public LineF(Vector2 start, Vector2 end)
        {
            Origin = start;
            var rawDir = start - end;
            Direction = rawDir.NormalizedCopy();
            Distance = rawDir.Length();
            _end = end;
        }

        public bool IntersectsWith(IShapeF shape)
        {
            if (shape is RectangleF rect)
            {
                if (rect.Contains(Origin) || rect.Contains(End)) return true;
                var corners = rect.GetCorners();
                return new []
                {
                    new LineF(corners[0], corners[1]),
                    new LineF(corners[1], corners[2]),
                    new LineF(corners[2], corners[3]),
                    new LineF(corners[3], corners[0]),
                }.Any(line => line.IntersectsWith(this));

            }
            
            Console.WriteLine($"Error unknown shape detected: {shape.GetType().FullName}");
            return false;
        }

        private bool IntersectsWith(LineF line)
        {
            return get_line_intersection(this.Origin.X, this.Origin.Y, this.End.X, this.End.Y, 
                line.Origin.X, line.Origin.Y, line.End.X, line.End.Y);
        }
        
        private bool get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y, 
            float p2_x, float p2_y, float p3_x, float p3_y)
        {
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = p1_x - p0_x;     
            s1_y = p1_y - p0_y;
            s2_x = p3_x - p2_x;    
            s2_y = p3_y - p2_y;

            float s, t;
            s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = ( s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                return true;
            }

            return false; // No collision
        }
    }
}