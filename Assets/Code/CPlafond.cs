﻿using UnityEngine;
using System.Collections;

public class CPlafond : MonoBehaviour {

	float m_fSize = 60.0f;

	// Use this for initialization
	void Start () 
	{
		float fWidth = gameObject.renderer.material.GetTexture("_BumpMap").width;
		float fHeight = gameObject.renderer.material.GetTexture("_BumpMap").height;
		float fX = gameObject.transform.localScale.x;
		float fY = gameObject.transform.localScale.z;
		
		gameObject.renderer.material.SetTextureScale("_BumpMap", new Vector2(fX*m_fSize/ fWidth, fY*m_fSize/ fHeight));	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
