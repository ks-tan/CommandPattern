using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private JammoController _playerController;
    [SerializeField] private JammoController _jammoPrefab;

    // For tracking replays
    private Dictionary<JammoController, Command[]> _jammoToCommandsMap = new Dictionary<JammoController, Command[]>();
    private Dictionary<JammoController, int> _jammoToCommandIndex = new Dictionary<JammoController, int>();
    private Dictionary<JammoController, float> _jammoCreationTimes = new Dictionary<JammoController, float>();

    private void Update()
    {
        if (_inputManager.TryGetInput(out Command input))
        {
            if (input.Action == Command.ActionType.OPTION_0 && input.State == Command.KeyState.DOWN)
            {
                // Reset all Jammos
                foreach (var jammo in _jammoToCommandsMap.Keys)
                {
                    jammo.Reset();
                    _jammoToCommandIndex[jammo] = 0;
                }
                _playerController.Reset();

                // Get all commands in InputManager's history and map it to a new JammoController
                var newJammo = Instantiate(_jammoPrefab);
                newJammo.Reset();
                Command[] commandHistory = new Command[_inputManager.CommandHistory.Count];
                _inputManager.CommandHistory.CopyTo(commandHistory, 0);
                _jammoToCommandsMap.Add(newJammo, commandHistory);
                _jammoToCommandIndex.Add(newJammo, 0);
                _jammoCreationTimes.Add(newJammo, Time.time);

                _inputManager.ClearHistory();
            }
            // Else, just feed command to the player controller
            else _playerController.ReadCommand(input);
        }

        // Feed commands to all other jammos
        foreach (var jammo in _jammoToCommandsMap.Keys)
        {
            var currentTimeStep = Time.time - _jammoCreationTimes[jammo];
            var commandHistory = _jammoToCommandsMap[jammo];
            var currentCommandIndex = _jammoToCommandIndex[jammo];
            var currentCommand = commandHistory[currentCommandIndex];
            if (currentTimeStep < currentCommand.Time) continue;
            jammo.ReadCommand(currentCommand);
            _jammoToCommandIndex[jammo]++;
        }
    }

    private void FixedUpdate()
    {
        // FixedUpdate player controller
        _playerController.FixedUpdateController();

        // FixedUpdate all other jammos
        foreach (var jammo in _jammoToCommandsMap.Keys)
            jammo.FixedUpdateController();
    }
}
