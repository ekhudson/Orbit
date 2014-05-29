using UnityEngine;
using System.Collections;

public class OrbitPlayerComponent : MonoBehaviour 
{
	private int mPlayerID = -1; //should get set from 0 - 4. If this is ever -1 it means this is not a valid player
	private int mAssociatedGamepad = -1; //again, if this is ever -1 it means no gamepad is assigned;
	private bool mIsActivePlayer = true;
	private bool mAIPlayer = false;
	private Color mPlayerColor = Color.white; //TODO: Move this to a PlayerCustomization component?
	private OrbitShipAttributes mShipAttributes = null;
	private OrbitShipController mShipController = null;

	public int PlayerID
	{
		get
		{
			return mPlayerID;
		}
	}
	
	public int AssociatedGamepad
	{
		get
		{
			return mAssociatedGamepad;
		}
	}

	public bool IsActivePlayer
	{
		get
		{
			return mIsActivePlayer;
		}
		set
		{
			mIsActivePlayer = value;
		}
	}

	public bool IsAI
	{
		get
		{
			return mAIPlayer;
		}
		set
		{
			mAIPlayer = value;
		}
	}

	public void SetID(int id)
	{
		mPlayerID = id;
	}
	
	public void SetGamepadID(int gamepadID)
	{
		mAssociatedGamepad = gamepadID;
	}

	public void SetPlayerColor(Color color)
	{
		mPlayerColor = color;
		
		SpriteRenderer sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
		
		if (sprite != null)
		{ 
			GetComponentInChildren<SpriteRenderer>().color = mPlayerColor;
		}
	}

	public Color PlayerColor
	{
		get
		{
			return mPlayerColor;
		}
	}

	public OrbitShipAttributes ShipAttributes
	{
		get
		{
			if (mShipAttributes == null)
			{
				mShipAttributes = gameObject.GetComponent<OrbitShipAttributes>();
			}

			return mShipAttributes;
		}
	}

	public OrbitShipController ShipController
	{
		get
		{
			if (mShipController == null)
			{
				mShipController = gameObject.GetComponent<OrbitShipController>();
			}

			return mShipController;
		}
	}

}
