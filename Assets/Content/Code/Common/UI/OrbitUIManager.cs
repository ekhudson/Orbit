using UnityEngine;
using System.Collections;

public class OrbitUIManager : Singleton<OrbitUIManager>
{
	public OrbitUISettings UISettings;

	private UIPanel mUIRootPanel;
	private GameObject mGameObject;


	public UIPanel RootPanel
	{
		get
		{
			return RootPanel;
		}
		set
		{
			mUIRootPanel = value;
		}
	}

	private void Start()
	{
		mUIRootPanel = GetComponent<UIPanel>();
		mGameObject = gameObject;
	}

	public GameObject AddUIElement(GameObject prefab)
	{
		GameObject newObject = NGUITools.AddChild(mGameObject, prefab);
		NGUITools.MakePixelPerfect(newObject.transform);
		return newObject;
	}
}
