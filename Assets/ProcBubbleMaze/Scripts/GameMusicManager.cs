using UnityEngine;
using System.Collections;

[System.Serializable]
public class MusicLevelSet {
    public AudioClip audio;
    public string[] levels;
}

[RequireComponent (typeof(AudioSource))]
public class GameMusicManager : MonoBehaviour {
    
    public MusicLevelSet[] listMusics;
    private string lastLevelChoosed = "";
        
    void Start () {
        DontDestroyOnLoad(gameObject);
		CheckLevel ();
    }

	void CheckLevel ()
	{
		
		// Verify if changed the level try to change the music
		if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals (lastLevelChoosed)) {
			lastLevelChoosed = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			// Prevents playing not configurated backgrounds sounds
			GetComponent<AudioSource>().mute = true;
			foreach (MusicLevelSet music in listMusics) {
				foreach (string level in music.levels) {
					if(lastLevelChoosed.Equals (level)) {
						GetComponent<AudioSource>().mute = false;
					}
					if (lastLevelChoosed.Equals (level) && !music.audio.Equals (GetComponent<AudioSource> ().clip)) {
						GetComponent<AudioSource> ().Stop ();
						GetComponent<AudioSource> ().clip = music.audio;
						GetComponent<AudioSource> ().Play ();
						return;
					}
				}
			}
		}
	}    
    
     void OnLevelWasLoaded(int levelIntID) {
        
        CheckLevel ();
    }
}
