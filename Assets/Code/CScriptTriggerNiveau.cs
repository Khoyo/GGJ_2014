using UnityEngine;
using System.Collections;

public class CScriptTriggerNiveau : MonoBehaviour {

	public bool m_bSortie;
	bool m_bWizz;
	bool m_bSoundPlayed;

	void Start()
	{
		m_bWizz = false;
		m_bSoundPlayed = false;
		if(!m_bSortie)
			CSoundEngine.postEvent("Play_ElevatorArrive", gameObject);
	}

	void Update()
	{
		if(m_bWizz)
		{
			gameObject.transform.parent.position += Random.insideUnitSphere / 90.0f;

			if(!m_bSoundPlayed)
			{
				CSoundEngine.postEvent("Play_ElevatorDepart", gameObject);
				m_bSoundPlayed = true;
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.name == "Player" && m_bSortie)
		{
			CGame.TakeElevator();
			gameObject.transform.parent.FindChild("TriggerPorte").GetComponent<CPorteTrigger>().Close();
			m_bWizz = true;
		}
	}
}
