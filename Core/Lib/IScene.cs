using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Lib
{
    public interface IScene: IDrawable, IUpdateable
    {
        public void Load(SpriteBatch spriteBatch, ContentManager contentManager);
        public void SetSceneManager(ISceneManager sceneManager);
        void Start();
    }
}