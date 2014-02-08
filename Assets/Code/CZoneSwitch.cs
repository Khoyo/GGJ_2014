using UnityEngine;
using System.Collections;

public class CZoneSwitch : MonoBehaviour {

	public bool Furtif;
	public bool Bourin;
	public bool Charismatique;
	public bool MauvaisGout;
	public string m_Text;
	bool m_bActivated;

	float m_fTimerAffichage;
	float m_fHeightText = 100.0f;

	// Use this for initialization
	void Start () 
	{
		m_bActivated = false;
		m_fTimerAffichage = 0.0f;

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_fTimerAffichage > 0.0f)
			m_fTimerAffichage -= Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") && !m_bActivated)
		{
			string dialogue = "caca";
			if(Furtif)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateFurtif();
				dialogue = "Play_DialFurtif";
			}
			if(Bourin)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateBourin();
				dialogue = "Play_DialBourin";
			}
			if(Charismatique)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateCharismatique();
				dialogue = "Play_DialGirl";
			}
			if(MauvaisGout)
			{
				other.gameObject.GetComponent<CPlayer>().GoToStateMauvaisGout();
				dialogue = "Play_DialMauvaisGout";
			}

			CSoundEngine.postEvent(dialogue, gameObject);
			m_fTimerAffichage = CGame.m_fTimerSwitchMax;
			m_bActivated = true;
		}
	}

	void OnGUI()
	{
		GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		if(m_fTimerAffichage > 0.0f)
		{
			GUI.skin.label.font = CGame.m_FontLarge; 
			GUI.Label(new Rect( 0, CGame.m_fHeight - m_fHeightText, CGame.m_fWidth, CGame.m_fHeight), m_Text, centeredStyle);
		}
	}
}

