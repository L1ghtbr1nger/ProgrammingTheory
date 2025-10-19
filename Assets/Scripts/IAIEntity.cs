using UnityEngine;

public interface IAIEntity
{
    /// <summary>
    /// Perform the AI logic for this entity (attacks, movement, abilities, etc.)
    /// </summary>
    void PerformAIAction();

    /// <summary>
    /// Get the current target object for AI to interact with.
    /// Could be a player, object, or another target in the environment.
    /// </summary>
    GameObject GetTargetObject();
}
