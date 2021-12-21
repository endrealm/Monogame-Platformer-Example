using MonoGame.Extended;

namespace Core.Lib.Physics
{
    public class StaticCollisionTarget: ICollisionTarget
    {
        public StaticCollisionTarget(IShapeF bounds)
        {
            Bounds = bounds;
        }

        public IShapeF Bounds { get; }
        public bool StaticCollider => true;
        public bool TriggerOnly => false;

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            // Do nothing... just be... there
        }
    }
}