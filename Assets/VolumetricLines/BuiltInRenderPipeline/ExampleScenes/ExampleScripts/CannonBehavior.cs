using UnityEngine;
using System.Collections;
using Unity.Netcode;

[RequireComponent(typeof(AudioSource))]
public class CannonBehavior : NetworkBehaviour {

	public Transform muzzle;
	public GameObject shotPrefab;
	public GameObject shot_spawn;
	public AudioClip shotSound;

	
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
		AudioSource.PlayClipAtPoint(shotSound, spawnPosition);
		GameObject.Destroy(laser, 3f);
	}

	public void Shoot()
	{
		//m_shotPrefab.GetComponent<Light>().color = new Color.yellow;
		RequestShootServerRpc(shot_spawn.transform.position, muzzle.rotation);
	}
}
