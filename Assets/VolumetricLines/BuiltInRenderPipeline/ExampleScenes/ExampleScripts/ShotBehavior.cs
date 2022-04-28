using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour {

	//private float range;
	

	// Use this for initialization
	void Start () {
		//range = transform.position.magnitude * 2f;		
	}
	
	// Update is called once per frame
	void Update () {
		//if (transform.position.magnitude < range){
		transform.position += transform.forward * Time.deltaTime * 20f;

		//}
	}
}
