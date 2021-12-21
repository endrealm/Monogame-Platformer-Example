using System.Linq;
using Core.Lib.Entities.Rendering;
using Core.Lib.Physics;
using Core.Lib.Physics.Locomotion;
using Core.Lib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Core.Lib.Entities.Impl
{
#nullable enable
    public class BasePlayer: BaseLivingEntity<BasePlayer>, IPlayer
    {
        private readonly WorldScene _worldScene;
        private GameLevel? _currentLevel;
        private readonly Vector2 size = new Vector2(32, 32);
        private readonly LocomotionBody _locomotionBody;
        // private readonly Vector2 size = new Vector2(16, 24);
        // private readonly Vector2 halfSize = new Vector2(16, 24)/2;

        public BasePlayer(WorldScene worldScene, RendererRegistry rendererRegistry) : base(rendererRegistry.GetRenderer<IPlayer>())
        {
            Transform.Position += new Vector2(40, 40);
            _worldScene = worldScene;
            _locomotionBody = new PlayerLocomotionBody(this, Transform);
        }
        
        public GameLevel? SwitchLevel(GameLevel gameLevel)
        {
            var old = _currentLevel;
            old?.RemoveEntity(this);
            _currentLevel = gameLevel;
            _currentLevel!.AddEntity(this);
            return old;
        }

        public RaycastContext GetRaycastContext()
        {
            return _currentLevel.CollsionManager;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            var keyboardState = Keyboard.GetState();

            const float movementSpeed = 200;
            Vector2 movementInput = new Vector2();

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                movementInput += new Vector2(0, -movementSpeed);

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                movementInput += new Vector2(-movementSpeed, 0);

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                movementInput += new Vector2(0, movementSpeed);

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                movementInput += new Vector2(movementSpeed, 0);

            _locomotionBody.Move(movementInput);
            _locomotionBody.Update(deltaTime);
            
            var center = BodyCenter;
            
            // Still same room so ignore
            if(_currentLevel?.GetBoundingRect().Contains(center) ?? true) return;
            var newLevel = _worldScene.Levels.FirstOrDefault(level => level.GetBoundingRect().Contains(center));
            if(newLevel == null) return;
            _worldScene.ChangeActiveLevel(newLevel);
            SwitchLevel(newLevel);
        }

        public IShapeF Bounds => new RectangleF(Transform.WorldPosition, size);
        public bool StaticCollider => false;

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            Transform.Position -= collisionInfo.PenetrationVector;
        }
    }
}