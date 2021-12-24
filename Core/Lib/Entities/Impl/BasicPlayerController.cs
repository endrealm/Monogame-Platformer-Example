using Core.Lib.Input;
using Core.Lib.Physics.Locomotion;
using Microsoft.Xna.Framework;

namespace Core.Lib.Entities.Impl
{
    public class BasicPlayerController: IPlayerController
    {
        private const long SlideEffectId = 1;
        private const long ClimbUpEffectId = 2;
        private const long ClimbDownEffectId = 3;

        private const float SlideSpeed = 60f;
        
        private const float ClimbUpSpeed = 25f;
        private const float ClimbDownSpeed = 25f;
        private const float MaxClimbTime = 3.5f;
        
        private const float SlideJumpXSpeed = 3.5f;
        private const float MovementSpeed = 6;

        
        private const float JumpSpeed = 220f;
        private const float CoyoteJumpGraceTime = 50/1000f;
        private const float CoyoteWallJumpGraceTime = 50/1000f;
        private const float JumpDecay = 115f;
        
        
        private float _timeSinceLastGround = 0f;
        private float _remainingClimbTime = MaxClimbTime;

        
        /// <summary>
        /// Slide wall direction
        /// </summary>
        private bool _slidingRight;
        
        
        private int _currentJumps;
        private bool _jumpedFromGround;
     
        private readonly LocomotionBody _locomotionBody;
        private readonly IPlayerInput _playerInput;
        private VelocityEffect _verticalMovementEffect = new VelocityEffect(new Vector2());
        private VelocityEffect _climbSlideCompensationEffect = new VelocityEffect(new Vector2());
        private VelocityEffect _climbMoveEffect = new VelocityEffect(new Vector2());
        private int _maxAirJumps = 1;
        
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
            _climbSlideCompensationEffect.Cancel(); // cancel effect at start
            _climbMoveEffect.Cancel(); // cancel effect at start
        }

        public void Update(float deltaTime)
        {

            #region Climb counter

            if (!_climbSlideCompensationEffect.IsCancelled())
            {
                _remainingClimbTime -= deltaTime;
            }

            if (_remainingClimbTime <= 0)
            {
                _climbSlideCompensationEffect.Cancel();
                _climbMoveEffect.Cancel();
            }

            #endregion

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
            {
                movementInput += new Vector2(-MovementSpeed, 0);
            }


            if (_playerInput.ShouldMoveRight())
            {
                movementInput += new Vector2(MovementSpeed, 0);
            }

            if (IsClimbing())
            {
                if (_playerInput.ShouldClimbUp())
                {
                    if(_climbMoveEffect.IsCancelled() || _climbMoveEffect.TypeId != ClimbUpEffectId)
                    {

                        _climbMoveEffect = new VelocityEffect(new Vector2(0, -ClimbUpSpeed))
                        {
                            TypeId = ClimbUpEffectId
                        };
                        _locomotionBody.AddVelocityEffect(_climbMoveEffect);
                    }
                } else if (_playerInput.ShouldClimbDown())
                {
                    
                    if(_climbMoveEffect.IsCancelled() || _climbMoveEffect.TypeId != ClimbDownEffectId)
                    {

                        _climbMoveEffect = new VelocityEffect(new Vector2(0, ClimbDownSpeed))
                        {
                            TypeId = ClimbDownEffectId
                        };
                        _locomotionBody.AddVelocityEffect(_climbMoveEffect);
                    }
                }
                else
                {
                    _climbMoveEffect.Cancel();
                }
            }

            _locomotionBody.Move(movementInput);
            
            #endregion

            #region Grounded Value Reset

            if (_locomotionBody.IsGrounded() && _verticalMovementEffect.IsCancelled())
            {
                _remainingClimbTime = MaxClimbTime;
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

                CancelAllVerticalEffects();
                
                _verticalMovementEffect = new LinearDecayingVelocityEffect(direction, modifier, true);
                _locomotionBody.AddVelocityEffect(_verticalMovementEffect);
            }

            #endregion

            #region Wall Sliding

            if (_verticalMovementEffect.IsCancelled())
            {
                if (_playerInput.ShouldGrab() && _locomotionBody.TouchingAnyWall())
                {
                    CancelAllVerticalEffects();
                    _verticalMovementEffect = new VelocityEffect(new Vector2(0, SlideSpeed), true)
                    {
                        TypeId = SlideEffectId
                    };
                    
                    _climbSlideCompensationEffect = new VelocityEffect(new Vector2(0, -SlideSpeed), true);

                    _locomotionBody.AddVelocityEffect(_verticalMovementEffect);
                    _locomotionBody.AddVelocityEffect(_climbSlideCompensationEffect);

                    _slidingRight = _locomotionBody.IsWallAtRight();
                }
            }

            if (IsSliding() && (!_locomotionBody.TouchingAnyWall() || _locomotionBody.IsGrounded()))
            {
                CancelAllVerticalEffects();
            }

            #endregion
        }

        private bool IsClimbing()
        {
            return !_climbSlideCompensationEffect.IsCancelled();
        }

        private bool CoyoteGraceTimeActive()
        {
            return _timeSinceLastGround <= (_lastSlideOrGround ? CoyoteJumpGraceTime : CoyoteWallJumpGraceTime);
        }

        private bool IsSliding()
        {
            return !_verticalMovementEffect.IsCancelled() && _verticalMovementEffect.TypeId == SlideEffectId;
        }

        private void CancelAllVerticalEffects()
        {
            _verticalMovementEffect.Cancel();
            _climbSlideCompensationEffect.Cancel();
            _climbMoveEffect.Cancel();
        }

        public void PostBodyUpdate(float deltaTime)
        {
            if (!_locomotionBody.IsCeilingAtHead() || IsSliding()) return;
            _verticalMovementEffect.Cancel();
        }
    }
}