using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(BrawlerHittable))]
[CanEditMultipleObjects]
public class BrawlerHittableEditor : GrendelEditor<BrawlerHittable> 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}
}
