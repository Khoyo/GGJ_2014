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

	static int m_instanceCount = 0;
	public bool isInitialized;

	// Use this for initialization
	void Start () 
	{
		isInitialized = false;
		if(m_instanceCount++ != 0){
			//We are not the first CGame object :'( we need to abort !!
			Debug.Log("Deleting redundant _Game");
			Object.Destroy(gameObject);
			gameObject.name = "_Game____todestroydonotuseseriouslyimeanit";
			return;
		}

		CApoilInput.Init();
		m_bDebug = false;
		CSoundEngine.Init();
		CSoundEngine.LoadBank(soundbankName);

		m_bLevelFixeSansSwitch = LD_LevelFixeSansSwitch;
		m_bStartWithElevator = LD_CeLevelCommenceParUnAscenseur;
		isInitialized = true;
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
