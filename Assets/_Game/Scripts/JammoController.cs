using UnityEngine;

public class JammoController : MonoBehaviour
{
    private Command _lastCommand = null;

    [Header("Physics")][Space]
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _jumpSpeed = 15f;
    [SerializeField] private float _gravity = 1f;
    [SerializeField] private float _fallMultiplier = 3f;
    private Vector3 _velocity = Vector3.zero;
    private bool _isMovingRight = false;
    private bool _isMovingLeft = false;
    private bool _shouldFaceLeft = false;
    private bool _isAttemptingJump = false;
    private bool _hasReleasedJump = true;
    private bool _isJumping = false;
    private bool _isRunning = false;
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
        {
            _isAttemptingJump = inCommand.State == Command.KeyState.DOWN;
            _hasReleasedJump = inCommand.State == Command.KeyState.UP;
        }

        if (inCommand.Action == Command.ActionType.ACTION_0)
            _isRunning = inCommand.State == Command.KeyState.DOWN;

        _lastCommand = inCommand;
    }

    /// <summary>
    /// Update physics and animation
    /// </summary>
    public void FixedUpdateController()
    {
        // Update animation
        var isMovingLeftOrRight = _isMovingLeft || _isMovingRight;
        if (_isGrounded && _isAttemptingJump) TriggerAnimation("Jump");
        else if (_isGrounded && _isJumping) { TriggerAnimation("Land"); _isJumping = false;}
        else if (_isGrounded && isMovingLeftOrRight && _isRunning) TriggerAnimation("Run");
        else if (_isGrounded && isMovingLeftOrRight && !_isRunning) TriggerAnimation("Walk");
        else if (_isGrounded && !isMovingLeftOrRight) TriggerAnimation("Idle");

        void TriggerAnimation(string inTrigger)
        {
            if (_lastAnimationTrigger == inTrigger) return;
            _lastAnimationTrigger = inTrigger;
            _animator.SetTrigger(inTrigger);
        }

        // Update horizontal velocity
        if (_isGrounded)
        {
            if (_isMovingLeft)
            {
                _velocity.x = -1 * Time.fixedDeltaTime * (_isRunning ? _runSpeed : _moveSpeed);
                _shouldFaceLeft = true;
            }
            else if (_isMovingRight)
            {
                _velocity.x = Time.fixedDeltaTime * (_isRunning ? _runSpeed : _moveSpeed);
                _shouldFaceLeft = false;
            }
            else _velocity.x = 0;
        }

        // Update vertical velocity
        if (_isAttemptingJump && _isGrounded)
        {
            _velocity.y = Time.fixedDeltaTime * _jumpSpeed;
            _isAttemptingJump = false;
            _isJumping = true;
        }
        _velocity.y -= Time.fixedDeltaTime * _gravity * (_hasReleasedJump ? _fallMultiplier : 1);
        
        // Update position and rotation
        var position = transform.position + _velocity;
        position.y = Mathf.Max(0, position.y);
        transform.position = position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_shouldFaceLeft ? Vector3.back : Vector3.forward), Time.fixedDeltaTime * _rotationSpeed);

    }

}
