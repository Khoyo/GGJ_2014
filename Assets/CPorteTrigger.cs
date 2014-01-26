using UnityEngine;
using System.Collections;

public class CPorteTrigger : MonoBehaviour {

	bool open;

	// Use this for initialization
	void Start () {
		open = false;
		foreach(AnimationState anim in transform.parent.GetComponent<Animation>())
		{
			anim.wrapMode = WrapMode.Once;
			anim.speed = 15;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void  OnTriggerEnter(Collider col){
		if(!open && col.name == "Player"){
			open = true;
			transform.parent.GetComponent<Animation>().Play();
		}
	}


}
