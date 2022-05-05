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
		if(IsOwner) {	
			if (Input.GetKeyDown(KeyCode.Space))
			{
				shoot();
			}
		}
	}


	public void shoot(){
		//m_shotPrefab.GetComponent<Light>().color = new Color.yellow;
		GameObject go = GameObject.Instantiate(shotPrefab, shot_spawn.transform.position, muzzle.rotation) as GameObject;
		Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>());
		GameObject.Destroy(go, 3f);
	}
}
