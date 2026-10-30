using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace GameSystemsCookbook
{
    public class ButtonEventBinder : MonoBehaviour
    {
        
        [Tooltip("The UI Toolkit document.")]
        [SerializeField] private UIDocument m_Document;
        [Tooltip("The name of the Button to query for.")]
        [SerializeField, Optional] private string m_ButtonID;

        [SerializeField] private UnityEvent m_Event;

        [Tooltip("Cooldown window between button clicks")]
        [SerializeField] private float m_Delay = 0.5f;

        private VisualElement m_Root;
        private Button m_Button;
        private float m_TimeToNextEvent;

        private void Awake()
        {
            Initialize();
        }

        private void Reset()
        {
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();
        }

        private void RaiseEvent()
        {
            if (Time.time < m_TimeToNextEvent)
                return;

            m_Event.Invoke();
            m_TimeToNextEvent = Time.time + m_Delay;
        }

        private void Initialize()
        {
            NullRefChecker.Validate(this);

            m_Root = m_Document.rootVisualElement;
            m_Button = m_Root.Q<Button>(m_ButtonID);

            if (m_Button == null)
            {
                Debug.LogError("UIButtonBinder: Invalid m_ButtonID", this);
                return;
            }

            m_Button.clicked += RaiseEvent;
        }

        private void OnDisable()
        {
            if (m_Button != null)
                m_Button.clicked -= RaiseEvent;
        }

    }
    
    /// <summary>
    /// Base class for binding a UI Toolkit button to a generic ScriptableObject-based event channel.
    /// </summary>
    public abstract class ButtonEventBinder<TChannel> : MonoBehaviour where TChannel : ScriptableObject
    {
        [Header("UI Elements")]
        [Tooltip("The UI Toolkit document.")]
        [SerializeField] protected UIDocument m_Document;

        [Tooltip("The name of the Button to query.")]
        [SerializeField] protected string m_ButtonID;

        [Header("Click Options")]
        [Tooltip("Cooldown window between button clicks.")]
        [SerializeField] protected float m_Delay = 0.5f;

        protected Button m_Button;
        protected float m_TimeToNextEvent;

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(m_ButtonID))
                Debug.LogError("[ButtonEventBinder]: Missing ButtonID in " + gameObject.name, this);
        }

        protected virtual void Start()
        {
            if (m_Document == null)
            {
                Debug.LogError("[ButtonEventBinder]: UIDocument is not assigned in " + gameObject.name, this);
                return;
            }

            var root = m_Document.rootVisualElement;
            m_Button = root.Q<Button>(m_ButtonID);

            if (m_Button == null)
            {
                Debug.LogError("[ButtonEventBinder]: Button not found: " + m_ButtonID, this);
                return;
            }

            m_Button.clicked += TryRaiseEvent;
        }

        protected virtual void OnDisable()
        {
            if (m_Button != null)
                m_Button.clicked -= TryRaiseEvent;
        }

        void TryRaiseEvent()
        {
            // Wait for cooldown window
            if (Time.time < m_TimeToNextEvent)
                return;

            // Button is hidden
            if (m_Button.style.opacity == 0)
                return;

            RaiseEvent();
            m_TimeToNextEvent = Time.time + m_Delay;
        }

        protected abstract void RaiseEvent();
    }
    
}
