using System;
using UnityEngine;

[Serializable]
public class Command
{
    public enum ActionType { UP, DOWN, LEFT, RIGHT, JUMP, ACTION_0, ACTION_1, ACTION_2, OPTION_0, OPTION_1 }
    public enum KeyState { UP, DOWN }

    [SerializeField] private ActionType _action;
    [SerializeField] private KeyCode _keyCode;
    [SerializeField] private string _debugString;

    public KeyState State { get; private set; }
    public float Time { get; private set; }
    public int Frame { get; private set; }

    public ActionType Action => _action;
    public KeyCode KeyCode => _keyCode;
    public string DebugString => _debugString;
    public override string ToString() => $"{Action},{KeyCode},{DebugString},{State},{Time},{Frame}";

    private Command(ActionType inAction, KeyCode inKeyCode, string inDebugString, KeyState inState, float inTime, int inFrame)
    {
        _action = inAction;
        _keyCode = inKeyCode;
        _debugString = inDebugString;
        State = inState;
        Time = inTime;
        Frame = inFrame;
    }

    public Command CreateCopy()
    {
        return new Command(Action, KeyCode, _debugString, State, Time, Frame);
    }

    public Command CreateCopy(KeyState inState, float inTime, int inFixedUpdateFrame)
    {
        return new Command(Action, KeyCode, _debugString, inState, inTime, inFixedUpdateFrame);
    }

    public static Command FromString(string inString)
    {
        var data = inString.Split(",");
        return new Command(
            inAction: (ActionType)Enum.Parse(typeof(ActionType), data[0]),
            inKeyCode: (KeyCode)Enum.Parse(typeof(KeyCode), data[1]),
            inDebugString: data[2],
            inState: (KeyState)Enum.Parse(typeof(KeyState), data[3]),
            inTime: float.Parse(data[4]),
            inFrame: int.Parse(data[5]));
    }
}
