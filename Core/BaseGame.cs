using System.Text;
using Core.Lib;
using Core.Lib.Entities.Rendering;
using Core.Lib.Entities.Rendering.Impl;
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
        private CameraController _cameraController;
        private Vector2 _worldPosition;
        private BitmapFont _bitmapFont;
        private RendererRegistry _rendererRegistry = new RendererRegistry();

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
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();
            
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, (int)_baseScreenSize.X, (int)_baseScreenSize.Y);
            _camera = new OrthographicCamera(viewportAdapter);

            _cameraController = new CameraController(_camera, 3);
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("Fonts/montserrat-32");
           
            // Register renderers here
            _rendererRegistry.RegisterRenderer(new PlayerRenderer());
            
            
            foreach (var renderer in _rendererRegistry.AllRenderers)
            {
                renderer.LoadContent(Content);
            }
            
            LoadActiveScene();
        }

        private void LoadActiveScene()
        {
            _activeScene.Load(_spriteBatch, Content);
            _activeScene.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var mouseState = Mouse.GetState();
            
            _activeScene.Update(deltaTime);
            _cameraController.Update(deltaTime);
            _worldPosition = _camera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_cameraController.BackColor);

            var transformMatrix = _camera.GetViewMatrix();
            // TODO add camera transposition
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            
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
            _activeScene = scene;
            LoadActiveScene();
        }

        public RendererRegistry EntityRendererRegistry => _rendererRegistry;

        public CameraController CameraController => _cameraController;
    }
}
