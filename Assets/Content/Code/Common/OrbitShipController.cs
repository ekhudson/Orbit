using UnityEngine;
using System.Collections;

[RequireComponent(typeof(OrbitPlayerComponent), typeof(OrbitObject))]
public class OrbitShipController : MonoBehaviour 
{
	public float MaxThrustAmount = 10f;
	public float ThrustPerSecond = 1f;
	public float Drag = 0.8f; //0 to 1;

	private Vector3 mCurrentThrust = Vector3.zero;
	private Vector3 mMouseWorldPosition = Vector3.zero;


	private const float kStopThreshold = 0.05f;

	//Component Refs
	private OrbitPlayerComponent mPlayerComponent;
	private OrbitObject mOrbitObject;
	private Transform mTransform;

	private void Start () 
	{
		mPlayerComponent = GetComponent<OrbitPlayerComponent>();
		mOrbitObject = GetComponent<OrbitObject>();
		mTransform = transform;
		EventManager.Instance.AddHandler<UserInputKeyEvent>(InputHandler);
	}	

	private void Update()
	{
		mTransform.position += mCurrentThrust;

		ApplyDrag();

		MouseLook();
	}

	private void MouseLook()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 100f;

		mMouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
		mMouseWorldPosition.y = mTransform.position.y;

		mTransform.LookAt(mMouseWorldPosition);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(mMouseWorldPosition, 5f);
	}

	private void ApplyDrag()
	{
		mCurrentThrust *= Drag;

		if (Mathf.Abs(mCurrentThrust.x) <= kStopThreshold)
		{
			mCurrentThrust.x = 0f;
		}

		if (Mathf.Abs(mCurrentThrust.z) <= kStopThreshold)
		{
			mCurrentThrust.z = 0f;
		}
	}

	public void InputHandler(object sender, UserInputKeyEvent evt)
	{
		if (!gameObject.activeSelf)
		{
			return;
		}

		if(evt.PlayerIndexInt != mPlayerComponent.PlayerID - 1 && evt.PlayerIndexInt != -1)
		{
			return;
		}

		if(evt.KeyBind == OrbitUserInput.Instance.MoveRight && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
		{
			mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + (ThrustPerSecond * Time.deltaTime), -MaxThrustAmount, MaxThrustAmount);
		}

		if(evt.KeyBind == OrbitUserInput.Instance.MoveLeft && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
		{
			mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + ((ThrustPerSecond * Time.deltaTime) * -1), -MaxThrustAmount, MaxThrustAmount);
		}

		if(evt.KeyBind == OrbitUserInput.Instance.MoveUp && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
		{
			mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + (ThrustPerSecond * Time.deltaTime), -MaxThrustAmount, MaxThrustAmount);
		}
		
		if(evt.KeyBind == OrbitUserInput.Instance.MoveDown && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
		{
			mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + ((ThrustPerSecond * Time.deltaTime) * -1), -MaxThrustAmount, MaxThrustAmount);
		}
	}

}
