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
	private Transform trans;
	private bool canPlay=false;
	private List<ParticleSystem> particles =new List<ParticleSystem>();
	[HideInInspector]
	public Pool parentPool;
	void Awake(){
		CanBePlayed();
		trans=transform;
		gameObject.SetActive(false);
	}
	public void Enable(float lifeTime,Vector3 position,Quaternion rotation){
		trans.position=position;
		trans.rotation=rotation;
		SetActive(true);
		parentPool.pooledItems.Remove(this);
		if (lifeTime!=0) StartCoroutine(DeSpawn(lifeTime));
	}
	void OnDisable(){
		if(parentPool!=null)
			parentPool.pooledItems.Add(this);
	}
	void OnDestroy(){
		if(parentPool==null)return;
		parentPool.items.Remove(this);
		if(parentPool.pooledItems.Contains(this))parentPool.pooledItems.Remove(this);
	}
	IEnumerator DeSpawn(float time){
		yield return new WaitForSeconds(time);
		SetActive(false);
	}
	void SetActive(bool active){
		if(!active && canPlay && parentPool.playOnSpawn) gameObject.BroadcastMessage("Stop");
		gameObject.SetActive(active);
		if (active && canPlay && parentPool.playOnSpawn) gameObject.BroadcastMessage("Play");
	}
	void CanBePlayed(){
		int ppshl=GetComponentsInChildren<PoolParticleHelper>(true).Length;
		int asl=GetComponentsInChildren<AudioSource>(true).Length;

#if UNITY_EDITOR
		int psl=0;
		foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>(true)){
			if (ps.transform.parent.particleSystem==null){
				psl++;
				ps.gameObject.AddComponent<PoolParticleHelper>();
			}
		}
		if(psl!=ppshl) Debug.LogError("It seems that there are PaticleSystems misconfigured, please attach a PoolParticleHelper Component to their root. It has been corrected but for performance won't work in the standalone version");
#endif
		if(ppshl>0)canPlay=true;
		if(asl>0)canPlay=true;
	}
}