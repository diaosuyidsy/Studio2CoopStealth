using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

  
  private float fps;
  private float frames;

  private GUIStyle guiStyleHeader = new GUIStyle();

  void Start()
  {
    guiStyleHeader.fontSize = 14;
    guiStyleHeader.normal.textColor = new Color(1, 1, 1);
    InvokeRepeating("UpdateFPS", 0, 1);
  }

  void UpdateFPS()
  {
    fps = frames;
    frames = 0;
  }

  private void OnGUI()
  {
    GUI.Label(new Rect(1, 1, 30, 30), "" + (int)fps, guiStyleHeader);
  }

  void Update()
  {
    ++frames;
  }
}
