using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DestroyIt
{
	/// <summary>
	/// This script pre-loads materials so you don't get the "blue flash" when an object is instantiated with a material the camera hasn't seen yet.
	/// This script executes every time a file is added/modified in the Unity editor.
	/// </summary>
	[ExecuteInEditMode]
	public class MaterialPreloader : MonoBehaviour
	{
		public List<DamageMaterial> materials;
	    private List<Texture2D> detailMasks; 

		private MaterialPreloader() { }
		private static MaterialPreloader _instance;
	    private bool isInitialized;

		// This is the public reference that other classes will use
		public static MaterialPreloader Instance
		{
			get
			{
				// If _instance hasn't been set yet, we grab it from the scene.
				// This will only happen the first time this reference is used.
			    if (_instance == null)
			        CreateInstance();

			    if (!_instance.isInitialized)
			        _instance.Start();

				return _instance;
			}
		}

	    private static void CreateInstance()
	    {
            MaterialPreloader[] matPreloaders = FindObjectsOfType<MaterialPreloader>();
            if (matPreloaders.Length > 1)
                Debug.LogError("DestroyIt: Multiple MaterialPreloader scripts found in scene. There can be only one.");
            if (matPreloaders.Length == 0)
                Debug.LogError("DestroyIt: MaterialPreloader script not found in scene. This is required for DestroyIt to work properly.");

            _instance = matPreloaders[0];
	    }

		void Start()
		{
            if (isInitialized) return;

            if (Application.isPlaying)
                ReloadMaterials();

		    isInitialized = true;
		}
		
		void Update()
		{
			if (!Application.isPlaying)
				ReloadMaterials();
		}

	    private void ReloadMaterials()
		{
			List<Material> sourceMats = new List<Material>();
			
            sourceMats.AddRange(Resources.LoadAll<Material>("Material_Preload"));
			detailMasks = Resources.LoadAll<Texture2D>("Material_Preload").ToList();

            // Create damage levels of all materials in \Resources\Material_Preload folders.
            materials = new List<DamageMaterial>();
            foreach (Material sourceMat in sourceMats)
            {
                // Make sure the material is a Unity 5 standard shader material with a Detail Mask - otherwise, don't add it to the list.
	            if (!sourceMat.HasProperty("_DetailMask"))
	            {
	            	Debug.Log("No detail mask found for material: " + sourceMat.name);
                    continue;
	            }
	            
                List<Material> damageMats = new List<Material>();
                for (int i = 0; i < detailMasks.Count; i++)
                {
                    Material damageMat = Instantiate(sourceMat);
                    damageMat.name = String.Format("{0}_D{1}", sourceMat.name, i);
                    damageMat.SetTexture("_DetailMask", detailMasks[i]);
                    damageMats.Add(damageMat);
                }
                materials.Add(new DamageMaterial {name = sourceMat.name, damageMaterials = damageMats});
            }
		}

        public Material GetDamagedMaterial(Material sourceMat, DamageLevel damageLevel)
        {
            if (sourceMat == null) return null;
            string sourceMatName = Regex.Replace(sourceMat.name, "_D[0-9]*$", "");
            foreach (DamageMaterial mat in materials)
            {
                if (mat.name == sourceMatName)
                    return mat.damageMaterials[damageLevel.visibleDamageLevel];
            }
            return sourceMat;
        }

	    public Material GetDestroyedMaterial(Material sourceMat, Destructible destObj)
	    {
            if (destObj.damageLevels != null && destObj.damageLevels.Count > 0)
	            return GetDamagedMaterial(sourceMat, destObj.damageLevels[destObj.damageLevels.Count - 1]);

            return sourceMat;
	    }
	};
}