using Microsoft.Xna.Framework;

namespace Core.Lib.Physics.Locomotion
{
    public abstract class LocomotionBody
    {
        public abstract void Move(Vector2 direction);
        public abstract FreezeEffect Freeze();
        public abstract void ClearAllFreezes();
        public abstract void AddVelocityEffect(VelocityEffect effect);

        public abstract bool IsGrounded();
        public abstract bool IsCeilingAtHead();
        public abstract bool MovingAgainstAnyWall();
        public abstract bool TouchingAnyWall();
        public abstract bool IsWallAtRight();
        public abstract bool IsWallAtLeft();
        public abstract bool MovingAgainstRightWall();
        public abstract bool MovingAgainstLeftWall();

        public abstract void AddImpulse(Vector2 velocity);

        
        public abstract void Update(float deltaTime);
        
        /**
         * Smoothed (by Time.deltaTime) and validated movement input. 
         */
        public abstract Vector2 GetLastMovementSmoothed();
    }
}