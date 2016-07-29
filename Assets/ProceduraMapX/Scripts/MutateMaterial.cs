/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;

public class MutateMaterial : MutateBehaviour
{
	public MeshRenderer meshR;

	public override void DoMutate () {
		HSBColor colorHue =  HSBColor.FromColor(meshR.material.color);
		colorHue.h = Random.Range (0.0f, 1.0f);
		meshR.material.color = HSBColor.ToColor (colorHue);
	}
}

