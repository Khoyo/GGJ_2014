using UnityEngine;
using System.Collections;

public class CPorteTrigger : MonoBehaviour {

	bool open;
	bool m_bClose;

	// Use this for initialization
	void Start () {
		open = false;
		foreach(AnimationState anim in transform.parent.GetComponent<Animation>())
		{
			anim.wrapMode = WrapMode.Once;
			anim.speed = 15;
		}
		m_bClose = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bClose)
		{
			transform.parent.GetComponent<Animation>().Play();
		}
	}

	public void Close()
	{
		foreach(AnimationState anim in transform.parent.GetComponent<Animation>())
		{
			anim.wrapMode = WrapMode.Once;
			anim.speed = -15;
		}
		m_bClose = true;
	}

	void  OnTriggerEnter(Collider col){
		if(!open && col.name == "Player"){
			open = true;
			transform.parent.GetComponent<Animation>().Play();
		}
	}


}
