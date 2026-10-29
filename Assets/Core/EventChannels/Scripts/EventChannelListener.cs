using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameSystemsCookbook
{
    /// <summary>
    /// Generic listener for any payload-based EventChannelSO<T>.
    /// </summary>
    /// <typeparam name="T">The type of data carried by the channel.</typeparam>
    public abstract class EventChannelListener<T> : MonoBehaviour
    {
        [Header("Listen To Event Channel")]
        [Tooltip("The channel asset to listen on")]
        [SerializeField] protected GenericEventChannelSO<T> m_EventChannel;

        [Space]
        [Tooltip("Invoked when the channel raises an event")]
        [SerializeField] protected UnityEvent<T> m_Response;

        [Tooltip("Optional delay before invoking the response")]
        [SerializeField] protected float m_Delay;

        protected virtual void OnEnable()
        {
            if (m_EventChannel != null)
                m_EventChannel.OnEventRaised += HandleEvent;
        }

        protected virtual void OnDisable()
        {
            if (m_EventChannel != null)
                m_EventChannel.OnEventRaised -= HandleEvent;
        }

        private void HandleEvent(T payload)
        {
            if (m_Delay > 0f)
                StartCoroutine(RaiseResponseDelayed(payload));
            else
                m_Response.Invoke(payload);
        }

        private IEnumerator RaiseResponseDelayed(T payload)
        {
            yield return new WaitForSeconds(m_Delay);
            m_Response.Invoke(payload);
        }
    }
}