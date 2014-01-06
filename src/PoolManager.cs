//------------------------------------------------------------------------------
// Simple Object Pool 
// Copyright © 2014 Enrique Uriarte
// You are free to redistribute, use, or modify this code in commercial or
// non commercial projects, but you may not resell it, keep this free
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PoolManager : MonoBehaviour {
	/// <summary>
	/// The pools.
	/// </summary>
	public Pool[] pools; 
	public static Dictionary<string,Pool> pool=new Dictionary<string, Pool>();
	/// <summary>
	/// If true won't be destroyed on scene change
	/// </summary>
	public bool persistent;
	void Start () {
		if(persistent)DontDestroyOnLoad(gameObject);
		foreach(Pool p in pools){
			if(p.prefab!=null){
				if(p.parent==null)p.parent=transform;
				p.Instatiate();
				if(!string.IsNullOrEmpty(p.name) && !pool.ContainsKey(p.name))pool.Add (p.name,p);
			}
			else Debug.LogWarning("The pool "+name+" doesn't have a prefab assigned, Skipped!");
		}
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
}
