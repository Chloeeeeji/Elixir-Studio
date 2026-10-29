using UnityEngine;
using UnityEngine.UIElements;


namespace Demos
{
    /// <summary>
    /// Manages pages of text content stored in a TextScreenSO, so the demo
    /// can show one page of text at a time.
    /// </summary>
    [System.Serializable]
    public class DemoTextScreen
    {
        [Header("Content")]
        [Tooltip("ScriptableObject that holds the text and events")]
        [SerializeField] TextScreenSO m_DemoScreenData;

        Button m_NextButton;
        Button m_LastButton;
        VisualElement m_Root;
        Label m_CurrentPage;
        Label m_Title;
        int m_PageIndex;
        
        // Used to prevent multiple page transitions from spam clicking
        float m_LastClickTime;
        const float k_ClickDelay = 0.1f;


        // Manages UI event registration/cleanup to prevent memory leaks
        EventRegistry m_EventRegistry;

        /// <summary>
        /// Sets up the text screen and its UI elements. Needs to be called first.
        /// </summary>
        /// <param name="root">Root VisualElement containing the text-screen elements</param>
        public void Initialize(VisualElement root)
        {
            m_EventRegistry = new EventRegistry();
            m_Root = root;
            m_PageIndex = 0;
            SetVisualElements();
            RegisterCallbacks();
            UpdatePageContent();
        }

        
        /// <summary>
        /// Queries and caches UI elements.
        /// </summary>
        private void SetVisualElements()
        {
            m_NextButton = m_Root.Q<Button>("text-screen__button-next");
            m_LastButton = m_Root.Q<Button>("text-screen__button-last");
            m_CurrentPage = m_Root.Q<Label>("text-screen__page-text");
            m_Title = m_Root.Q<Label>("text-screen__title");

            m_Title.text = m_DemoScreenData.Title;
            
        }
        
        /// <summary>
        /// Sets up click handlers for navigation buttons.
        /// Uses EventRegistry to ensure proper cleanup when disabled.
        /// </summary>
        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterCallback<ClickEvent>(m_NextButton, ShowNextPage);
            m_EventRegistry.RegisterCallback<ClickEvent>(m_LastButton, ShowLastPage);
        }

        /// <summary>
        /// Cleanup method called when the component is disabled.
        /// </summary>
        private void OnDisable()
        {
            m_EventRegistry.Dispose();
        }

        /// <summary>
        /// Handles next page navigation and notifies listeners of page changes.
        /// </summary>
        void ShowNextPage(ClickEvent evt)
        {
            // Prevent spam clicking or double-clicking
            float currentTime = Time.time;
            if (currentTime - m_LastClickTime < k_ClickDelay) 
                return;
            
            m_LastClickTime = currentTime;

            if (m_PageIndex < m_DemoScreenData.BodyText.Count - 1)
            {
                m_PageIndex++;
                UpdatePageContent();
                
                // Notify listeners
                DemoEvents.TextPageChanged?.Invoke();
                DemoEvents.TextPageIndexUpdated?.Invoke(m_PageIndex);
            }
        }

        /// <summary>
        /// Handles last page navigation and notifies listeners of page changes.
        /// </summary>
        void ShowLastPage(ClickEvent evt)
        {
            float currentTime = Time.time;
            if (currentTime - m_LastClickTime < k_ClickDelay) 
                return;
            
            m_LastClickTime = currentTime;

            if (m_PageIndex > 0)
            {
                m_PageIndex--;
                UpdatePageContent();
                
                // Notify listeners
                DemoEvents.TextPageChanged?.Invoke();
                DemoEvents.TextPageIndexUpdated?.Invoke(m_PageIndex);
            }
        }
        
        /// <summary>
        /// Updates displayed content and navigation button states.
        /// Should be called after any change to m_PageIndex.
        /// </summary>
        void UpdatePageContent()
        {
            m_CurrentPage.text = m_DemoScreenData.BodyText[m_PageIndex];
            
     
            UpdateNextLastButtons();
        }

        /// <summary>
        /// Toggle the m_NextButton and m_LastButton depending on index.
        /// Disables button visibility if at end or beginning of range.
        /// </summary>

        void UpdateNextLastButtons()
        {
            if (m_DemoScreenData.BodyText.Count <= 1)
            {
                FadeElement(m_NextButton, false);
                FadeElement(m_LastButton, false);
                return;
            }

            if (m_PageIndex == 0)
            {
                FadeElement(m_NextButton, true);
                FadeElement(m_LastButton, false);
                return;
            }

            if (m_PageIndex >= m_DemoScreenData.BodyText.Count - 1)
            {
                FadeElement(m_NextButton, false);
                FadeElement(m_LastButton, true);
                return;
            }

            FadeElement(m_NextButton, true);
            FadeElement(m_LastButton, true);
        }

        /// <summary>
        /// Utility method to control element visibility through display style.
        /// Complete hide/show when not transitioning.
        /// </summary>
        void DisplayElement(VisualElement element, bool state)
        {
            element.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Controls element visibility through opacity with USS transitions for smooth fading.
        /// </summary>
        void FadeElement(VisualElement element, bool state)
        {
            element.style.opacity = (state) ? 1f : 0f;
        }
    }
}