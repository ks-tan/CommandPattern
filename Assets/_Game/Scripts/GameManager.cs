using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private JammoController _playerController;

    private void Update()
    {
        if (_inputManager.TryGetInput(out Command input))
        {
            if (input.Action == Command.ActionType.OPTION_0)
            {
                // TODO: Get all commands in InputManager's history and play it on a new JammoController
                Command[] commandHistory = new Command[_inputManager.CommandHistory.Count];
                _inputManager.CommandHistory.CopyTo(commandHistory, 0);

                // Reset player's position and clear InputManager's command history
                _inputManager.ClearHistory();
                _playerController.Reset();
            }
            // Else, just feed command to the player controller
            else _playerController.ReadCommand(input);
        }

    }

    private void FixedUpdate()
    {
        _playerController.FixedUpdateController();
    }
}
