using UnityEngine;
using System.Collections;

public class CGameCreator : MonoBehaviour {

	static int m_instanceCount = 0;
	public GameObject m_prefabGame;
	// Use this for initialization
	void Awake() {
		Object.DontDestroyOnLoad (gameObject);
		if(m_instanceCount++ == 0){
			GameObject.Instantiate(m_prefabGame);
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
