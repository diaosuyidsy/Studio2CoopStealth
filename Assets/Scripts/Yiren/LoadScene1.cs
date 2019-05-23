using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class LoadScene1 : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (ReInput.players.GetPlayer(0).GetButtonUp("A") || ReInput.players.GetPlayer(1).GetButtonUp("A"))
			SceneManager.LoadScene(1);
	}

	public void LoadFirstScene()
	{
		SceneManager.LoadScene(1);
	}
}
