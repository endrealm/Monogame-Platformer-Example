using Core.Lib.Entities;
using MonoGame.Extended;

namespace Core.Lib.Physics
{
    public class SpikeCollisionTarget: ICollisionTarget, IDamageSource
    {
        public SpikeCollisionTarget(IShapeF bounds)
        {
            Bounds = bounds;
        }

        public IShapeF Bounds { get; }
        public bool StaticCollider => false;
        public bool TriggerOnly => true;

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            // Do nothing... just be... there
            var other = collisionInfo.Other;

            if (other is ILivingEntity livingEntity)
            {
                livingEntity.GetDamage(this, DamageReason.Environment, 999);
            }
        }
    }
}