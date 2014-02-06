using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNeon : MonoBehaviour 
{
	public bool m_bIsNeon = true;
	bool m_bActiveCracking;
	List<GameObject> m_pLights;
	float m_fIntensity;
	float m_fTimerCracking;
	float m_fTimerCrackingMax;
	int m_nNbCracking;
	int m_nIdLightCracking;
	int m_nNbLight;
	int m_nRandStarting = 1000;

	// Use this for initialization
	void Start () 
	{
		m_pLights = new List<GameObject>();
		m_nNbCracking = 2;
		m_nIdLightCracking = 0;
		m_nNbLight = 0;
		m_fTimerCrackingMax = m_nNbCracking * 0.5f;
		m_fTimerCracking = m_fTimerCrackingMax;
		GameObject[] pLightsLevel = GameObject.FindGameObjectsWithTag("light");
		foreach(GameObject currentLight in pLightsLevel)
		{
			if(currentLight.transform.parent.gameObject == this.gameObject)
			{
				m_pLights.Add(currentLight);
				m_fIntensity = currentLight.GetComponent<Light>().intensity;
				m_nNbLight++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_fTimerCracking <= m_fTimerCrackingMax)
			m_fTimerCracking += Time.deltaTime;
		else
		{
			if(Random.Range(0, m_nRandStarting) >= (m_nRandStarting - 1))
				StartCracking();
		}

		if(CApoilInput.DebugF12)
			StartCracking();

		float fDecalage = 3.14159f/2.0f;
		float fTimer = CApoilMath.InterpolationLinear(m_fTimerCracking, 0.0f, m_fTimerCrackingMax, 0.0f, m_nNbCracking*2.0f*3.14159f);

		m_pLights[m_nIdLightCracking].GetComponent<Light>().intensity = m_fIntensity * ((Mathf.Cos (fTimer) + 1.0f)/2.0f);
		if(!m_bIsNeon)
			gameObject.transform.FindChild("Capsule_Lampe").FindChild("Spotlight").GetComponent<Light>().intensity = m_pLights[m_nIdLightCracking].GetComponent<Light>().intensity;
	}

	void StartCracking()
	{
		m_nIdLightCracking = Random.Range(0, m_nNbLight);
		m_fTimerCracking = 0.0f;
		CSoundEngine.postEvent("Play_NeonBuzz", gameObject);
	}
}
