//------------------------------------------------------------------------------
// Simple Object Pool 
// Copyright © 2014 Enrique Uriarte
// You are free to redistribute, use, or modify this code in commercial or
// non commercial projects, but you may not resell it, keep this free
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class PoolManager : MonoBehaviour {
	public static Transform instanceT;
	public static PoolManager instance;
	/// <summary>
	/// The pools.
	/// </summary>
	public List<Pool> pools = new List<Pool>();
	public List<PoolGroup> poolGroups = new List<PoolGroup>();
	public static Dictionary<string,Pool> pool=new Dictionary<string, Pool>();
	public static Dictionary<string,PoolGroup> poolGroup=new Dictionary<string, PoolGroup>();
	public static Dictionary<GameObject,string> dynamicIndex=new Dictionary<GameObject,string>();
	/// <summary>
	/// If true won't be destroyed on scene change
	/// </summary>
	public bool persistent;
	public bool hideInHierarchy;
	public bool populateOnStart;
	public bool dynamic;
	public int dynamicSize;
	public int dynamicMax;
	public float dynamicLifeTime;
	public static bool started{get; private set;}

	static Pool p;

	void Start () {
		if (instance==null){
			instanceT=transform;
			instance=this;
		}
		else Destroy(this);
		if(persistent)DontDestroyOnLoad(gameObject);
		foreach(Pool p in pools){
			if(p.prefab!=null){
				if(p.parent==null)p.parent=transform;
				p.Init();
				if(!string.IsNullOrEmpty(p.name) && !pool.ContainsKey(p.name))pool.Add (p.name,p);
			}
			else Debug.LogWarning("A pool in "+name+" doesn't have a prefab assigned, Skipped!");
		}
		foreach(PoolGroup pg in poolGroups){
			poolGroup.Add(pg.name,pg);	
		}
		started=true;
	}
#if UNITY_EDITOR
	public void CreatePool(){
		p = new Pool ();
		pools.Add(p);
	}
#endif
	public static bool CreatePool(string name, GameObject prefab, Transform parent, int size, int maxsize, float lifeTime, bool playOnSpawn){
		if(prefab!=null){
			if(string.IsNullOrEmpty(name))name=prefab.name;
			if(pool.ContainsKey(name)){
				if(pool[name].lifeTime==lifeTime && pool[name].prefab==prefab){
					pool[name].size+=size;
					pool[name].maxsize+=maxsize;
				}
				else Debug.LogError("There is a different pool with the same name, Aborting!");
			}
			else {
				p = new Pool (prefab, parent, size, maxsize, lifeTime, playOnSpawn);
				if(p.Init()){
					pool.Add(name,p);
#if UNITY_EDITOR
					instance.pools.Add(p);
#endif
				}
			}
		}
		return false;
	}
	/// <summary>
	/// Spawn item from the specified poolName at transform.
	/// </summary>
	/// <param name="poolName">Pool name.</param>
	/// <param name="trans">Transform </param>
	public static PoolItem Spawn(string poolName,Transform trans){
		return Spawn(poolName,trans.position,trans.rotation);
	}
	/// <summary>
	/// Spawn item from the specified poolName, at position and rotation.
	/// </summary>
	/// <param name="poolName">Pool name.</param>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	public static PoolItem Spawn(string poolName, Vector3 position,Quaternion rotation){
		return (pool.ContainsKey(poolName))?pool[poolName].Spawn(position,rotation):null;
	}
	public static PoolItem Spawn(GameObject prefab, Vector3 position,Quaternion rotation){
		if(instance.dynamic)	{
			string poolName="";
			if (dynamicIndex.ContainsKey(prefab))poolName=dynamicIndex[prefab];
			else {
				string sub="";
				int counter=0;
				while (string.IsNullOrEmpty(sub)){
					if (!pool.ContainsKey(prefab.name+counter.ToString())){
						sub=counter.ToString();
						poolName=prefab.name+sub;
					}
					counter++;
				}
				CreatePool(poolName,prefab,instance.transform,instance.dynamicSize,instance.dynamicMax,instance.dynamicLifeTime,true);
			}
			return Spawn(poolName,position,rotation);
		}
#if UNITY_EDITOR
		Debug.LogWarning("Dynamic pool is disabled");
#endif
		return null;
	}
	public static PoolItem[] SpawnGroup(string groupName, Transform transform){
		return SpawnGroup(groupName, transform.position,transform.rotation);
	}
	public static PoolItem[] SpawnGroup(string groupName, Vector3 position, Quaternion rotation){
		return (poolGroup.ContainsKey(groupName))?poolGroup[groupName].Spawn(position,rotation):null;
	}
	public static PoolItem SpawnRandom(string groupName, Transform transform){
		return SpawnRandom(groupName,transform.position,transform.rotation);		
	}
	public static PoolItem SpawnRandom(string groupname,Vector3 position, Quaternion rotation){
		return (poolGroup.ContainsKey(groupname))?poolGroup[groupname].SpawnRandom(position,rotation):null;
	}
	public static PoolItem[] SpawnSet(string groupName,Transform transform){
		return SpawnSet(groupName,transform.position,transform.rotation);
	}
	public static PoolItem[] SpawnSet(string groupName, Vector3 position, Quaternion rotation){
		return (poolGroup.ContainsKey(groupName))?poolGroup[groupName].SpawnSet(position,rotation):null;
	}
}
