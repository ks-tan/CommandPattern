using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<Command> _commandInputMap = new List<Command>();

    private List<Command> _commandHistory = new List<Command>();

    private float _startTime = 0;

    public bool TryGetInput(out Command input)
    {
        input = null;

        foreach (var command in _commandInputMap)
        {
            if (Input.GetKeyDown(command.KeyCode))
                input = command.CreateCopy(Command.KeyState.DOWN, Time.time - _startTime, Time.frameCount);
            if (Input.GetKeyUp(command.KeyCode))
                input = command.CreateCopy(Command.KeyState.UP, Time.time - _startTime, Time.frameCount);
        }

        if (input == null)
            return false;
            
        _commandHistory.Add(input);
        return true;
    }

    public List<Command> CopyHistory()
    {
        var copy = new List<Command>();
        foreach (var command in _commandHistory)
            copy.Add(command.CreateCopy());
        return copy;
    }

    public void ClearHistory()
    {
        _commandHistory.Clear();
        _startTime = Time.time;
    }
}
