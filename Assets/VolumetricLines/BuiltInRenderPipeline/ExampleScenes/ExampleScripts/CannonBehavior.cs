using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class CannonBehavior : NetworkBehaviour {

	public Transform muzzle;
	public GameObject shotPrefab;
	public GameObject shot_spawn;

	
	// Update is called once per frame
	void Update () 
	{	
		//if(IsOwner) {	
		//	if (Input.GetKeyDown(KeyCode.Space))
		//	{
		//		shoot();
		//	}
		//}
	}


	public void shoot(){
		//m_shotPrefab.GetComponent<Light>().color = new Color.yellow;
		GameObject laser = GameObject.Instantiate(shotPrefab, shot_spawn.transform.position, muzzle.rotation);
		Physics.IgnoreCollision(laser.GetComponent<BoxCollider>(), GetComponent<CapsuleCollider>(), true);
		GameObject.Destroy(laser, 3f);
	}
}
