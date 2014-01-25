using UnityEngine;
using System.Collections;

public class CPlayer : MonoBehaviour 
{
	public Texture m_TexturCrossHair;
	public GameObject m_Impact;

	enum EState
	{
		e_Furtif,
		e_Bourin,
		e_Charismatique,
		e_MauvaisGout,
	}

	EState m_eState;
	float m_fVelocityWalk = 15.0f;
	float m_fVelocityRun;
	float m_fVelocityRotation = 0.2f;
	float m_fVelocityJump = 8.0f;
	float m_fAngleY;
	float m_fTimerJump;
	float m_fTimerJumpMax = 1.0f;
	float m_fTimerGateling;
	float m_fCadenceGateling = 1/10.0f;
	float m_fTimerCut;
	float m_fCadenceCut = 1.0f;
	float m_fCoeffVelocityGateling;
	float m_fRadiusCut = 2.0f;

	Vector3 vMoveDirection = Vector3.zero;

	bool m_bCanSneak; //s'accroupir
	bool m_bJump;
	bool m_bCanRun;
	bool m_SwitchState;
	bool m_bSneak;
	int m_nNbFrameGatling;

	public GameObject m_Batteuse, m_Couteau, m_Boobs;

	// Use this for initialization
	void Start () 
	{
		m_fAngleY = 0.0f;
		m_fTimerJump = 0.0f;
		m_fVelocityRun = 1.0f;
		m_fTimerGateling = 0.0f;
		m_eState = EState.e_Furtif;

		m_bCanSneak = false;
		m_bJump = false;
		m_bCanRun = false;
		m_SwitchState = false;
		SwitchState();
		m_fCoeffVelocityGateling = 0.0f;

		m_bSneak = false;
		m_nNbFrameGatling = 0;
		gameObject.transform.FindChild("Head").FindChild("light").gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		Move();
		MoveHead();
		InputsPlayer();

		if(m_SwitchState)
		{
			SwitchState();
			m_SwitchState = false;
		}

		switch(m_eState)
		{
			case EState.e_Furtif:
			{
				if(CApoilInput.InputPlayer.Fire)
				{
					FireCut();
				}
				break;
			}
			case EState.e_Bourin:
			{
				if(CApoilInput.InputPlayer.Fire)
				{
					FireGatling();
				}
				else
				{
					m_nNbFrameGatling = 0;
					gameObject.transform.FindChild("Head").FindChild("light").gameObject.SetActive(false);
				}
				m_Batteuse.transform.Rotate(Vector3.forward,m_fCoeffVelocityGateling*600* Time.deltaTime);

				break;
			}
			case EState.e_Charismatique:
			{
				break;
			}
			case EState.e_MauvaisGout:
			{
				break;
			}
		}

		if(m_fTimerJump >= 0.0f)
			m_fTimerJump -= Time.deltaTime;

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
	}

	void DisplayDebug()
	{
		GUI.Label(new Rect(10, 10, 100, 20), System.Convert.ToString(m_eState));
	}

	void Move()
	{
		float fAngleX = gameObject.transform.rotation.eulerAngles.y * 2*3.14f/360.0f;
		Vector3 vForward = new Vector3(Mathf.Sin(fAngleX),0, Mathf.Cos(fAngleX));
		Vector3 vRight = new Vector3(Mathf.Sin(fAngleX + 3.14f/2.0f),0, Mathf.Cos(fAngleX + 3.14f/2.0f));
		
		CharacterController controller = GetComponent<CharacterController>();

		float vspeed = vMoveDirection.y;

		vMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		vMoveDirection = transform.TransformDirection(vMoveDirection);
		vMoveDirection *= m_fVelocityWalk * m_fVelocityRun;

		vMoveDirection.y = vspeed;


		vMoveDirection.y -= 20 * Time.deltaTime;

		if (controller.isGrounded) {
			if (CApoilInput.InputPlayer.Jump && m_bCanRun)
				vMoveDirection.y = m_fVelocityJump;
			else
				vMoveDirection.y = Mathf.Max(0, vMoveDirection.y);
			
		}
		controller.Move(vMoveDirection * Time.deltaTime);


		gameObject.transform.RotateAround(new Vector3(0,1,0),m_fVelocityRotation * CApoilInput.InputPlayer.MouseAngleX);
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

	void Die()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	void InputsPlayer()
	{
		Vector3 vUp = new Vector3(0,1,0);
		
		if(CApoilInput.InputPlayer.SwitchFurtif)
		{
			m_eState = EState.e_Furtif;
			m_SwitchState = true;
		}
		if(CApoilInput.InputPlayer.SwitchBourin)
		{
			m_eState = EState.e_Bourin;
			m_SwitchState = true;
		}
		if(CApoilInput.InputPlayer.SwitchCharismatique)
		{
			m_eState = EState.e_Charismatique;
			m_SwitchState = true;
		}
		
		//DEBUG
		if(CApoilInput.DebugNum4)
		{
			m_eState = EState.e_MauvaisGout;
			m_SwitchState = true;
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

	void FireGatling()
	{
		RaycastHit hit;
		GameObject cam = gameObject.transform.FindChild("Head").FindChild("MainCamera").gameObject;
		Physics.Raycast(cam.transform.position, cam.transform.forward,out hit);

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
		
		if (hit.collider != null && hit.collider.CompareTag("Ennemies") && m_fTimerCut < 0.0f)
		{
			//print ("Blocked by " + hit.collider.name);
			//Object.Destroy(collider.gameObject);
			m_fTimerCut = m_fCadenceCut;
			hit.collider.gameObject.GetComponent<CEnnemi>().TakeCut();
			//GameObject newImpact;
			//newImpact = (Instantiate(m_Impact, hit.collider.gameObject. transform.position, Quaternion.identity) as GameObject);
			//newImpact.transform.parent = hit.collider.gameObject.transform;
			
		}
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
}
