namespace Core.Lib.Entities
{
    public interface IPlayerController: IUpdateable
    {
        public void PostBodyUpdate(float deltaTime);
    }
}