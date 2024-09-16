using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class fps : MonoBehaviour
{
	string label = "";
	float count;
	public TMP_Text text;
	IEnumerator Start()
	{
		GUI.depth = 2;
		while (true)
		{
			if (Time.timeScale == 1)
			{
				yield return new WaitForSeconds(0.1f);
				count = (1 / Time.deltaTime);
				label = "FPS :" + (Mathf.Round(count));
			}
			else
			{
				label = "Pause";
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	void OnGUI()
	{
		text.text = label;

	}
}
