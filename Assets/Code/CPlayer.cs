using UnityEngine;
using System.Collections;

public class CPlayer : MonoBehaviour 
{

	float m_fVelocityWalk = 15.0f;
	float m_fVelocityRotation = 0.2f;
	float m_fVelocityJump = 250.0f;
	float m_fAngleY;
	float m_fTimerJump;
	float m_fTimerJumpMax = 1.0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void Move()
	{
		float fAngleX = gameObject.transform.rotation.eulerAngles.y * 2*3.14f/360.0f;
		Vector3 vForward = new Vector3(Mathf.Sin(fAngleX),0, Mathf.Cos(fAngleX));
		Vector3 vRight = new Vector3(Mathf.Sin(fAngleX + 3.14f/2.0f),0, Mathf.Cos(fAngleX + 3.14f/2.0f));
		Vector3 vUp = new Vector3(0,1,0);
		bool bAnimActive = false;
		
		if(CApoilInput.InputPlayer.MoveForward)
		{
			gameObject.rigidbody.AddForce(m_fVelocityWalk*vForward);
			bAnimActive = true;
		}
		if(CApoilInput.InputPlayer.MoveBackward)
		{
			gameObject.rigidbody.AddForce(-m_fVelocityWalk*vForward);
			bAnimActive = true;
			
		}
		if(CApoilInput.InputPlayer.MoveLeft)
		{
			gameObject.rigidbody.AddForce(-m_fVelocityWalk*vRight);
			bAnimActive = true;
		}
		if(CApoilInput.InputPlayer.MoveRight)
		{
			gameObject.rigidbody.AddForce(m_fVelocityWalk*vRight);
			bAnimActive = true;
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
}
