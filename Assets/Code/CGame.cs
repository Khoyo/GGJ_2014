using UnityEngine;
using System.Collections;

public class CGame : MonoBehaviour 
{
	public bool LD_bDebug = false;
	public static bool m_bDebug;
	// Use this for initialization
	void Start () 
	{
		CApoilInput.Init();
		m_bDebug = LD_bDebug;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CApoilInput.Process(Time.deltaTime);

		if(CApoilInput.Quit)
			Application.Quit();
	}
}
