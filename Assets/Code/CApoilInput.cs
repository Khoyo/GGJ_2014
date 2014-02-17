using UnityEngine;
using System.Collections;

public struct SPlayerInput
{
	public bool MoveLeft;
	public bool MoveRight;
	public bool MoveForward;
	public bool MoveBackward;
	public bool Move;
	public bool Jump;
	public bool Sneak;
	public bool Run;
	public bool SwitchFurtif;
	public bool SwitchBourin;
	public bool SwitchCharismatique;
	public bool Fire;
	public float MouseAngleX;
	public float MouseAngleY;
}

public class CApoilInput
{
	public static SPlayerInput InputPlayer;
	
	public static bool Quit;

	//Debug
	public static bool DebugNum4;
	public static bool DebugF9;
	public static bool DebugF10;
	public static bool DebugF11;
	public static bool DebugF12;
	
	
	public static void Init()
	{
		if(!Application.isEditor)
			Screen.lockCursor = true;
	}
	
	//-------------------------------------------------------------------------------
	///
	//-------------------------------------------------------------------------------
	public static void Process(float fDeltatime) 
	{	
		InputPlayer.MoveForward = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
		InputPlayer.MoveBackward = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
		InputPlayer.MoveLeft = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);;
		InputPlayer.MoveRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);		

		InputPlayer.Move = InputPlayer.MoveForward || InputPlayer.MoveBackward || InputPlayer.MoveLeft || InputPlayer.MoveRight;

		InputPlayer.MouseAngleX = Input.GetAxis("Mouse X");
		InputPlayer.MouseAngleY = -Input.GetAxis("Mouse Y");

		InputPlayer.SwitchFurtif = Input.GetKeyDown(KeyCode.Alpha1);
		InputPlayer.SwitchBourin = Input.GetKeyDown(KeyCode.Alpha2);
		InputPlayer.SwitchCharismatique = Input.GetKeyDown(KeyCode.Alpha3);

		InputPlayer.Jump = Input.GetKeyDown(KeyCode.Space);
		InputPlayer.Sneak = false;//Input.GetKey(KeyCode.LeftControl);
		InputPlayer.Run = Input.GetKey(KeyCode.LeftShift);

		InputPlayer.Fire =  Input.GetMouseButton(0);
		
		Quit = Input.GetKeyDown(KeyCode.Escape);

		DebugNum4 = false;// Input.GetKeyDown(KeyCode.Alpha4);
		DebugF9 = false;// Input.GetKeyDown(KeyCode.F9);
		DebugF10 = false;// Input.GetKeyDown(KeyCode.F10);
		DebugF11 = false;// Input.GetKeyDown(KeyCode.F11);
		DebugF12 = false;// Input.GetKeyDown(KeyCode.F12);
	}	

}
