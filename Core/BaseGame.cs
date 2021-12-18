using Core.Lib;
using Core.Lib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class BaseGame : Game, ISceneManager
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private int _backbufferWidth, _backbufferHeight;
        private readonly Vector2 _baseScreenSize = new Vector2(800, 480);
        private Matrix _globalTransformation;
        private IScene _activeScene;

        protected BaseGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _activeScene = new MenuScene(_baseScreenSize);
            _activeScene.SetSceneManager(this);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _activeScene.Load(_spriteBatch, Content);

            ScalePresentationArea();
            
            _activeScene.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            _activeScene.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        private void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            _backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            _backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            var horScaling = _backbufferWidth / _baseScreenSize.X;
            var verScaling = _backbufferHeight / _baseScreenSize.Y;
            var screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            _globalTransformation = Matrix.CreateScale(screenScalingFactor);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            
            // TODO add camera transposition
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null,null, _globalTransformation);
            
            _activeScene.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void LoadScene(IScene scene)
        {
            scene.SetSceneManager(this);
            scene.Load(_spriteBatch, Content);
            scene.Start();
            _activeScene = scene;
        }
    }
}
