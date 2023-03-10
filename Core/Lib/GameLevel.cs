using System;
using System.Collections.Generic;
using System.Linq;
using Core.Lib.Data;
using Core.Lib.Entities;
using Core.Lib.Physics;
using LDtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Core.Lib
{
    public class GameLevel: IUpdateable, IDrawable
    {
        private readonly LDtkLevel _level;
        private HashSet<IEntity> _entities = new HashSet<IEntity>();
        private CollisionManager _collisionManager;

        public GameLevel(LDtkLevel level)
        {
            _level = level;
            _collisionManager = new CollisionManager(new RectangleF(
                level.Position,
                level.Size
            ));

            LoadStaticColliders();
            foreach (var spawnPoint in _level.GetEntities<SpawnPointLDtkEntity>("Spawn_Zone"))
            {
                _collisionManager.Insert(new SpawnPointCollisionTarget(
                    new RectangleF(spawnPoint.Position, spawnPoint.Size),
                    spawnPoint.Spawn * new Vector2(8,8) + level.Position
                    )
                );
            }
        }

        public CollisionManager CollsionManager => _collisionManager;

        private void LoadStaticColliders()
        {
            foreach (var layerData in _level.LayerData.Where(data => data != null))
            {
                for (var x = 0; x < layerData.TileData.Length; x++)
                {
                    var columnList = layerData.TileData[x];
                    for (var y = 0; y < columnList.Length; y++)
                    {
                        var data = columnList[y];
                        if(data == null) continue;
                        
                        if(!data.EnumData.EnumName.Equals("CollisionMaterial")) continue;

                        if (data.EnumData.Values.Contains("Deadly"))
                        {
                            _collisionManager.Insert(new SpikeCollisionTarget(new RectangleF(_level.Position + new Vector2(x, y) * layerData.GridCellSize + new Vector2(1.5f, 1.5f), new Vector2(1,1) * layerData.GridCellSize - new Vector2(3, 3))));
                            continue;
                        }
                        
                        _collisionManager.Insert(new StaticCollisionTarget(new RectangleF(_level.Position + new Vector2(x, y) * layerData.GridCellSize, new Vector2(1,1) * layerData.GridCellSize)));
                    }
                }
            }
        }

        public void AddEntity(IEntity entity)
        {
            _entities.Add(entity);
            
            if (entity is ICollisionTarget target)
            {
                _collisionManager.Insert(target);
            }
        }
        
        public void RemoveEntity(IEntity entity)
        {
            _entities.Remove(entity);
            
            if (entity is ICollisionTarget target)
            {
                _collisionManager.Remove(target);
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in _entities.ToArray())
            {
                entity.Update(deltaTime);
            }
            
            _collisionManager.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var index = _level.Layers.Length - 1; index >= 0; index--)
            {
                var render = _level.Layers[index];
                spriteBatch.Draw(render, _level.Position, Color.White);
            }

            foreach (var entity in _entities)
            {
                entity.Draw(spriteBatch);
            }

            _collisionManager.DebugDraw(spriteBatch);
        }

        public void Load(ContentManager contentManager)
        {
            foreach (var entity in _entities)
            {
                entity.LoadContent(contentManager);
            }
        }

        public void Start(CameraController cameraController)
        {
            cameraController.UpdateBounds(_level.Position, _level.Size);
        }

        public string GetId()
        {
            return _level.Identifier;
        }

        public RectangleF GetBoundingRect()
        {
            return new RectangleF(_level.Position, _level.Size);
        }

        public LDtkLevel GetLevel()
        {
            return _level;
        }
    }
}