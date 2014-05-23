using UnityEngine;
using System.Collections;

public class TempOrbitGUI : MonoBehaviour 
{

	private void OnGUI()
	{
		GUILayout.BeginHorizontal(GUI.skin.box);

		if(GUILayout.Button("Add Player"))
		{
			OrbitPlayerManager.Instance.AddPlayer();
		}

		GUILayout.EndHorizontal();
	}


}
