using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityManager : Singleton<EntityManager> 
{

	public int UpdateAmount = 5;
	public int CreationAmount = 5;
	public static int MaxEntities = 1000; //limit the max number of entities allowed in the scene
	public static Dictionary<int, Entity> EntityDictionary = new Dictionary<int, Entity>();	//list of entities
	
	private static int _lastOpenIndex = 0; //a reference to the last open index for adding new entities
	//private static Entity[] _toUpdateArray = new Entity[0];
	private static List<Entity> mToUpdateList = new List<Entity>();
	private int _updateIndex = 0; //current spot in the _toUpdate Array
	private int _updateTargetIndex = 0; //where we'll end our update cycle
	//private static List<int> _removeList = new List<int>(); //for removing entities from the EntityDictionary during updating
	//private static List<Entity> _addList = new List<Entity>(); //for adding entities to the EntityDictionary during updating
	private static Entity _testEntity; //this test entity is used for updating
	
	// Use this for initialization
	void Awake () 
	{		
		base.Awake();
		//_toUpdateArray = new Entity[MaxEntities];
	}
	
	void Start()
	{
		StartCoroutine( ManagedUpdate() );
	}
	
	IEnumerator ManagedUpdate()
	{		
		while (true)
		{				
			_updateTargetIndex = Mathf.Clamp(_updateTargetIndex + UpdateAmount, 0, mToUpdateList.Count);			
		
			for ( ; _updateIndex <= _updateTargetIndex && _updateIndex < MaxEntities; _updateIndex++)
			{	
				if (_updateIndex > mToUpdateList.Count - 1)
				{
					continue;
				}

				_testEntity = mToUpdateList[_updateIndex];	

				if(_testEntity != null)
				{ 
					_testEntity.CalledUpdate(); 
				}				
			}			
			
			if ( _updateIndex >= (MaxEntities - 1) || _updateIndex >= mToUpdateList.Count - 1) 
			{
				_updateTargetIndex = 0; 
				_updateIndex = 0;
			} //reset the cycle
			
			yield return new WaitForSeconds(Time.deltaTime); //pace the coroutine
		}
	}
					
	public static void AddToUpdateList(Entity entity)
	{		
		if (mToUpdateList.Count < MaxEntities)
		{			
			mToUpdateList.Add(entity);
			//_toUpdateArray[_lastOpenIndex] = entity;
			//_lastOpenIndex++;
		}
		else
		{
			Destroy(entity.BaseGameObject);
			Debug.LogWarning("Too many entities!");
		}					
	}	
	
	/// <summary>
	/// AddToDictionary
	/// 
	/// This function is called by entities when they wake, adding them
	/// to the list for managed updates and for quick finding through InstanceID
	/// 
	/// </summary>
	/// <param name="ID">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="entity">
	/// A <see cref="Entity"/>
	/// </param>
	public static void AddToDictionary(int ID, Entity entity)
	{		
		EntityDictionary[ID] = entity; //add the entity to the dictionary		
	}
	
	/// <summary>
	/// Removes from dictionary.
	/// </summary>
	/// <param name='ID'>
	/// I.
	/// </param>
	public static void RemoveFromDictionary(int ID)
	{
		try
		{
			mToUpdateList.Remove(EntityDictionary[ID]);
			EntityDictionary.Remove(ID); //attempt to remove an entity from the dictionary
		}
		catch
		{
			Debug.LogWarning("Entity Dictionary does not contain an entry for Entity " + ID);
		}
	}

}
