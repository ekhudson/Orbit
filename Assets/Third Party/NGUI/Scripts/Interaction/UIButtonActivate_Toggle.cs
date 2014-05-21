//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Very basic script that will activate or deactivate an object (and all of its children) when clicked.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button Activate")]
public class UIButtonActivate_Toggle : MonoBehaviour
{
    public GameObject target;
    public bool state = true;
	
	void Awake()
	{
		NGUITools.SetActive(target,state);
	}

    void OnClick () 
	{ 
		
		if (target != null) 
		{
			if(target.activeSelf == !true)
			{				
				NGUITools.SetActive(target, !state);
			}
			else
				NGUITools.SetActive(target, state);
		}
	}
}