using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrawlerPlayerManager : Singleton<BrawlerPlayerManager> 
{
	public List<GameObject> CharacterList = new List<GameObject>();
	public List<BrawlerPlayerComponent> PlayerList = new List<BrawlerPlayerComponent>();
	public Transform[] SpawnPoints;

	private int mCurrentActivePlayers = 1;

	public int CurrentActivePlayers
	{
		get
		{
			return mCurrentActivePlayers;
		}
	}	

	public List<Color> PlayerColours = new List<Color>
	{
		Color.white,
		Color.red,
		GrendelColor.GrendelYellow,
		Color.green,
	};

	private void Start()
	{
		//PlayerList = new List<BrawlerPlayerComponent>((BrawlerPlayerComponent[])Object.FindObjectsOfType(typeof(BrawlerPlayerComponent)));
		SetupPlayerData();
	}

	private void SetupPlayerData()
	{
		int id = 1;

		for(int i = 0; i < 4; i++)
		{

			if (BrawlerUserInput.Instance.IsGamePadActive(id - 1))
		    {
				mCurrentActivePlayers++;
				GameObject go = (GameObject)GameObject.Instantiate(CharacterList[0], RandomSpawnPoint().position, Quaternion.identity);
				BrawlerPlayerComponent player = go.GetComponent<BrawlerPlayerComponent>();
				PlayerList.Add(player);
				player.SetID(id);
				player.SetGamepadID(id - 1);
				player.IsActivePlayer = true;
				player.SetPlayerColor(PlayerColours[id]);
			}
			else if (id != 1) //don't disable the first player
			{
				//player.IsActivePlayer = false;
				//player.gameObject.SetActive(false);
			}

			id++;
		}
	}

	public void AddPlayer()
	{
		int id = PlayerList.Count + 1;

		//if (BrawlerUserInput.Instance.IsGamePadActive(id - 1))
		//{
			mCurrentActivePlayers++;
			GameObject go = (GameObject)GameObject.Instantiate(CharacterList[0], RandomSpawnPoint().position, Quaternion.identity);
			BrawlerPlayerComponent player = go.GetComponent<BrawlerPlayerComponent>();
			PlayerList.Add(player);
			player.SetID(id);
			player.SetGamepadID(id - 1);
			player.IsActivePlayer = true;
			player.SetPlayerColor(PlayerColours[id]);
		//}
	}

	public Transform RandomSpawnPoint()
	{
		return SpawnPoints[Random.Range(0, SpawnPoints.Length)];
	}

}
