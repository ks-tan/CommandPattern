using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private JammoController _playerController;

    private void Update()
    {
        if (_inputManager.TryGetInput(out Command input))
            _playerController.ReadCommand(input);
    }

    private void FixedUpdate()
    {
        _playerController.FixedUpdateController();
    }
}
