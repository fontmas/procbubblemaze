/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Map {

	[RequireComponent(typeof(MapGenerator))]
	public class PlaceObjectMap : MonoBehaviour {
		public bool mutatePatterns = true;
		public bool loadDificultLevel = true;
		public string PlayerPrefsLevelDificultKey = "LevelDifficultyPlacement";
		public ObjectPlaceableOnMap[] listObjects = new ObjectPlaceableOnMap[1];
		private float loadedDificultLevel = 1.0f;
		private int watermarkIndexListObjects = 0;

		public IEnumerator Start() {
			watermarkIndexListObjects = listObjects.Length;
			yield return StartCoroutine( RefreshPlaceObjectMap() );
		}

		public IEnumerator RefreshPlaceObjectMap() {
			loadedDificultLevel = PlayerPrefs.GetFloat (PlayerPrefsLevelDificultKey,loadedDificultLevel);
			cleanListObjectsFurtherThanWatermark ();
			Breed ();
			ApplyLevelAjustment ();
			UpdatePlaceObjectMap ();
			yield return 0;
		}

		private void cleanListObjectsFurtherThanWatermark ()
		{
			List<ObjectPlaceableOnMap> newList = new List<ObjectPlaceableOnMap> ();
			for (int i = 0; i < watermarkIndexListObjects ; i++) {
				newList.Add(listObjects[i]);
			}

			if(newList.Count > 0){
				listObjects = newList.ToArray();
			}
		}

		public void ApplyLevelAjustment() {
			foreach (ObjectPlaceableOnMap obj in listObjects) {
				obj.AdjustLevelDificult (loadedDificultLevel);
			}
		}
		
		public void UpdatePlaceObjectMap() {
			foreach (ObjectPlaceableOnMap obj in listObjects) {
				obj.UpdateLines ();
			}
		}

		void Breed ()
		{
			List<ObjectPlaceableOnMap> newList = new List<ObjectPlaceableOnMap> ();
			for(int i = 0; i < listObjects.Length;i++) {
				newList.Add(listObjects[i]);
				if(listObjects[i].canBreed) {
					for(int j = 0; j < listObjects.Length;j++) {
						if(listObjects[j].canBreed && j != i) {
							ObjectPlaceableOnMap newBreed = new ObjectPlaceableOnMap(listObjects[j]);
							newBreed.textPlaceMatrix = listObjects[i].textPlaceMatrix;
							newBreed.deltaPosition = listObjects[i].deltaPosition;
							newBreed.deltaRotation = listObjects[i].deltaRotation;
							newBreed.AmountMax = listObjects[i].AmountMax;
							newBreed.AmountMin = listObjects[i].AmountMin;
							newList.Add(newBreed);
						}
					}
				}
			}
			if(newList.Count > 0){
				listObjects = newList.ToArray();
			}
		}
	}

	[System.Serializable]
	public class LineIDsToPlace {
		[HideInInspector]
		public List<int> line = new List<int>();

		public LineIDsToPlace(string text = "0,0,0") {
			string[] tempS = text.Split (',');
			line.Clear ();
			foreach (string s in tempS) {
				line.Add(int.Parse(s));
			}
		}

		public void Flip(System.Random rand) {
			if (rand.Next (0, 1000) < 500) {
				List<int> tmp = new List<int> (line.Count);
				for (int i = line.Count-1; i >= 0; i--) {
					tmp.Add (line [i]);
				}
				line = tmp;
			}
		}
		public void Invert() {
			for (int i = 0; i < line.Count; i++) {
				line[i] = line[i]==1?0:1;
			}
		}
		

	}

	[System.Serializable]
	public class ObjectPlaceableOnMap 
	{
		public int ID = 2;
		public bool useFixAmount;
		public int amountFix;
		[Range(0,100)]
		public int AmountMin = 1;
		[Range(0,100)]
		public int AmountMax = 1;
		public GameObject objectPrefab = null;
		public bool DontUseSeededMapRandom = false;
		public bool usePlaceBehaviour = false;
		public PlaceBehaviour placebehaviour;
		public Vector3 deltaPosition = Vector3.zero;
		public Vector3 deltaRotation = Vector3.zero;
		public bool canMutateFlip = false;
		public bool canBreed = false;
		public float loadedDificultLevel = 0.0f;

		[Multiline()]
		public string textPlaceMatrix = "0,0,0\n0,0,0\n0,0,0";

		[HideInInspector]
		public List<LineIDsToPlace> patternToPlace = new List<LineIDsToPlace>();

		private int GetAmountMin() {
			return AmountMin + (int)((AmountMax-AmountMin) * (loadedDificultLevel));
		}

		private int GetAmountMax() {
			return AmountMax - (int)((AmountMax-AmountMin) * (1.0f-loadedDificultLevel));
		}

		public void AdjustLevelDificult (float loadedDificultLevel)
		{
			this.loadedDificultLevel = loadedDificultLevel;
		}

		public ObjectPlaceableOnMap(ObjectPlaceableOnMap other = null) {
			if (other != null) {
				this.ID = other.ID;
				this.AmountMin = other.AmountMin;
				this.AmountMax = other.AmountMax;
				this.objectPrefab = other.objectPrefab;
				this.DontUseSeededMapRandom = other.DontUseSeededMapRandom;
				this.usePlaceBehaviour = other.usePlaceBehaviour;
				this.placebehaviour = other.placebehaviour;
				this.deltaPosition = other.deltaPosition;
				this.deltaRotation = other.deltaRotation;
				this.canMutateFlip = other.canMutateFlip;
				this.canBreed = other.canBreed;
				this.loadedDificultLevel = other.loadedDificultLevel;
			}
		}

		public void UpdateLines() {
			string[] tempS = textPlaceMatrix.Split ('\n');
			patternToPlace.Clear();
			foreach (string s in tempS) {
				patternToPlace.Add(new LineIDsToPlace(s));
			}
		}

		public int GetAmount(System.Random rand, MapGenerator mapGen) {
			if (useFixAmount)
				return amountFix;
			return (int)((rand.Next (GetAmountMin(),GetAmountMax())*mapGen.GetMapSize())/100.0f);
		}

		public void Mutate(System.Random rand) {
			if(canMutateFlip) {
				// Try a chance to mutate
				if(rand.Next(0,1000) < 500) {
					Flip(rand);
				}
			}
		}
		
		public void Invert() {
			for (int i = 0; i < patternToPlace.Count; i++) {
				patternToPlace[i].Invert();
			}
		}

		public void Flip(System.Random rand) {
			List<LineIDsToPlace> tmp = new List<LineIDsToPlace> ();
			for (int i = patternToPlace.Count-1; i >= 0; i--) {
				patternToPlace[i].Flip(rand);
				tmp.Add(patternToPlace[i]);
			}
			patternToPlace = tmp;
		}
	}
}