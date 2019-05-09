using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteGroundTextures : MonoBehaviour {
	void OnApplicationQuit()
   {
	   Terrain terrain = this.GetComponent<Terrain>();
     terrain.terrainData.splatPrototypes = null;
   }
}
