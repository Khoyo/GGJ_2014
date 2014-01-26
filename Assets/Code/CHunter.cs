using UnityEngine;
using System.Collections;

public class CHunter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<NavMeshAgent>().updateRotation = true; 
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find("Player");
		if(GetComponent<CEnnemi>().m_bHaveLineOfSight && player.GetComponent<CPlayer>().getState() != CPlayer.EState.e_Charismatique)
		{	Alert(player);

			RaycastHit hit;
			Vector3 lineOfSight = player.transform.position-transform.position;
			Physics.Raycast(new Ray(transform.position, transform.TransformDirection(new Vector3(0, 0, 1))), out hit, lineOfSight.magnitude, ~(1<<8));
			if(hit.collider != null && hit.collider.name == "Player")
				hit.collider.GetComponent<CPlayer>().Die();

			/*if(hit.collider)
				print ("Blocked by " + hit.collider.name);*/

			Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1))*50, Color.red);

			}

	}

	public void Alert(GameObject player){
		GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
	}
}
