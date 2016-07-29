/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;

public class BubbleEnergy : MonoBehaviour {

	public int maxEnergy = 100;
	public int damage = 25;
	public float maxBubbleSize = 3.0f;
	public float minBubbleSize = 1.5f;
	private int energy = 0;

	private Vector3 originalPosition = Vector3.zero;
	
	private GameManager gm;
	public string EnemyTag = "Enemy";
	public ParticleSystem myEffects;

	void Start () {
		originalPosition = transform.parent.position;
		if(myEffects !=null) myEffects = GetComponent<ParticleSystem> ();
		energy = maxEnergy;
		gm = FindObjectOfType<GameManager> ();
		CalcBubbleSize ();
	}

	float CalcBubbleSize ()
	{
		return minBubbleSize + ((maxBubbleSize - minBubbleSize) * ((energy*1.0f) / (maxEnergy*1.0f)));
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals (EnemyTag)) {
			energy -= damage;
			gm.IncrementHits();
			// Player died
			if(energy <= 0) {
				transform.parent.position = originalPosition;
				energy = maxEnergy;
				if(gm!=null) gm.DecrementLife();
			}
			transform.localScale = new Vector3 (CalcBubbleSize (), CalcBubbleSize (), transform.localScale.z);
			//if(myEffects !=null) myEffects.enableEmission = true;
		}
	}

}
