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
	[HideInInspector]
	public bool isChecked;
	public bool open;
	public string name="";
	/// <summary>
	/// Object where the item will be pooled
	/// </summary>
	public Transform parent;
	/// <summary>
	/// Item to be pooled
	/// </summary>
	public GameObject prefab;
//	[HideInInspector]
	public List<PoolItem> items =new List<PoolItem>();
//	[HideInInspector]
	public List<PoolItem> pooledItems =new List<PoolItem>();
	public List<PoolItem> spawnedItems =new List<PoolItem>();
	/// <summary>
	/// Default number of pooled items
	/// </summary>
	public int size=10;
	/// <summary>
	/// Max size of the pool 0 -> unlimited
	/// </summary>
	public int maxsize=30;
	/// <summary>
	/// The life time.
	/// </summary>
	public float lifeTime=0;
	public bool _hideInHierarchy;
	public LayerMask layer;
	

	/// <summary>
	/// If true the items won't be displayed in the hierarchy
	/// </summary>
	public bool hideInHierarchy{
		get{return (PoolManager.instance.hideInHierarchy)?true:_hideInHierarchy;}
		set{_hideInHierarchy=value;}
	}
	public bool _recyclable;
	public bool recyclable {
		get{return (PoolManager.instance.recyclable)?true:_recyclable;}
		set{_recyclable=value;}	
	}
	public bool playOnSpawn=false;
	#region Constructors
	public Pool(){}
	public Pool(GameObject _prefab, Transform _parent, int _size, int _maxsize, float _lifeTime, bool _playOnSpawn){
		if(_prefab!=null)
			SetData(_prefab,_parent,_size,_maxsize,_lifeTime,_playOnSpawn,PoolManager.instance.hideInHierarchy);
	}
	public Pool(GameObject _prefab, Transform _parent, int _size, int _maxsize, float _lifeTime, bool _playOnSpawn,bool _hideInHierarchy){
		if(_prefab!=null)
			SetData(_prefab,_parent,_size,_maxsize,_lifeTime,_playOnSpawn,_hideInHierarchy);
	}
	void SetData(GameObject _prefab, Transform _parent, int _size, int _maxsize, float _lifeTime, bool _playOnSpawn,bool _hideInHierarchy){
		prefab=_prefab;
		name=_prefab.name;
		parent=(_parent==null)?PoolManager.instanceT:_parent;
		size=(_size>0)?_size:10;
		maxsize=_maxsize;
		lifeTime=(_lifeTime>=0)?_lifeTime:0;
		playOnSpawn=_playOnSpawn;
		hideInHierarchy=_hideInHierarchy;
	}
	#endregion

	public bool Init(){
		isChecked=false;
		if(prefab==null)return false;
		if(string.IsNullOrEmpty(name))name=prefab.name;
		if(parent==null)parent=PoolManager.instanceT;
		if(PoolManager.instance.populateOnStart && !PoolManager.started){
			while(items.Count<size)	AddItem();
		}
		return true;
	}
	public void ReParent(Transform transform){
		parent=transform;
		foreach(PoolItem p in items)ReParent (transform);
	}
	public void ReParent(){
		parent=PoolManager.instanceT;
		foreach(PoolItem p in items)ReParent();
	}
	public void RecicleAll(){
		for(int i=spawnedItems.Count-1;i>=0;i--)spawnedItems[i].Recycle();	
	}
	public void Recicle(PoolItem poolItem){
		if(!items.Contains(poolItem))return;
		if(!pooledItems.Contains(poolItem)) pooledItems.Add(poolItem);
		if(spawnedItems.Contains(poolItem)) spawnedItems.Remove(poolItem);
	}
	public void AddItem(){
		if(items.Count<maxsize||maxsize<1||items.Count<size){
			GameObject go = (GameObject)MonoBehaviour.Instantiate(prefab);
			go.name=name+items.Count.ToString("000");
			if(go.GetComponent<PoolItem>()==null)	go.AddComponent<PoolItem>();
			PoolItem p=go.GetComponent<PoolItem>();
			items.Add(p);
			p.Init(this);
			if(hideInHierarchy) p.gameObject.hideFlags=HideFlags.HideInHierarchy;
			p.transform.parent=parent;
		}
	}
	public PoolItem Spawn (Vector3 position, Quaternion rotation){
		if (pooledItems.Count<1 || items.Count<size){
			if(items.Count>=maxsize && maxsize>1)	{
				if(recyclable){
					spawnedItems[0].Recycle();
				}
				else return null;
			}
			else AddItem();
		}
		if(pooledItems[0]!=null){
			PoolItem pooled= pooledItems[0];
			pooledItems[0].Spawn(lifeTime,position,rotation);
			return pooled;
		}
		return null;
	}
	public override bool Equals (object obj)
	{
		return (name==((Pool)obj).name && prefab==((Pool)obj).prefab && lifeTime==((Pool)obj).lifeTime)?true:false;
	}
	public override int GetHashCode ()
	{
		return name.GetHashCode ();
	}
}
