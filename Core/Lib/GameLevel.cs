using System.Collections.Generic;
using Core.Lib.Entities;
using LDtk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
            foreach (var entity in _entities)
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

        public void Start()
        {
            
        }

        public string GetId()
        {
            return _level.Identifier;
        }
    }
}