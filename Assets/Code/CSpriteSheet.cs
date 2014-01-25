using UnityEngine;
using System.Collections;

public class CSpriteSheet : MonoBehaviour {

	public int m_nColumns = 8;
	public int m_nRows = 13;
	public float m_fFramesPerSecond = 20f;
	
	//the current frame to display
	private int m_nIndex = 0;
	bool m_bActive;
	
	void Start()
	{
		m_bActive = true;
		StartCoroutine(updateTiling());
		
		//set the tile size of the texture (in UV units), based on the rows and columns
		Vector2 size = new Vector2(1f / m_nColumns, 1f / m_nRows);
		renderer.sharedMaterial.SetTextureScale("_MainTex", size);

	}
	
	private IEnumerator updateTiling()
	{
		while (true)
		{
			//move to the next index
			if(m_bActive)
				m_nIndex++;
			if (m_nIndex >= m_nRows * m_nColumns)
				m_nIndex = 0;
			
			//split into x and y indexes
			Vector2 offset = new Vector2((float)m_nIndex / m_nColumns - (m_nIndex / m_nColumns), //x index
			                             (m_nIndex / m_nColumns) / (float)m_nRows);          //y index
			
			renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
			
			yield return new WaitForSeconds(1f / m_fFramesPerSecond);
		}
		
	}

	public void SetAnim(bool bActive)
	{
		m_bActive = bActive;
	}
}
