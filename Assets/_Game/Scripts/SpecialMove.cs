using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialMove", menuName = "ScriptableObjects/SpecialMove", order = 1)]
public class SpecialMove : ScriptableObject
{
    [Serializable]
    public class MoveSequence
    {
        [SerializeField] private List<Command.ActionType> _actions;
        public ReadOnlyCollection<Command.ActionType> Actions => _actions.AsReadOnly();
    }

    public enum MoveName { FIREBALL }

    [SerializeField] private MoveName _name;
    [SerializeField] private List<MoveSequence> _sequences;

    public MoveName Name => _name;
    public ReadOnlyCollection<MoveSequence> Sequences => _sequences.AsReadOnly();
}
