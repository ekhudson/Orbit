using UnityEngine;
using System.Collections;

using XInputDotNetPure;

public class BrawlerUserInput : UserInput <BrawlerUserInput>
{
    public GrendelKeyBinding UseKey01 = new GrendelKeyBinding("Use01", KeyCode.W, KeyCode.W, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
    public GrendelKeyBinding UseKey02 = new GrendelKeyBinding("Use02", KeyCode.S, KeyCode.S, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);    
    public GrendelKeyBinding RotateObjectRight = new GrendelKeyBinding("RotateObjectRight", KeyCode.E, KeyCode.DownArrow, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
    public GrendelKeyBinding InspectObject = new GrendelKeyBinding("InspectObject", KeyCode.F, KeyCode.DownArrow, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
	public GrendelKeyBinding MoveCharacter = new GrendelKeyBinding("Move Character", KeyCode.None, KeyCode.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
	public GrendelKeyBinding Jump = new GrendelKeyBinding("Jump", KeyCode.Space, KeyCode.Return, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
	public GrendelKeyBinding ToggleConsole = new GrendelKeyBinding("Toggle Console", KeyCode.BackQuote, KeyCode.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
	public GrendelKeyBinding Attack = new GrendelKeyBinding("Attack", KeyCode.Space, KeyCode.R, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.GamePadButtonValues.RightThumb);
	public GrendelKeyBinding Kick = new GrendelKeyBinding("Kick", KeyCode.D, KeyCode.R, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.GamePadButtonValues.B);
	public GrendelKeyBinding Block = new GrendelKeyBinding("Block", KeyCode.Space, KeyCode.R, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.GamePadButtonValues.Y);

	public bool IsGamePadActive(int gamepadID)
	{
		PlayerIndex testPlayerIndex = (PlayerIndex)gamepadID;
		GamePadState testState = GamePad.GetState(testPlayerIndex);
		if (testState.IsConnected)
		{
			return true;
		}	
		else
		{
			return false;
		}
	}
}
