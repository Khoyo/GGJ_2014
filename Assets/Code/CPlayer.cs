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

	public float walkSpeed = 15.0f;
	public float runSpeed = 20.0f;
	public bool limitDiagonalSpeed = true;
	public bool toggleRun = false;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	public float fallingDamageThreshold = 10.0f;
	public bool slideWhenOverSlopeLimit = false;
	public bool slideOnTaggedObjects = false;
	public float slideSpeed = 12.0f;
	public bool airControl = true;
	public float antiBumpFactor = .75f;
	public int antiBunnyHopFactor = 1;
	
	private Vector3 moveDirection = Vector3.zero;
	private bool grounded = false;
	private CharacterController controller;
	private Transform myTransform;
	private float speed;
	private RaycastHit hit;
	private float fallStartLevel;
	private bool falling;
	private float slideLimit;
	private float rayDistance;
	private Vector3 contactPoint;
	private bool playerControl = false;
	private int jumpTimer;

	EState m_eState;
	EState m_eStateToGo;


	float m_fVelocityRotation = 0.2f;
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
	float m_fTimerSwitchMax = 3.0f;
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
		controller = GetComponent<CharacterController>();
		myTransform = transform;
		speed = walkSpeed;
		rayDistance = controller.height * .5f + controller.radius;
		slideLimit = controller.slopeLimit - .1f;
		jumpTimer = antiBunnyHopFactor;

		m_fAngleY = 0.0f;
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
			if (m_bIsOnLadder)
			{
				MoveOnLadder ();
			}
			else
				SoundFootStep();
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

		if (toggleRun && grounded && Input.GetButtonDown("Run"))
			speed = (speed == walkSpeed? runSpeed : walkSpeed);
	}

	void FixedUpdate(){
		if (!m_bIsOnLadder && !m_bIsInSwitch)
			Move ();
	}

	// Store point that we're in contact with for use in FixedUpdate if needed
	void OnControllerColliderHit (ControllerColliderHit hit) {
		contactPoint = hit.point;
	}
	
	// If falling damage occured, this is the place to do something about it. You can make the player
	// have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
	void FallingDamageAlert (float fallDistance) {
		print ("Ouch! Fell " + fallDistance + " units!");   
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
			float fPosY = CApoilMath.InterpolationLinear(m_fTimerSwitch, 0.0f, m_fTimerSwitchMax, 0.0f, 3.14159f);
			GUI.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Sqrt( Mathf.Sin(fPosY)));
			GUI.DrawTexture(new Rect(0, 0, CGame.m_fWidth, CGame.m_fHeight),  m_TextureBlack);
		}
	}

	void DisplayDebug()
	{
		GUI.Label(new Rect(10, 10, 100, 20), System.Convert.ToString(m_eState));
	}

	/*
	void Move()
	{
		float fAngleX = gameObject.transform.rotation.eulerAngles.y * 2*3.14f/360.0f;
		/*Vector3 vForward = new Vector3(Mathf.Sin(fAngleX),0, Mathf.Cos(fAngleX));
		Vector3 vRight = new Vector3(Mathf.Sin(fAngleX + 3.14f/2.0f),0, Mathf.Cos(fAngleX + 3.14f/2.0f));
		*//*
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
	}*/


	void Move(){
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		// If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;
		
		if (grounded) {
			bool sliding = false;
			// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
			// because that interferes with step climbing amongst other annoyances
			if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) {
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			else {
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			
			// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
			if (falling) {
				falling = false;
				if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
					FallingDamageAlert (fallStartLevel - myTransform.position.y);
			}
			
			// If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
			if (!toggleRun)
				speed = CApoilInput.InputPlayer.Run ? runSpeed : walkSpeed;
			
			// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
			if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
				Vector3 hitNormal = hit.normal;
				moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
				Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
				moveDirection *= slideSpeed;
				playerControl = false;
			}
			// Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
			else {
				moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
				moveDirection = myTransform.TransformDirection(moveDirection) * speed;
				playerControl = true;
			}
			
			// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
			if (!Input.GetButton("Jump"))
				jumpTimer++;
			else if (jumpTimer >= antiBunnyHopFactor) {
				moveDirection.y = jumpSpeed;
				jumpTimer = 0;
			}
		}
		else {
			// If we stepped over a cliff or something, set the height at which we started falling
			if (!falling) {
				falling = true;
				fallStartLevel = myTransform.position.y;
			}
			
			// If air control is allowed, check movement but don't touch the y component
			if (airControl && playerControl) {
				moveDirection.x = inputX * speed * inputModifyFactor;
				moveDirection.z = inputY * speed * inputModifyFactor;
				moveDirection = myTransform.TransformDirection(moveDirection);
			}
		}
		
		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller, and set grounded true or false depending on whether we're standing on something
		grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
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
		
		gameObject.transform.RotateAround(new Vector3(0,1,0),m_fVelocityRotation * CApoilInput.InputPlayer.MouseAngleX);
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

		
	}

	public void GoToStateFurtif()
	{
		m_eStateToGo = EState.e_Furtif;
		m_fTimerSwitch = m_fTimerSwitchMax;
		m_bIsInSwitch = true;
		CSoundEngine.postEvent("Play_Switch", gameObject);
	}

	public void GoToStateBourin()
	{
		m_eStateToGo = EState.e_Bourin;
		m_fTimerSwitch = m_fTimerSwitchMax;
		m_bIsInSwitch = true;
		CSoundEngine.postEvent("Play_Switch", gameObject);
	}

	public void GoToStateCharismatique()
	{
		m_eStateToGo = EState.e_Charismatique;
		m_fTimerSwitch = m_fTimerSwitchMax;
		m_bIsInSwitch = true;
		CSoundEngine.postEvent("Play_Switch", gameObject);
	}

	public void GoToStateMauvaisGout()
	{
		m_eStateToGo = EState.e_MauvaisGout;
		m_fTimerSwitch = m_fTimerSwitchMax;
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
			if(m_fTimerSwitch < m_fTimerSwitchMax/2.0f)
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
