using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core.Lib.Scenes
{
    public class MenuScene: BaseScene
    {
        public MenuScene(Vector2 dimensions) : base(dimensions)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }

        public override void Update(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                SceneManager.LoadScene(new WorldScene(Dimensions, ""));
            }
        }

        public override void Start()
        {
            
        }

        protected override void LoadContent(SpriteBatch spriteBatch, ContentManager contentManager)
        {
            
        }
    }
}