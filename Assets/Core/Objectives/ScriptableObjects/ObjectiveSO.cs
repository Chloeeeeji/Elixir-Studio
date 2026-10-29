using UnityEngine;

namespace GameSystemsCookbook
{

    /// <summary>
    /// Base class for a ScriptableObject-based game objective. Implement the logic within each concrete
    /// class and customize the win/lose conditions.
    ///
    /// Common objectives might include:
    ///
    ///     -Reaching a score goal (example shown here)
    ///     -Defeating a specific number of enemies
    ///     -Reaching a specific location
    ///     -Picking up a specific item
    ///     -Complete a task within a timeframe
    ///
    ///  Combine them in the ObjectiveManager more interesting results (e.g. pickup x number of items within n seconds)
    /// 
    /// </summary>

    public class ObjectiveSO : DescriptionSO
    {
        [Space]
        [Tooltip("On-screen name")]
        [SerializeField] private string m_Title;

        // Unused in this demo, but could be useful for creating secondary objectives for quests,
        // achievements, etc.
        //[Tooltip("Is the objective required to win")]
        //[SerializeField] private bool m_IsOptional;

        [Header("Broadcast on Event Channels")]
        [Tooltip("Event sent that objective is complete")]
        [SerializeField] private VoidEventChannelSO m_ObjectiveCompleted;

        // Unused here, but could track if a primary objective fails
        //[Tooltip("Signal that we cannot complete objective (optional)")]
        //[SerializeField] private ObjectiveEventChannelSO m_ObjectiveFailed;

        private bool m_IsCompleted;

        // Properties
        public bool IsCompleted => m_IsCompleted;
        public VoidEventChannelSO ObjectiveComplete => m_ObjectiveCompleted;

        // Methods

        // Called by the ObjectiveManager when it starts, rather than from Awake or OnEnable.
        // A ScriptableObject's Awake and OnEnable run when the asset loads -- in the Editor
        // outside Play mode, unordered against scene load, and skipped entirely when Fast
        // Enter Play mode disables the domain reload. Derived objectives override this to
        // acquire whatever they need.
        public virtual void Initialize()
        {
            NullRefChecker.Validate(this);
        }

        // Called by the ObjectiveManager when it stops. Release anything Initialize acquired.
        public virtual void Cleanup()
        {
        }

        // Completes the current ObjectiveSO and notifies any listeners (e.g. the ObjectiveManager)
        protected virtual void CompleteObjective()
        {
            m_IsCompleted = true;
            m_ObjectiveCompleted.RaiseEvent();
        }
 
        public void ResetObjective()
        {
            m_IsCompleted = false;
        }

        // Not used in this demo, but could be useful in a different game
        //protected virtual void FailObjective()
        //{
        //    m_IsComplete = false;
        //    m_ObjectiveFailed.RaiseEvent();
        //}

    }
}
