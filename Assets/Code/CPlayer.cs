using UnityEngine;
using System.Collections;

public class CPlayer : MonoBehaviour 
{
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

	Vector3 vMoveDirection = Vector3.zero;

	bool m_bCanSneak; //s'accroupir
	bool m_bJump;
	bool m_bCanRun;
	bool m_SwitchState;

	// Use this for initialization
	void Start () 
	{
		m_fAngleY = 0.0f;
		m_fTimerJump = 0.0f;
		m_fVelocityRun = 1.0f;
		m_eState = EState.e_Furtif;

		m_bCanSneak = false;
		m_bJump = false;
		m_bCanRun = false;
		m_SwitchState = false;
		SwitchState();
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
				break;
			}
			case EState.e_Bourin:
			{
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
	}

	void OnGUI()
	{
		if(CGame.m_bDebug)
			DisplayDebug();
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
		if (controller.isGrounded) {
			vMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			vMoveDirection = transform.TransformDirection(vMoveDirection);
			vMoveDirection *= m_fVelocityWalk;
			if (CApoilInput.InputPlayer.Jump && m_bCanRun)
				vMoveDirection.y = m_fVelocityJump;
			
		}
		vMoveDirection.y -= 20 * Time.deltaTime;
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
/*
		if(CApoilInput.InputPlayer.Jump && m_fTimerJump < 0.0f && m_bCanRun)
		{
			gameObject.rigidbody.AddForce(m_fVelocityJump*vUp);
			m_fTimerJump = m_fTimerJumpMax;
		}
*/
		if(CApoilInput.InputPlayer.Sneak && m_bCanSneak)
		{

		}

		if(CApoilInput.InputPlayer.Run && m_bCanRun)
		{
			m_fVelocityRun = 2.0f;
		}
		else
			m_fVelocityRun = 1.0f;
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
				break;
			}
			case EState.e_Bourin:
			{
				m_bCanSneak = false;
				m_bJump = false;
				m_bCanRun = true;
				break;
			}
			case EState.e_Charismatique:
			{
				m_bCanSneak = false;
				m_bJump = true;
				m_bCanRun = false;
				break;
			}
			case EState.e_MauvaisGout:
			{
				m_bCanSneak = false;
				m_bJump = true;
				m_bCanRun = true;
				break;
			}
		}
	}
}
