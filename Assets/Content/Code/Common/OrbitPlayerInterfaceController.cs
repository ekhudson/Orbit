using UnityEngine;
using System.Collections;

public class OrbitPlayerInterfaceController : MonoBehaviour 
{
	public GameObject CrosshairSpritePrefab;

	//Component References
	private OrbitShipController mShipController;
	private OrbitPlayerComponent mPlayerComponent;

	private UISprite mCrosshairSprite = null;
	private UISprite mGhostCrosshairSprite = null;

	private void Start()
	{
		mShipController = GetComponent<OrbitShipController>();
		mPlayerComponent = GetComponent<OrbitPlayerComponent>();

		if (CrosshairSpritePrefab != null)
		{
			mCrosshairSprite = OrbitUIManager.Instance.AddUIElement(CrosshairSpritePrefab).GetComponent<UISprite>();
			mCrosshairSprite.color = GrendelColor.CustomAlpha(mPlayerComponent.PlayerColor, OrbitUIManager.Instance.UISettings.CrosshairAlpha);
		}
	}

	private void FixedUpdate()
	{
		UpdateCrosshairs();
	}

	private void UpdateCrosshairs()
	{
		mCrosshairSprite.color = GrendelColor.CustomAlpha(mPlayerComponent.PlayerColor, OrbitUIManager.Instance.UISettings.CrosshairAlpha);
		Vector3 crosshairPoint = gameObject.transform.position + gameObject.transform.forward * OrbitUIManager.Instance.UISettings.CrosshairDistance;
		Vector3 convertedCrosshairPoint = Camera.main.WorldToViewportPoint(crosshairPoint);
		Vector3 crosshairUIPos = OrbitUICamera.Instance.UICamera.ViewportToWorldPoint(convertedCrosshairPoint);
		crosshairUIPos.z = 0f;
		mCrosshairSprite.transform.position = crosshairUIPos;
	}

}
