using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private JammoController _playerController;

    private void Start()
    {
        _inputManager.Setup(_playerController);
    }

    private void Update()
    {
        _inputManager.UpdateInput();
        _playerController.UpdateController();
    }

    private void FixedUpdate()
    {
        _playerController.FixedUpdateController();
    }
}
