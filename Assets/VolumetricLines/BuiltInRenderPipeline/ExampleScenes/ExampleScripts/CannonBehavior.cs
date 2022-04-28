using UnityEngine;
using System.Collections;

public class CannonBehavior : MonoBehaviour {

	public Transform m_cannonRot;
	public Transform m_muzzle;
	public GameObject m_shotPrefab;
	public GameObject shot_spawn;
	// public Texture2D m_guiTexture;
	private Vector3 temp;
	public float power;

	// Use this for initialization
	void Start () 
	{
		//test = m_muzzle.GetComponent<Transform>.position;
		//temp = m_muzzle.position;
		//temp.x = - 3.5f;
		//temp.y = 1.73f;
		//temp.z = 0f;
		

		//m_muzzle.position = temp;
		//power = 0.2f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//temp = m_muzzle.position;

		//temp.x = m_muzzle.position.x + 0.8894f;
		//temp.y = m_muzzle.position.y + 0.26f;
		//temp.z = m_muzzle.position.z + 0f;

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			m_cannonRot.transform.Rotate(Vector3.up, -Time.deltaTime * 100f);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			m_cannonRot.transform.Rotate(Vector3.up, Time.deltaTime * 100f);
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameObject go = GameObject.Instantiate(m_shotPrefab, shot_spawn.transform.position, m_muzzle.rotation) as GameObject;
			GameObject.Destroy(go, power);
		}
	}

	void OnGUI()
	{
	//	GUI.DrawTexture(new Rect(0f, 0f, m_guiTexture.width / 2, m_guiTexture.height / 2), m_guiTexture);
	}
}
