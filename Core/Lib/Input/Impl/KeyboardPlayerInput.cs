using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Core.Lib.Input.Impl
{
    public class KeyboardPlayerInput: IPlayerInput
    {
        private bool _shouldMoveLeft;
        private bool _shouldMoveRight;
        private bool _shouldJump;

        private readonly KeyWatcher[] jumpKeys =
        {
            new KeyWatcher(Keys.W),
            new KeyWatcher(Keys.Up),
            new KeyWatcher(Keys.Space),
        };
        
        public void Update(float deltaTime)
        {
            var keyboardState = Keyboard.GetState();
            
            // Update keys
            foreach (var watcher in jumpKeys)
            {
                watcher.Update(deltaTime, keyboardState);
            }
            
            _shouldMoveLeft = keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left);
            _shouldMoveRight = keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right);
            _shouldJump = jumpKeys.Any(watcher => watcher.KeyPressedThisFrame);
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
    }
}