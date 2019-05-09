using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoNavigation : MonoBehaviour 
{
	public void LoadMainScenariosDemoScene()
	{
		Cursor.lockState = CursorLockMode.Locked;
		SceneManager.LoadScene("Main Scenarios Scene");
	}

	public void LoadSUVShowcaseDemoScene()
	{
		Cursor.lockState = CursorLockMode.Locked;
		SceneManager.LoadScene("SUV Showcase Scene");
	}
}
