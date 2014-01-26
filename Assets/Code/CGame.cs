using UnityEngine;
using System.Collections;

public class CGame : MonoBehaviour 
{
	string soundbankName = "SoundBank_GGJ2014.bnk";

	public bool LD_bDebug = false;
	public static bool m_bDebug;
	public static int m_fWidth = 1280;
	public static int m_fHeight = 720;
	// Use this for initialization
	void Start () 
	{
		CApoilInput.Init();
		m_bDebug = LD_bDebug;
		CSoundEngine.Init();
		CSoundEngine.LoadBank(soundbankName);
	}
	
	// Update is called once per frame
	void Update () 
	{
		CApoilInput.Process(Time.deltaTime);

		if(CApoilInput.Quit)
			Application.Quit();

		if(CApoilInput.DebugF10)
			GoToNextLevel();
	}

	public void GoToNextLevel()
	{
		if(Application.loadedLevel < Application.levelCount)
		{
			CSoundEngine.postEvent("Play_BreathEnd", gameObject);
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}
}
