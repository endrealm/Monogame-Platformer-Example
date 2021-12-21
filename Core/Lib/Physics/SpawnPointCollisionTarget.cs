using Core.Lib.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib.Physics
{
    public class SpawnPointCollisionTarget: ICollisionTarget, IDamageSource
    {
        private readonly Vector2 _spawnPoint;

        public SpawnPointCollisionTarget(IShapeF bounds, Vector2 spawnPoint)
        {
            _spawnPoint = spawnPoint;
            Bounds = bounds;
        }

        public IShapeF Bounds { get; }
        public bool StaticCollider => false;
        public bool TriggerOnly => true;

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            var other = collisionInfo.Other;

            if (other is IPlayer player)
            {
                player.SetSpawn(_spawnPoint);
            }
        }
    }
}