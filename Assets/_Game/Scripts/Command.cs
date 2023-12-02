using System;
using UnityEngine;

[Serializable]
public class Command
{
    public enum ActionType { UP, DOWN, LEFT, RIGHT, JUMP, ACTION_0, ACTION_1, ACTION_2, OPTION_0 }
    public enum KeyState { UP, DOWN }

    [SerializeField] private ActionType _action;
    [SerializeField] private KeyCode _keyCode;
    [SerializeField] private string _debugString;

    public KeyState State { get; private set; }
    public float Time { get; private set; }
    public int Frame { get; private set; }

    public ActionType Action => _action;
    public KeyCode KeyCode => _keyCode;
    public override string ToString() => _debugString;

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
}
