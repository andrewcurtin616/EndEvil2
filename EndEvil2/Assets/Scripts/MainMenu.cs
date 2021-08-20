using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool isStart;
    public bool isQuit;
	// Start is called before the first frame update
	void OnMouseUp()
	{
		if (isStart)
		{
			SceneManager.LoadScene("SampleScene 2");
		}
		if (isQuit)
		{
			Application.Quit();
		}
	}
}
