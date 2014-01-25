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
	float m_fVelocityRotation = 0.2f;
	float m_fVelocityJump = 250.0f;
	float m_fAngleY;
	float m_fTimerJump;
	float m_fTimerJumpMax = 1.0f;

	// Use this for initialization
	void Start () 
	{
		m_fAngleY = 0.0f;
		m_fTimerJump = 0.0f;
		m_eState = EState.e_Furtif;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Move();
		MoveHead();
		SwitchState();
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
		Vector3 vUp = new Vector3(0,1,0);

		if(CApoilInput.InputPlayer.MoveForward)
		{
			gameObject.rigidbody.AddForce(m_fVelocityWalk*vForward);
		}
		if(CApoilInput.InputPlayer.MoveBackward)
		{
			gameObject.rigidbody.AddForce(-m_fVelocityWalk*vForward);
		}
		if(CApoilInput.InputPlayer.MoveLeft)
		{
			gameObject.rigidbody.AddForce(-m_fVelocityWalk*vRight);
		}
		if(CApoilInput.InputPlayer.MoveRight)
		{
			gameObject.rigidbody.AddForce(m_fVelocityWalk*vRight);
		}
		if(!CApoilInput.InputPlayer.MoveForward && !CApoilInput.InputPlayer.MoveBackward && !CApoilInput.InputPlayer.MoveLeft && !CApoilInput.InputPlayer.MoveRight)
		{
			Vector3 vel = gameObject.rigidbody.velocity;
			vel.x *= 0.85f;
			vel.z *= 0.85f;
			gameObject.rigidbody.velocity = vel;
		}

		gameObject.transform.RotateAround(new Vector3(0,1,0),m_fVelocityRotation * CApoilInput.InputPlayer.MouseAngleX);
		
		if(CApoilInput.InputPlayer.Jump && m_fTimerJump < 0.0f)
		{
			gameObject.rigidbody.AddForce(m_fVelocityJump*vUp);
			m_fTimerJump = m_fTimerJumpMax;
		}
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

	void SwitchState()
	{
		if(CApoilInput.InputPlayer.SwitchFurtif)
		{
			m_eState = EState.e_Furtif;
		}
		if(CApoilInput.InputPlayer.SwitchBourin)
		{
			m_eState = EState.e_Bourin;
		}
		if(CApoilInput.InputPlayer.SwitchCharismatique)
		{
			m_eState = EState.e_Charismatique;
		}

		//DEBUG
		if(CApoilInput.DebugNum4)
		{
			m_eState = EState.e_MauvaisGout;
		}
	}
}
