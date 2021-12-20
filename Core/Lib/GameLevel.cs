using System.Collections.Generic;
using System.Linq;
using Core.Lib.Entities;
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

        public GameLevel(LDtkLevel level)
        {
            _level = level;
        }
        
        public void AddEntity(IEntity entity)
        {
            _entities.Add(entity);
        }
        
        public void RemoveEntity(IEntity entity)
        {
            _entities.Remove(entity);
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in _entities.ToArray())
            {
                entity.Update(deltaTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var render in _level.Layers)
            {
                spriteBatch.Draw(render, _level.Position, Color.White);
            }

            foreach (var entity in _entities)
            {
                entity.Draw(spriteBatch);
            }
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
    }
}