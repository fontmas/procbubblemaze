using UnityEngine;
using UnityEngine.SceneManagement;
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

	void OnEnable()
	{
		//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		CheckLevel ();
	}

}
