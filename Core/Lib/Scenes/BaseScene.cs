using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Lib.Scenes
{
    public abstract class BaseScene: IScene
    {
        protected readonly Vector2 Dimensions;
        protected ISceneManager SceneManager { get; private set; }
        private bool Loaded { get; set; }

        protected BaseScene(Vector2 dimensions)
        {
            this.Dimensions = dimensions;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update(float deltaTime);
        public void Load(SpriteBatch spriteBatch, ContentManager contentManager)
        {
            if(Loaded) return;
            LoadContent(spriteBatch, contentManager);
            Loaded = true;
        }

        public void SetSceneManager(ISceneManager sceneManager)
        {
            this.SceneManager = sceneManager;
        }

        public abstract void Start();

        protected abstract void LoadContent(SpriteBatch spriteBatch, ContentManager contentManager);
    }
}