using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class ClimbLadder : MonoBehaviour
{
    public List<GameObject> Meshes;

    public void SetOutline(bool isOutline)
    {
        if (isOutline)
        {
            foreach (var mesh in Meshes)
            {
                if (mesh.GetComponents<Outline>() != null)
                {
                    mesh.GetComponent<Outline>().EnableOutline();
                }
            }
        }
        else
        {
            foreach (var mesh in Meshes)
            {
                if (mesh.GetComponents<Outline>() != null)
                {
                    mesh.GetComponent<Outline>().DisableOutline();
                }
            }
        }
       
    }
}
