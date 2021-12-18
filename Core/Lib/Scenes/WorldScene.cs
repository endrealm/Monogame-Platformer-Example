using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using LDtk;

namespace Core.Lib.Scenes
{
    
    public class WorldScene: BaseScene
    {
        private GameLevel _level;

        private readonly string worldName;

        public WorldScene(Vector2 dimensions, string worldName): base(dimensions)
        {
            this.worldName = worldName;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _level.Draw(spriteBatch);
        }

        public override void Update(float deltaTime)
        {
            _level.Update(deltaTime);

        }

        public override void Start()
        {
            _level.Start();
        }

        protected override void LoadContent(SpriteBatch spriteBatch, ContentManager contentManager)
        {
            var world = contentManager.Load<LDtkWorld>("GameWorld.ldtk"); 
            _level = new GameLevel(world.GetLevel("Start"));
            
            _level.Load(contentManager);
        }
    }
}