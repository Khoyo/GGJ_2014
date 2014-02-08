using UnityEngine;
using System.Collections;

public class CPlayer : MonoBehaviour 
{
	public Texture m_TexturCrossHair;
	public Texture m_TextureBlack;
	public GameObject m_Impact;

	public enum EState
	{
		e_Furtif,
		e_Bourin,
		e_Charismatique,
		e_MauvaisGout,
	}

	EState m_eState;
	EState m_eStateToGo;


	float m_fVelocityWalk = 15.0f;
	float m_fVelocityRun;
	float m_fVelocityRotation = 0.2f;
	float m_fVelocityJump = 8.0f;
	float m_fAngleY;
	float m_fTimerGateling;
	float m_fCadenceGateling = 1/10.0f;
	float m_fTimerPisse;
	float m_fCadencePisse = 1/10.0f;
	float m_fTimerCut;
	float m_fCadenceCut = 1/6.0f;
	float m_fCoeffVelocityGateling;
	float m_fRadiusCut = 2.0f;
	float m_fRadiusPisse = 4.0f;
	float m_fTimerSwitch;
	float m_fTimerStopSoundFootStep;
	float m_fTimerStopSoundFootStepMax = 0.5f;
	float m_fParamFootStepVelocity = 0.5f;
	float m_fParamFootStepVelocityRun = 0.8f;

	Vector3 vMoveDirection = Vector3.zero;

	bool m_bCanSneak; //s'accroupir
	bool m_bJump;
	bool m_bCanRun;
	bool m_SwitchState;
	bool m_bIsInSwitch;
	bool m_bSneak;
	bool m_bIsOnLadder;
	bool m_bSoundCutIsPlayed;
	bool m_bSoundPisseIsPlayed;
	bool m_bSoundFootstepLaunched;
	int m_nNbFrameGatling;

	public GameObject m_Batteuse, m_Couteau, m_Boobs, m_Pisse;

	// Use this for initialization
	void Start () 
	{
		m_fAngleY = 0.0f;
		m_fVelocityRun = 1.0f;
		m_fTimerGateling = 0.0f;
		m_fTimerSwitch = 0.0f;
		m_fTimerPisse = 0.0f;
		m_fTimerCut = 0.0f;
		m_eState = EState.e_Furtif;

		m_eStateToGo = m_eState;
		m_bCanSneak = false;
		m_bJump = false;
		m_bCanRun = false;
		m_SwitchState = false;
		m_bIsInSwitch = false;
		m_bSoundCutIsPlayed = false;
		m_bSoundPisseIsPlayed = false;
		SwitchState();
		m_fCoeffVelocityGateling = 0.0f;
		m_bSoundFootstepLaunched = false;

		m_bSneak = false;
		m_bIsOnLadder = false;
		m_nNbFrameGatling = 0;
		gameObject.transform.FindChild("Head").FindChild("light").gameObject.SetActive(false);
		StopPisse();

		foreach(AnimationState anim in m_Couteau.animation)
		{
			//anim.wrapMode = WrapMode.Once;
			anim.speed = 6;
		}
		ResetHeadRotation();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!m_bIsInSwitch /*&& m_fStartingLevel <= 0.0f*/) 
		{
			if (!m_bIsOnLadder)
				Move ();
			else 
			{
				MoveOnLadder ();
			}
			MoveHead ();
			InputsPlayer ();

			switch (m_eState) {
				case EState.e_Furtif:
				{
					if (CApoilInput.InputPlayer.Fire) {
							FireCut ();
					}
					break;
				}
				case EState.e_Bourin:
				{
					if (CApoilInput.InputPlayer.Fire) 
					{
						FireGatling ();
					} 
					else 
					{
						m_nNbFrameGatling = 0;
						gameObject.transform.FindChild ("Head").FindChild ("light").gameObject.SetActive (false);
					}
					m_Batteuse.transform.Rotate (Vector3.forward, m_fCoeffVelocityGateling * 600 * Time.deltaTime);

					break;
				}
				case EState.e_Charismatique:
				{
					break;
				}
				case EState.e_MauvaisGout:
				{
					gameObject.transform.FindChild ("Head").Rotate (Vector3.forward, Mathf.Cos (Time.time) / 2.0f);
					if (CApoilInput.InputPlayer.Fire) 
					{
						FirePisse ();
					} 
					else 
					{
						StopPisse ();
					}
					break;
				}
			}
		}

		if(m_bIsInSwitch)
		{
			EntreDeuxChangement();
		}

		if(m_fCoeffVelocityGateling > 0.0f)
			m_fCoeffVelocityGateling -= Time.deltaTime;
		else
			m_fCoeffVelocityGateling = 0.0f;
	}

	void OnGUI()
	{
		if(CGame.m_bDebug)
			DisplayDebug();
		Rect position = new Rect((Screen.width - m_TexturCrossHair.width) / 2, (Screen.height -  m_TexturCrossHair.height) /2, m_TexturCrossHair.width, m_TexturCrossHair.height);
		GUI.DrawTexture(position, m_TexturCrossHair);

		float fHeight = 450.0f;

		if(m_bIsInSwitch)
		{
			float fPosY = CApoilMath.InterpolationLinear(m_fTimerSwitch, 0.0f, CGame.m_fTimerSwitchMax, 0.0f, 3.14159f);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Sqrt( Mathf.Sin(fPosY)));
			GUI.DrawTexture(new Rect(0, 0, CGame.m_fWidth, CGame.m_fHeight),  m_TextureBlack);
		}
	}

	void DisplayDebug()
	{
		GUI.Label(new Rect(10, 10, 100, 20), System.Convert.ToString(m_eState));
	}

	void Move()
	{
		float fAngleX = gameObject.transform.rotation.eulerAngles.y * 2*3.14f/360.0f;
		/*Vector3 vForward = new Vector3(Mathf.Sin(fAngleX),0, Mathf.Cos(fAngleX));
		Vector3 vRight = new Vector3(Mathf.Sin(fAngleX + 3.14f/2.0f),0, Mathf.Cos(fAngleX + 3.14f/2.0f));
		*/
		CharacterController controller = GetComponent<CharacterController>();

		float vspeed = vMoveDirection.y;

		//vMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		vMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		vMoveDirection = transform.TransformDirection(vMoveDirection);
		vMoveDirection *= m_fVelocityWalk * m_fVelocityRun;

		vMoveDirection.y = vspeed;


		vMoveDirection.y -= 20 * Time.deltaTime;

		if (controller.isGrounded) 
		{
			if (CApoilInput.InputPlayer.Jump && m_bCanRun)
				vMoveDirection.y = m_fVelocityJump;
			else
				vMoveDirection.y = Mathf.Max(0, vMoveDirection.y);
			
		}
		controller.Move(vMoveDirection * Time.deltaTime);

		gameObject.transform.RotateAround(new Vector3(0,1,0),m_fVelocityRotation * CApoilInput.InputPlayer.MouseAngleX);

		SoundFootStep();
	}
	
	void MoveHead()
	{
		float fAngleMax = -4.0f;
		float fAngleMin = 4.0f;
		float fAngleBeforeY = m_fAngleY;
		
		m_fAngleY += CApoilInput.InputPlayer.MouseAngleY;
		
		if(m_fAngleY < fAngleMax)
			m_fAngleY = fAngleMax;
		else if(m_fAngleY > fAngleMin)
			m_fAngleY = fAngleMin;
		
		gameObject.transform.FindChild("Head").RotateAroundLocal(new Vector3(1,0,0), m_fVelocityRotation * (m_fAngleY - fAngleBeforeY));
	}

	void MoveOnLadder()
	{
		Vector3 vDirection = new Vector3(0, CApoilInput.InputPlayer.MoveForward ? 1.0f : 0.0f, 0);
		gameObject.transform.Translate(vDirection/5);
	}

	void SoundFootStep()
	{
		if(CApoilInput.InputPlayer.Move)
		{
			if(!m_bSoundFootstepLaunched)
			{
				CSoundEngine.postEvent ("Play_footStep", gameObject);
				m_bSoundFootstepLaunched = true;
				m_fTimerStopSoundFootStep = m_fTimerStopSoundFootStepMax;
			}
		}
		else if(m_fTimerStopSoundFootStep < 0.0f)
		{
			CSoundEngine.postEvent ("Stop_footStep", gameObject);
			m_bSoundFootstepLaunched = false;
		}

		m_fTimerStopSoundFootStep -= Time.deltaTime;

		if(CApoilInput.InputPlayer.Run)
			CSoundEngine.setRTPC ("Parameter_PitchFootStep", m_fParamFootStepVelocityRun, gameObject);
		else 
			CSoundEngine.setRTPC ("Parameter_PitchFootStep", m_fParamFootStepVelocity, gameObject);
	}

	public void Die()
	{
		StopPisse();
		CSoundEngine.postEvent("Play_DiePlayer", gameObject);
		CSoundEngine.postEvent("Stop_footStep", gameObject);
		Application.LoadLevel(Application.loadedLevel);
	}

	void InputsPlayer()
	{
		Vector3 vUp = new Vector3(0,1,0);

		if(!CGame.m_bLevelFixeSansSwitch)
		{
			if(CApoilInput.InputPlayer.SwitchFurtif)
			{
				GoToStateFurtif();
			}
			if(CApoilInput.InputPlayer.SwitchBourin)
			{
				GoToStateBourin();
			}
			if(CApoilInput.InputPlayer.SwitchCharismatique)
			{
				GoToStateCharismatique();
			}
			
			//DEBUG
			if(CApoilInput.DebugNum4)
			{
				GoToStateMauvaisGout();
			}
		}

		//DEBUG
		if(CApoilInput.DebugF9)
			Die ();

		if(CApoilInput.InputPlayer.Sneak && m_bCanSneak && !m_bSneak)
		{
			gameObject.transform.FindChild("Head").Translate(new Vector3(0,-1,0));
			m_bSneak = true;
		}
		else if(!CApoilInput.InputPlayer.Sneak && m_bSneak)
		{
			gameObject.transform.FindChild("Head").Translate(new Vector3(0,1,0));
			m_bSneak = false;
		}

		if(CApoilInput.InputPlayer.Run && m_bCanRun)
		{
			m_fVelocityRun = 2.0f;
		}
		else
			m_fVelocityRun = 1.0f;

	}

	public void GoToStateFurtif()
	{
		m_eStateToGo = EState.e_Furtif;
		m_fTimerSwitch = CGame.m_fTimerSwitchMax;
		m_bIsInSwitch = true;
		CSoundEngine.postEvent("Play_Switch", gameObject);
	}

	public void GoToStateBourin()
	{
		m_eStateToGo = EState.e_Bourin;
		m_fTimerSwitch = CGame.m_fTimerSwitchMax;
		m_bIsInSwitch = true;
		CSoundEngine.postEvent("Play_Switch", gameObject);
	}

	public void GoToStateCharismatique()
	{
		m_eStateToGo = EState.e_Charismatique;
		m_fTimerSwitch = CGame.m_fTimerSwitchMax;
		m_bIsInSwitch = true;
		CSoundEngine.postEvent("Play_Switch", gameObject);
	}

	public void GoToStateMauvaisGout()
	{
		m_eStateToGo = EState.e_MauvaisGout;
		m_fTimerSwitch = CGame.m_fTimerSwitchMax;
		m_bIsInSwitch = true;
		CSoundEngine.postEvent("Play_Switch", gameObject);
	}

	void FireGatling()
	{
		RaycastHit hit;
		GameObject cam = gameObject.transform.FindChild("Head").FindChild("MainCamera").gameObject;
		Physics.Raycast(cam.transform.position, cam.transform.forward,out hit, 100000, ~(1<<9));

		if(m_fTimerGateling >= 0.0f)
			m_fTimerGateling -= Time.deltaTime;

		if(m_fTimerGateling < 0.0f)
		{
			m_nNbFrameGatling++;
			if(m_nNbFrameGatling%2 == 0)
			{
				CSoundEngine.postEvent("Play_GunFire", gameObject);
				m_nNbFrameGatling = 0;
			}
			m_fTimerGateling = m_fCadenceGateling;
			if(hit.collider != null)
				Debug.Log ("blocked by "+hit.collider.name);
			if (hit.collider != null && hit.collider.CompareTag("Ennemies"))
			{
				//print ("Blocked by " + hit.collider.name);
				//Object.Destroy(collider.gameObject);
				hit.collider.gameObject.GetComponent<CEnnemi>().TakeBullet();
				//GameObject newImpact;
				//newImpact = (Instantiate(m_Impact, hit.collider.gameObject. transform.position, Quaternion.identity) as GameObject);
				//newImpact.transform.parent = hit.collider.gameObject.transform;
				
			}
		}

		m_fCoeffVelocityGateling = 1.0f;
		gameObject.transform.FindChild("Head").FindChild("light").gameObject.SetActive(true);
	}

	void FireCut()
	{
		RaycastHit hit;
		GameObject cam = gameObject.transform.FindChild("Head").FindChild("MainCamera").gameObject;
		Physics.Raycast(cam.transform.position, cam.transform.forward,out hit, m_fRadiusCut);
		
		if(m_fTimerCut >= 0.0f)
			m_fTimerCut -= Time.deltaTime;
		if(m_fTimerCut < 0.0f)
		{
			//faire le mvt
			m_bSoundCutIsPlayed = false;

			if (hit.collider != null && hit.collider.CompareTag("Ennemies"))
			{
				//print ("Blocked by " + hit.collider.name);
				//Object.Destroy(collider.gameObject);
				CSoundEngine.postEvent("Play_CutHit", gameObject);
				hit.collider.gameObject.GetComponent<CEnnemi>().TakeCut();
				m_fTimerCut = m_fCadenceCut;
				gameObject.transform.FindChild("Head").FindChild("MainEtCouteau").animation.Play();
				//GameObject newImpact;
				//newImpact = (Instantiate(m_Impact, hit.collider.gameObject. transform.position, Quaternion.identity) as GameObject);
				//newImpact.transform.parent = hit.collider.gameObject.transform;
				
			}
			else if(!m_bSoundCutIsPlayed)
			{
				CSoundEngine.postEvent("Play_CutMiss", gameObject);
				m_bSoundCutIsPlayed = true;
				m_fTimerCut = m_fCadenceCut;
				gameObject.transform.FindChild("Head").FindChild("MainEtCouteau").animation.Play();
			}
		}

	}
	
	void FirePisse()
	{
		//m_Pisse.SetActive(true);
		m_Pisse.GetComponent<ParticleSystem>().enableEmission = true;
		if(!m_bSoundPisseIsPlayed)
		{
			CSoundEngine.postEvent("Play_Pisse", gameObject);
			m_bSoundPisseIsPlayed = true;
		}

		RaycastHit hit;
		GameObject cam = gameObject.transform.FindChild("Head").FindChild("MainCamera").gameObject;
		Physics.Raycast(cam.transform.position, cam.transform.forward,out hit, m_fRadiusPisse, ~(1<<9));

		if(m_fTimerPisse >= 0.0f)
			m_fTimerPisse -= Time.deltaTime;
		
		if(m_fTimerPisse < 0.0f)
		{

			if (hit.collider != null && hit.collider.CompareTag("Ennemies"))
			{
				//print ("Blocked by " + hit.collider.name);
				//Object.Destroy(collider.gameObject);
				hit.collider.gameObject.GetComponent<CEnnemi>().TakePisseHit();
				m_fTimerPisse = m_fCadencePisse;
				//GameObject newImpact;
				//newImpact = (Instantiate(m_Impact, hit.collider.gameObject. transform.position, Quaternion.identity) as GameObject);
				//newImpact.transform.parent = hit.collider.gameObject.transform;
				
			}
		}
	}

	void StopPisse()
	{
		//m_Pisse.SetActive(false);
		m_Pisse.GetComponent<ParticleSystem>().enableEmission = false;
		CSoundEngine.postEvent("Stop_Pisse", gameObject);
		m_bSoundPisseIsPlayed = false;
	}

	void EntreDeuxChangement()
	{
		if(m_fTimerSwitch > 0.0f)
		{
			m_fTimerSwitch -= Time.deltaTime;
			if(m_fTimerSwitch < CGame.m_fTimerSwitchMax/2.0f)
			{
				m_eState = m_eStateToGo;
				SwitchState();
			}
		}
		else
		{
			m_bIsInSwitch = false;

		}
	}

	public EState getState()
	{
		return m_eState;
	}

	void SwitchState()
	{
		switch(m_eState)
		{
			case EState.e_Furtif:
			{
				m_bCanSneak = true;
				m_bJump = true;
				m_bCanRun = true;
				m_Batteuse.SetActive(false);
				m_Couteau.SetActive(true);
				m_Boobs.SetActive(false);
				ResetHeadRotation();
				break;
			}
			case EState.e_Bourin:
			{
				m_bCanSneak = false;
				m_bJump = false;
				m_bCanRun = true;
				m_Batteuse.SetActive(true);
				m_Couteau.SetActive(false);
				m_Boobs.SetActive(false);
				ResetHeadRotation();
				break;
			}
			case EState.e_Charismatique:
			{
				m_bCanSneak = false;
				m_bJump = true;
				m_bCanRun = false;
				m_Batteuse.SetActive(false);
				m_Couteau.SetActive(false);
				m_Boobs.SetActive(true);
				ResetHeadRotation();
				break;
			}
			case EState.e_MauvaisGout:
			{
				m_bCanSneak = false;
				m_bJump = true;
				m_bCanRun = true;
				m_Batteuse.SetActive(false);
				m_Couteau.SetActive(false);
				m_Boobs.SetActive(false);
				break;
			}
		}
	}

	void ResetHeadRotation()
	{
		Quaternion rot = gameObject.transform.FindChild ("Head").localRotation;
		rot.y = 0;
		rot.z = 0;
		gameObject.transform.FindChild ("Head").localRotation = rot;
	}

	void OnTriggerStay(Collider other)
	{
		if(other.CompareTag("Echelle") && m_eState == EState.e_Furtif)
		{
			m_bIsOnLadder = true;
		}
		if(other.name == "Killzone")
			Die();
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Echelle"))
		{
			m_bIsOnLadder = false;
		}
	}
}
