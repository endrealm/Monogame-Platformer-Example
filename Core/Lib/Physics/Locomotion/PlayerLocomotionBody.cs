using System;
using System.Collections.Generic;
using System.Linq;
using Core.Lib.Entities;
using Core.Lib.Math;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib.Physics.Locomotion
{
    public class PlayerLocomotionBody: LocomotionBody, IUpdateable
    {
        
        private readonly float gravityMultiplier = 20f;
        private readonly float gravityThreshold = 200f;

        private RectangleF Hitbox => (RectangleF) _target.Bounds;
        
        private readonly float hitDensity = 1f;
        private readonly float hitExt = .5f;
        
        private readonly float castDensity = 1f;
        
        private readonly List<FreezeEffect> _freezeEffects = new List<FreezeEffect>();
        private readonly List<VelocityEffect> _velocityEffects = new List<VelocityEffect>();
        private Vector2 _movement;
        private float _gravity;
        
        private ICollisionTarget[] rightHits = Array.Empty<ICollisionTarget>();
        private ICollisionTarget[] leftHits = Array.Empty<ICollisionTarget>();
        private ICollisionTarget[] upHits = Array.Empty<ICollisionTarget>();
        private ICollisionTarget[] downHits = Array.Empty<ICollisionTarget>();
        private Vector2 movementRaw;
        private Vector2 movementSmoothed;

        private readonly IPlayer _target;
        private readonly Transform2 _transform;

        public PlayerLocomotionBody(IPlayer target, Transform2 transform)
        {
            _target = target;
            _transform = transform;
        }


        public override void Move(Vector2 direction)
        {
            _movement = direction;
        }
        

        public override FreezeEffect Freeze()
        {
            var effect = new FreezeEffect();
            _freezeEffects.Add(effect);
            return effect;
        }

        public override void ClearAllFreezes()
        {
            _freezeEffects.ForEach(effect => effect.Cancel());
        }

        public override void AddVelocityEffect(VelocityEffect effect)
        {
            _velocityEffects.Add(effect);
        }

        public override bool IsGrounded()
        {
            return downHits.Length > 0;
        }

        public override bool MovingAgainstAnyWall()
        {
            return MovingAgainstLeftWall() || MovingAgainstRightWall();
        }
        public override bool MovingAgainstRightWall()
        {
            return IsWallAtRight() && _movement.X > 0;
        }
        public override bool MovingAgainstLeftWall()
        {
            return _movement.X < 0 && IsWallAtLeft();
        }

        public override bool TouchingAnyWall()
        {
            return IsWallAtLeft() || IsWallAtRight();

        }

        
        public override bool IsCeilingAtHead()
        {
            return upHits.Length > 0;
        }

        public override bool IsWallAtRight()
        {
            return rightHits.Length > 0;
        }

        public override bool IsWallAtLeft()
        {
            return leftHits.Length > 0;
        }

        public override Vector2 GetLastMovementRaw()
        {
            return movementRaw;
        }

        public override Vector2 GetLastMovementSmoothed()
        {
            return movementSmoothed;
        }

        // Update is called once per frame
        public override void Update(float deltaTime)
        {
            CleanUp();
            CalculateGravity(deltaTime);
            PositionUpdate(deltaTime);
        }

        private void CalculateGravity(float deltaTime)
        {
            _gravity += gravityMultiplier * gravityMultiplier * deltaTime;
            if (_gravity > gravityThreshold)
            {
                _gravity = gravityThreshold;
            }
        }

        private void PositionUpdate(float deltaTime)
        {
            var direction = _movement;
            

            bool gravitySuppressed = false;
            _velocityEffects.ForEach(effect =>
            {
                direction += effect.Direction * deltaTime * 50;
                effect.Update(deltaTime);

                if (effect.SuppressGravity())
                {
                    gravitySuppressed = true;
                }
            });
            // Apply gravity

            if (gravitySuppressed)
            {
                _gravity = 0;
            }
            
            direction += new Vector2(0, _gravity);
            var delta = new Vector2(direction.X, direction.Y) * deltaTime;
            
            #region Validate new location

            var leftDistance = LeftBlocked(ref delta);
            var rightDistance = RightBlocked(ref delta);
            var downDistance = DownBlocked(ref delta);
            var upDistance = UpBlocked(ref delta);
            if(delta.X <= -leftDistance)
            {
                delta.X = -leftDistance;
            }
            
            if(delta.X >= rightDistance)
            {
                delta.X = rightDistance;
            }

            if (delta.Y >= downDistance)
            {
                delta.Y = downDistance;
                _gravity = 0; // Reset gravity
            }

            if (delta.Y<= -upDistance)
            {
                delta.Y = -upDistance;
            }

            #endregion

            movementRaw = direction;
            movementSmoothed = delta;
            _transform.Position += delta;
        }

        private void CleanUp()
        {
            _freezeEffects.RemoveAll(effect => effect.IsCancelled());
            _velocityEffects.RemoveAll(effect => effect.IsCancelled());
        }

        #region HitBox Calls

        float LeftBlocked(ref Vector2 delta)
        {
            leftHits = Array.Empty<ICollisionTarget>();
            
            var xDelta = -0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedX(xDelta, Vectors.Left);
                if (hits.Length > 0){
                    leftHits = hits;
                    return iterations * hitDensity;
                }
                xDelta -= hitDensity;
                iterations++;
            } while (xDelta > delta.X);

            return iterations * hitDensity;
        }
        float RightBlocked(ref Vector2 delta)
        {
            rightHits = Array.Empty<ICollisionTarget>();
            
            var xDelta = 0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedX(xDelta, Vectors.Right);
                if (hits.Length > 0)
                {
                    rightHits = hits;
                    return iterations * hitDensity;
                }
                xDelta += hitDensity;
                iterations++;
            } while (xDelta < delta.X);
            return iterations * hitDensity;
        }
        float UpBlocked(ref Vector2 delta)
        {
            upHits = Array.Empty<ICollisionTarget>();
            
            var yDelta = -0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedY(yDelta, -Vectors.Up);
                if (hits.Length > 0)
                {
                    upHits = hits;
                    return iterations * hitDensity;
                }
                yDelta -= hitDensity;
                iterations++;
            } while (yDelta > delta.Y);
            return iterations * hitDensity;
        }
        float DownBlocked(ref Vector2 delta)
        {
            downHits = Array.Empty<ICollisionTarget>();
            
            var yDelta = 0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedY(yDelta, -Vectors.Down);
                if (hits.Length > 0)
                {
                    downHits = hits;
                    return iterations * hitDensity;
                }
                yDelta += hitDensity;
                iterations++;
            } while (yDelta < delta.Y);

            return iterations * hitDensity;
        }
        
        ICollisionTarget[] BlockedX(float delta, Vector2 direction)
        {
            var modifier = delta > 0 ? 1 : -1;
            var start = Hitbox.Center;
            start.X += modifier * (Hitbox.Width / 2);

            var halfHeight = Hitbox.Height / 2;
            var list = new List<ICollisionTarget>();

            for (var i = -halfHeight+0.05f; i <= halfHeight-0.05f; i += castDensity)
            {
                var hit = _target.GetRaycastContext()?.Raycast(start + new Vector2(delta,i), direction, hitDensity + hitExt, target => !target.TriggerOnly && target != _target);
                // var hit = Physics2D.Raycast(start + new Vector2(delta,i), direction, hitDensity, worldMask);
                // Debug.DrawRay(start + new Vector2(delta,i), direction, Color.green, Time.deltaTime);
                if (hit != null)
                {
                    list.Add(hit.collider);
                }
            }
            
            return list.Distinct().ToArray();
        }
        ICollisionTarget[] BlockedY(float delta, Vector2 direction)
        {
            var modifier = delta > 0 ? 1 : -1;
            var start = Hitbox.Center;
            start.Y += modifier * (Hitbox.Height / 2);
            
            var halfWidth = Hitbox.Width / 2;
            var list = new List<ICollisionTarget>();
            for (var i = -halfWidth+2f; i <= halfWidth-2f; i += castDensity)
            {
                var hit = _target.GetRaycastContext()?.Raycast(start + new Vector2(i,delta), direction, hitDensity, target => !target.TriggerOnly && target != _target);
                // var hit = Physics2D.Raycast(start + new Vector2(i,delta), direction, hitDensity, worldMask);
                DebugDrawer.DrawLine(new LineF(start + new Vector2(i,delta), direction, hitDensity + hitExt), Color.Pink);
                if (hit != null)
                {
                    list.Add(hit.collider);
                }
            }

            return list.Distinct().ToArray();
        }
        #endregion
    }
}