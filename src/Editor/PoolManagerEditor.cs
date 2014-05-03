using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor( typeof( PoolManager) )]
public class PoolManagerEditor : Editor {
	Pool rem;
	PoolGroup remg;
	PoolManager pm;
	Color warningColor;
	List<Pool> tempaddplace;
	List<Pool> tempadd=new List<Pool>();

	public override void OnInspectorGUI () {
		if (tempadd.Count>0){
			foreach(Pool pool in tempadd){
				if(tempaddplace.Contains(pool)){
					int option=EditorUtility.DisplayDialogComplex("Already exist a similar pool ["+pool.name+"]","There is a pool for "+pool.name+" what do you want to do? ","Create New", "Increase Limit", "Skip");
					switch(option){
					case 0:	
						pool.name+="-"+tempaddplace.Count.ToString();
						tempaddplace.Add(pool);
						break;
					case 1:
						tempaddplace[tempaddplace.IndexOf(pool)].size+=pool.size;
						tempaddplace[tempaddplace.IndexOf(pool)].maxsize+=pool.maxsize;
						break;
					}
				}
				else tempaddplace.Add(pool);
			}
			tempadd.Clear();
		}
		EditorStyles.textField.wordWrap = true;
		pm = (PoolManager)target;
		if(PoolManager.instance==null){
			PoolManager.instance=pm;
			PoolManager.instanceT=pm.transform;
		}
		
		pm.populateOnStart=EditorGUILayout.Toggle("Populate on Start",pm.populateOnStart);
		pm.autoConfig=EditorGUILayout.Toggle("Auto Config Effects",pm.autoConfig);
		pm.recyclable=EditorGUILayout.Toggle("Recyclable",pm.recyclable);
		pm.hideInHierarchy=EditorGUILayout.Toggle("Hide in Hierarchy",pm.hideInHierarchy);
		pm.persistent=EditorGUILayout.Toggle("Dont Destroy On Load",pm.persistent);
		pm.stackPools=EditorGUILayout.Toggle("Stack Pools",pm.stackPools);
		pm.dynamic=EditorGUILayout.Foldout(pm.dynamic,"Default Values");
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
			pm.dynamicParent=(Transform)EditorGUILayout.ObjectField("Default Parent",pm.dynamicParent,typeof(Transform),true);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Pools");
		if(GUILayout.Button("Add New",GUILayout.Width(70))){
			pm.CreatePool();
		}
		EditorGUILayout.EndHorizontal();
		DropInPools(null,GUILayoutUtility.GetLastRect(),Event.current);
		foreach(Pool p in pm.pools){
			Pool (p);
		}
		EditorGUILayout.EndVertical();
		if(rem!=null){
			pm.pools.Remove(rem);
			rem=null;
		}

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Pool Groups");
		if(GUILayout.Button("Add New",GUILayout.Width(70))){
			pm.CreatePoolGroup();
		}
		EditorGUILayout.EndHorizontal();
		foreach(PoolGroup pg in pm.poolGroups){
			PoolGroup (pg);
		}
		EditorGUILayout.EndVertical();
		if(remg!=null){
			pm.poolGroups.Remove(remg);
			remg=null;
		}
	}
	
	void PoolGroup(PoolGroup pg){
		GUI.backgroundColor=(string.IsNullOrEmpty(pg.name)&&PoolManager.instance.poolGroups.Contains(pg))?new Color(0.7F,0.1F,0.1F,0.8F):new Color(0.6F,0.6F,0.9F,0.8F);
		EditorGUILayout.BeginVertical("Box");
		GUI.backgroundColor=new Color(1,1,1,0.7F);
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button(new GUIContent("≡","Drag\nSpawn"),"Label",GUILayout.Width(14))){
			PoolManager.SpawnGroup(pg.name,PoolManager.instanceT);
		}
		if(GUILayout.Button((pg.open)?new GUIContent("▲","Colapse"):new GUIContent("▼","Expand"),"Label",GUILayout.Width(14)))pg.open=!pg.open;
		EditorGUILayout.LabelField("Group Name:",GUILayout.Width(80));
		pg.name=EditorGUILayout.TextField(pg.name);
		if(GUILayout.Button("—",GUILayout.Width(19))){
			remg=pg;
		}
		EditorGUILayout.EndHorizontal();
		if(pg.open){
			EditorGUILayout.BeginHorizontal();
			pg.propagateLayer=EditorGUILayout.Toggle("Propagate Layer",pg.propagateLayer);
			pg.layer=(int)EditorGUILayout.LayerField(pg.layer);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Pools");
			if(GUILayout.Button("Add New",GUILayout.Width(70))){
				pg.pools.Add(new Pool());
			}
			EditorGUILayout.EndHorizontal();
			DropInPools(pg,GUILayoutUtility.GetLastRect(),Event.current);
			foreach(Pool p in pg.pools)Pool (p);
			if(rem!=null){
				pg.pools.Remove(rem);
				rem=null;
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Sets");
			if(GUILayout.Button("Add New",GUILayout.Width(70))){
				pg.sets.Add(new PoolGroup());
			}
			EditorGUILayout.EndHorizontal();
			foreach(PoolGroup poolgroup in pg.sets)PoolGroup(poolgroup);
			if(remg!=null && pg.sets.Contains(remg)){
				pg.sets.Remove(remg);
				remg=null;
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
	}
	void Pool(Pool p){
		Color prev=GUI.backgroundColor;
		GUI.backgroundColor=(p.prefab==null)?new Color(0.86F,0.47F,0.2F,1):new Color(0.5F,(p.playOnSpawn)?0.9f:0.70f,0.5F,1);
		EditorGUILayout.BeginVertical("Box");
		GUI.backgroundColor=new Color(1,1,1,0.7F);
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("≡","Label",GUILayout.Width(14))){
			PoolManager.Spawn(p.name,PoolManager.instanceT);
		}
		if(GUILayout.Button((p.open)?"▲":"▼","Label",GUILayout.Width(14)))p.open=!p.open;
		p.name=EditorGUILayout.TextField(p.name);
		if (!p.open){
			if(GUILayout.Button((pm.recyclable && !p._recyclable)?"○":(p._recyclable)?"●":"","Label",GUILayout.Width(14))){
				p.recyclable=!p._recyclable;
			}
			if(GUILayout.Button((pm.hideInHierarchy && !p._hideInHierarchy)?"✓":(p._hideInHierarchy)?"✔":"","Label",GUILayout.Width(14))){
				p.hideInHierarchy=!p._hideInHierarchy;
			}
			if(GUILayout.Button( (p.playOnSpawn)?"►":"","Label",GUILayout.Width(14))){
				p.playOnSpawn=!p.playOnSpawn;
			}
			p.size=EditorGUILayout.IntField(p.size,GUILayout.Width(25));
			p.maxsize=EditorGUILayout.IntField(p.maxsize,GUILayout.Width(25));
			p.lifeTime=EditorGUILayout.FloatField(p.lifeTime,GUILayout.Width(25));
			
		}
		if(GUILayout.Button("—",GUILayout.Width(19))){
			rem=p;
		}
		EditorGUILayout.EndHorizontal();
		DropInPool(p,GUILayoutUtility.GetLastRect(),Event.current);
		if(p.open){
			EditorGUILayout.BeginVertical("Box");
			p.name=EditorGUILayout.TextField("Pool Name",p.name);
			p.prefab=(GameObject)EditorGUILayout.ObjectField("Item",p.prefab,typeof(GameObject),true);
			p.recyclable=EditorGUILayout.Toggle("Recyclable",p._recyclable);
			p.hideInHierarchy=EditorGUILayout.Toggle("Hide In Hierarchy",p._hideInHierarchy);
			p.playOnSpawn=EditorGUILayout.Toggle("Play on Spawn",p.playOnSpawn);
			p.size=EditorGUILayout.IntField("Pool Size",p.size);
			p.maxsize=EditorGUILayout.IntField("Pool Max Size",p.maxsize);
			p.lifeTime=EditorGUILayout.FloatField("Item LifeTime",p.lifeTime);
			p.layer=(int)EditorGUILayout.LayerField("Layer",p.layer);
			p.parent=(Transform)EditorGUILayout.ObjectField("Default Parent",p.parent,typeof(Transform),true);
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		GUI.backgroundColor=prev;
	}
	
	void DropInPool(Pool p,Rect drop_area, Event evt){
		switch (evt.type) {			
		case EventType.DragUpdated:
		case EventType.DragPerform:
			if (drop_area.Contains (evt.mousePosition)){
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag ();
					p.prefab=(GameObject)DragAndDrop.objectReferences[0];
					p.name=p.prefab.name;
					p.layer=p.prefab.layer;
					if(pm.autoConfig)AutoConfig(p);
				}
			}
		break;
		} 
	}
	void DropInPools(PoolGroup pg,Rect drop_area, Event evt){
		switch (evt.type) {
		case EventType.DragUpdated:
		case EventType.DragPerform:
			if (drop_area.Contains (evt.mousePosition)){
			tempaddplace=(pg==null)?pm.pools:pg.pools;
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag ();
					tempadd.Clear ();
					foreach(Object obj in DragAndDrop.objectReferences){
						if(pg!=null){	
						}
						Pool p= new Pool((GameObject)obj,null,pm.dynamicSize,pm.dynamicMax,pm.dynamicLifeTime,false);
						p.name=p.prefab.name;
						p.layer=p.prefab.layer;
						if(pm.autoConfig)AutoConfig(p);
						tempadd.Add(p);
					}
				}
			}
		break;
		} 
	}
	void AutoConfig(Pool p){
		float maxlength=0;
		foreach(AudioSource audioSource in p.prefab.GetComponentsInChildren<AudioSource>(true)){
			if(!audioSource.loop && audioSource.clip!=null && audioSource.clip.length>maxlength)maxlength=audioSource.clip.length;
		}
		Debug.Log(maxlength);
		foreach(ParticleSystem ps in p.prefab.GetComponentsInChildren<ParticleSystem>(true)){
			if (!ps.loop){
				SerializedObject so = new SerializedObject(ps);
				float time=so.FindProperty("InitialModule.startLifetime.scalar").floatValue;
				if(time>maxlength)maxlength=time;
			}
		}
		if(maxlength==Mathf.Infinity)maxlength=0;
		p.playOnSpawn=(maxlength>0)?true:false;
		p.lifeTime=maxlength;
	}
}
