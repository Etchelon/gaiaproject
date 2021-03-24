using System.Collections.Generic;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;

namespace GaiaProject.Engine.Logic.Abstractions
{
    /// <summary>
    /// Interface implemented by classes than can handle actions
    /// </summary>
    public interface IHandleAction
    {
        /// <summary>
        /// Handles an action and returns the effects caused by the action
        /// </summary>
        /// <param name="game">The game where the action was taken</param>
        /// <param name="action">The action to apply</param>
        /// <returns></returns>
        List<Effect> Handle(GaiaProjectGame game, PlayerAction action);

        /// <summary>
        /// Enables or disables action validation (e.g.: off when reconstructing the game state by reapplying all actions)
        /// </summary>
        /// <returns></returns>
        void ToggleActionValidation(bool on);
    }
}
