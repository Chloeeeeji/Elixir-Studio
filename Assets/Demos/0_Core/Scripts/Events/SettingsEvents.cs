using System;
using UnityEngine;

namespace Demos
{
    /// <summary>
    /// Public static delegates to manage settings changes (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class SettingsEvents 
    {
        // Notify set up script that UI is ready
        public static Action SettingsInitialized;

        // Presenter --> View: update UI sliders
        public static Action<float> MasterSliderSet;
        public static Action<float> SFXSliderSet;
        public static Action<float> MusicSliderSet;

        // View -> Presenter: update UI sliders
        public static Action<float> MasterSliderChanged;
        public static Action<float> SFXSliderChanged;
        public static Action<float> MusicSliderChanged;

        //// Presenter -> Model: update volume settings
        public static Action<float> MasterVolumeChanged;
        public static Action<float> SFXVolumeChanged;
        public static Action<float> MusicVolumeChanged;

        // Model -> Presenter: model values changed (e.g. loading saved values)
        public static Action<float> ModelMasterVolumeChanged;
        public static Action<float> ModelSFXVolumeChanged;
        public static Action<float> ModelMusicVolumeChanged;


        // Static events keep their subscribers between Play sessions when Fast Enter Play mode
        // skips the domain reload, which double-invokes handlers and leaves them pointing at
        // destroyed objects. Clear them before each run.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticEvents()
        {
            SettingsInitialized = null;
            MasterSliderSet = null;
            SFXSliderSet = null;
            MusicSliderSet = null;
            MasterSliderChanged = null;
            SFXSliderChanged = null;
            MusicSliderChanged = null;
            MasterVolumeChanged = null;
            SFXVolumeChanged = null;
            MusicVolumeChanged = null;
            ModelMasterVolumeChanged = null;
            ModelSFXVolumeChanged = null;
            ModelMusicVolumeChanged = null;
        }

    }
}
