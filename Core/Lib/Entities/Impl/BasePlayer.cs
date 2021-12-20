using System.Linq;
using Core.Lib.Entities.Rendering;
using Core.Lib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Core.Lib.Entities.Impl
{
#nullable enable
    public class BasePlayer: BaseLivingEntity<BasePlayer>, IPlayer
    {
        private readonly WorldScene _worldScene;
        private GameLevel? _currentLevel;
        public GameLevel? SwitchLevel(GameLevel gameLevel)
        {
            var old = _currentLevel;
            old?.RemoveEntity(this);
            _currentLevel = gameLevel;
            _currentLevel!.AddEntity(this);
            return old;
        }

        public BasePlayer(WorldScene worldScene, RendererRegistry rendererRegistry) : base(rendererRegistry.GetRenderer<IPlayer>())
        {
            _worldScene = worldScene;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            var keyboardState = Keyboard.GetState();

            const float movementSpeed = 200;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                Move(new Vector2(0, -movementSpeed) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                Move(new Vector2(-movementSpeed, 0) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                Move(new Vector2(0, movementSpeed) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                Move(new Vector2(movementSpeed, 0) * deltaTime);

            var center = BodyCenter;
            
            // Still same room so ignore
            if(_currentLevel?.GetBoundingRect().Contains(center) ?? true) return;
            var newLevel = _worldScene.Levels.FirstOrDefault(level => level.GetBoundingRect().Contains(center));
            if(newLevel == null) return;
            _worldScene.ChangeActiveLevel(newLevel);
            SwitchLevel(newLevel);
        }
    }
}