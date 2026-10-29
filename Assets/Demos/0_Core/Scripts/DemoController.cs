using UnityEngine;
using GameSystemsCookbook.Demos.PaddleBall;
using Demos;
using System.Collections.Generic;

namespace GameSystemsCookbook
{
    /// <summary>
    /// This script initializes all Ball objects in the scene using shared GameData.
    /// </summary>
    public class DemoController : MonoBehaviour
    {
        [Tooltip("Shared gameplay data")] [SerializeField]
        GameDataSO m_GameData;
        
        [Tooltip("Ball settings")]
        [SerializeField] [Optional]
        List<BallTypeSO> m_BallTypes;
        

        [Header("Listen to Event Channels")]
        [Tooltip("Signal that gameplay has begun")] [SerializeField, Optional]
        VoidEventChannelSO m_GameStarted;

        [Tooltip("Signal to switch BallTypeSO for all balls in the scene.")] [SerializeField, Optional]
        IntEventChannelSO m_SwitchBallTypeEventChannel;
 
        Ball[] m_Balls;

        void OnEnable()
        {
            if (m_SwitchBallTypeEventChannel)
                m_SwitchBallTypeEventChannel.OnEventRaised += SwitchBallTypes;
        }
        
        void OnDisable()
        {
            if (m_SwitchBallTypeEventChannel)
                m_SwitchBallTypeEventChannel.OnEventRaised -= SwitchBallTypes;
        }

        void Start()
        {
            NullRefChecker.Validate(this);

            // Find all Ball objects in the scene and initialize them
            m_Balls = FindObjectsByType<Ball>();
            foreach (var ball in m_Balls)
            {
                ball.Initialize(m_GameData);
            }

            // Start the game manually for the demo
            if (m_GameStarted != null)
                m_GameStarted.RaiseEvent();
            
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        void SwitchBallTypes(int ballTypeIndex)
        {
            SetBallTypeByIndex(ballTypeIndex);
        }

        public void SetBallTypeByIndex(int index)
        {
            if (m_BallTypes == null || m_BallTypes.Count == 0)
            {
                Debug.LogWarning("[SetBallTypeByIndex]: BallTypes list is null or empty.");
                return;
            }

            if (index < 0 || index >= m_BallTypes.Count)
            {
                Debug.LogWarning($"[SetBallTypeByIndex]: index {index} is out of range.");
                return;
            }
            
            BallTypeSO selectedType = m_BallTypes[index];
            SetBallTypeForAll(selectedType);
        }
        
        /// <summary>
        /// Sets the BallTypeSO for all balls in the scene.
        /// </summary>
        public void SetBallTypeForAll(BallTypeSO ballType)
        {
            if (m_Balls == null || m_Balls.Length == 0)
            {
                Debug.LogWarning("[SetBallTypeForAll]: No balls found.");
                return;
            }
            
            foreach (var ball in m_Balls)
            {
                ball.SetBallType(ballType);
            }
        }
        
    }
}