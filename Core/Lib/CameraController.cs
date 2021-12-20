using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Core.Lib
{
    public class CameraController
    {
        private readonly OrthographicCamera _camera;
        private RectangleF? _boundingBox = null;
        private bool centeredLastFrame;
        public CameraController(OrthographicCamera camera)
        {
            _camera = camera;
        }

        public void UpdateBounds(Vector2 offset, Vector2 shape)
        {
            _boundingBox = new RectangleF(offset, shape);
        }

        public void Update(float deltaTime)
        {
            var insideArea = _boundingBox?.IsInside(_camera.BoundingRectangle) ?? false;

            if (!insideArea)
            {
                if(centeredLastFrame || _boundingBox == null) return;
                
                var boundingCenter = _boundingBox!.Value.Center;
                _camera.Position = new Vector2(boundingCenter.X, boundingCenter.Y) - (_camera.Center - _camera.Position);
                centeredLastFrame = true;
                return;
            }

            centeredLastFrame = false;

        }
    }
}