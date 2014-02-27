//------------------------------------------------------------------------------
// Simple Object Pool 
// Copyright © 2014 Enrique Uriarte
// You are free to redistribute, use, or modify this code in commercial or
// non commercial projects, but you may not resell it, keep this free
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

//This sample emulates a physical projectile's basic behavior


[RequireComponent(typeof(Rigidbody))]
public class PoolItemCustom : PoolItem {
	
	PoolItem child;
	public string trailName="Trail";
	public string impactName="Explosion";

	public override void Spawn (float lifeTime, Vector3 position, Quaternion rotation){
		base.Spawn (lifeTime, position, rotation);
		rigidbody.AddForce(trans.forward*50,ForceMode.VelocityChange);
		if(!string.IsNullOrEmpty(trailName)){
			child= PoolManager.Spawn(trailName,trans);
			child.ReParent(trans);
		}
	}
	public override void Recycle (){
		if(child!=null)child.ReParent();
		base.Recycle ();
	}
	void OnCollisionEnter(Collision collision){
		ContactPoint col =collision.contacts[0];
		if(!string.IsNullOrEmpty(impactName))
			PoolManager.SpawnRandom(impactName,col.point, Quaternion.FromToRotation(Vector3.up, col.normal));
		Recycle();
	}
	
}
