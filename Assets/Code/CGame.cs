using UnityEngine;
using System.Collections;

public class CGame : MonoBehaviour 
{
	string soundbankName = "SoundBank_GGJ2014.bnk";

	public bool LD_bDebug = false;
	public static bool m_bDebug;
	public static int m_fWidth = 1280;
	public static int m_fHeight = 720;
	public bool LD_LevelFixeSansSwitch;
	public static bool m_bLevelFixeSansSwitch;
	public bool LD_CeLevelCommenceParUnAscenseur;
	public static bool m_bStartWithElevator;
	
	public bool isInitialized = false;
	public int tmp;

	// Use this for initialization
	void Start () 
	{
		Object.DontDestroyOnLoad (gameObject);
		CApoilInput.Init();
		m_bDebug = false;
		CSoundEngine.Init();
		CSoundEngine.LoadBank(soundbankName);

		m_bLevelFixeSansSwitch = LD_LevelFixeSansSwitch;
		m_bStartWithElevator = LD_CeLevelCommenceParUnAscenseur;

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

	public static void GoToNextLevel()
	{
		if(Application.loadedLevel < Application.levelCount)
		{
			CSoundEngine.postEvent("Play_BreathEnd", null);
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}
}
