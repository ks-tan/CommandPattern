using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class JammoController : MonoBehaviour
{
    private Command _lastCommand = null;

    [Header("Physics")][Space]
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _jumpSpeed = 12f;
    [SerializeField] private float _gravity = 1f;
    private Vector3 _velocity = Vector3.zero;
    private bool _isMovingRight = false;
    private bool _isMovingLeft = false;
    private bool _shouldFaceLeft = false;
    private bool _isAttemptingJump = false;
    private bool _isGrounded => transform.position.y <= 0;

    [Header("Model")][Space]
    [SerializeField] private Animator _animator = null;
    private string _lastAnimationTrigger = null;

    public void ReadCommand(Command inCommand)
    {
        if (inCommand.Action == Command.ActionType.RIGHT)
            _isMovingRight = inCommand.State == Command.KeyState.DOWN;
        if (inCommand.Action == Command.ActionType.LEFT)
            _isMovingLeft = inCommand.State == Command.KeyState.DOWN;
        if (inCommand.Action == Command.ActionType.JUMP)
            _isAttemptingJump = inCommand.State == Command.KeyState.DOWN;
        _lastCommand = inCommand;
    }

    /// <summary>
    /// Update physics and animation
    /// </summary>
    public void FixedUpdateController()
    {
        // Update horizontal velocity
        if (_isGrounded)
        {
            if (_isMovingLeft)
            {
                _velocity.x = -1 * Time.fixedDeltaTime * _moveSpeed;
                _shouldFaceLeft = true;
            }
            else if (_isMovingRight)
            {
                _velocity.x = Time.fixedDeltaTime * _moveSpeed;
                _shouldFaceLeft = false;
            }
            else _velocity.x = 0;
        }

        // Update vertical velocity
        if (_isAttemptingJump && _isGrounded)
            _velocity.y = Time.fixedDeltaTime * _jumpSpeed;
        _velocity.y -= Time.fixedDeltaTime * _gravity;
        
        // Update position and rotation
        var position = transform.position + _velocity;
        position.y = Mathf.Max(0, position.y);
        transform.position = position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_shouldFaceLeft ? Vector3.back : Vector3.forward), Time.fixedDeltaTime * _rotationSpeed);
        
        // Update animation
        TriggerAnimation(_isMovingLeft || _isMovingRight ? "Walk" : "Idle");
        
        void TriggerAnimation(string inTrigger)
        {
            if (_lastAnimationTrigger == inTrigger) return;
            _lastAnimationTrigger = inTrigger;
            _animator.SetTrigger(inTrigger);
        }
    }

}
