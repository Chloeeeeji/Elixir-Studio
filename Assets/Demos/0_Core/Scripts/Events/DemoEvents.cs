using System;
using UnityEngine;


namespace Demos
{
    /// <summary>
    /// Public static delegates to manage gameplay specifically for the Demo scenes (note these are "events"
    /// in the conceptual sense and not the strict C# sense). Invoked by external objects.
    /// </summary>
    public static class DemoEvents
    {
        // Notify other components that Presenter and Screen are ready
        public static Action SetupCompleted;

        // Notify the Setup component to unload the scene
        public static Action BackButtonClicked;

        // Moused over the Button in the DemoSelectionScreen
        public static Action<int> ButtonHighlighted;

        // Additively load the Demo scene with given index
        public static Action<int> SceneIndexLoaded;

        // Unload the previous scene
        public static Action LastSceneUnloaded;

        public static Action TextPageChanged;

        public static Action<int> TextPageIndexUpdated;


        // Static events keep their subscribers between Play sessions when Fast Enter Play mode
        // skips the domain reload, which double-invokes handlers and leaves them pointing at
        // destroyed objects. Clear them before each run.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticEvents()
        {
            SetupCompleted = null;
            BackButtonClicked = null;
            ButtonHighlighted = null;
            SceneIndexLoaded = null;
            LastSceneUnloaded = null;
            TextPageChanged = null;
            TextPageIndexUpdated = null;
        }

    }
}
