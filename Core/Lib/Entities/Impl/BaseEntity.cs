using Core.Lib.Entities.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Core.Lib.Entities.Impl
{
    /// <summary>
    /// The base implementation for entities
    /// </summary>
    /// <typeparam name="T">The top level class using this</typeparam>
    public class BaseEntity<T>: IEntity where T:BaseEntity<T>
    {
        public Transform2 Transform { get; } = new Transform2();
        protected readonly IEntityRenderer<T> Renderer;
        private bool _loaded;

        public BaseEntity(IEntityRenderer<T> renderer)
        {
            Renderer = renderer;
        }

        public virtual void Update(float deltaTime)
        {
            Renderer.Update(deltaTime, (T) this);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Renderer.Render(spriteBatch, (T) this);
        }

        public void LoadContent(ContentManager contentManager)
        {
            // Does nothing additional by default. See renderers
        }
        
        protected void Move(Vector2 deltaPos)
        {
            Transform.Position += deltaPos;
        }
    }
}