using UnityEngine;
using System.Collections;

public class CPorte : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Open()
	{
		CSoundEngine.postEvent("Play_OpenDoor", gameObject);
	}
}
