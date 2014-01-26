using UnityEngine;
using System.Collections;

public class CInfirmiere : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find("Player");
		if(GetComponent<CEnnemi>().m_bHaveLineOfSight)
			Alert(player);
	}


	public void Alert(GameObject player){
		GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
	}
}
