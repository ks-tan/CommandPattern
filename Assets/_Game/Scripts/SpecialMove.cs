using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialMove", menuName = "ScriptableObjects/SpecialMove", order = 1)]
public class SpecialMove : ScriptableObject
{
    [Serializable]
    public class MoveSequence
    {
        [SerializeField] private List<Command.ActionType> _actions;

        public bool IsSequenceFound(Queue<Command> commandsArray) 
        {
            var commandActions = new List<Command.ActionType>();
            
            foreach (var command in commandsArray)
                if (command.State == Command.KeyState.DOWN)
                    commandActions.Add(command.Action);
            
            var intersect = commandActions.Intersect(_actions).ToArray();
            
            if (intersect.Count() != _actions.Count)
                return false;

            for (int i = 0; i < intersect.Count(); i++)
                if (intersect[i] != _actions[i])
                    return false;

            return true;
        }
    }

    // Enum values should match special move triggers found in AnimatorControllers
    public enum Move { Fireball }

    [SerializeField] private Move _name;
    [SerializeField] private List<MoveSequence> _sequences;

    public Move Name => _name;

    public bool IsSequenceFound(Queue<Command> commandsArray)
    {
        foreach (var sequence in _sequences)
            if (sequence.IsSequenceFound(commandsArray))
                return true;
        return false;
    }
}
