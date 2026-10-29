using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace Demos
{
        [CreateAssetMenu(fileName = "BallTypeSO", menuName = "Demos/BallTypeSO")]

        public class BallTypeSO : ScriptableObject
        {
                [Tooltip("Sprite used to render the ball")]
                [SerializeField] private Sprite m_Sprite;

                [Tooltip("Tint color for the ball")] 
                [SerializeField] private Color m_Color = Color.white;

                [FormerlySerializedAs("m_VFXGraph")] [Tooltip("VFX played when ball bounces")] [SerializeField]
                VisualEffectAsset m_ImpactVFXGraph;

                [Tooltip("Sound played when ball bounces")]
                [SerializeField] private AudioClip m_BounceAudioClip;

                // Add additional styling data here (e.g., particles, trail effects, etc.)

                public Sprite Sprite => m_Sprite;
                public Color Color => m_Color;
                public AudioClip BounceAudioClip => m_BounceAudioClip;
                public VisualEffectAsset VFXGraph   => m_ImpactVFXGraph;

        }

}