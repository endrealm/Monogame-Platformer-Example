using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Core.Lib.Input.Impl
{
    public class KeyboardPlayerInput: IPlayerInput
    {
        private bool _shouldMoveLeft;
        private bool _shouldMoveRight;
        private bool _shouldClimbUp;
        private bool _shouldClimbDown;
        private bool _shouldGrab;
        private bool _shouldJump;

        private readonly KeyWatcher[] _jumpKeys =
        {
            new KeyWatcher(Keys.Space),
        };
        
        public void Update(float deltaTime)
        {
            var keyboardState = Keyboard.GetState();
            
            // Update keys
            foreach (var watcher in _jumpKeys)
            {
                watcher.Update(deltaTime, keyboardState);
            }
            
            _shouldClimbUp = keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up);
            _shouldClimbDown = keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down);
            _shouldMoveLeft = keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left);
            _shouldMoveRight = keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right);
            _shouldGrab = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            _shouldJump = _jumpKeys.Any(watcher => watcher.KeyPressedThisFrame);
        }

        public bool ShouldJump()
        {
            return _shouldJump;
        }

        public bool ShouldMoveLeft()
        {
            return _shouldMoveLeft;
        }

        public bool ShouldMoveRight()
        {
            return _shouldMoveRight;
        }

        public bool ShouldGrab()
        {
            return _shouldGrab;
        }

        public bool ShouldClimbUp()
        {
            return _shouldClimbUp;
        }

        public bool ShouldClimbDown()
        {
            return _shouldClimbDown;
        }
    }
}