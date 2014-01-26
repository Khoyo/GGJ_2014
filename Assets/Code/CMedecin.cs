using UnityEngine;
using System.Collections.Generic;

public class CMedecin : MonoBehaviour {

	List<GameObject> m_Assistants;

	bool AlertedThisFrame;

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate(){
		m_Assistants = new List<GameObject>();
		AlertedThisFrame = false;
	}

	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find("Player");
		if(GetComponent<CEnnemi>().m_bHaveLineOfSight && player.GetComponent<CPlayer>().getState() != CPlayer.EState.e_Charismatique && player.GetComponent<CPlayer>().getState() != CPlayer.EState.e_MauvaisGout)
			Alert(player);

		foreach(GameObject ass in m_Assistants)
			Debug.DrawRay(transform.position, ass.transform.position-transform.position, Color.yellow);
	}

	public void Alert(GameObject player){
		if(AlertedThisFrame)
			return;

		AlertedThisFrame = true;

		GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
		foreach(GameObject ass in m_Assistants){

			Vector3 lineOfSight = ass.transform.position-transform.position;
			RaycastHit hit;
			Physics.Raycast(new Ray(transform.position, lineOfSight), out hit, lineOfSight.magnitude, ~((1<<8)|(1<<9)));
			if (hit.collider != null && hit.collider.gameObject != ass){
				continue;
			}
			
			if(ass.GetComponent<CInfirmier>() != null){
				ass.GetComponent<CInfirmier>().Alert(player);

			}

			if(ass.GetComponent<CInfirmiere>() != null)
				ass.GetComponent<CInfirmiere>().Alert(player);
			
			if(ass.GetComponent<CMedecin>() != null)
				ass.GetComponent<CMedecin>().Alert(player);
			
			if(ass.GetComponent<CHunter>() != null)
				ass.GetComponent<CHunter>().Alert(player);
			
			if(ass.GetComponent<CBoss>() != null)
				ass.GetComponent<CBoss>().Alert(player);
		}

	}

	public void Register(GameObject ass){
		if(ass != gameObject)
			m_Assistants.Add(ass);

	}
}
