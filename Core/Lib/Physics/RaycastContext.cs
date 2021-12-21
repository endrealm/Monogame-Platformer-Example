using System;
using Microsoft.Xna.Framework;

namespace Core.Lib.Physics
{
    public interface RaycastContext
    {
        RaycastHit Raycast(Vector2 origin, Vector2 direction, float distance, Func<ICollisionTarget, bool> shouldHit);
    }

    public class RaycastHit
    {
        public ICollisionTarget collider { get; set; }
    }
}