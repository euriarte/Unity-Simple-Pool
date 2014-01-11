using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor( typeof( PoolManager) )]
public class PoolManagerEditor : Editor {
	Pool remove;


	public override void OnInspectorGUI () {
		EditorStyles.textField.wordWrap = true;
		PoolManager pm = (PoolManager)target;
		if(remove!=null)pm.pools.Remove(remove);
		pm.hideInHierarchy=EditorGUILayout.Toggle("Hide in Hierarchy",pm.hideInHierarchy);
		pm.persistent=EditorGUILayout.Toggle("Dont Destroy On Load",pm.persistent);
		pm.dynamic=EditorGUILayout.Toggle("Dynamic Pool",pm.dynamic);
		if(pm.dynamic){
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			pm.dynamicSize=(int)Mathf.Clamp(EditorGUILayout.IntField("Default Size",pm.dynamicSize),0,Mathf.Infinity);
			pm.dynamicMax=(int)Mathf.Clamp(EditorGUILayout.IntField("Default Max Size",pm.dynamicMax),0,Mathf.Infinity);
			if (pm.dynamicMax==0) EditorGUILayout.TextArea("This pool size will grow without limit if necessary");
			pm.dynamicLifeTime=Mathf.Clamp(EditorGUILayout.FloatField("Default Life Time",pm.dynamicLifeTime),0,Mathf.Infinity);
			if (pm.dynamicLifeTime<0.001F) EditorGUILayout.TextArea("Pooled objects will remain forever in the scene until removed by script");
			else if (pm.dynamicLifeTime<0.5F) EditorGUILayout.TextArea("The live Time of the objects is quite short, Are you sure?");
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Name",GUILayout.Width(80));
		EditorGUILayout.LabelField("Prefab",GUILayout.Width(Screen.width-283));
		EditorGUILayout.LabelField(new GUIContent("H","Hide In Herarchy"),GUILayout.Width(16));
		EditorGUILayout.LabelField(new GUIContent("►","Play On Spawn"),GUILayout.Width(16));
		EditorGUILayout.LabelField(new GUIContent("s","Size"),GUILayout.Width(25));
		EditorGUILayout.LabelField(new GUIContent("max","Max size"),GUILayout.Width(35));
		EditorGUILayout.LabelField(new GUIContent("L","Life Time"),GUILayout.Width(30));
		EditorGUILayout.LabelField(new GUIContent("×","Delete"),GUILayout.Width(14));
		EditorGUILayout.EndHorizontal();
		foreach(Pool p in pm.pools){
			EditorGUILayout.BeginHorizontal();
			p.name=EditorGUILayout.TextField(p.name,GUILayout.Width(80));
			p.prefab=(GameObject)EditorGUILayout.ObjectField(p.prefab,typeof(GameObject),true);
			if(GUILayout.Button((pm.hideInHierarchy && !p._hideInHierarchy)?"○":(p._hideInHierarchy)?"●":"","Label",GUILayout.Width(16))){
				p.hideInHierarchy=!p._hideInHierarchy;
			}
			if(GUILayout.Button( (p.playOnSpawn)?"►":"","Label",GUILayout.Width(16))){
				p.playOnSpawn=!p.playOnSpawn;
			}
			p.size=EditorGUILayout.IntField(p.size,GUILayout.Width(25));
			p.maxsize=EditorGUILayout.IntField(p.maxsize,GUILayout.Width(35));
			p.lifeTime=EditorGUILayout.FloatField(p.lifeTime,GUILayout.Width(30));
			if(GUILayout.Button("×",GUILayout.Width(19))){
				remove=p;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Add New Pool")){
			pm.CreatePool();
		}
		EditorGUILayout.EndHorizontal();

	}
}
