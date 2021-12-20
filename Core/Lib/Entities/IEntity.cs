using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;

namespace Core.Lib.Entities
{
    public interface IEntity: IUpdateable, IDrawable
    {
        public Transform2 Transform { get; }
        public void LoadContent(ContentManager contentManager);
    }
}