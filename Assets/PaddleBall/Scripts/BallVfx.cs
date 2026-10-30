using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Demos;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// Handles all VisualEffect bursts and trails for a Ball.
    /// Subscribes to the same event channels as the Ball and drives:
    ///  • Impact VFX on bounce
    ///  • Score VFX on point scored
    ///  • Trail VFX while in play
    ///  • Stops all VFX when the game is over
    /// </summary>
    [RequireComponent(typeof(Ball))] // so we can filter collisions
    public class BallVfx : MonoBehaviour
    {
        [Header("VFX Graphs")] [SerializeField]
        private VisualEffect m_ImpactVfx;

        [SerializeField] private VisualEffect m_TrailVfx;
        [SerializeField] private VisualEffect m_ScoreVfx;

        [Header("Event Channels")] [SerializeField]
        private VoidEventChannelSO m_GameStarted;

        [SerializeField] private PlayerIDEventChannelSO m_PointScored;
        [SerializeField] private BallCollisionEventChannelSO m_BallCollided;
        [SerializeField] private VoidEventChannelSO m_GameOver;

        [Header("Timing")] [SerializeField, Tooltip("Match the game data's DelayBetweenPoints")]
        private float m_DelayBetweenPoints = 1f;

        // cache for filtering
        private Ball m_Ball;
        public VisualEffect ImpactVfx
        {
            get => m_ImpactVfx;
            set => m_ImpactVfx = value;
        }
        

        void Awake()
        {
            m_Ball = GetComponent<Ball>();
        }

        void OnEnable()
        {
            if (m_GameStarted != null)
                m_GameStarted.OnEventRaised += OnGameStarted;
            
            if (m_PointScored != null)
                m_PointScored.OnEventRaised += OnPointScored;
            
            if (m_BallCollided != null)
                m_BallCollided.OnEventRaised += OnBallCollided;
            
            if (m_GameOver != null)
                m_GameOver.OnEventRaised += OnGameOver;
        }

        void OnDisable()
        {
            if (m_GameStarted != null)
                m_GameStarted.OnEventRaised -= OnGameStarted;
            
            if (m_PointScored != null)
                m_PointScored.OnEventRaised -= OnPointScored;
            
            if (m_BallCollided != null)
                m_BallCollided.OnEventRaised -= OnBallCollided;
            
            if (m_GameOver != null)
                m_GameOver.OnEventRaised -= OnGameOver;
        }
        
        void OnGameOver() => StopAllVfx();


        // Event handlers ______________________________________________________________
        
        private void OnGameStarted()
        {
            // Delay the trail until the ball actually serves
            StartCoroutine(RestartTrailAfterDelay());
        }
        
        private void OnPointScored(PlayerIDSO player)
        {
            StopTrail();
            if (m_ScoreVfx != null)
            {
                m_ScoreVfx.Stop();
                m_ScoreVfx.SendEvent("OnScore");
            }

            StartCoroutine(RestartTrailAfterDelay());
        }

        private void OnBallCollided(BallCollisionData data)
        {
            // Check if data refers to this Ball instance
            if (data.Ball != m_Ball)
                return;

            if (m_ImpactVfx == null)
                return;

            m_ImpactVfx.Stop();
            m_ImpactVfx.SendEvent("Burst");
        }

        private IEnumerator RestartTrailAfterDelay()
        {
            yield return new WaitForSeconds(m_DelayBetweenPoints);
            StartTrail();
        }

        public void StopTrail()
        {
            if (m_TrailVfx == null)
                return;

            m_TrailVfx.Stop();
        }

        public void StartTrail()
        {
            if (m_TrailVfx == null)
                return;

            m_TrailVfx.gameObject.SetActive(true);
            m_TrailVfx.Reinit();
        }

        private void StopAllVfx()
        {
            StopTrail();

            if (m_ImpactVfx != null)
                m_ImpactVfx.Stop();

            if (m_ScoreVfx != null)
                m_ScoreVfx.Stop();
        }
        
        
    }
}