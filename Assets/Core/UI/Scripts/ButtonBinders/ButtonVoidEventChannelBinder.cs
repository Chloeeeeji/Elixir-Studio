using UnityEngine;

namespace GameSystemsCookbook
{
    /// <summary>
    /// Connects a UI Toolkit Button to a VoidEventChannelSO.
    /// </summary>
    public class ButtonVoidEventChannelBinder : ButtonEventBinder<VoidEventChannelSO>
    {
        [Header("Broadcast on Event Channel")] [Tooltip("The event channel to raise.")] [SerializeField]
        VoidEventChannelSO m_EventChannel;

        protected override void RaiseEvent()
        {
            if (m_EventChannel != null)
                m_EventChannel.RaiseEvent();
        }
    }
}
