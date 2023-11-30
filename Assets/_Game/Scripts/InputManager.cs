using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<Command> _commandInputMap = new List<Command>();

    private JammoController _playerController;

    public void Setup(JammoController inPlayer)
    {
        _playerController = inPlayer;
    }

    public void UpdateInput()
    {
        foreach (var command in _commandInputMap)
        {
            if (Input.GetKeyDown(command.KeyCode))
                _playerController.AddCommand(command.CreateCopy(Command.KeyState.DOWN, Time.time, Time.frameCount));
            if (Input.GetKeyUp(command.KeyCode))
                _playerController.AddCommand(command.CreateCopy(Command.KeyState.UP, Time.time, Time.frameCount));
        }
    }
}
