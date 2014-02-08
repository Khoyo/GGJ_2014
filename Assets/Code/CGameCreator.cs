using UnityEngine;
using System.Collections;

public class CGameCreator : MonoBehaviour {

	static int m_instanceCount = 0;
	public GameObject m_prefabGame;
	public bool LD_CeLevelCommenceParUnAscenseur;
	public bool LD_LevelFixeSansSwitch;
	public bool LD_bDebug;
	public Font LD_FontLarge;

	// Use this for initialization
	void Awake() {
		if(m_instanceCount++ == 0){
			CGame.m_bLevelFixeSansSwitch = LD_LevelFixeSansSwitch;
			CGame.m_bStartWithElevator = LD_CeLevelCommenceParUnAscenseur;
			CGame.m_bDebug = LD_bDebug;
			CGame.m_FontLarge = LD_FontLarge;
			CGame game = ((GameObject) GameObject.Instantiate(m_prefabGame)).GetComponent<CGame>();
		}

	}
	
	// Update is called once per frame
	void OnDestroy() {
		m_instanceCount--;
	}
}
