using GameSystemsCookbook;
using UnityEngine;

namespace GameSystemsCookbook
{
    public class ButtonVector2EventChannelBinder : ButtonEventBinder<Vector2EventChannelSO>
    {
        [Header("Broadcast on Event Channel")] [Tooltip("The event channel to raise.")] [SerializeField]
        Vector2EventChannelSO m_EventChannel;

        [Tooltip("Vector2 value to send with the event.")] [SerializeField]
        Vector2 m_Value;

        protected override void RaiseEvent()
        {
            if (m_EventChannel != null)
                m_EventChannel.RaiseEvent(m_Value);
        }
    }
}
