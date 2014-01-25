using UnityEngine;
using System.Collections;

public class CEnnemi : MonoBehaviour {

	public bool m_bHaveLineOfSight;
	public int m_nLife = 10;
	const bool m_DebugRay = true;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find("Player");
		RaycastHit hit;
		if ((hit = Physics.RaycastAll(new Ray(transform.position, player.transform.position-transform.position), (player.transform.position-transform.position).magnitude)[0]).collider.name != "Player" ) {
			//print ("Blocked by " + hit.collider.name);
			m_bHaveLineOfSight = false;
		}
		else {
			m_bHaveLineOfSight = true;
		}

		if(m_DebugRay)
			Debug.DrawRay(transform.position, player.transform.position-transform.position, m_bHaveLineOfSight?Color.green:Color.red);

	}

	public void TakeBullet()
	{
		//Debug.Log ("Die");
		m_nLife--;
		if(m_nLife < 0)
			Object.Destroy(gameObject);
	}

}
