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
            return IntersectsWith(shape, out var unused);
        }
        
        public bool IntersectsWith(IShapeF shape, out Vector2 intersectionPoint)
        {
            intersectionPoint = default;
            
            if (shape is RectangleF rect)
            {
                var corners = rect.GetCorners();
                var hits =  new []
                {
                    new LineF(corners[0], corners[1]),
                    new LineF(corners[1], corners[2]),
                    new LineF(corners[2], corners[3]),
                    new LineF(corners[3], corners[0]),
                }.Select(line =>
                {
                    var hit = line.IntersectsWith(this, out var collisionPoint);

                    return (hit, collisionPoint);
                }).Where(tuple => tuple.hit).Select(tuple => tuple.collisionPoint).ToArray();

                // Find point closes to line origin
                if (hits.Length > 0)
                {
                    var dist = 0f;
                    var first = true;
                    foreach (var hit in hits)
                    {
                        var length = (hit - Origin).LengthSquared();
                        if(!first && dist <= length) continue;
                        first = false;
                        intersectionPoint = hit;
                        dist = length;
                    }

                    return true;
                }
                
                if (rect.Contains(Origin) || rect.Contains(End))
                {
                    intersectionPoint = rect.Center; // Somewhere inside rect, maybe replace by line start and end
                    return true;
                }

                return false;
            }
            
            Console.WriteLine($"Error unknown shape detected: {shape.GetType().FullName}");
            return false;
        }

        private bool IntersectsWith(LineF line, out Vector2 collisionPoint)
        {
            return get_line_intersection(Origin, End, line.Origin, line.End, out collisionPoint);
        }
        
        private bool get_line_intersection(Vector2 origin, Vector2 end, 
            Vector2 otherOrigin, Vector2 otherEnd, out Vector2 collisionPoint)
        {
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = end.X - origin.X;     
            s1_y = end.Y - origin.Y;
            s2_x = otherEnd.X - otherOrigin.X;    
            s2_y = otherEnd.Y - otherOrigin.Y;

            float s, t;
            s = (-s1_y * (origin.X - otherOrigin.X) + s1_x * (origin.Y - otherOrigin.Y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = ( s2_x * (origin.Y - otherOrigin.Y) - s2_y * (origin.X - otherOrigin.X)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                collisionPoint = new Vector2(
                    origin.X + (t * s1_x),
                    origin.Y + (t * s1_y));

                return true;
            }

            collisionPoint = new Vector2();
            return false; // No collision
        }
    }
}