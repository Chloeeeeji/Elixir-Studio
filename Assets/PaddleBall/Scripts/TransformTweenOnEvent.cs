using System.Collections;
using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    [RequireComponent(typeof(Transform))]
    public class TransformTweenOnEvent : MonoBehaviour
    {
        [Header("Event Trigger")]
        [Tooltip("Listens to this Event Channel (e.g., GameStarted, GameOver, etc.).")]
        [SerializeField] private VoidEventChannelSO m_EventTrigger;

        [Header("Tween Settings")]
        [Tooltip("Local position to tween to")]
        [SerializeField] private Vector3 m_TargetLocalPosition;
        [Tooltip("Local rotation (Euler angles) to tween to")]
        [SerializeField] private Vector3 m_TargetLocalRotationEuler;
        [Tooltip("How long the tween should take (seconds)")]
        [SerializeField] private float m_Duration = 0.5f;
        [Tooltip("Optional ease curve (time 0–1 → value 0–1), defaults to ease in-ease out")]
        [SerializeField] private AnimationCurve m_EaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 m_OriginalLocalPosition;
        private Quaternion m_OriginalLocalRotation;

        private void Awake()
        {
            m_OriginalLocalPosition = transform.localPosition;
            m_OriginalLocalRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            if (m_EventTrigger != null)
                m_EventTrigger.OnEventRaised += OnEventRaised;
        }

        private void OnDisable()
        {
            if (m_EventTrigger != null)
                m_EventTrigger.OnEventRaised -= OnEventRaised;
        }

        private void OnEventRaised()
        {
            StopAllCoroutines();
            StartCoroutine(TweenTransform());
        }

        private IEnumerator TweenTransform()
        {
            float elapsed = 0f;
            var targetRotation = Quaternion.Euler(m_TargetLocalRotationEuler);

            while (elapsed < m_Duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / m_Duration);
                float easedT = m_EaseCurve.Evaluate(t);

                transform.localPosition = Vector3.Lerp(
                    m_OriginalLocalPosition,
                    m_TargetLocalPosition,
                    easedT
                );

                transform.localRotation = Quaternion.Lerp(
                    m_OriginalLocalRotation,
                    targetRotation,
                    easedT
                );

                yield return null;
            }

            // Set explicitly to final state
            transform.localPosition = m_TargetLocalPosition;
            transform.localRotation = targetRotation;
        }
    }
}
