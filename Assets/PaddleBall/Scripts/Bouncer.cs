using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// Any surface (wall or paddle) that reflects the ball. Upon collision with a Ball,
    /// it returns the normal direction of the 2D collision.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class Bouncer : MonoBehaviour
    {
        [Header("Broadcast on Event Channels")]
        [Tooltip("Signal when ball collides with this wall or paddle")]
        [SerializeField]
        private BallCollisionEventChannelSO m_BallCollided;

        [Tooltip("Optional event that sends location of sound playback")] [SerializeField, Optional]
        private Vector2EventChannelSO m_SoundPlayed;

        // Verify necessary fields in the Inspector
        private void Awake()
        {
            NullRefChecker.Validate(this);
        }

        // Checks if have made contact with a Ball. If so, reflect the normal direction
        // of the collision and then raise an event with the new direction
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Ball ball = collision.collider.GetComponent<Ball>();
            if (ball == null)
                return;
            
            // Reverse the normal direction and create the collision data
            Vector2 normalDirection = collision.GetContact(0).normal;

            BallCollisionData data = new BallCollisionData
            {
                Ball = ball,
                BounceVector = -normalDirection
            };

            // Raise the event with the collision data
            m_BallCollided.RaiseEvent(data);

            if (m_SoundPlayed != null)
                m_SoundPlayed.RaiseEvent(collision.GetContact(0).point);
        }
    }
}

