using GameSystemsCookbook.Demos.PaddleBall;
using UnityEngine;

namespace GameSystemsCookbook
{
    
    [System.Serializable]
    public struct BallCollisionData
    {
        public Ball Ball;   // which instance collided
        public Vector2 BounceVector;    // where it bounced
    }

    [CreateAssetMenu(fileName = "BallCollisionEventChannelSO",
        menuName = "Scriptable Objects/BallCollisionEventChannelSO")]
    public class BallCollisionEventChannelSO : GenericEventChannelSO<BallCollisionData>
    {

        // Inherits:
        //   public UnityAction<BallCollisionData> OnEventRaised;
        //   public void RaiseEvent(BallCollisionData data);
        //
        // Use RaiseEvent(...) to broadcast a bounce
        
       // Balls listen this event channel and check the data.Ball reference
       // so only the correct Ball reacts.
    }

}