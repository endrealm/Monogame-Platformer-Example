using System;
using Microsoft.Xna.Framework;

namespace Core.Lib.Physics
{
    public class CollisionEventArgs : EventArgs
    {
        public ICollisionTarget Other { get; internal set; }

        public Vector2 PenetrationVector { get; internal set; }
    }
}