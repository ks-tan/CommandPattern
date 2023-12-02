using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JammoController : MonoBehaviour
{
    [Header("Physics")][Space]
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _jumpSpeed = 15f;
    [SerializeField] private float _gravity = 1f;
    [SerializeField] private float _fallMultiplier = 3f;

    private Vector3 _velocity = Vector3.zero;

    // Movement state flags
    private bool _isMovingRight = false;
    private bool _isMovingLeft = false;
    private bool _shouldFaceLeft = false;
    private bool _isAttemptingJump = false;
    private bool _hasReleasedJump = true;
    private bool _isJumping = false;
    private bool _isRunning = false;
    private bool _isAttemptingPunch = false;
    private bool _isGrounded => transform.position.y <= 0;

    [Header("Model")][Space]
    [SerializeField] private Animator _animator = null;
    private string _lastAnimationTrigger = null;

    // Input buffering on punches to help us execute a 1-2-3 punch!
    private bool _isPunching => _punchCoroutine != null;
    private Coroutine _punchCoroutine = null;
    private int _punchChainStep = 0;
    private Queue<IEnumerator> _punchCommandQueue = new Queue<IEnumerator> ();

    // Input buffering for special moves
    [Header("Special Moves")][Space]
    [SerializeField] private float _maxTimeBetweenCommands = 0.2f;
    [SerializeField] private int _maxCommandsInSpecialMovesBuffer = 10;
    [SerializeField] private List<SpecialMove> _specialMoves = null;
    private bool _isExecutingSpecialMove => _specialMoveCoroutine != null;
    private Coroutine _specialMoveCoroutine = null;
    private Queue<Command> _specialMovesCommandQueue = new Queue<Command> ();

    private bool _isAttacking => _isPunching || _isExecutingSpecialMove;

    private Command _lastInputCommand = null;

    public void Reset()
    {
        _velocity = Vector3.zero;
        _isMovingRight = false;
        _isMovingLeft = false;
        _shouldFaceLeft = false;
        _isAttemptingJump = false;
        _hasReleasedJump = true;
        _isJumping = false;
        _isRunning = false;
        _isAttemptingPunch = false;
        _lastInputCommand = null;
        _lastAnimationTrigger = null;
        _punchChainStep = 0;
        _punchCoroutine = null;
        _punchCommandQueue.Clear();
        _specialMoveCoroutine = null;
        _specialMovesCommandQueue.Clear();
        transform.position = new Vector3(0, 0, -6);
    }

    /// <summary>
    /// Read a command and change the flags representing the character's state
    /// </summary>
    /// <param name="inCommand"></param>
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

        if (inCommand.Action == Command.ActionType.ACTION_1)
            _isAttemptingPunch = inCommand.State == Command.KeyState.DOWN;

        // Enqueue the current command into the special moves command queue
        // But first, check whether it should be cleared (more than time limit) or dequeued (max commands reached)
        if (_lastInputCommand != null)
        {
            if (inCommand.Time - _lastInputCommand.Time > _maxTimeBetweenCommands)
                _specialMovesCommandQueue.Clear();
            else if (_specialMovesCommandQueue.Count >= _maxCommandsInSpecialMovesBuffer)
                _specialMovesCommandQueue.Dequeue();
        }
        _specialMovesCommandQueue.Enqueue(inCommand);

        _lastInputCommand = inCommand;
    }

    /// <summary>
    /// Update physics and animation
    /// </summary>
    public void FixedUpdateController()
    {
        if (!_isExecutingSpecialMove)
        {
            // Update animation
            if (_isGrounded)
            {
                if (_isAttemptingPunch)
                {
                    // Do not enqueue more than 3 punch commands (there are only max 3 chained punches)
                    if (_punchCommandQueue.Count < 3)
                        _punchCommandQueue.Enqueue(PunchCommand());

                    // If there is only 1 punch command enqueued, trigger it.
                    // Every punch command will trigger the next punch command when completed.
                    if (_punchCommandQueue.Count == 1)
                        _punchCoroutine = StartCoroutine(_punchCommandQueue.Peek());

                    _isAttemptingPunch = false;
                }
                else if (!_isAttacking)
                {
                    var isMovingLeftOrRight = _isMovingLeft || _isMovingRight;
                    if (_isAttemptingJump) TriggerAnimation("Jump");
                    else if (_isJumping) { TriggerAnimation("Land"); _isJumping = false; }
                    else if (isMovingLeftOrRight && _isRunning) TriggerAnimation("Run");
                    else if (isMovingLeftOrRight && !_isRunning) TriggerAnimation("Walk");
                    else if (!isMovingLeftOrRight) TriggerAnimation("Idle");
                }
            }

            // Check for special moves (note: always override other moves)
            foreach (var specialMove in _specialMoves)
            {
                if (specialMove.IsSequenceFound(_specialMovesCommandQueue))
                {
                    _specialMoveCoroutine = StartCoroutine(ExecuteSpecialMove(specialMove.Name));
                    _specialMovesCommandQueue.Clear();
                    break;
                }
            }
        }

        // Update horizontal velocity
        if (_isGrounded)
        {
            if (_isMovingLeft && !_isAttacking)
            {
                _velocity.x = -1 * Time.fixedDeltaTime * (_isRunning ? _runSpeed : _moveSpeed);
                _shouldFaceLeft = true;
            }
            else if (_isMovingRight && !_isAttacking)
            {
                _velocity.x = Time.fixedDeltaTime * (_isRunning ? _runSpeed : _moveSpeed);
                _shouldFaceLeft = false;
            }
            else _velocity.x = 0;
        }

        // Update vertical velocity
        if (_isAttemptingJump && _isGrounded && !_isAttacking)
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

    /// <summary>
    /// An IEnumerator can also be a "command"! This helps us execute chained punches (1-2-3!).
    /// It stores information on behaviour to be executed and triggers the next command in the queue when it is completed.
    /// </summary>
    private IEnumerator PunchCommand()
    {
        TriggerAnimation("Punch" + _punchChainStep.ToString());

        _punchChainStep++;

        if (_punchChainStep == 1) yield return new WaitForSeconds(0.25f);
        if (_punchChainStep == 2) yield return new WaitForSeconds(0.5f);
        if (_punchChainStep == 3) yield return new WaitForSeconds(0.5f);

        _punchChainStep %= 3;

        _punchCommandQueue.Dequeue();

        if (_punchCommandQueue.Count <= 0)
        {
            _punchChainStep = 0;
            _punchCoroutine = null;
        }
        else _punchCoroutine = StartCoroutine(_punchCommandQueue.Peek());
    }

    private IEnumerator ExecuteSpecialMove(SpecialMove.Move inMove)
    {
        TriggerAnimation(inMove.ToString());
        yield return new WaitForSeconds(1f);
        _specialMoveCoroutine = null;
    }

    private void TriggerAnimation(string inTrigger)
    {
        if (_lastAnimationTrigger == inTrigger) return;
        _lastAnimationTrigger = inTrigger;
        _animator.SetTrigger(inTrigger);
    }
}
