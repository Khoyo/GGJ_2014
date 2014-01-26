using UnityEngine;
using System.Collections;

public class CScriptTriggerNiveau : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		if(col.name == "Player" )
		{
			CGame.GoToNextLevel();
		}
	}
}
