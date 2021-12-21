using Microsoft.Xna.Framework.Input;

namespace Core.Lib.Input
{
    public class KeyWatcher
    {
        private readonly Keys _key;
        public bool KeyPressedThisFrame { get; private set; }
        public bool KeyReleasedThisFrame { get; private set; }
        private bool _wasDownLastFrame;

        public KeyWatcher(Keys key)
        {
            _key = key;
        }

        public void Update(float deltaTime, KeyboardState keyboardState)
        {
            var isDown = keyboardState.IsKeyDown(_key);

            KeyPressedThisFrame = isDown && !_wasDownLastFrame;
            KeyReleasedThisFrame = !isDown && _wasDownLastFrame;

            _wasDownLastFrame = isDown;
        }
    }
}