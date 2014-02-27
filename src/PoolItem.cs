//------------------------------------------------------------------------------
// Simple Object Pool 
// Copyright © 2014 Enrique Uriarte
// You are free to redistribute, use, or modify this code in commercial or
// non commercial projects, but you may not resell it, keep this free
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolItem : MonoBehaviour {
	[HideInInspector]
	public Transform trans;
	private bool canPlay=false;
	private Pool parentPool;
	public virtual void Init(Pool parent){
		parentPool=parent;
		if(parent.playOnSpawn && !parent.isChecked)CanBePlayed();
		trans=transform;
		if(PoolManager.instance.persistent)DontDestroyOnLoad(gameObject);
		gameObject.layer=parent.layer;
		SetActive(false);


	}
	public void ReParent(){
		trans.parent=(parentPool.parent!=null)?parentPool.parent:PoolManager.instanceT;
	}
	public void ReParent(Transform transform){
		trans.parent=transform;
	}
	public virtual void Spawn(float lifeTime,Vector3 position,Quaternion rotation){
		trans.position=position;
		trans.rotation=rotation;
		SetActive(true);
		parentPool.pooledItems.Remove(this);
		if(parentPool.recyclable){
			parentPool.spawnedItems.Add(this);
			StopAllCoroutines();
		}
		if (lifeTime>0) StartCoroutine(DeSpawn(lifeTime));
	}
	public virtual void Recycle(){
		if(parentPool==null)Destroy(gameObject);
		else SetActive(false);
	}
	void OnDisable(){
		if(parentPool!=null)
			parentPool.pooledItems.Add(this);
	}
	void OnDestroy(){
		if(parentPool==null)return;
#if UNITY_EDITOR
		Debug.Log ("Avoid to destroy pooled items, use Recycle() instead");
#endif
		parentPool.items.Remove(this);
		if(parentPool.pooledItems.Contains(this))parentPool.pooledItems.Remove(this);
	}
	IEnumerator DeSpawn(float time){
		yield return new WaitForSeconds(time);
		SetActive(false);
	}
	protected virtual void SetActive(bool active){ 
		if(!active && canPlay && parentPool.playOnSpawn) gameObject.BroadcastMessage("Stop",SendMessageOptions.DontRequireReceiver);
		gameObject.SetActive(active);
		if (active && canPlay && parentPool.playOnSpawn) gameObject.BroadcastMessage("Play",SendMessageOptions.DontRequireReceiver);
	}
	void CanBePlayed(){
		int asl=GetComponentsInChildren<AudioSource>(true).Length;
		int psl=0;
		bool fix=false;
		foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>(true)){
			if (ps.transform.parent.particleSystem==null ){
				psl++;
				if (ps.gameObject.GetComponent<PoolParticleHelper>()==null){
					ps.gameObject.AddComponent<PoolParticleHelper>();
					fix=true;
				}
			}
		}
		if(fix){
			parentPool.prefab=gameObject;
#if UNITY_EDITOR
			Debug.Log ("The prefab "+ name +" seems to be misconfigured tried to fix, ensure that the particle systems had a PoolParticleHelper component in their root");
#endif
		}
		parentPool.isChecked=true;
		if(psl>0 || asl>0)canPlay=true;
	}
}