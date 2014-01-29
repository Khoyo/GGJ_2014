using UnityEngine;
using System.Collections;

public class CScriptTriggerNiveau : MonoBehaviour {

	public bool m_bSortie;

	void OnTriggerEnter(Collider col)
	{
		if(col.name == "Player" && m_bSortie)
		{
			CGame.GoToNextLevel();
		}
	}
}
