using Core.Lib.Physics;
using Microsoft.Xna.Framework;

namespace Core.Lib.Entities
{
    public interface IPlayer: ILivingEntity, ICollisionTarget
    {
        /// <summary>
        /// Switches the currently assigned level
        /// </summary>
        /// <param name="gameLevel">the new level</param>
        /// <returns>the old level or null</returns>
        GameLevel? SwitchLevel(GameLevel gameLevel);

        RaycastContext GetRaycastContext();
        
        Vector2 HalfSize { get; }
    }
}