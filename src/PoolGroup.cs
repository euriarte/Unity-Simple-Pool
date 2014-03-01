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
public class PoolGroup{
	
	public string name;
	public bool open;
	public List<Pool> pools=new List<Pool>();
	public List<PoolGroup> sets=new List<PoolGroup>();
	public bool propagateLayer;
	public LayerMask layer;
	
	public PoolGroup(){}	
	public PoolGroup(string name){
		this.name=name;	
	}
	public void Init(){
		for(int i =pools.Count-1;i>=0;i--){
			if (pools[i].prefab!=null){
				if(propagateLayer)pools[i].layer=layer;
				if(PoolManager.instance.stackPools && PoolManager.pool.ContainsKey(pools[i].name) && pools[i].Equals(PoolManager.pool[pools[i].name])){
					pools[i]=PoolManager.pool[pools[i].name];	
				}
				else pools[i].Init();
			}
			else pools.RemoveAt(i);
		}
		foreach(PoolGroup each in sets){
			if(propagateLayer)each.layer=layer;
			each.Init();
		}
	}
	
	public bool Add(Pool pool){
		if(!pools.Contains(pool))pools.Add(pool);
		else return false;
		return true;
	}
	public bool Remove(Pool pool){
		if (pools.Contains(pool))pools.Remove(pool);
		else return false;
		return true;
	}
	public PoolItem SpawnRandom(Vector3 position, Quaternion rotation){
		return pools[Random.Range(0,pools.Count)].Spawn(position,rotation);	
	}
	public PoolItem[] Spawn(Vector3 position,Quaternion rotation){
		PoolItem[] ret = new PoolItem[pools.Count];
		for (int i =0; i<pools.Count; i++){
			ret[i]=pools[i].Spawn(position,rotation);	
		}
		return ret;
	}
	public PoolItem[] SpawnSet(Vector3 position, Quaternion rotation){
		List<PoolItem> ret = new List<PoolItem>();
		for (int i=0; i<sets.Count;i++){
			if(sets[i].sets.Count>0)ret.AddRange(sets[i].SpawnSet(position,rotation));
			else ret.Add(sets[i].SpawnRandom(position,rotation));
		}
		return ret.ToArray();
	}
}
