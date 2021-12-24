using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace Core.Lib.Entities.Rendering.Impl
{
    public class PlayerRenderer: IEntityRenderer<IPlayer>
    {
        private AnimatedSprite sprite;
        private bool _facingRight = false;
        
        public void LoadContent(ContentManager contentManager)
        {
            var doc = contentManager.Load<AsepriteDocument>("Entities/player");
            sprite = new AnimatedSprite(doc);
        }
        
        public void Update(float deltaTime, IPlayer entity)
        {
            sprite.Update(deltaTime);
            var xMove = entity.GetLocomotionBody().GetLastMovementSmoothed().X;

            if (xMove > 0)
            {
                _facingRight = true;
            } else if (xMove < 0)
            {
                _facingRight = false;
            }
        }
        
        public void Render(SpriteBatch spriteBatch, IPlayer entity)
        {
            sprite.Position = entity.Transform.WorldPosition - sprite.CurrentFrame.Bounds.Size.ToVector2()/2;
            sprite.Scale = entity.Transform.WorldScale;
            sprite.SpriteEffect = !_facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            sprite.Rotation = entity.Transform.WorldRotation;
            sprite.Render(spriteBatch);
        }

    }
}