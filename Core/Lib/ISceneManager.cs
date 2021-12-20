using Core.Lib.Entities.Rendering;

namespace Core.Lib
{
    public interface ISceneManager
    {
        void LoadScene(IScene scene);
        RendererRegistry EntityRendererRegistry { get; }
    }
}