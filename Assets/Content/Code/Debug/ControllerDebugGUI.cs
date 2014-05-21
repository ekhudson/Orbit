using UnityEngine;
using UnityEditor;

using System.Collections;

using XInputDotNetPure; // Required in C#

public class ControllerDebugGUI : MonoBehaviour 
{
	public BrawlerUserInput UserInputReference;

	public Texture2D JoystickCircleTexture; 
	public Texture2D JoystickCircleRing;
	public Texture2D JoystickCenterTexture;

	private const float kDebugStripHeightPercentage = 0.25f;
	private const float kControllerChoicePercent = 0.03f;
	private const float kJoystickCenterWidthPercent = 0.05f;
	private const float kShoulderTriggerWidthPercent = 0.03f;

	private float mStripHeight = 0f;
	private Color mJoystickCircleColor = new Color(1,1,1, 0.35f);

	private int mControllerChoice = 0;
	private string[] mChoiceOptions = new string[]{"Controller 01", "Controller 02", "Controller 03", "Controller 04"};
	private float mChoiceHeight = 0f;
	private float mTriggerWidth = 0f;
	private float mTriggerHeight = 32f;

	private GamePadState mCurrentState;

	private Texture2D mWhiteTexture;

	private void OnGUI()
	{
		if (mWhiteTexture == null)
		{
			mWhiteTexture = new Texture2D(1,1);
			mWhiteTexture.SetPixel(1,1, Color.white);
			mWhiteTexture.Apply();
		}

		Screen.showCursor = true;
		DebugStrip();
	}

	private void DebugStrip()
	{
		mStripHeight = Screen.height * kDebugStripHeightPercentage;
		mChoiceHeight = Screen.height * kControllerChoicePercent;
		mTriggerWidth = Screen.width * kShoulderTriggerWidthPercent;

		Color guiColor = BrawlerPlayerManager.Instance.PlayerColours[mControllerChoice];
		guiColor.a = 0.35f;
		GUI.color = guiColor;

		Rect stripRect = new Rect(0f, Screen.height - mStripHeight, Screen.width, mStripHeight);

		GUI.DrawTexture(stripRect, mWhiteTexture);
	
		GUI.color = Color.white;

		Rect choiceRect = new Rect(0f, (Screen.height - mStripHeight) - mChoiceHeight, Screen.width, mChoiceHeight);

		mControllerChoice = GUI.SelectionGrid(choiceRect, mControllerChoice, mChoiceOptions, 4); 

		if (!GatherGamepadInput(mControllerChoice))
		{
			GUI.Label(stripRect, "Controller not connected");
		}
		else
		{
			GatherGamepadInput(mControllerChoice);


			TriggerDebug(mCurrentState.Triggers.Left, new Rect(0, Screen.height - mStripHeight, mTriggerWidth, mTriggerHeight));
			BumperDebug("Left", mCurrentState.Buttons.LeftShoulder, new Rect(0, (Screen.height - mStripHeight) + mTriggerHeight, mTriggerWidth, mTriggerHeight));

			Rect leftStickRect = new Rect(0f, Screen.height - mStripHeight, mStripHeight, mStripHeight);
			leftStickRect.x += mTriggerWidth;

			DPadDebug(new Rect(leftStickRect.x + leftStickRect.width + (leftStickRect.width * 0.125f), (Screen.height - mStripHeight) +(leftStickRect.width * 0.25f) + leftStickRect.width * 0.125f, leftStickRect.width * 0.25f, leftStickRect.width * 0.25f));

			Rect rightStickRect = leftStickRect;
			rightStickRect.x += leftStickRect.width + (leftStickRect.width * 0.5f);

			JoystickDebug(new Vector2(mCurrentState.ThumbSticks.Left.X, mCurrentState.ThumbSticks.Left.Y), leftStickRect, mCurrentState.Buttons.LeftStick);
			JoystickDebug(new Vector2(mCurrentState.ThumbSticks.Right.X, mCurrentState.ThumbSticks.Right.Y), rightStickRect, mCurrentState.Buttons.RightStick);

			TriggerDebug(mCurrentState.Triggers.Right, new Rect(rightStickRect.x + rightStickRect.width, Screen.height - mStripHeight, mTriggerWidth, mTriggerHeight));	
			BumperDebug("Right", mCurrentState.Buttons.RightShoulder, new Rect(rightStickRect.x + rightStickRect.width, (Screen.height - mStripHeight) + mTriggerHeight, mTriggerWidth, mTriggerHeight));

			FaceButtonDebug(new Rect(rightStickRect.x + rightStickRect.width + (rightStickRect.width * 0.125f), (Screen.height - mStripHeight) +(rightStickRect.width * 0.25f) + rightStickRect.width * 0.125f, rightStickRect.width * 0.25f, rightStickRect.width * 0.25f));

		}

	}

	private void TriggerDebug(float amount, Rect position)
	{
		GUI.Box(position, amount.ToString(),GUI.skin.button);

		position.width *= amount / 1f;

		GUI.color = Color.green;

		GUI.Box(position, string.Empty, GUI.skin.button);

		GUI.color = Color.white;
	}

	private void BumperDebug(string label, ButtonState buttonState, Rect position)
	{
		if (buttonState == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}

		GUI.Button(position, label);

		GUI.color = Color.white;
	}

	private void DPadDebug(Rect position)
	{
		float buttonWidth = position.width * 0.33f;

		GUI.Box(position, string.Empty);

		if (mCurrentState.DPad.Up == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}

		GUI.Button(new Rect(position.x + buttonWidth, position.y, buttonWidth, buttonWidth), "U");

		GUI.color = Color.white;

		if (mCurrentState.DPad.Left == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}

		GUI.Button(new Rect(position.x, position.y + buttonWidth, buttonWidth, buttonWidth), "L");
		
		GUI.color = Color.white;

		if (mCurrentState.DPad.Right == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}

		GUI.Button(new Rect(position.x + (buttonWidth * 2), position.y + buttonWidth, buttonWidth, buttonWidth), "R");
		
		GUI.color = Color.white;
		
		if (mCurrentState.DPad.Down == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}

		GUI.Button(new Rect(position.x + buttonWidth, position.y + (buttonWidth * 2), buttonWidth, buttonWidth), "D");
		
		GUI.color = Color.white;

		if (mCurrentState.Buttons.Back == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}

		GUI.Button(new Rect(position.x - (buttonWidth * 2), position.y - (position.height * 1.25f), position.width, buttonWidth), "Back");

		GUI.color = Color.white;

		if (mCurrentState.Buttons.Start == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}

		GUI.Button(new Rect(position.x + (buttonWidth * 2), position.y - (position.height * 1.25f), position.width, buttonWidth), "Start");
		
		GUI.color = Color.white;

	}

	private void FaceButtonDebug(Rect position)
	{
		float buttonWidth = position.width * 0.33f;
		
		GUI.Box(position, string.Empty);
		
		if (mCurrentState.Buttons.Y == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}
		
		GUI.Button(new Rect(position.x + buttonWidth, position.y, buttonWidth, buttonWidth), "Y");
		
		GUI.color = Color.white;
		
		if (mCurrentState.Buttons.X == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}
		
		GUI.Button(new Rect(position.x, position.y + buttonWidth, buttonWidth, buttonWidth), "X");
		
		GUI.color = Color.white;
		
		if (mCurrentState.Buttons.B == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}
		
		GUI.Button(new Rect(position.x + (buttonWidth * 2), position.y + buttonWidth, buttonWidth, buttonWidth), "B");
		
		GUI.color = Color.white;
		
		if (mCurrentState.Buttons.A == ButtonState.Pressed)
		{
			GUI.color = Color.green;
		}
		
		GUI.Button(new Rect(position.x + buttonWidth, position.y + (buttonWidth * 2), buttonWidth, buttonWidth), "A");
		
		GUI.color = Color.white;
	}

	private void JoystickDebug(Vector2 thumbStickData, Rect joystickRect, ButtonState buttonState)
	{
		if (buttonState == ButtonState.Pressed)
		{
			Color green = Color.green;
			green.a = 0.25f;
			GUI.color = Color.green;
		}
		else
		{
			GUI.color = mJoystickCircleColor;
		}

		GUI.DrawTexture(joystickRect, JoystickCircleTexture);
		GUI.color = Color.white;

		float deadZoneDiameter = mStripHeight;

		Rect deadZoneRect = new Rect(joystickRect.center.x - ((deadZoneDiameter * 0.5f) * UserInputReference.JoystickDeadzoneX), joystickRect.center.y - ((deadZoneDiameter * 0.5f) * UserInputReference.JoystickDeadzoneY), deadZoneDiameter * UserInputReference.JoystickDeadzoneX, deadZoneDiameter * UserInputReference.JoystickDeadzoneY);

		GUI.color = Color.black;
		GUI.DrawTexture(deadZoneRect, JoystickCircleRing);
		GUI.color = Color.white;

		float joystickCenterWidth = joystickRect.width * kJoystickCenterWidthPercent;
		Vector2 joystickCenter = new Vector2(joystickRect.center.x + (joystickRect.width * (thumbStickData.x * 0.5f)), joystickRect.center.y -(joystickRect.width * (thumbStickData.y * 0.5f)));

		Rect joystickCenterRect = new Rect(joystickCenter.x - (joystickCenterWidth * 0.5f), joystickCenter.y - (joystickCenterWidth * 0.5f), joystickCenterWidth, joystickCenterWidth);
		GUI.DrawTexture(joystickCenterRect, JoystickCenterTexture);

		GUI.Label(new Rect(joystickRect.x, Screen.height - 20f, 256f, 20f), string.Format("X:{0}, Y:{1}", thumbStickData.x.ToString(), thumbStickData.y.ToString()));
	}

	private bool GatherGamepadInput(int gamepadNumber)
	{			
		PlayerIndex playerIndex = (PlayerIndex)gamepadNumber;
		mCurrentState = new GamePadState();
		
		mCurrentState = GamePad.GetState(playerIndex, GamePadDeadZone.None);
		
		if (!mCurrentState.IsConnected)
		{
			return false;
		}
		
		return true;
	}

}
