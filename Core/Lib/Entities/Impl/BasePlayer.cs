using Core.Lib.Entities.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core.Lib.Entities.Impl
{
    public class BasePlayer: BaseLivingEntity<BasePlayer>, IPlayer
    {
        #nullable enable
        private GameLevel? _currentLevel;
        public GameLevel? SwitchLevel(GameLevel gameLevel)
        {
            var old = _currentLevel;
            old?.RemoveEntity(this);
            _currentLevel = gameLevel;
            _currentLevel!.AddEntity(this);
            return old;
        }

        public BasePlayer(RendererRegistry rendererRegistry) : base(rendererRegistry.GetRenderer<IPlayer>())
        {
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            var keyboardState = Keyboard.GetState();

            // the camera properties of the camera can be conrolled to move, zoom and rotate
            const float movementSpeed = 200;
            const float rotationSpeed = 0.5f;
            const float zoomSpeed = 0.5f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                Move(new Vector2(0, -movementSpeed) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                Move(new Vector2(-movementSpeed, 0) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                Move(new Vector2(0, movementSpeed) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                Move(new Vector2(movementSpeed, 0) * deltaTime);
        }
    }
}