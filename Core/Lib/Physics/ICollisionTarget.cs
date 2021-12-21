using MonoGame.Extended;

namespace Core.Lib.Physics
{
    public interface ICollisionTarget
    {
        IShapeF Bounds { get; }
        bool StaticCollider { get; }

        void OnCollision(CollisionEventArgs collisionInfo);
    }
}