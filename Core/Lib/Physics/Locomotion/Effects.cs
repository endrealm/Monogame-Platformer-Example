using Microsoft.Xna.Framework;

namespace Core.Lib.Physics.Locomotion
{
    public abstract class Effect
    {
        private bool _cancelled;

        public void Cancel()
        {
            _cancelled = true;
        }

        public bool IsCancelled()
        {
            return _cancelled;
        }
    }
    
    public class VelocityEffect: Effect, IUpdateable
    {
        public Vector2 Direction { get; protected set; }
        private bool _suppressGravity;
        public VelocityEffect(Vector2 direction, bool suppressGravity = false)
        {
            Direction = direction;
            _suppressGravity = suppressGravity;
        }

        public virtual void Update(float deltaTime)
        {
            
        }

        public virtual bool SuppressGravity()
        {
            return _suppressGravity;
        }
    }

    public class LinearDecayingVelocityEffect : VelocityEffect
    {
        private Vector2 _modifier; 
        public LinearDecayingVelocityEffect(Vector2 effect, Vector2 modifier, bool suppressGravity) : base(effect, suppressGravity)
        {
            _modifier = modifier;
        }

        public override void Update(float deltaTime)
        {
            Direction += deltaTime * 10 * _modifier;
            
            if (_modifier.X > 0)
            {
                if (!(Direction.X >= 0)) return;
                Direction = new Vector2(0, Direction.Y);
            }
            else if(_modifier.X < 0)
            {
                if (!(Direction.X <= 0)) return;
                Direction = new Vector2(0, Direction.Y);
            }
            
            if (_modifier.Y > 0)
            {
                if (!(Direction.Y >= 0)) return;
                Direction = new Vector2(Direction.Y, 0);
            }
            else if(_modifier.Y < 0)
            {
                if (!(Direction.Y <= 0)) return;
                Direction = new Vector2(Direction.X, 0);
            }
            
            if(Direction.X == 0f && Direction.Y == 0f)
            {
                Cancel();
            }
        }
    }
    
    
    public class FreezeEffect: Effect {}
}