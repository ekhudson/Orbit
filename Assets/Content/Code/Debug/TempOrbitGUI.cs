using UnityEngine;
using System.Collections;

public class TempOrbitGUI : MonoBehaviour 
{

	private const float kPlayerNameWidth = 256f;

	private void OnGUI()
	{
		GUILayout.BeginHorizontal(GUI.skin.box);

		if(GUILayout.Button("Add Player"))
		{
			OrbitPlayerManager.Instance.AddPlayer();
		}

		DrawToolbar ();

		GUILayout.EndHorizontal();
	}

	private void DrawToolbar()
	{
		GUILayout.BeginHorizontal ();

		foreach (OrbitPlayerComponent player in OrbitPlayerManager.Instance.PlayerList) 
		{
			GUI.color = player.PlayerColor;

			GUILayout.Label(string.Format("Player {0} - Health: {1}", player.PlayerID, player.ShipAttributes.Health.ToString()), GUI.skin.button);
		}

		GUI.color = Color.white;

		GUILayout.EndHorizontal();
	}


}
