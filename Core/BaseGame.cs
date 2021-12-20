using System.Text;
using Core.Lib;
using Core.Lib.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;

namespace Core
{
    public class BaseGame : Game, ISceneManager
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private readonly Vector2 _baseScreenSize = new Vector2(1920, 1080);
        private IScene _activeScene;
        private OrthographicCamera _camera;
        private Vector2 _worldPosition;
        private BitmapFont _bitmapFont;

        protected BaseGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _activeScene = new MenuScene(_baseScreenSize);
            _activeScene.SetSceneManager(this);
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, (int)_baseScreenSize.X, (int)_baseScreenSize.Y);
            _camera = new OrthographicCamera(viewportAdapter);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("Fonts/montserrat-32");
            // TODO: use this.Content to load your game content here
            _activeScene.Load(_spriteBatch, Content);
            
            _activeScene.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // the camera properties of the camera can be conrolled to move, zoom and rotate
            const float movementSpeed = 200;
            const float rotationSpeed = 0.5f;
            const float zoomSpeed = 0.5f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                _camera.Move(new Vector2(0, -movementSpeed) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                _camera.Move(new Vector2(-movementSpeed, 0) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                _camera.Move(new Vector2(0, movementSpeed) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                _camera.Move(new Vector2(movementSpeed, 0) * deltaTime);

            if (keyboardState.IsKeyDown(Keys.E))
                _camera.Rotation += rotationSpeed * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Q))
                _camera.Rotation -= rotationSpeed * deltaTime;

            if (keyboardState.IsKeyDown(Keys.R))
                _camera.ZoomIn(zoomSpeed * deltaTime);

            if (keyboardState.IsKeyDown(Keys.F))
                _camera.ZoomOut(zoomSpeed * deltaTime);

            _worldPosition = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));
            
            _activeScene.Update(deltaTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var transformMatrix = _camera.GetViewMatrix();
            // TODO add camera transposition
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            
            _activeScene.Draw(_spriteBatch);

            _spriteBatch.End();

            
            DebugCamera();

            base.Draw(gameTime);
        }

        private void DebugCamera()
        {
            // not all sprite batches need to be affected by the camera
            var rectangle = _camera.BoundingRectangle;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"WASD: Move [{_camera.Position.X:0}, {_camera.Position.Y:0}]");
            stringBuilder.AppendLine($"EQ: Rotate [{MathHelper.ToDegrees(_camera.Rotation):0.00}]");
            stringBuilder.AppendLine($"RF: Zoom [{_camera.Zoom:0.00}]");
            stringBuilder.AppendLine($"World Pos: [{_worldPosition.X:0}, {_worldPosition.Y:0}]");
            stringBuilder.AppendLine($"Bounds: [{rectangle.X:0}, {rectangle.Y:0}, {rectangle.Width:0}, {rectangle.Height:0}]");

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            _spriteBatch.DrawString(_bitmapFont, stringBuilder.ToString(), new Vector2(5, 5), Color.DarkBlue);
            _spriteBatch.End();
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
