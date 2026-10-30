using UnityEngine;

namespace GameSystemsCookbook
{
    /// <summary>
    /// Connects a UI Toolkit Button to an IntEventChannelSO.
    /// </summary>
    public class ButtonIntEventChannelBinder : ButtonEventBinder<IntEventChannelSO>
    {
        [Header("Broadcast on Event Channel")]
        [Tooltip("The event channel to raise.")]
        [SerializeField] IntEventChannelSO m_EventChannel;

        [Tooltip("Integer value to send with the event.")]
        [SerializeField] int m_Value;

        protected override void RaiseEvent()
        {
            if (m_EventChannel != null)
                m_EventChannel.RaiseEvent(m_Value);
        }
    }
}
