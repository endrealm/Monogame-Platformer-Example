using System.Linq;
using Core.Lib.Entities.Rendering;
using Core.Lib.Input;
using Core.Lib.Input.Impl;
using Core.Lib.Physics;
using Core.Lib.Physics.Locomotion;
using Core.Lib.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib.Entities.Impl
{
#nullable enable
    public class BasePlayer: BaseLivingEntity<BasePlayer>, IPlayer
    {
        private readonly WorldScene _worldScene;
        private GameLevel? _currentLevel;
        private readonly Vector2 _size = new Vector2(16, 24);
        private readonly Vector2 _halfSize = new Vector2(16, 24)/2;
        public Vector2 HalfSize => _halfSize;
        public void SetSpawn(Vector2 spawnPoint)
        {
            _lastCheckpoint = spawnPoint - new Vector2(-_size.X / 2, _size.Y / 2);
        }

        private readonly IPlayerInput _playerInput;
        private readonly LocomotionBody _locomotionBody;
        private readonly IPlayerController _playerController;
        
        private Vector2 _lastCheckpoint;

        public BasePlayer(WorldScene worldScene, RendererRegistry rendererRegistry, Vector2 initialSpawn) : base(rendererRegistry.GetRenderer<IPlayer>(), 1)
        {
            SetSpawn(initialSpawn);
            Transform.Position = _lastCheckpoint;
            
            _worldScene = worldScene;
            _playerInput = new KeyboardPlayerInput();
            _locomotionBody = new PlayerLocomotionBody(this, Transform);
            _playerController = new BasicPlayerController(_locomotionBody, _playerInput);
        }
        
        public GameLevel? SwitchLevel(GameLevel gameLevel)
        {
            var old = _currentLevel;
            old?.RemoveEntity(this);
            _currentLevel = gameLevel;
            _currentLevel!.AddEntity(this);
            return old;
        }

        public RaycastContext? GetRaycastContext()
        {
            return _currentLevel?.CollsionManager ?? null;
        }
        
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
            _playerInput.Update(deltaTime);
            
            _playerController.Update(deltaTime);

            _locomotionBody.Update(deltaTime);
            
            _playerController.PostBodyUpdate(deltaTime);
            
            
            CheckAndChangeRoom();
        }

        public override bool Die()
        {
            FullHeal();
            Transform.Position = _lastCheckpoint;
            CheckAndChangeRoom();
            return false;
        }

        private void CheckAndChangeRoom()
        {
            // Detect if room changed
            var center = BodyCenter;

            // Still same room so ignore
            if (_currentLevel?.GetBoundingRect().Contains(center) ?? true) return;
            var newLevel = _worldScene.Levels.FirstOrDefault(level => level.GetBoundingRect().Contains(center));
            if (newLevel == null) return;
            _worldScene.ChangeActiveLevel(newLevel);
            SwitchLevel(newLevel);
        }

        public IShapeF Bounds => new RectangleF(Transform.WorldPosition - _halfSize, _size);
        public bool StaticCollider => false;
        public bool TriggerOnly => false;

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            var other = collisionInfo.Other;
            if (other.TriggerOnly)
            {
                return;
            }
            
            Transform.Position -= collisionInfo.PenetrationVector;
        }
    }
}