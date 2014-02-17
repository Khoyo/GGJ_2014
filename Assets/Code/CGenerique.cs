using UnityEngine;
using System.Collections;

public class CGenerique : MonoBehaviour {

	public enum EmenuState
	{
		e_splash1,
		e_splash2,
	}
	
	float m_fWidth;
	float m_fHeight;
	
	EmenuState m_EState;
	float m_fTempsSplash1;
	float m_fTempsSplash1Init = 10.0f;
	float m_fTempsSplash2;
	float m_fTempsSplash2Init = 10.0f;

	public Texture m_Texture_Generique1;
	public Texture m_Texture_Generique2;
	
	
	// Use this for initialization
	void Start () 
	{
		m_fTempsSplash1 = m_fTempsSplash1Init;
		m_fTempsSplash2 = m_fTempsSplash2Init;
		m_EState = EmenuState.e_splash1;

		m_fWidth = CGame.m_fWidth;
		m_fHeight = CGame.m_fHeight;

		CSoundEngine.Init();
		CSoundEngine.LoadBank("SoundBank_GGJ2014.bnk");

		CSoundEngine.postEvent("Play_MusiqueFin", gameObject);

		if(GameObject.Find("_Game") != null)
			Object.Destroy(GameObject.Find("_Game"));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_EState == EmenuState.e_splash1 && m_fTempsSplash1>0.0f)
			m_fTempsSplash1 -= Time.deltaTime;
		if(m_EState == EmenuState.e_splash2 && m_fTempsSplash2>0.0f)
			m_fTempsSplash2 -= Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}
	
	void OnGUI() 
	{
		switch(m_EState)
		{
		case EmenuState.e_splash1:
		{
			if(m_fTempsSplash1 > 0.0f)
			{
				float fCoeffScale = 1.0f + (m_fTempsSplash1Init - m_fTempsSplash1)/(10.0f*m_fTempsSplash1Init);
				float fWidth = m_fWidth * fCoeffScale;
				float fHeight = m_fHeight * fCoeffScale;
				GUI.DrawTexture(new Rect((m_fWidth - fWidth)/2.0f, (m_fHeight - fHeight)/2.0f, fWidth, fHeight), m_Texture_Generique1);
			}
			else
				m_EState = EmenuState.e_splash2;
			break;
		}	
			
		case EmenuState.e_splash2:
		{
			if(m_fTempsSplash2 > 0.0f)
			{
				float fCoeffScale = 1.0f + (m_fTempsSplash2Init - m_fTempsSplash2)/(10.0f*m_fTempsSplash2Init);
				float fWidth = m_fWidth * fCoeffScale;
				float fHeight = m_fHeight * fCoeffScale;
				GUI.DrawTexture(new Rect((m_fWidth - fWidth)/2.0f, (m_fHeight - fHeight)/2.0f, fWidth, fHeight), m_Texture_Generique2);
			}
			else
				Application.LoadLevel (0);
			break;
		}
		}
	}
}
