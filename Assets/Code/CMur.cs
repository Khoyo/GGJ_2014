using UnityEngine;
using System.Collections;

public class CMur : MonoBehaviour 
{

	float m_fSizeX = 15000.0f;
	float m_fSizeY = 200.0f;
	
	// Use this for initialization
	void Start () 
	{
		float fWidth = gameObject.renderer.material.GetTexture("_BumpMap").width;
		float fHeight = gameObject.renderer.material.GetTexture("_BumpMap").height;
		float fX = gameObject.transform.localScale.x;
		float fY = gameObject.transform.localScale.z;
		
		gameObject.renderer.material.SetTextureScale("_BumpMap", new Vector2(fY *m_fSizeY / fHeight, 1));	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
