﻿namespace Core.Lib.Entities
{
    public interface IPlayer: ILivingEntity
    {
        /// <summary>
        /// Switches the currently assigned level
        /// </summary>
        /// <param name="gameLevel">the new level</param>
        /// <returns>the old level or null</returns>
        GameLevel? SwitchLevel(GameLevel gameLevel);
    }
}