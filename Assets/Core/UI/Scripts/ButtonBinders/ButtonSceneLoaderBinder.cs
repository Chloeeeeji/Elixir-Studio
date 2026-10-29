using UnityEngine;

namespace GameSystemsCookbook
{
    /// <summary>
    /// Connects a UI Toolkit Button to a StringEventChannelSO ScriptableObject that requests a scene load.
    /// </summary>
    public class ButtonSceneLoadBinder : ButtonEventBinder<StringEventChannelSO>
    {
        [Header("Broadcast on Event Channel")]
        [Tooltip("Scene path to load.")]
        [SerializeField] string m_ScenePath = "";

        [Tooltip("The event channel to raise.")]
        [SerializeField] StringEventChannelSO m_SceneLoadChannel;

        protected override void RaiseEvent()
        {
            if (!string.IsNullOrEmpty(m_ScenePath) && m_SceneLoadChannel != null)
                m_SceneLoadChannel.RaiseEvent(m_ScenePath);
        }
    }
}