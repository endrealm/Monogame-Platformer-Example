using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib
{
    public class CameraController
    {
        private readonly OrthographicCamera _camera;
        private readonly float _defaultZoom;
        private RectangleF? _boundingBox = null;
        private bool centeredLastFrame;
        public Color BackColor { get; private set; } = Color.CornflowerBlue;
        public CameraController(OrthographicCamera camera, float defaultZoom)
        {
            _camera = camera;
            _defaultZoom = defaultZoom;
            _camera.Zoom = defaultZoom;
        }

        public void UpdateBounds(Vector2 offset, Vector2 shape)
        {
            _boundingBox = new RectangleF(offset, shape);
            CenterToBoundingBox();
        }

        private void CenterToBoundingBox()
        {
            var boundingCenter = _boundingBox!.Value.Center;
            _camera.Position = new Vector2(boundingCenter.X, boundingCenter.Y) - (_camera.Center - _camera.Position);
        }

        public void Update(float deltaTime)
        {
            var insideArea = _boundingBox?.IsInside(_camera.BoundingRectangle) ?? false;

            if (!insideArea)
            {
                if(centeredLastFrame || _boundingBox == null) return;
                
                CenterToBoundingBox();

                centeredLastFrame = true;
                return;
            }

            centeredLastFrame = false;

        }

        public void ChangeColor(Color newColor)
        {
            BackColor = newColor;
        }
    }
}