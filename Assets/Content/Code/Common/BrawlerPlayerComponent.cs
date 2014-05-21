using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrawlerPlayerComponent : BrawlerHittable
{
	private int mPlayerID = -1; //should get set from 0 - 4. If this is ever -1 it means this is not a valid player
	private int mAssociatedGamepad = -1; //again, if this is ever -1 it means no gamepad is assigned;
	private Color mPlayerColor = Color.white;
	private bool mIsActivePlayer = true;
	private bool mAIPlayer = false;

	//TODO: Wrap this in a PlayerAttributes class
	#region PlayerAttributes
	public float PlayerMinStrength = 1f;
	public float PlayerMaxStrength = 10f;
	public float AttackTime = 0.4f;
	public float AttackChargeTime = 1f;
	public float MinimumTimeBetweenAttacks = 0.5f;
	public float MoveSpeed = 1.0f;
	public float ClimbSpeed = 1.0f;
	public float JumpForce = 2.0f;
	public float MinimumJumpTime = 0.5f;
	public AnimationCurve JumpCurve = new AnimationCurve();
	public float DropMaxAmountX = 0.4f; //maximum amount of movement along X when attempting to drop
	public float DropForce = 2f;
	public float LandingTime = 0.2f;
	public float HurtTime = 0.3f;
	public float AirControl = 0.9f;
	public float ConstantFriction =  0.9f;
	public TriggerVolume PunchBox;
	public Transform HitParticle;
	#endregion

	#region Sprites
	public Sprite DefaultSprite;
    public int CurrentPlayerOrientation = 1; //1 == right, -1 == left
	#endregion

	protected Vector3 mTarget = Vector3.zero;
	protected CharacterEntity mController;
	protected BrawlerCharacterStateController mStateController;
	public BrawlerHitboxController HitboxController;
	
	private bool mIsDropping = false;  
	
	private Vector3 mInitialRotation;
	private SpriteRenderer mSpriteRenderer;
	private float mLastAttackEndTime;
	private Vector3 mLastMovingDirection;

	private Vector3 mCurrentAttackDirection;
	
	public enum PlayerStates
	{
		IDLE,
		MOVING,
		JUMPING,
		FALLING,
		LANDING,
		FROZEN,
		ATTACKING_GROUND,
		ATTACKING_AIR,
		JUMPING_JOYSTICK,
		HURT,
		ATTACKING_GROUND_CHARGING,
		ATTACKING_AIR_CHARGING,
		ATTACKING_CROUCH_CHARGING,
		ATTACKING_CROUCH,
		KICK_GROUND,
		KICK_AIR,
		KICK_CROUCH,
		KICK_GROUND_CHARGING,
		KICK_CROUCH_CHARGING,
		KICK_AIR_CHARGING,
		BLOCK_GROUND,
		BLOCK_AIR,
		BLOCK_CROUCH,
		CROUCH,
	}
	
	protected PlayerStates mPlayerState = PlayerStates.IDLE;
	protected float mTimeInState = 0.0f;

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

	public Color PlayerColor
	{
		get
		{
			return mPlayerColor;
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

	public PlayerStates GetState
	{
		get
		{
			return mPlayerState;
		}
	}
	
	public CharacterEntity GetEntity
	{
		get
		{
			return mController;
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

    public SpriteRenderer PlayerSpriteRenderer
    {
        get
        {
            return mSpriteRenderer;
        }
        set
        {
            mSpriteRenderer = value;
        }
         
    }

	public float CurrentMoveSpeed
	{
		get
		{
			return MoveSpeed;
		}
	}
	
	protected override void Start()
	{
		base.Start ();
		EventManager.Instance.AddHandler<UserInputKeyEvent>(InputHandler);
		mController = GetComponent<CharacterEntity>();
		mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
		mStateController = GetComponent<BrawlerCharacterStateController>();
		rigidbody.WakeUp();        
	}  
	
	protected void FixedUpdate()
	{
		if (mController == null || mPlayerState == PlayerStates.FROZEN)
		{
			return;
		}
		
		switch(GetState)
		{
		case PlayerStates.IDLE:

			if (Mathf.Abs(mTarget.x) > 0f)
			{
				SetState(PlayerStates.MOVING);
			}

			if (!mController.IsGrounded)
			{
				SetState(PlayerStates.FALLING);
			}

			break;
			
		case PlayerStates.MOVING:

			if (Mathf.Abs(mTarget.x) <= 0f)
			{
				//SetState(PlayerStates.IDLE);

				//This is not a good way to figure out if the player is idle. Need to find something else.
			}

			if (!mController.IsGrounded)
			{
				SetState(PlayerStates.FALLING);
			}
			
			break;

		case PlayerStates.JUMPING_JOYSTICK:
		case PlayerStates.JUMPING:
			
			if (mTimeInState > JumpCurve.keys[JumpCurve.length - 1].time && mTimeInState > MinimumJumpTime)
			{
				SetState(PlayerStates.FALLING);
				return;
			}
			
			mController.SetGrounded(false);
			mController.Move( mController.BaseTransform.up * (JumpForce * JumpCurve.Evaluate(mTimeInState)));	

			mLastMovingDirection = mTarget;

			mTarget *= AirControl;
			
			break;
			
		case PlayerStates.FALLING:
			
			if (mController.IsGrounded)
			{
				SetState(PlayerStates.LANDING);
			}

			mLastMovingDirection = mTarget;

			mTarget *= AirControl;

			if (mIsDropping)
			{
				mTarget += Physics.gravity * DropForce;
			}
			
			break;
			
		case PlayerStates.LANDING:

			if (mTimeInState > LandingTime)
			{
				SetState(PlayerStates.IDLE);
			}
			
			break;
			
		case PlayerStates.ATTACKING_GROUND:

			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.IDLE);
				mLastAttackEndTime = Time.realtimeSinceStartup;
				ResetAttack();
			}
			
			break;
		
		case PlayerStates.ATTACKING_AIR:
			
			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.FALLING);
				mLastAttackEndTime = Time.realtimeSinceStartup;
				ResetAttack();
			}

			if (mController.IsGrounded)
			{
				SetState(PlayerStates.ATTACKING_GROUND, true);
				return;
			}
			
			break;
		
		case PlayerStates.ATTACKING_CROUCH:

			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.CROUCH);
				mLastAttackEndTime = Time.realtimeSinceStartup;
				ResetAttack();
			}

			break;
			
		case PlayerStates.HURT:

			if (mTimeInState > HurtTime)
			{
				SetState(PlayerStates.IDLE);
			}
			
			break;

		case PlayerStates.ATTACKING_GROUND_CHARGING:

			mSpriteRenderer.color = Color.Lerp(mSpriteRenderer.color, Color.white, mTimeInState);

			if (mTimeInState > AttackChargeTime)
			{
				SetState(PlayerStates.ATTACKING_GROUND);
				return;
			}

			ChargeAttack();

			break;

		case PlayerStates.ATTACKING_AIR_CHARGING:

			mSpriteRenderer.color = Color.Lerp(mSpriteRenderer.color, Color.white, mTimeInState);

			if (mTimeInState > AttackChargeTime)
			{
				SetState(PlayerStates.ATTACKING_AIR);
				return;
			}

			if (mController.IsGrounded)
			{
				SetState(PlayerStates.ATTACKING_GROUND_CHARGING, true);
				return;
			}

			ChargeAttack();

			mTarget += mLastMovingDirection;

			break;
		
		case PlayerStates.ATTACKING_CROUCH_CHARGING:
			
			mSpriteRenderer.color = Color.Lerp(mSpriteRenderer.color, Color.white, mTimeInState);
			
			if (mTimeInState > AttackChargeTime)
			{
				SetState(PlayerStates.ATTACKING_CROUCH);
				return;
			}

			ChargeAttack();
			
			break;
			
		case PlayerStates.BLOCK_GROUND:			

			
			break;
			
		case PlayerStates.BLOCK_CROUCH:			

			
			break;
			
		case PlayerStates.BLOCK_AIR:			

			
			break;
			
		case PlayerStates.KICK_AIR:
			
			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.FALLING);
				mLastAttackEndTime = Time.realtimeSinceStartup;
				ResetAttack();
			}
			
			if (mController.IsGrounded)
			{
				SetState(PlayerStates.KICK_GROUND, true);
				return;
			}

			break;
			
		case PlayerStates.KICK_AIR_CHARGING:
			
			mSpriteRenderer.color = Color.Lerp(mSpriteRenderer.color, Color.white, mTimeInState);
			
			if (mTimeInState > AttackChargeTime)
			{
				SetState(PlayerStates.KICK_AIR);
				return;
			}

			if (mController.IsGrounded)
			{
				SetState(PlayerStates.KICK_GROUND_CHARGING, true);
				return;
			}

			ChargeAttack();
			
			mTarget += mLastMovingDirection;
			
			break;
			
		case PlayerStates.KICK_CROUCH:
			
			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.CROUCH);
				mLastAttackEndTime = Time.realtimeSinceStartup;
				ResetAttack();
			}
			
			break;
			
		case PlayerStates.KICK_CROUCH_CHARGING:
			
			mSpriteRenderer.color = Color.Lerp(mSpriteRenderer.color, Color.white, mTimeInState);
			
			if (mTimeInState > AttackChargeTime)
			{
				SetState(PlayerStates.KICK_CROUCH);
				return;
			}

			ChargeAttack();
			
			break;
			
		case PlayerStates.KICK_GROUND:

			if (mTimeInState > AttackTime)
			{
				SetState(PlayerStates.IDLE);
				mLastAttackEndTime = Time.realtimeSinceStartup;
				ResetAttack();
			}
			
			break;
			
		case PlayerStates.KICK_GROUND_CHARGING:
			
			mSpriteRenderer.color = Color.Lerp(mSpriteRenderer.color, Color.white, mTimeInState);
			
			if (mTimeInState > AttackChargeTime)
			{
				SetState(PlayerStates.KICK_GROUND);
				return;
			}

			ChargeAttack();

			break;

		}		
		
		Vector3 norm = mTarget;//.normalized; //normalize was cancelling out the effects of drag
		mController.Move( ((new Vector3(norm.x, 0, norm.z) * (CurrentMoveSpeed)) + new Vector3(0, mTarget.y, 0)) * Time.deltaTime);
		mTarget = Vector3.zero; 
		
		if (ConstantFriction > 0)
		{
			Vector3 currentVelocity = mController.BaseRigidbody.velocity;
			mController.BaseRigidbody.velocity = new Vector3(currentVelocity.x * ConstantFriction, currentVelocity.y, currentVelocity.z * ConstantFriction);
		}
		
		mTimeInState += Time.deltaTime;
	}	
	
	public void MoveEntity(Vector3 direction)
	{
		mTarget += direction;
	}      

	public void SetState(PlayerStates state)
	{
		SetState(state, false);
	}

	public void SetState(PlayerStates state, bool carryOverTimeInState)
	{
		if (state == mPlayerState)
		{
			return;
		}
		
		switch(state)
		{
		case PlayerStates.IDLE:
			
			rigidbody.useGravity = true;
			mIsDropping = false;
			
			break;
			
		case PlayerStates.MOVING:
			
			rigidbody.useGravity = true;
			mIsDropping = false;
			
			break;

		case PlayerStates.JUMPING_JOYSTICK:

			if (mPlayerState == PlayerStates.FALLING)
			{
				return;
			}
			
			mController.SetGrounded(false);

			break;

		case PlayerStates.JUMPING:
			
			if (mPlayerState == PlayerStates.FALLING)
			{
				return;
			}
			
			mController.SetGrounded(false);
			
			break;
			
		case PlayerStates.FALLING:
			
			rigidbody.useGravity = true;
			
			break;
			
		case PlayerStates.LANDING:

			break;

		case PlayerStates.ATTACKING_GROUND:

			mSpriteRenderer.color = mPlayerColor;
			break;

		case PlayerStates.ATTACKING_AIR:
			
			mSpriteRenderer.color = mPlayerColor;				
			break;

		case PlayerStates.HURT:

			mIsDropping = false;
			
			break;

		case PlayerStates.ATTACKING_AIR_CHARGING:

			ChargeAttack();

			break;

		case PlayerStates.ATTACKING_GROUND_CHARGING:

			break;

		case PlayerStates.CROUCH:

			break;

		case PlayerStates.ATTACKING_CROUCH:

			mSpriteRenderer.color = mPlayerColor;

			break;
		
		case PlayerStates.ATTACKING_CROUCH_CHARGING:

			break;

		case PlayerStates.BLOCK_GROUND:

			break;

		case PlayerStates.BLOCK_CROUCH:

			break;

		case PlayerStates.BLOCK_AIR:

			break;

		case PlayerStates.KICK_AIR:

			mSpriteRenderer.color = mPlayerColor;

			break;

		case PlayerStates.KICK_AIR_CHARGING:

			break;

		case PlayerStates.KICK_CROUCH:

			mSpriteRenderer.color = mPlayerColor;

			break;

		case PlayerStates.KICK_CROUCH_CHARGING:

			break;

		case PlayerStates.KICK_GROUND:

			mSpriteRenderer.color = mPlayerColor;

			break;

		case PlayerStates.KICK_GROUND_CHARGING:

			break;		
		
		}	

		if (!carryOverTimeInState)
		{
			mTimeInState = 0.0f;
		}
		
		mPlayerState = state;
        mStateController.SetState(mPlayerState.ToString()); //prepare new animation
	}

	private void ChargeAttack()
	{
		float time = Mathf.Clamp (mTimeInState / AttackChargeTime, 0f, 1f);
		HitboxController.AttackCollider.AttackStrength = Mathf.Lerp (PlayerMinStrength, PlayerMaxStrength, time);
	}

	private void ResetAttack()
	{
		HitboxController.AttackCollider.AttackStrength = PlayerMinStrength;
	}
	
	public void InputHandler(object sender, UserInputKeyEvent evt)
	{
		if (!gameObject.activeSelf)
		{
			return;
		}

		if (evt.KeyBind != BrawlerUserInput.Instance.MoveCharacter)
		{
			//Debug.Log (string.Format("player {2} getting {0} for player {1}", evt.KeyBind.BindingName, (evt.PlayerIndexInt + 1).ToString(), mPlayerID.ToString()));
		}
		
		if(evt.PlayerIndexInt != mPlayerID - 1 && evt.PlayerIndexInt != -1)
		{
			return;
		}


		if (mPlayerState != PlayerStates.FROZEN || mPlayerState != PlayerStates.HURT)
		{
			
			if(evt.KeyBind == BrawlerUserInput.Instance.MoveLeft && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
				{
					//return;
				}

				if (mPlayerState == PlayerStates.CROUCH || mPlayerState == PlayerStates.BLOCK_CROUCH || mPlayerState == PlayerStates.BLOCK_GROUND)
				{
					return;
				}

				mTarget += -Camera.main.transform.right;

				if (mSpriteRenderer.transform.rotation.eulerAngles.y == 0)
				{
					mSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
					CurrentPlayerOrientation = -1;
				}

			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.MoveRight && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
				{
					//return;
				}

				if (mPlayerState == PlayerStates.CROUCH || mPlayerState == PlayerStates.BLOCK_CROUCH || mPlayerState == PlayerStates.BLOCK_GROUND)
				{
					return;
				}

				mTarget += Camera.main.transform.right;

				if (mSpriteRenderer.transform.rotation.eulerAngles.y != 0)
				{
					mSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
					CurrentPlayerOrientation = 1;
				}
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.MoveUp && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				
			}

			if(evt.KeyBind == BrawlerUserInput.Instance.MoveDown && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				if (mPlayerState == PlayerStates.FALLING && !mIsDropping)
				{
					mIsDropping = true;
				}
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.Jump)
			{
				if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING)
				{
					return;
				}

				switch(evt.Type)
				{
				case UserInputKeyEvent.TYPE.KEYDOWN: 
					
					SetState(PlayerStates.JUMPING);
					
					break;
					
				case UserInputKeyEvent.TYPE.KEYUP:
					
					SetState(PlayerStates.FALLING);
					
					break;
					
				case UserInputKeyEvent.TYPE.KEYHELD:

					if (mPlayerState == PlayerStates.IDLE || mPlayerState == PlayerStates.MOVING)
					{
						SetState(PlayerStates.JUMPING);
					}
					
					break;
					
				case UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN: 
					
					switch(mPlayerState)
					{
					case PlayerStates.IDLE:
						
						SetState(PlayerStates.JUMPING);
						
						break;
						
					case PlayerStates.MOVING:
						
						SetState(PlayerStates.JUMPING);
						
						break;
						
					case PlayerStates.JUMPING:
						
						break;
						
					case PlayerStates.FALLING:						
						
						
						break;
						
					case PlayerStates.LANDING:
						
						break;
						
					case PlayerStates.HURT:
						
						break;
					}
					
					break;
					
				case UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_UP:
					
					if (mPlayerState == PlayerStates.JUMPING)
					{
						SetState(PlayerStates.FALLING);
					}
					
					break;
					
				case UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_HELD:

					if (mPlayerState == PlayerStates.IDLE || mPlayerState == PlayerStates.MOVING)
					{
						SetState(PlayerStates.JUMPING);
					}
					
					break;
				}
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.PrimaryFire && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN))
			{
				
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.UseKey01 && evt.Type == UserInputKeyEvent.TYPE.KEYDOWN)
			{
				
			}
			
			if(evt.KeyBind == BrawlerUserInput.Instance.UseKey02 && evt.Type == UserInputKeyEvent.TYPE.KEYDOWN)
			{
				
			}

			if (evt.KeyBind == BrawlerUserInput.Instance.Attack && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN) ||
			    evt.KeyBind == BrawlerUserInput.Instance.Kick && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN) ||
			    evt.KeyBind == BrawlerUserInput.Instance.Block && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN))
			{
				if ( (Time.realtimeSinceStartup - mLastAttackEndTime) < MinimumTimeBetweenAttacks )
				{
					return;
				}

				if (mPlayerState == PlayerStates.IDLE || mPlayerState == PlayerStates.MOVING)
				{
					if (evt.KeyBind == BrawlerUserInput.Instance.Attack)
					{
						SetState(PlayerStates.ATTACKING_GROUND_CHARGING);
					}
					else if (evt.KeyBind == BrawlerUserInput.Instance.Kick)
					{
						SetState(PlayerStates.KICK_GROUND_CHARGING);
					}
					else if (evt.KeyBind == BrawlerUserInput.Instance.Block)
					{
						SetState(PlayerStates.BLOCK_GROUND);
					}
				}
				else if (mPlayerState == PlayerStates.JUMPING || mPlayerState == PlayerStates.JUMPING_JOYSTICK || mPlayerState == PlayerStates.FALLING)
				{
					if (evt.KeyBind == BrawlerUserInput.Instance.Attack)
					{
						SetState(PlayerStates.ATTACKING_AIR_CHARGING);
					}
					else if (evt.KeyBind == BrawlerUserInput.Instance.Kick)
					{
						SetState(PlayerStates.KICK_AIR_CHARGING);
					}
					else if (evt.KeyBind == BrawlerUserInput.Instance.Block)
					{
						SetState(PlayerStates.BLOCK_AIR);
					}

				}
				else if (mPlayerState == PlayerStates.CROUCH)
				{
					if (evt.KeyBind == BrawlerUserInput.Instance.Attack)
					{
						SetState(PlayerStates.ATTACKING_CROUCH_CHARGING);
					}
					else if (evt.KeyBind == BrawlerUserInput.Instance.Kick)
					{
						SetState(PlayerStates.KICK_CROUCH_CHARGING);
					}
					else if (evt.KeyBind == BrawlerUserInput.Instance.Block)
					{
						SetState(PlayerStates.BLOCK_CROUCH);
					}
				}
				else if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING || 
				         mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING ||
				         mPlayerState == PlayerStates.ATTACKING_CROUCH_CHARGING)
				{
					//do nothing, charging
				}
			}
			
			if (evt.KeyBind == BrawlerUserInput.Instance.MoveCharacter)
			{

				if (mPlayerState != PlayerStates.CROUCH && mPlayerState != PlayerStates.BLOCK_CROUCH && mPlayerState != PlayerStates.BLOCK_GROUND)
				{
					mTarget.x += (evt.JoystickInfo.AmountX);			

					if (evt.JoystickInfo.AmountX < 0 && mSpriteRenderer.transform.rotation.eulerAngles.y == 0)
					{
						mSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
                        CurrentPlayerOrientation = -1;
					}
					else if (evt.JoystickInfo.AmountX > 0 && mSpriteRenderer.transform.rotation.eulerAngles.y != 0)
					{
						mSpriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
                        CurrentPlayerOrientation = 1;
					}
				}
				//else
				//{
					//mCurrentAttackDirection = new Vector3(evt.JoystickInfo.AmountX, evt.JoystickInfo.AmountY, 0).normalized;
				//}

				if (evt.JoystickInfo.AmountY > 0)
				{
					switch(mPlayerState)
					{
					case PlayerStates.IDLE:
						
						SetState(PlayerStates.JUMPING_JOYSTICK);
						
						break;
						
					case PlayerStates.MOVING:
						
						SetState(PlayerStates.JUMPING_JOYSTICK);
						
						break;
						
					case PlayerStates.JUMPING:
						
						break;
						
					case PlayerStates.FALLING:						

						
						break;
						
					case PlayerStates.LANDING:
						
						break;
						
					case PlayerStates.HURT:
						
						break;

					case PlayerStates.ATTACKING_AIR_CHARGING:

						//mCurrentAttackDirection.y = new Vector3(evt.JoystickInfo.AmountX, evt.JoystickInfo.AmountY, 0).normalized.y;

						break;

					case PlayerStates.ATTACKING_GROUND_CHARGING:

						//mCurrentAttackDirection.y = new Vector3(evt.JoystickInfo.AmountX, evt.JoystickInfo.AmountY, 0).normalized.y;

						break;
					}
				}
				else if (evt.JoystickInfo.AmountY <= 0)
				{
					switch(mPlayerState)
					{
					case PlayerStates.IDLE:
						
						if (evt.JoystickInfo.AmountY != 0)
						{
							SetState(PlayerStates.CROUCH);
						}
						
						break;
						
					case PlayerStates.MOVING:
						
						if (evt.JoystickInfo.AmountY != 0)
						{
							SetState(PlayerStates.CROUCH);
						}
						
						break;

					case PlayerStates.CROUCH:

						if (evt.JoystickInfo.AmountY == 0)
						{
							SetState(PlayerStates.IDLE);
						}

						break;

					case PlayerStates.BLOCK_CROUCH:

						if (evt.JoystickInfo.AmountY == 0)
						{
							SetState(PlayerStates.BLOCK_GROUND);
						}

						break;

					case PlayerStates.BLOCK_GROUND:

						if(evt.JoystickInfo.AmountY < 0)
						{
							SetState(PlayerStates.BLOCK_CROUCH);
						}

						break;
					
					case PlayerStates.ATTACKING_CROUCH_CHARGING:

						if (evt.JoystickInfo.AmountY == 0)
						{
							SetState(PlayerStates.ATTACKING_GROUND_CHARGING, true);
						}

						break;
					
					case PlayerStates.KICK_CROUCH_CHARGING:
						
						if (evt.JoystickInfo.AmountY == 0)
						{
							SetState(PlayerStates.KICK_GROUND_CHARGING, true);
						}
						
						break;
						
					case PlayerStates.JUMPING_JOYSTICK:

						SetState(PlayerStates.FALLING);

						break;
						
					case PlayerStates.FALLING:						
						
						
						break;
						
					case PlayerStates.LANDING:
						
						break;
						
					case PlayerStates.HURT:
						
						break;
						
					case PlayerStates.ATTACKING_AIR_CHARGING:						

						
						break;
						
					case PlayerStates.ATTACKING_GROUND_CHARGING:						

						
						break;
					}
				}
				
				if (evt.JoystickInfo.AmountY < 0)
				{
					if (mPlayerState == PlayerStates.FALLING && !mIsDropping && Mathf.Abs(evt.JoystickInfo.AmountX) < DropMaxAmountX)
					{
						mIsDropping = true;
					}
				}

			}
		}

		if (evt.KeyBind == BrawlerUserInput.Instance.Attack && (evt.Type == UserInputKeyEvent.TYPE.KEYUP || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_UP) ||
		    evt.KeyBind == BrawlerUserInput.Instance.Kick && (evt.Type == UserInputKeyEvent.TYPE.KEYUP || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_UP) ||
		    evt.KeyBind == BrawlerUserInput.Instance.Block && (evt.Type == UserInputKeyEvent.TYPE.KEYUP || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_UP))
		{
			if (mPlayerState == PlayerStates.ATTACKING_AIR_CHARGING && evt.KeyBind == BrawlerUserInput.Instance.Attack)
			{
				SetState(PlayerStates.ATTACKING_AIR);
			}
			else if (mPlayerState == PlayerStates.ATTACKING_GROUND_CHARGING && evt.KeyBind == BrawlerUserInput.Instance.Attack)
			{
				SetState(PlayerStates.ATTACKING_GROUND);
			}
			else if (mPlayerState == PlayerStates.ATTACKING_CROUCH_CHARGING && evt.KeyBind == BrawlerUserInput.Instance.Attack)
			{
				SetState(PlayerStates.ATTACKING_CROUCH);
			}
			else if (mPlayerState == PlayerStates.KICK_AIR_CHARGING && evt.KeyBind == BrawlerUserInput.Instance.Kick)
			{
				SetState(PlayerStates.KICK_AIR);
			}
			else if (mPlayerState == PlayerStates.KICK_GROUND_CHARGING && evt.KeyBind == BrawlerUserInput.Instance.Kick)
			{
				SetState(PlayerStates.KICK_GROUND);
			}
			else if (mPlayerState == PlayerStates.KICK_CROUCH_CHARGING && evt.KeyBind == BrawlerUserInput.Instance.Kick)
			{
				SetState(PlayerStates.KICK_CROUCH);
			}
			else if (mPlayerState == PlayerStates.BLOCK_AIR && evt.KeyBind == BrawlerUserInput.Instance.Block)
			{
				SetState(PlayerStates.FALLING);
			}
			else if (mPlayerState == PlayerStates.BLOCK_CROUCH && evt.KeyBind == BrawlerUserInput.Instance.Block)
			{
				SetState(PlayerStates.CROUCH);
			}
			else if (mPlayerState == PlayerStates.BLOCK_GROUND && evt.KeyBind == BrawlerUserInput.Instance.Block)
			{
				SetState(PlayerStates.IDLE);
			}
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

	public void Hurt()
	{
		SetState(PlayerStates.HURT);
	}

	protected override void OnHit(HitEvent hitEvent)
	{
		if (hitEvent.Sender == this) 
		{
			return;
		}

		mTarget += (hitEvent.HitVector * hitEvent.HitForce);
						
		Transform go = (Transform)Instantiate(HitParticle, hitEvent.HitPoint, Quaternion.identity);
		
		ParticleSystem hitParticle = go.GetComponent<ParticleSystem>();
		
		if (hitParticle != null)
		{
			hitParticle.startColor = PlayerColor;
			Destroy (go.gameObject, hitParticle.duration);
		}

		Hurt ();	
						
	}

	public void Hit(HitEvent hitEvent)
	{
		OnHit(hitEvent);
	}
	
}


