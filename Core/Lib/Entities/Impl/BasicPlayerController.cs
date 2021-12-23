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
        
        private const float SlideJumpXSpeed = 3.5f;
        private const float MovementSpeed = 6;

        
        private const float JumpSpeed = 220f;
        private const float CoyoteJumpGraceTime = 50/1000f;
        private const float CoyoteWallJumpGraceTime = 50/1000f;
        private const float JumpDecay = 115f;
        
        
        private float _timeSinceLastGround = 0f;
        
        
        /// <summary>
        /// Slide wall direction
        /// </summary>
        private bool _slidingRight;
        
        
        private int _currentJumps;
        private bool _jumpedFromGround;
        
        /// <summary>
        /// True if last action was to be grounded or sliding (for coyote jump)
        /// </summary>
        private bool _lastSlideOrGround;

        public BasicPlayerController(LocomotionBody locomotionBody, IPlayerInput playerInput)
        {
            _locomotionBody = locomotionBody;
            _playerInput = playerInput;
            _currentJumps = _maxAirJumps;
            _verticalMovementEffect.Cancel(); // cancel effect at start
        }

        public void Update(float deltaTime)
        {

            #region Coyote Jump Calculation

            if (IsSliding() || _locomotionBody.IsGrounded())
            {
                _lastSlideOrGround = _locomotionBody.IsGrounded();
                _timeSinceLastGround = 0;
            }
            else
            {
                _timeSinceLastGround += deltaTime;
            }

            #endregion

            #region Movement Input
            
            var movementInput = new Vector2();

            if (_playerInput.ShouldMoveLeft())
                movementInput += new Vector2(-MovementSpeed, 0);


            if (_playerInput.ShouldMoveRight())
                movementInput += new Vector2(MovementSpeed, 0);

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

                if(!_locomotionBody.IsGrounded() && !IsSliding() && !CoyoteGraceTimeActive())
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
                    _locomotionBody.AddImpulse( new Vector2(dir * SlideJumpXSpeed, 0));
                }

                _verticalMovementEffect.Cancel();
                _verticalMovementEffect = new LinearDecayingVelocityEffect(direction, modifier, true);
                _locomotionBody.AddVelocityEffect(_verticalMovementEffect);
            }

            #endregion

            #region Wall Sliding

            if (_verticalMovementEffect.IsCancelled())
            {
                if (_playerInput.ShouldGrab() && _locomotionBody.TouchingAnyWall())
                {
                    _verticalMovementEffect = new VelocityEffect(new Vector2(0, SlideSpeed), true)
                    {
                        TypeId = SlideEffectId
                    };

                    _locomotionBody.AddVelocityEffect(_verticalMovementEffect);

                    _slidingRight = _locomotionBody.IsWallAtRight();
                }
            }

            if (IsSliding() && (!_locomotionBody.TouchingAnyWall() || _locomotionBody.IsGrounded()))
            {
                _verticalMovementEffect.Cancel();
            }

            #endregion
        }

        private bool CoyoteGraceTimeActive()
        {
            return _timeSinceLastGround <= (_lastSlideOrGround ? CoyoteJumpGraceTime : CoyoteWallJumpGraceTime);
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