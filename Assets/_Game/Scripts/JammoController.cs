using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class JammoController : MonoBehaviour
{
    // Commands
    private List<Command> _commandHistory = new List<Command>();
    private Command _lastHistoryCommand => _commandHistory.Count == 0 ? null : _commandHistory[_commandHistory.Count - 1];
    private Command _lastReadCommand = null;
    
    // States
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _jumpHeight = 5f;
    [SerializeField] private float _gravity = 9.81f;
    private Vector3 _velocity = Vector3.zero;
    private bool _isMovingRight = false;
    private bool _isMovingLeft = false;
    private bool _isFacingLeft = false;
    private bool _isAttemptingJump = false;
    private bool _isGrounded => transform.position.y <= 0;

    // Animation
    [SerializeField] private Animator _animator = null;
    private string _lastAnimationTrigger = null;


    public void AddCommand(Command inCommand)
    {
        _commandHistory.Add(inCommand);
    }

    /// <summary>
    /// Update states, velocity and animation
    /// </summary>
    public void UpdateController()
    {
        if (_lastHistoryCommand != null && _lastHistoryCommand != _lastReadCommand)
        {
            _lastReadCommand = _lastHistoryCommand;
            if (_lastReadCommand.Action == Command.ActionType.RIGHT)
                _isMovingRight = _lastReadCommand.State == Command.KeyState.DOWN;
            if (_lastReadCommand.Action == Command.ActionType.LEFT)
                _isMovingLeft = _lastReadCommand.State == Command.KeyState.DOWN;
            if (_lastReadCommand.Action == Command.ActionType.JUMP)
                _isAttemptingJump = _lastReadCommand.State == Command.KeyState.DOWN;
        }

        // Update horizontal velocity
        if (_isMovingLeft)
        {
            _velocity.x = -1 * Time.fixedDeltaTime * _moveSpeed;
            _isFacingLeft = true;
        }
        else if (_isMovingRight)
        {
            _velocity.x = Time.fixedDeltaTime * _moveSpeed;
            _isFacingLeft = false;
        }
        else _velocity.x = 0;

        // Update vertical velocity
        if (_isAttemptingJump && _isGrounded)
        {
            _velocity.y = Time.fixedDeltaTime * _jumpHeight;
        }
        _velocity.y -= Time.fixedDeltaTime * _gravity;

        // Update animation
        TriggerAnimation(_isMovingLeft || _isMovingRight ? "Walk" : "Idle");
    }

    /// <summary>
    /// Update position / movement / physics
    /// </summary>
    public void FixedUpdateController()
    {
        var position = transform.position + _velocity;
        position.y = Mathf.Max(0, position.y);
        transform.position = position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_isFacingLeft ? Vector3.back : Vector3.forward), Time.fixedDeltaTime * _rotationSpeed);
    }

    private void TriggerAnimation(string inTrigger)
    {
        if (_lastAnimationTrigger == inTrigger) return;
        _lastAnimationTrigger = inTrigger;
        _animator.SetTrigger(inTrigger);
    }
}
