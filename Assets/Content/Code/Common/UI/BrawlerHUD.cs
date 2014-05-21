using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrawlerHUD : Singleton<BrawlerHUD>
{   
    public bool HideWindowsMouse = true;
	public GameObject TestEffect;

	private Rect mScreenRect = new Rect();
	   
    private void Start()
    {
		if (HideWindowsMouse)
        {
            Screen.showCursor = false;
        }

		mScreenRect = new Rect(0,0, Screen.width, Screen.height);
	}

    private void OnGUI()
    {
		GUILayout.BeginArea(mScreenRect);

		DrawPlayerStatuses();

//		if(GUI.Button(new Rect(500, 500, 100, 100), "Test FX"))
//		{
//			GrendelFXManager.Instance.SpawnEffect(TestEffect, BrawlerPlayerManager.Instance.PlayerList[Random.Range(0, BrawlerPlayerManager.Instance.PlayerList.Count)].transform.position, Quaternion.identity, true);			                                    
//		}

		GUILayout.EndArea();
    }  


	private void DrawPlayerStatuses()
	{
		GUILayout.BeginHorizontal();

		foreach (BrawlerPlayerComponent player in BrawlerPlayerManager.Instance.PlayerList)
		{
			DrawPlayerStatus(player);
		}

		if (BrawlerPlayerManager.Instance.PlayerList.Count < 4) 
		{
			if (GUILayout.Button("Add Player"))
			{
				BrawlerPlayerManager.Instance.AddPlayer();
			}
		}

		GUILayout.FlexibleSpace();

		GUILayout.EndHorizontal();
	}

	private void DrawPlayerStatus(BrawlerPlayerComponent player)
	{
		GUI.color = player.PlayerColor;
		GUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Width(256f));
		GUILayout.Label(player.PlayerID.ToString());
		GUILayout.Label(string.Format("Gamepad Active: {0}",BrawlerUserInput.Instance.IsGamePadActive(player.AssociatedGamepad).ToString()));

		if (!player.IsActivePlayer)
		{
			if (GUILayout.Button("Make AI"))
			{
				BrawlerAIBase ai = player.gameObject.AddComponent<BrawlerAIBase>();
				ai.WakeAI();
			}
		}

		GUILayout.EndHorizontal();
		GUI.color = Color.white;
		GUILayout.FlexibleSpace();
	}

}
