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
		Vector3 lineOfSight = player.transform.position-transform.position;
		RaycastHit hit;
		Physics.Raycast(new Ray(transform.position, lineOfSight), out hit, lineOfSight.magnitude, ~(1<<8));
		if (hit.collider.name != "Player") {
			//print ("Blocked by " + hit.collider.name);
			m_bHaveLineOfSight = false;
		}
		else 
		{
			m_bHaveLineOfSight = true;
		}

		if(m_nLife < 0)
			Die ();

		if(m_DebugRay)
			Debug.DrawRay(transform.position, player.transform.position-transform.position, m_bHaveLineOfSight?Color.green:Color.red);

	}

	public void TakeBullet()
	{
		m_nLife--;

	}

	public void TakeCut()
	{
		m_nLife = -1;
	}

	void Die()
	{
		CSoundEngine.postEvent("Play_WilhelmScream", gameObject);
		Object.Destroy(gameObject);
	}

}
