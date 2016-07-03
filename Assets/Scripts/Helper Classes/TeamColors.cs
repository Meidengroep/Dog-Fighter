using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamColors : MonoBehaviour {
	//Give the alternative color (material) arrays, in the same order
	//Materials that are the same for both teams do not have to be listed
	//HINT: Team 1 generally has red colors, Team 2 blue colors
	public Material[] team1Materials;
	public Material[] team2Materials;

	//private IList<Material>

	// Use this for initialization
	void Start () {
		//Convert to ILists. These are not visible in the inspector, but easier to use.
		IList<Material> team1List = new List<Material>(team1Materials);
		IList<Material> team2List = new List<Material>(team2Materials);

		//Check own team
		//Make a list of material NAMES to replace, because direct comparison is hard in Unity :(
		if (TeamHelper.GetTeamNumber(transform.gameObject.layer) == 1) {
			IList<string> matNames = new List<string>();
			foreach (Material mat in team2List) {
				matNames.Add(mat.name);
			}
			ReplaceMaterialsWith(matNames, team1List);

		} else{
			IList<string> matNames = new List<string>();
			foreach (Material mat in team1List) {
				matNames.Add(mat.name);
			}
			ReplaceMaterialsWith(matNames, team2List);
		}

	}

	//Replaces the materials with the names in fromList with the same position Material in toList/
	//The Meshrenderers of all children are checked for Materials.
	//The reason that strings are used for the first list, is that material comparison is hard in Unity.
	public void ReplaceMaterialsWith (IList<string> fromList, IList<Material> toList) {
		//Go through all meshrenderers, that may need their material changed
		IList<MeshRenderer> meshes = GetComponentsInChildren<MeshRenderer>();

		foreach (MeshRenderer mesh in meshes) {
			//Remove from name: " (Instance)"
			string name = mesh.material.name.Substring(0, mesh.material.name.Length-11);

			int index = fromList.IndexOf(name);
			//IndexOf will return -1 if the material is not in fromList
			if (index != -1) {
				//Replace the material with that at the same position in toList
				mesh.material = toList[index];
			}
		}
	}
}
//Debug.Log ("");