# Fighting Game Template in Unity using the Command Pattern

This is an educational resource to explain the "Command Pattern" and it's usefulness in game development.

Note that this resource is created to be pedagogical in nature and not for production, so optimization and other improvements that could still be made are outside the scope of this project.

## Getting started

Here are the controls for "SampleScene". They can be remapped in the InputManager component of the GameManager game object.

- `LEFT/RIGHT`: Moving left and right
- `SPACE`: Jump
- `Z`: Hold to run
- `X`: Punch. Also executes chained punches with consecutive presses
- `DOWN + RIGHT/LEFT + X`: "Hadouken" / Fireball attack
- `Q`: Create an instant copy of Jammo that replays your last sequence of moves
- `W`: Write all recorded player inputs into a .txt "Replay" file

If you run the scene with `GameManager.PlayReplayFile = true` (assignable from the scene's Inspector), the game will read all recorded player inputs in "Replay.txt" and play them out automatically.

## Explaining the Command Pattern

tbc

## Credits

- Developer: Tan Kang Soon (me), 2023
- Character Model: "Jammo" from "Mix and Jam"
- Animations: "Mixamo" from Adobe
- Particle Effects: "Particle Pack" from Unity Technologies
