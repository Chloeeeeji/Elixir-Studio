using UnityEngine;
using UnityEngine.UIElements;

using System.Collections.Generic;

namespace Demos
{
    /// <summary>
    /// Base class for setting up UI Document and root element for the Demo Scene UIs
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class DemoBase : MonoBehaviour
    {
        [Tooltip("Required UI Document")] [SerializeField]
        protected UIDocument m_Document;

        [Header("Instructions")] [Tooltip("Pages of text paired with next/last buttons")] [SerializeField]
        protected DemoTextScreen m_TextScreen;

        [Header("Page Settings")] [Tooltip("Total number of text pages")] [SerializeField]
        protected int m_TotalPages = 3; // Adjust based on your needs

        [Header("Debug Settings")] [Tooltip("Enable to show debug warnings in the console.")] [SerializeField]
        protected bool m_ShowDebug;

        /// <summary>
        /// USS classes "page-0","page-1","page-2", etc. to correspond to text screens
        /// </summary>
        const string k_PageBaseName = "page-";

        protected VisualElement m_Root;

        // Use a helper class to simplify registering and unregistering callbacks
        protected EventRegistry m_EventRegistry;

        // Cached list of all page VisualElements
        private List<VisualElement> m_AllPages = new List<VisualElement>();

        protected virtual void OnEnable()
        {
            m_EventRegistry = new EventRegistry();
            SetVisualElements();

            // Subscribe to the TextPageIndexUpdated event
            DemoEvents.TextPageIndexUpdated += OnTextPageIndexUpdate;

            CacheAllPages();
        }

        protected virtual void OnDisable()
        {
            // Unsubscribe from the TextPageIndexUpdated event to prevent memory leaks
            DemoEvents.TextPageIndexUpdated -= OnTextPageIndexUpdate;

            // One call to Dispose unregisters all the EventRegistry's managed callbacks. Otherwise,
            // register and unregister each callback individually.
            m_EventRegistry.Dispose();
        }

        // Set references to the visual elements
        protected virtual void SetVisualElements()
        {
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();

            m_Root = m_Document.rootVisualElement;

            // Verify required fields in the Inspector
            NullRefChecker.Validate(this);

            m_TextScreen.Initialize(m_Root);
        }

        /// <summary>
        /// Caches references to all page VisualElements based on their class names.
        /// </summary>
        void CacheAllPages()
        {
            for (int i = 0; i < m_TotalPages; i++)
            {
                string pageClassName = $"{k_PageBaseName}{i}";
                // Query all VisualElements with the specific class name
                var pages = m_Root.Query<VisualElement>(className: pageClassName).ToList();

                foreach (var page in pages)
                {
                    if (page != null)
                    {
                        m_AllPages.Add(page);

                        if (m_ShowDebug)
                            Debug.Log($"Caching {page.name} as {pageClassName} ");
                    }
                    else if (m_ShowDebug)
                    {
                        Debug.LogWarning(
                            $"[DemoBase]: Page with class '{pageClassName}' not found in the UI during caching.");
                    }
                }
            }
        }


        /// <summary>
        /// Updates the UI to show only the page that matches the current text page index.
        /// </summary>
        /// <param name="index">The current text page index.</param>
        void OnTextPageIndexUpdate(int index)
        {
            // Clamp the index to ensure it stays within valid bounds
            int clampedIndex = Mathf.Clamp(index, 0, m_TotalPages - 1);

            // Hide all pages first
            foreach (var page in m_AllPages)
            {
                if (page != null)
                {
                    page.style.display = DisplayStyle.None;
                    page.pickingMode = PickingMode.Ignore;
                }
            }

            // Construct the class name for the current page
            string currentPageClassName = $"{k_PageBaseName}{clampedIndex}";

            // Find and show all elements with the current page class name
            var currentPages = m_Root.Query<VisualElement>(className: currentPageClassName).ToList();

            bool pageFound = false;

            foreach (var page in currentPages)
            {
                if (page != null)
                {
                    page.pickingMode = PickingMode.Position;
                    page.style.display = DisplayStyle.Flex;
                    pageFound = true;
                }
            }

            // Optionally log a warning if the current page wasn't found
            if (!pageFound && m_ShowDebug)
            {
                Debug.LogWarning($"[DemoBase]: Current page with class '{currentPageClassName}' not found in the UI.");
            }
        }
    }
}