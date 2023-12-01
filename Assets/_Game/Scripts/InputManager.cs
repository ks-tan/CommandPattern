using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<Command> _commandInputMap = new List<Command>();

    private List<Command> _commandHistory = new List<Command>();

    public bool TryGetInput(out Command input)
    {
        input = null;

        foreach (var command in _commandInputMap)
        {
            if (Input.GetKeyDown(command.KeyCode))
                input = command.CreateCopy(Command.KeyState.DOWN, Time.time, Time.frameCount);
            if (Input.GetKeyUp(command.KeyCode))
                input = command.CreateCopy(Command.KeyState.UP, Time.time, Time.frameCount);
        }

        if (input == null)
            return false;
            
        _commandHistory.Add(input);
        return true;
    }
}
