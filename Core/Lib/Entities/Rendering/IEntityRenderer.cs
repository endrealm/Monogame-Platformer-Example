using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Lib.Entities.Rendering
{
    public interface IEntityRenderer
    {
        void LoadContent(ContentManager contentManager);
    }
    public interface IEntityRenderer<in T>:IEntityRenderer where T: IEntity
    {
        public void Update(float deltaTime, T entity);
        void Render(SpriteBatch spriteBatch, T entity);
    }
}