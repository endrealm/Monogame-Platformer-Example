namespace Core.Lib.Input
{
    public interface IPlayerInput: IUpdateable
    {
        bool ShouldJump();
        bool ShouldMoveLeft();
        bool ShouldMoveRight();
        bool ShouldGrab();
        bool ShouldClimbUp();
        bool ShouldClimbDown();
    }
}