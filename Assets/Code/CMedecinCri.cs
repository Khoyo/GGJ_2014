using UnityEngine;
using System.Collections;

public class CMedecinCri : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider col){
		if(col.gameObject.CompareTag("Ennemies"))  
			transform.parent.GetComponent<CMedecin>().Register(col.gameObject);
	}
}
