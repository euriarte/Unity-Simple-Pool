//------------------------------------------------------------------------------
// Simple Object Pool 
// Copyright © 2014 Enrique Uriarte
// You are free to redistribute, use, or modify this code in commercial or
// non commercial projects, but you may not resell it, keep this free
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Pool {
	public string name="";
	/// <summary>
	/// Object where the item will be pooled
	/// </summary>
	public Transform parent;
	/// <summary>
	/// Item to be pooled
	/// </summary>
	public GameObject prefab;
	[HideInInspector]
	public List<PoolItem> items =new List<PoolItem>();
	[HideInInspector]
	public List<PoolItem> pooledItems =new List<PoolItem>();
	/// <summary>
	/// Default number of pooled items
	/// </summary>
	public int size=20;
	/// <summary>
	/// Max size of the pool 0 -> unlimited
	/// </summary>
	public int maxsize=30;
	/// <summary>
	/// The life time.
	/// </summary>
	public float lifeTime=5;
	/// <summary>
	/// If true the items won't be displayed in the hierarchy
	/// </summary>
	public bool hideInHierarchy=false;
	public bool playOnSpawn=false;

	public void Instatiate(){
		if(string.IsNullOrEmpty(name))name=prefab.name;
		while(items.Count<size){
			AddItem();
		}
	}
	public void AddItem(){
		if(items.Count<maxsize||maxsize<1||items.Count<size){
			GameObject go = (GameObject)MonoBehaviour.Instantiate(prefab);
			go.name=name;
			if(go.GetComponent<PoolItem>()==null)	go.AddComponent<PoolItem>();
			PoolItem p=go.GetComponent<PoolItem>();
			items.Add(p);
			pooledItems.Add(p);
			p.parentPool=this;
			if(hideInHierarchy) p.gameObject.hideFlags=HideFlags.HideInHierarchy;
			p.transform.parent=parent;
		}
	}
	public PoolItem Spawn (Vector3 position, Quaternion rotation){
		if (pooledItems.Count<1 || items.Count<size){
			if(items.Count>=maxsize && maxsize>1)	return null;
			else AddItem();
		}
		PoolItem pooled= pooledItems[0];
		pooledItems[0].Enable(lifeTime,position,rotation);
		return pooled;
	}
}
