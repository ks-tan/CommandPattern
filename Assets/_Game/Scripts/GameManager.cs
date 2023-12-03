using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private JammoController _playerController;
    [SerializeField] private JammoController _jammoPrefab;
    [SerializeField] private Transform _inputDisplayTextHolder;
    [SerializeField] private TextMeshProUGUI _inputDisplayText;

    // For tracking replays
    private Dictionary<JammoController, List<Command>> _jammoToCommandsMap = new Dictionary<JammoController, List<Command>>();
    private Dictionary<JammoController, int> _jammoToCommandIndex = new Dictionary<JammoController, int>();
    private Dictionary<JammoController, float> _jammoCreationTimes = new Dictionary<JammoController, float>();

    private void Update()
    {
        if (_inputManager.TryGetInput(out Command input))
        {
            var shouldCreateReplay = input.Action == Command.ActionType.OPTION_0 && input.State == Command.KeyState.DOWN;
            if (shouldCreateReplay) CreateReplay();
            else _playerController.ReadCommand(input);
            ShowInputDisplay(input);
        }

        FeedCommandToAllReplays();
    }

    private void FixedUpdate()
    {
        // FixedUpdate player controller
        _playerController.FixedUpdateController();

        // FixedUpdate all other jammos
        foreach (var jammo in _jammoToCommandsMap.Keys)
            jammo.FixedUpdateController();
    }

    private void CreateReplay()
    {
        // Get all commands in InputManager's history and map it to a new JammoController
        var newJammo = Instantiate(_jammoPrefab);
        newJammo.ResetController(shouldChangeColor: true);
        _jammoToCommandsMap.Add(newJammo, _inputManager.CopyHistory());
        _jammoToCommandIndex.Add(newJammo, 0);
        _jammoCreationTimes.Add(newJammo, Time.time);

        // Reset player Jammo and clear input history
        _playerController.ResetController();
        _inputManager.ClearHistory();
    }

    private void FeedCommandToAllReplays()
    {
        // Feed commands to all replay-jammos
        foreach (var jammo in _jammoToCommandsMap.Keys)
        {
            var currentTimeStep = Time.time - _jammoCreationTimes[jammo];
            var commandHistory = _jammoToCommandsMap[jammo];
            var currentCommandIndex = _jammoToCommandIndex[jammo];

            if (currentCommandIndex >= commandHistory.Count)
            {
                // No more commands to feed current jammo. Start over from first command.
                _jammoToCommandIndex[jammo] = 0;
                _jammoCreationTimes[jammo] = Time.time;
                jammo.ResetController();
            }
            else
            {
                // Feed command to current jammo and move to the next command
                var currentCommand = commandHistory[currentCommandIndex];
                if (currentTimeStep < currentCommand.Time) continue;
                jammo.ReadCommand(currentCommand);
                _jammoToCommandIndex[jammo]++;
            }
        }
    }

    private void ShowInputDisplay(Command inCommand)
    {
        if (inCommand.State != Command.KeyState.DOWN) return;
        var tmp = Instantiate(_inputDisplayText, _inputDisplayTextHolder);
        tmp.gameObject.SetActive(true);
        tmp.text = inCommand.ToString();
        StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(1);
            var elapsed = 0f;
            while (elapsed < 1)
            {
                elapsed += Time.deltaTime;
                tmp.alpha = Mathf.Lerp(1, 0, elapsed / 1);
                yield return null;
            }
            Destroy(tmp.gameObject);
        }
    }
}
