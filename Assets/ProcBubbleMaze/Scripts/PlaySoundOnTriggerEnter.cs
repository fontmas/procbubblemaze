/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class PlaySoundOnTriggerEnter : MonoBehaviour {
	public bool playOnTrigger = true;
	public bool playOnStart = true;
	public AudioClip[] listSoundClip = null;
    public string[] tagsOnCollide = null;
    
	void Start() {
		if(playOnStart) ApplySound ();
	}

	public virtual void OnTriggerEnter2D(Collider2D other) {
		if (CheckCollideTag (other.gameObject) && playOnTrigger) {
			ApplySound ();
		}
	}

	public void ApplySound ()
	{
		if (listSoundClip == null)
			return;
//		if(GetComponent<AudioSource> ().isPlaying == false) {
			GetComponent<AudioSource> ().clip = listSoundClip[Random.Range(0,listSoundClip.Length)];
			GetComponent<AudioSource> ().Play ();
//		}
	}

	public virtual void OnTriggerEnter(Collider other) {
		if (CheckCollideTag (other.gameObject) && playOnTrigger) {
			ApplySound ();
		}
	}    
    
    private bool CheckCollideTag(GameObject other) {
        if(tagsOnCollide == null) return false;
        foreach( string tag in tagsOnCollide ) {
            if(other.tag.Equals(tag)) {
                return true;
            }
        }
        return false;
    }
}
