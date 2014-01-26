using UnityEngine;
using System.Collections;

public class CZoneSwitch : MonoBehaviour {

	public bool Furtif;
	public bool Bourin;
	public bool Charismatique;
	public bool MauvaisGout;
	bool m_bActivated;

	// Use this for initialization
	void Start () 
	{
		m_bActivated = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && !m_bActivated)
		{
			if(Furtif)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateFurtif();
			}
			if(Bourin)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateBourin();
			}
			if(Charismatique)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateCharismatique();
			}
			if(MauvaisGout)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateMauvaisGout();
			}



			m_bActivated = true;
		}
	}
}
