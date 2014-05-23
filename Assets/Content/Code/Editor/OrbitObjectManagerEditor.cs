using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(OrbitObjectManager))]
public class OrbitObjectManagerEditor : GrendelEditor<OrbitObjectManager>
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.Label("Orbit Objects");

		foreach(OrbitObject obj in Target.OrbitObjectList)
		{
			GUILayout.BeginHorizontal();	
			
			if(GUILayout.Button(obj.gameObject.name))
			{
				EditorGUIUtility.PingObject(obj.gameObject);
			}	
			
			GUILayout.EndHorizontal();
		}

		GUILayout.Label("Gravity Objects");
		
		foreach(OrbitGravitator obj in Target.OrbitGravitatorList)
		{
			GUILayout.BeginHorizontal();	
			
			if(GUILayout.Button(obj.gameObject.name))
			{
				EditorGUIUtility.PingObject(obj.gameObject);
			}	
			
			GUILayout.EndHorizontal();
		}

		
	}
}
