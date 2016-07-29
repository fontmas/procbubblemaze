/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;

public class MutateTexture : MutateBehaviour {

	public Texture[] listTextures = null;
	public MeshRenderer targetMeshRender = null;

	void Start() {
		// Try to get this gameObject Meshrender
		if (targetMeshRender == null) targetMeshRender = gameObject.GetComponent<MeshRenderer> ();
	}

	public override void  DoMutate () {
		if (listTextures == null)
			return;
		if (targetMeshRender == null)
			return;
		targetMeshRender.material.mainTexture = listTextures [Random.Range (0, listTextures.Length)];
	}
}
