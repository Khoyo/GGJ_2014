using UnityEngine;
using System.Collections;

public class CInfirmier : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find("Player");
		if(GetComponent<CEnnemi>().m_bHaveLineOfSight)
			GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
	}
}
