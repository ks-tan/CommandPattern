# Fighting Game Template using the Command Pattern

![KeyMap](./Screenshots/SavedReplays.gif)

This is an educational resource to explain the "Command Pattern" and it's usefulness in game development.

Note that this resource is created to be pedagogical in nature and not for production, so optimization and other improvements that could still be made are outside the scope of this Unity project.

## Getting started

![KeyMap](./Screenshots/Combos.gif)

Here are the controls for "SampleScene". They can be remapped in the InputManager component of the GameManager game object.

![KeyMap](./Screenshots/KeyMap.png)

- `LEFT/RIGHT`: Moving left and right
- `SPACE`: Jump
- `Z`: Hold to run
- `X`: Punch. Also executes chained punches with consecutive presses
- `DOWN + RIGHT/LEFT + X`: "Hadouken" / Fireball attack
- `Q`: Create an instant copy of Jammo that replays your last sequence of moves
- `W`: Write all recorded player inputs into a .txt "Replay" file

If you run the scene with `GameManager.PlayReplayFile = true` (assignable from the scene's Inspector), the game will read all recorded inputs in "Replay.txt" and play them automatically.

## Explaining the Command Pattern

![KeyMap](./Screenshots/CommandQueueDiagram.png)

The purpose of the Command pattern is to encapsulate a request (in our case, a user input) as an object, thereby allowing it to be passed as a parameter to other objects (in our case, a character controller). These "Command" objects can also stored as part of a collection for delayed/scheduled execution, in what is known as "input buffering".

This setup enables the InputManager to remain unaware of the implementation details of the character controller and vice-versa, hence achieving loose coupling. It also allows us to easily extend our game with cool features, such as instant replays or to create chained actions/combos.

## "no-replays" branch

As mentioned, one of the biggest advantages of the Command Pattern is the decoupling of the "requestor" (i.e. InputManager) from the "executor" (i.e. "PlayerController"). To fully showcase the simplicity of such an implementation, you may go to the "no-replays" branch. You would see that:

- The API (i.e. public methods) of these classes is few and straightforward. By reducing the surface through which different components can interact with each other, it also reduces the chances of bugs and makes code easy to trace.

- InputManager and JammoController are 100% agnostic to each other's implementation. You can see that from how they hold no direct references/handlers to each other!

If designed well, we can have a highly extensible and de-coupled input system with tiny code footprint!

```c#
// InputManager.cs
public class InputManager : MonoBehaviour
{
    // For reading player input e.g. "Input.GetKeyDown(...)"
    // And converting player input to a Command object
    public bool TryGetInput(out Command input) { ... }
}

// JammoController.cs
public class JammoController : MonoBehaviour
{
    // For accepting Command objects from InputManager
    public void ReadCommand(Command inCommand) { ... }
} 

// GameManager.cs
public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private JammoController _playerController;

    private void Update()
    {
        // Simply takes Command objects received from InputManager
        // and pipe it to JammoController (player)
        if (_inputManager.TryGetInput(out Command input))
            _playerController.ReadCommand(input);
    }
}
```

## License and Credits

This project is licensed under the terms of the GNU General Public License v3.0.

- Developer: Tan Kang Soon, 2023
- Character Model: "Jammo" from "Mix and Jam"
- Animations: "Mixamo" from Adobe
- Particle Effects: "Particle Pack" from Unity Technologies
