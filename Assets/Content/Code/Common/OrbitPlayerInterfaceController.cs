using UnityEngine;
using System.Collections;

public class OrbitPlayerInterfaceController : MonoBehaviour 
{
	private OrbitShipController mShipController;


	private void Start()
	{
		mShipController = GetComponent<OrbitShipController>();
	}

}
