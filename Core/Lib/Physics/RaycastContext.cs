using System;
using Microsoft.Xna.Framework;

namespace Core.Lib.Physics
{
    public interface RaycastContext
    {
        RaycastHit? Raycast(Vector2 origin, Vector2 direction, float distance, Func<ICollisionTarget, bool> shouldHit);
    }

    public struct RaycastHit
    {
        public Vector2 IntersectionPoint { get; set; }
        public ICollisionTarget Collider { get; set; }
    }
}