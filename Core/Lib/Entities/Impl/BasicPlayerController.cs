using Core.Lib.Input;
using Core.Lib.Physics.Locomotion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core.Lib.Entities.Impl
{
    public class BasicPlayerController: IPlayerController
    {
        private readonly LocomotionBody _locomotionBody;
        private readonly IPlayerInput _playerInput;
        private VelocityEffect jumpEffect = new VelocityEffect(new Vector2());
        private int maxJumps = 2;
        private int currentJumps;

        public BasicPlayerController(LocomotionBody locomotionBody, IPlayerInput playerInput)
        {
            _locomotionBody = locomotionBody;
            _playerInput = playerInput;
            currentJumps = maxJumps;
        }

        public void Update(float deltaTime)
        {

            const float movementSpeed = 100;
            
            Vector2 movementInput = new Vector2();

            if (_playerInput.ShouldMoveLeft())
                movementInput += new Vector2(-movementSpeed, 0);


            if (_playerInput.ShouldMoveRight())
                movementInput += new Vector2(movementSpeed, 0);

            _locomotionBody.Move(movementInput);

            if (_locomotionBody.IsGrounded() && jumpEffect.IsCancelled())
            {
                currentJumps = maxJumps;
            }
            
            if (currentJumps > 0 && _playerInput.ShouldJump())
            {
                currentJumps--;
                jumpEffect.Cancel();
                jumpEffect = new LinearDecayingVelocityEffect(new Vector2(0, -200f), new Vector2(0, 70f), true);
                _locomotionBody.AddVelocityEffect(jumpEffect);
            }
        }

        public void PostBodyUpdate(float deltaTime)
        {
            if (_locomotionBody.IsCeilingAtHead())
            {
                jumpEffect.Cancel();
            }
        }
    }
}