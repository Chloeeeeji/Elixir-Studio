using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


namespace GameSystemsCookbook.Demos.PaddleBall
{

    /// <summary>
    /// This serves as an intermediary object between the InputActions and the GameObjects receiving input.
    /// GameObjects listen for UnityActions instead of subscribing to InputSystem events directly.
    /// </summary>
    [CreateAssetMenu(fileName = "InputReader")]
    public class InputReaderSO : DescriptionSO, PaddleBallControls.IGameplayActions
    {
        // Private fields
        private PaddleBallControls m_PaddleBallControls;
        private InputAction m_MoveP1;
        private InputAction m_MoveP2;
        private InputAction m_RestartGame;

        // Events to relay. Other objects subscribe to these. The default empty
        // delegate lets us skip the usual null check. 
        public event UnityAction<float> P1Moved = delegate { };
        public event UnityAction<float> P2Moved = delegate { };
        public event UnityAction GameRestarted = delegate { };

        // This creates a new PaddleBallControls instance and enables the GamePlayActions from
        // the Input System. The event-handling methods can then subscribe to the MoveP1
        // and MoveP2 events from the InputActions and also listen for game restarts.
        //
        // Called explicitly by GameManager rather than from OnEnable. A ScriptableObject's
        // OnEnable runs when the asset loads, which happens in the Editor outside Play mode,
        // is not ordered against scene load, is not reliably paired with OnDisable, and is
        // skipped altogether when Fast Enter Play mode disables the domain reload. Explicit
        // setup matches how Ball, Paddle and GameSetup are already initialized.
        public void EnableInput()
        {
            // Release any previous instance before replacing it, so an earlier one cannot be
            // left enabled -- that leaks and trips the generated class's finalizer assert.
            ReleaseControls();

            m_PaddleBallControls = new PaddleBallControls();
            m_PaddleBallControls.Gameplay.Enable();
            m_PaddleBallControls.Gameplay.SetCallbacks(this);

            m_MoveP1 = m_PaddleBallControls.Gameplay.MoveP1;
            m_MoveP2 = m_PaddleBallControls.Gameplay.MoveP2;
            m_RestartGame = m_PaddleBallControls.Gameplay.RestartGame;

            m_MoveP1.performed += OnMoveP1;
            m_MoveP2.performed += OnMoveP2;
            m_RestartGame.performed += OnRestartGame;
        }

        // Stops listening and releases the input actions. Called by GameManager.
        public void DisableInput()
        {
            ReleaseControls();
        }

        // Unsubscribes, disables and disposes the controls. PaddleBallControls is IDisposable
        // and owns a generated InputActionAsset, so disabling alone is not enough. Safe to call
        // when nothing has been set up.
        private void ReleaseControls()
        {
            if (m_PaddleBallControls == null)
                return;

            m_MoveP1.performed -= OnMoveP1;
            m_MoveP2.performed -= OnMoveP2;
            m_RestartGame.performed -= OnRestartGame;

            m_PaddleBallControls.Gameplay.Disable();
            m_PaddleBallControls.Dispose();
            m_PaddleBallControls = null;
        }

        // Event handling methods

        // Implements the interface from auto-generated PaddleBallControls class.
        // Invoke the P1Moved event when receiving non-zero input from Player1.
        public void OnMoveP1(InputAction.CallbackContext context)
        {
            // pass the axis value or 0
            if (m_MoveP1.WasPerformedThisFrame())
            {
                P1Moved.Invoke(context.ReadValue<float>());
            }
            else
            {
                P1Moved.Invoke(0f);
            }
        }

        // Implements the interface from auto-generated PaddleBallControls class.
        // Invoke the P2Moved event when receiving non-zero input from Player2.
        public void OnMoveP2(InputAction.CallbackContext context)
        {
            // pass the axis value or 0
            if (m_MoveP2.WasPerformedThisFrame())
            {
                P2Moved.Invoke(context.ReadValue<float>());
            }
            else
            {
                P2Moved.Invoke(0f);
            }
        }

        // Invokes the GameRestarted event when receiving a signal that the game
        // has restarted.
        public void OnRestartGame(InputAction.CallbackContext context)
        {
            if (m_RestartGame.WasPerformedThisFrame())
            {
                GameRestarted.Invoke();
            }
        }
    }
}
