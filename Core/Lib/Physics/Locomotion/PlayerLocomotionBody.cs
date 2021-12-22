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
        private const float GravityMultiplier = 20f;
        private const float GravityThreshold = 200f;

        private RectangleF Hitbox => (RectangleF) _target.Bounds;

        private const float HitDensity = 1f;
        private const float HitExt = .5f;
        private const float CastDensity = 1f;
        private const float AirDampening = 8f;
        private const float GroundDampening = 15f;

        private readonly List<FreezeEffect> _freezeEffects = new List<FreezeEffect>();
        private readonly List<VelocityEffect> _velocityEffects = new List<VelocityEffect>();
        private Vector2 _movement;
        private Vector2 _movementSpeedCap = new Vector2(2, 2);
        private Vector2 _baseVelocity;
        private float _gravity;
        
        private ICollisionTarget[] _rightHits = Array.Empty<ICollisionTarget>();
        private ICollisionTarget[] _leftHits = Array.Empty<ICollisionTarget>();
        private ICollisionTarget[] _upHits = Array.Empty<ICollisionTarget>();
        private ICollisionTarget[] _downHits = Array.Empty<ICollisionTarget>();
        private Vector2 _movementSmoothed;
 
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
            return _downHits.Length > 0;
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

        public override void AddImpulse(Vector2 velocity)
        {
            _baseVelocity += velocity;
        }

        public override bool TouchingAnyWall()
        {
            return IsWallAtLeft() || IsWallAtRight();

        }

        
        public override bool IsCeilingAtHead()
        {
            return _upHits.Length > 0;
        }

        public override bool IsWallAtRight()
        {
            return _rightHits.Length > 0;
        }

        public override bool IsWallAtLeft()
        {
            return _leftHits.Length > 0;
        }
        public override Vector2 GetLastMovementSmoothed()
        {
            return _movementSmoothed;
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
            _gravity += GravityMultiplier * GravityMultiplier * deltaTime;
            if (_gravity > GravityThreshold)
            {
                _gravity = GravityThreshold;
            }
        }

        private void PositionUpdate(float deltaTime)
        {

            var dampening = IsGrounded() ? GroundDampening : AirDampening;
            
            #region Apply Player Movement

            if (_movement.X < 0)
            {
                if (MathF.Abs(_baseVelocity.X) < _movementSpeedCap.X)
                {
                    // clamp value to max by regular move
                    _baseVelocity.X = MathF.Max(_baseVelocity.X + (_movement.X - dampening) * deltaTime, -_movementSpeedCap.X);
                }
            } else if (_movement.X > 0)
            {
                if (MathF.Abs(_baseVelocity.X) < _movementSpeedCap.X)
                {
                    // clamp value to max by regular move
                    _baseVelocity.X = MathF.Min(_baseVelocity.X + (_movement.X + dampening) * deltaTime, _movementSpeedCap.X);
                }
            }
            
            if (_movement.Y < 0)
            {
                if (MathF.Abs(_baseVelocity.Y) < _movementSpeedCap.Y)
                {
                    // clamp value to max by regular move
                    _baseVelocity.Y = MathF.Max(_baseVelocity.Y + _movement.Y * deltaTime, -_movementSpeedCap.Y);
                }
            } else if (_movement.Y > 0)
            {
                if (MathF.Abs(_baseVelocity.Y) < _movementSpeedCap.Y)
                {
                    // clamp value to max by regular move
                    _baseVelocity.Y = MathF.Min(_baseVelocity.Y + _movement.Y * deltaTime, _movementSpeedCap.Y);
                }
            }

            #endregion

            #region Dampening

            if (_baseVelocity.X > 0)
            {
                _baseVelocity.X = MathF.Max(_baseVelocity.X - dampening * deltaTime, 0);
            } else if (_baseVelocity.X < 0)
            {
                _baseVelocity.X = MathF.Min(_baseVelocity.X + dampening * deltaTime, 0);
            }

            #endregion
            
            // Copy velocity to prevent manipulation by effect
            var direction = new Vector2(_baseVelocity.X, _baseVelocity.Y);
            

            bool gravitySuppressed = false;
            _velocityEffects.ForEach(effect =>
            {
                direction += effect.Direction * deltaTime;
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
            
            var delta = direction + new Vector2(0, _gravity) * deltaTime;
            
            #region Validate new location

            var leftDistance = LeftBlocked(ref delta);
            var rightDistance = RightBlocked(ref delta);
            var downDistance = DownBlocked(ref delta);
            var upDistance = UpBlocked(ref delta);
            if(delta.X <= -leftDistance && leftDistance > -1)
            {
                delta.X = -leftDistance;
                _baseVelocity.X = 0;
                Console.WriteLine("1");
            }
            
            if(delta.X >= rightDistance && rightDistance > -1)
            {
                delta.X = rightDistance;
                _baseVelocity.X = 0;
                Console.WriteLine("2");
            }

            if (delta.Y >= downDistance && downDistance > -1)
            {
                delta.Y = downDistance;
                _gravity = 0; // Reset gravity
                _baseVelocity.Y = 0;
            }

            if (delta.Y<= -upDistance && upDistance > -1)
            {
                delta.Y = -upDistance;
                _baseVelocity.Y = 0;
            }

            #endregion

            _movementSmoothed = delta;
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
            _leftHits = Array.Empty<ICollisionTarget>();
            
            var xDelta = -0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedX(xDelta, Vectors.Left);
                if (hits.Length > 0){
                    _leftHits = hits;
                    return iterations * HitDensity;
                }
                xDelta -= HitDensity;
                iterations++;
            } while (xDelta > delta.X);

            return -2;
        }
        float RightBlocked(ref Vector2 delta)
        {
            _rightHits = Array.Empty<ICollisionTarget>();
            
            var xDelta = 0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedX(xDelta, Vectors.Right);
                if (hits.Length > 0)
                {
                    _rightHits = hits;
                    return iterations * HitDensity;
                }
                xDelta += HitDensity;
                iterations++;
            } while (xDelta < delta.X);
            return -2;
        }
        float UpBlocked(ref Vector2 delta)
        {
            _upHits = Array.Empty<ICollisionTarget>();
            
            var yDelta = -0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedY(yDelta, -Vectors.Up);
                if (hits.Length > 0)
                {
                    _upHits = hits;
                    return iterations * HitDensity;
                }
                yDelta -= HitDensity;
                iterations++;
            } while (yDelta > delta.Y);
            return -2;
        }
        float DownBlocked(ref Vector2 delta)
        {
            _downHits = Array.Empty<ICollisionTarget>();
            
            var yDelta = 0.000001f;
            var iterations = 0;
            do
            {
                var hits = BlockedY(yDelta, -Vectors.Down);
                if (hits.Length > 0)
                {
                    _downHits = hits;
                    return iterations * HitDensity;
                }
                yDelta += HitDensity;
                iterations++;
            } while (yDelta < delta.Y);

            return -2;
        }
        
        ICollisionTarget[] BlockedX(float delta, Vector2 direction)
        {
            var modifier = delta > 0 ? 1 : -1;
            var start = Hitbox.Center;
            start.X += modifier * (Hitbox.Width / 2);

            var halfHeight = Hitbox.Height / 2;
            var list = new List<ICollisionTarget>();

            for (var i = -halfHeight+0.05f; i <= halfHeight-0.05f; i += CastDensity)
            {
                var hit = _target.GetRaycastContext()?.Raycast(start + new Vector2(delta,i), direction, HitDensity + HitExt, target => !target.TriggerOnly && target != _target);
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
            for (var i = -halfWidth+2f; i <= halfWidth-2f; i += CastDensity)
            {
                var hit = _target.GetRaycastContext()?.Raycast(start + new Vector2(i,delta), direction, HitDensity, target => !target.TriggerOnly && target != _target);
                // var hit = Physics2D.Raycast(start + new Vector2(i,delta), direction, hitDensity, worldMask);
                DebugDrawer.DrawLine(new LineF(start + new Vector2(i,delta), direction, HitDensity + HitExt), Color.Pink);
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