using UnityEngine;
using System.Collections;

public class CSoundEngine {

	//CGame game;
	static uint bankID;
	static bool mute = false;
	
	// Use this for initialization
	public static void Init() 
	{		

	}
	
	public static void LoadBank(string soundbankName) 
	{
		AKRESULT result;
		if((result = AkSoundEngine.LoadBank(soundbankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID)) != AKRESULT.AK_Success){
			Debug.LogError("Unable to load "+soundbankName+" with result: " + result);
		}
		
	}
	
	public static void setSwitch(string name, string val, GameObject obj){
		AkSoundEngine.SetSwitch(name, val, obj);
	}
	
	public static void setRTPC(string name, float val, GameObject obj){
		AkSoundEngine.SetRTPCValue(name, val, obj);
	}
	
	public static void postEvent(string name, GameObject obj){
		if(!mute)
			AkSoundEngine.PostEvent(name, obj);
		//Debug.Log ("Posted "+name+" to sound engine");
	}


}
