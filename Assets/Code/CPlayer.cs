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
	float m_fVelocityJump = 250.0f;
	float m_fAngleY;
	float m_fTimerJump;
	float m_fTimerJumpMax = 1.0f;

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
		Vector3 vVelocity = gameObject.rigidbody.velocity;
		//Vector3 vVelocity = Vector3.zero;

		if(CApoilInput.InputPlayer.MoveForward)
		{
			vVelocity = m_fVelocityRun*m_fVelocityWalk*vForward;
		}
		if(CApoilInput.InputPlayer.MoveBackward)
		{
			vVelocity = -m_fVelocityRun*m_fVelocityWalk*vForward;
		}
		if(CApoilInput.InputPlayer.MoveLeft)
		{
			vVelocity = -m_fVelocityRun*m_fVelocityWalk*vRight;
		}
		if(CApoilInput.InputPlayer.MoveRight)
		{
			vVelocity = m_fVelocityRun*m_fVelocityWalk*vRight;
		}
		if(!CApoilInput.InputPlayer.MoveForward && !CApoilInput.InputPlayer.MoveBackward && !CApoilInput.InputPlayer.MoveLeft && !CApoilInput.InputPlayer.MoveRight)
		{
			Vector3 vel = gameObject.rigidbody.velocity;
			vel.x *= 0.85f;
			vel.z *= 0.85f;
			gameObject.rigidbody.velocity = vel;
		}

		//gameObject.rigidbody.velocity = vVelocity;
		gameObject.rigidbody.AddForce(vVelocity);

		if(gameObject.rigidbody.velocity.magnitude > m_fVelocityWalk * m_fVelocityRun)
			gameObject.rigidbody.velocity *= m_fVelocityWalk * m_fVelocityRun/gameObject.rigidbody.velocity.magnitude;



		Debug.Log (gameObject.rigidbody.velocity.magnitude);

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

		if(CApoilInput.InputPlayer.Jump && m_fTimerJump < 0.0f && m_bCanRun)
		{
			gameObject.rigidbody.AddForce(m_fVelocityJump*vUp);
			m_fTimerJump = m_fTimerJumpMax;
		}

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
