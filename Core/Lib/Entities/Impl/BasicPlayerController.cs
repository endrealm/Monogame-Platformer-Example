using Core.Lib.Input;
using Core.Lib.Physics.Locomotion;
using Microsoft.Xna.Framework;

namespace Core.Lib.Entities.Impl
{
    public class BasicPlayerController: IPlayerController
    {
        private readonly LocomotionBody _locomotionBody;
        private readonly IPlayerInput _playerInput;
        private VelocityEffect _verticalMovementEffect = new VelocityEffect(new Vector2());
        private int _maxAirJumps = 1;
        
        private const float SlideSpeed = 25f;
        private const long SlideEffectId = 1;
        
        private const float SlideJumpXSpeed = 100f;
        private const float SlideJumpXDecay = 20f;
        
        private const float JumpSpeed = 200f;
        private const float JumpDecay = 70f;
        
        
        /// <summary>
        /// Slide wall direction
        /// </summary>
        private bool _slidingRight;
        
        
        private int _currentJumps;
        private bool _jumpedFromGround;

        public BasicPlayerController(LocomotionBody locomotionBody, IPlayerInput playerInput)
        {
            _locomotionBody = locomotionBody;
            _playerInput = playerInput;
            _currentJumps = _maxAirJumps;
            _verticalMovementEffect.Cancel(); // cancel effect at start
        }

        public void Update(float deltaTime)
        {
            #region Movement Input

            const float movementSpeed = 100;
            
            var movementInput = new Vector2();

            if (_playerInput.ShouldMoveLeft())
                movementInput += new Vector2(-movementSpeed, 0);


            if (_playerInput.ShouldMoveRight())
                movementInput += new Vector2(movementSpeed, 0);

            _locomotionBody.Move(movementInput);
            
            #endregion

            #region Double Jump Reset

            if (_locomotionBody.IsGrounded() && _verticalMovementEffect.IsCancelled())
            {
                _currentJumps = _maxAirJumps;
                _jumpedFromGround = false;
            }

            #endregion

            #region Jumping

            if (_playerInput.ShouldJump())
            {

                if(!_locomotionBody.IsGrounded() && !IsSliding())
                {
                    if(_currentJumps <= 0) return; // No air jump available
                    _currentJumps--;
                }

                var direction = new Vector2(0, -JumpSpeed);
                var modifier = new Vector2(0, JumpDecay);

                if (IsSliding())
                {
                    var dir = _slidingRight ? -1 : 1;
                    // Add horizontal direction boost
                    direction += new Vector2(dir * SlideJumpXSpeed, 0);
                    modifier += new Vector2(-dir * SlideJumpXDecay, 0);
                    // TODO: Replace by velocity system
                }

                _verticalMovementEffect.Cancel();
                _verticalMovementEffect = new LinearDecayingVelocityEffect(direction, modifier, true);
                _locomotionBody.AddVelocityEffect(_verticalMovementEffect);
            }

            #endregion

            #region Wall Sliding

            if (_verticalMovementEffect.IsCancelled())
            {
                if (!_locomotionBody.MovingAgainstAnyWall()) return;
                
                _verticalMovementEffect = new VelocityEffect(new Vector2(0, SlideSpeed), true)
                {
                    TypeId = SlideEffectId
                };
                
                _locomotionBody.AddVelocityEffect(_verticalMovementEffect);

                _slidingRight = _locomotionBody.MovingAgainstRightWall();

            }
            
            if (IsSliding() && (!_locomotionBody.TouchingAnyWall() || _locomotionBody.IsGrounded()))
            {
                _verticalMovementEffect.Cancel();
            }

            #endregion
        }

        private bool IsSliding()
        {
            return !_verticalMovementEffect.IsCancelled() && _verticalMovementEffect.TypeId == SlideEffectId;
        }

        public void PostBodyUpdate(float deltaTime)
        {
            if (_locomotionBody.IsCeilingAtHead())
            {
                _verticalMovementEffect.Cancel();
            }
        }
    }
}