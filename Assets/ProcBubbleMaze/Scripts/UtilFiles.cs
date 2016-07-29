using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UtilFiles {
	public static string[] fileContent;
	public static Texture textureContent;
	public static AudioClip audioClipContent;

	public static string rootURL = "file:///";


	public static void SaveWavGoogleTTSFile(string words = "teste") {

	}

	public static string GetDataPath ()
	{
		string resultDataPath = ((Application.platform != RuntimePlatform.Android) ? Application.dataPath : Application.persistentDataPath);
		if(Application.platform == RuntimePlatform.OSXPlayer)  resultDataPath += "/../../";
		if(Application.platform == RuntimePlatform.WindowsPlayer)  resultDataPath += "/../";
		if(Application.platform == RuntimePlatform.LinuxPlayer)  resultDataPath += "/../";
		return resultDataPath;
	}
	
	public static void DebugDataPath(UnityEngine.UI.Text text ) {
		text.text = GetDataPath ();
	}
	
	public static IEnumerator ReadExternalAudios (string prefixFileName, string relativePath = "/Config/Sounds/")
	{
		List<string> listResult = new List<string> ();
		int counterAudios = 1;
		WWW www = new WWW (rootURL + GetDataPath () + relativePath + prefixFileName + counterAudios + ".wav");

		while (!www.isDone) {
			yield return 0;
		}

		while(www.error == null) {
			listResult.Add (www.url);
			counterAudios++;
			www = new WWW (rootURL + GetDataPath () + relativePath + prefixFileName + counterAudios + ".wav");
			while (!www.isDone) {
				yield return 0;
			}
		}
		fileContent = listResult.ToArray ();
	}
	
	public static IEnumerator ReadExternalTextAsset(string file,char splitChar, string relativePath = "/Config/") {
		WWW www = new WWW(rootURL + GetDataPath () + relativePath + file);
		while(!www.isDone) {
			yield return 0;
		}
		fileContent = www.text.Split(splitChar);
	}
	
	public static IEnumerator LoadAnAudio(string url, AudioType audioType = AudioType.WAV, bool flag3D=false, bool flagStream=false) {
		WWW www = new WWW(url);
		
		while(!www.isDone) {
			yield return 0;
		}
		audioClipContent = www.GetAudioClip (flag3D, flagStream, audioType);
		
	}
	
	public static void WriteToFile(string strText,string baseDirectory,string fileName) {
		if (System.IO.Directory.Exists (baseDirectory) == false) {
			System.IO.Directory.CreateDirectory(baseDirectory);
		}
		System.IO.File.WriteAllText (baseDirectory + fileName, strText);
	}
	
	public static IEnumerator LoadAnImage(string imageName,string relativePath = "/Config/Images/") {
		WWW www = new WWW(rootURL + GetDataPath () + relativePath + imageName);
		
		while(!www.isDone) {
			yield return 0;
		}	
		
		if (www.texture != null) {
			textureContent = www.texture;
		}
	}
}
