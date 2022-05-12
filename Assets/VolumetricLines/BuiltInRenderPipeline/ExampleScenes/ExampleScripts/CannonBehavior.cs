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

	[ServerRpc]
	void RequestShootServerRpc(Vector3 spawnPosition, Quaternion muzzleRotation)
    {

		ShootClientRpc(spawnPosition, muzzleRotation);
    }

	[ClientRpc]
	void ShootClientRpc(Vector3 spawnPosition, Quaternion muzzle){
		//m_shotPrefab.GetComponent<Light>().color = new Color.yellow;
		GameObject laser = GameObject.Instantiate(shotPrefab, spawnPosition, muzzle);
		Physics.IgnoreCollision(laser.GetComponent<BoxCollider>(), GetComponent<CapsuleCollider>(), true);
		GameObject.Destroy(laser, 3f);
	}

	public void Shoot()
	{
		//m_shotPrefab.GetComponent<Light>().color = new Color.yellow;
		RequestShootServerRpc(shot_spawn.transform.position, muzzle.rotation);
	}
}
