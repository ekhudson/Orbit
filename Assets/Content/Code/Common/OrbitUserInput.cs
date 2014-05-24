using UnityEngine;
using System.Collections;

using XInputDotNetPure;

public class OrbitUserInput : UserInput <OrbitUserInput>
{
	public GrendelKeyBinding MoveCharacter = new GrendelKeyBinding("Move Character", KeyCode.None, KeyCode.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
	public GrendelKeyBinding Look = new GrendelKeyBinding("Look", KeyCode.None, KeyCode.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
	public GrendelKeyBinding ToggleConsole = new GrendelKeyBinding("Toggle Console", KeyCode.BackQuote, KeyCode.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None);
	public GrendelKeyBinding PrimaryWeapon = new GrendelKeyBinding("Primary Weapon", KeyCode.Space, KeyCode.R, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.MouseButtons.None, GrendelKeyBinding.GamePadButtonValues.RightThumb);

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
