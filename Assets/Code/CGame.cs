﻿using UnityEngine;
using System.Collections;

public class CGame : MonoBehaviour 
{
	public static string soundbankName = "SoundBank_GGJ2014.bnk";

	public bool LD_bDebug = false;
	public static bool m_bDebug;
	public static int m_fWidth = 1280;
	public static int m_fHeight = 720;
	public static bool m_bLevelFixeSansSwitch;
	public static bool m_bStartWithElevator;
	public static Font m_FontLarge;
	public static float m_fTimerSwitchMax = 3.0f;

	static bool m_bEndLevel;
	static bool m_bStartLevel;
	float m_fTimerEndLevel;
	float m_fTimerEndLevelMax = 5.0f;
	float m_fStartingLevel;
	float m_fStartingLevelMax = 1.0f;
	static Texture m_TextureBlack;
		
	public bool isInitialized = false;
	public int tmp;

	// Use this for initialization
	void Start () 
	{
		CApoilInput.Init();
		CSoundEngine.Init();
		CSoundEngine.LoadBank(CGame.soundbankName);
		m_fTimerEndLevel = m_fTimerEndLevelMax;
		m_fStartingLevel = m_fStartingLevelMax;
		m_bEndLevel = false;
		m_bStartLevel = true;
		CGame.m_TextureBlack = GameObject.Find("Player").GetComponent<CPlayer>().m_TextureBlack;

		CSoundEngine.postEvent("Play_HospitalAmbiance", gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		CApoilInput.Process(Time.deltaTime);

		if(CApoilInput.Quit)
			Application.Quit();

		if(CApoilInput.DebugF10)
			GoToNextLevel();

		if(m_bStartLevel)
		{
			if(m_fStartingLevel > 0)
			{
				m_fStartingLevel -= Time.deltaTime;
			}
			else
				m_bStartLevel = false;
		}

		if(m_bEndLevel)
		{
			if(m_fTimerEndLevel > 0)
			{
				m_fTimerEndLevel -= Time.deltaTime;
			}
			else
			{
				GoToNextLevel();
				m_bEndLevel = false;
			}
		}
	}

	void OnGUI()
	{
		if(m_bStartLevel) 
		{
			GUI.color = new Color(1.0f, 1.0f, 1.0f, m_fStartingLevel / m_fStartingLevelMax);
			GUI.DrawTexture(new Rect(0, 0, CGame.m_fWidth, CGame.m_fHeight),  CGame.m_TextureBlack);

		}

		if(m_bEndLevel)
		{
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - m_fTimerEndLevel / m_fTimerEndLevelMax);
			GUI.DrawTexture(new Rect(0, 0, CGame.m_fWidth, CGame.m_fHeight),  CGame.m_TextureBlack);
		}
	}

	public static void TakeElevator()
	{
		CSoundEngine.postEvent("Play_BreathEnd", null);
		CGame.m_bEndLevel = true; 

	}

	void GoToNextLevel()
	{
		if(Application.loadedLevel < Application.levelCount)
		{
			CSoundEngine.postEvent("Stop_All", null);
			m_bStartLevel = true;
			m_fTimerEndLevel = m_fTimerEndLevelMax;
			m_fStartingLevel = m_fStartingLevelMax;
			Application.LoadLevel(Application.loadedLevel+1);

		}
	}
}
