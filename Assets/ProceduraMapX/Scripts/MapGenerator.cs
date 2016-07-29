/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 * 
 * This file was inspired on the ideas Unity Tutorial:
 * http://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Utils.Map {
	
	/**
	 * Legend:
	 * X,Y,Z = axis
	 * w = width
	 * h = height
	 * d = depth
	 */ 
	public enum MapAxisOrientation {XwYhZd=1,XwYdZh=2,XdYwZh=3,XhYwZd=4,XhYdZw=5,XdYhZw=6};
	
	/**
	 * MapGenerator - Generate a random maze based on configurations
	 */
	[RequireComponent(typeof(MeshGenerator))]
	public class MapGenerator : MonoBehaviour {
		public MapAxisOrientation mapOrientation = MapAxisOrientation.XwYhZd;
		[Range(40,180)]
		public int MinMapSize = 40;
		[Range(40,180)]
		public int MaxMapSize = 180;
		public float defaultDifficulty = 0.5f;
		public string PlayerPrefsLevelDificultKey = "LevelDifficultyPlacement";
		private int mapSizeWH = 100;
		private int originalMapMin;
		private int originalMapMax;
		private int width;
		private int height;
		
		[Range(1,180)]
		public int borderSize = 1;
		
		public string seed;
		public bool useRandomSeed;
		public bool forceConnectMainRoom = false;
		public bool RandomMainRoom = false;

		[Range(25,75)]
		public int randomFillPercent = 50;
		
		[Range(1,90)]
		public int wallThresholdSize = 50;
		[Range(1,90)]
		public int roomThresholdSize = 50;
		
		[Range(5,90)]
		public int PassageSize = 5;
		
		[Range(1,5)]
		public int smothingFactor = 5;
		
		[Range(1,100)]
		public int tileScale = 1;

		public Text buildingTextGUI;

		private int[,] map;
		
		private PlaceObjectMap place = null;

		IEnumerator Start() {
			originalMapMax = MaxMapSize;
			originalMapMin = MinMapSize;
			place = GetComponent<PlaceObjectMap> ();
			yield return StartCoroutine(GenerateMap());
		}

		public void CleanMap() {
			Transform[] objs = GetComponentsInChildren<Transform>() as Transform[];
			foreach(Transform obj in objs) {
				if(obj.gameObject.Equals(gameObject) == false) Destroy(obj.gameObject);
			}
		}

		/* Event to generate a new map with a "R" keyboard event
		void Update() {
			if (Input.GetKeyDown(KeyCode.R)) {
				GenerateMap();
			}
		}*/
		
		public IEnumerator GenerateMap() {
			{
				if (buildingTextGUI != null) {
					buildingTextGUI.gameObject.SetActive (true);
				}
				yield return new WaitForSeconds (0.02f);
			}
			place.ApplyLevelAjustment ();
			CleanMap ();
			MaxMapSize = originalMapMax - (int)((originalMapMax-originalMapMin) * (1-PlayerPrefs.GetFloat (PlayerPrefsLevelDificultKey)));
			MinMapSize = originalMapMin + (int)((originalMapMax-originalMapMin) * (PlayerPrefs.GetFloat (PlayerPrefsLevelDificultKey)));
			mapSizeWH = GenerateRandom(useRandomSeed).Next(MinMapSize,MaxMapSize);
			width = height = mapSizeWH;
			map = new int[width,height];

			place.UpdatePlaceObjectMap ();

			RandomFillMap();
			
			for (int i = 0; i < smothingFactor; i ++) {
				SmoothMap();
			}
			
			ProcessMap ();

			GenerateBorder ();
			
			MeshGenerator meshGen = GetComponent<MeshGenerator> ();
			meshGen.GenerateMesh (map, tileScale);
			
			ProcessObjectsOnMap (meshGen.wallHeight);
			
			if (buildingTextGUI != null) {
				buildingTextGUI.gameObject.SetActive (false);
			}
		}
		
		void GenerateBorder ()
		{
			int[,] borderedMap = new int[width + borderSize * 2,height + borderSize * 2];
			for (int x = 0; x < borderedMap.GetLength (0); x++) {
				for (int y = 0; y < borderedMap.GetLength (1); y++) {
					if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
						borderedMap [x, y] = map [x - borderSize, y - borderSize];
					}
					else {
						borderedMap [x, y] = 1;
					}
				}
			}
			map = borderedMap;
		}
		
		void ProcessMap() {
			List<List<Coord>> wallRegions = GetRegions (1);
			
			foreach (List<Coord> wallRegion in wallRegions) {
				if (wallRegion.Count < wallThresholdSize) {
					foreach (Coord tile in wallRegion) {
						map[tile.tileX,tile.tileY] = 0;
					}
				}
			}
			
			List<List<Coord>> roomRegions = GetRegions (0);
			List<Room> survivingRooms = new List<Room> ();
			
			foreach (List<Coord> roomRegion in roomRegions) {
				if (roomRegion.Count < roomThresholdSize) {
					foreach (Coord tile in roomRegion) {
						map[tile.tileX,tile.tileY] = 1;
					}
				}
				else {
					survivingRooms.Add(new Room(roomRegion, map));
				}
			}
			
			survivingRooms.Sort();
			System.Random pseudoRandom = GenerateRandom(useRandomSeed);
			int choosedMainRoom = pseudoRandom.Next (0, survivingRooms.Count);
			survivingRooms[RandomMainRoom?choosedMainRoom:0].isMainRoom = true;
			survivingRooms[RandomMainRoom?choosedMainRoom:0].isAccessibleFromMainRoom = true;
			
			ConnectClosestRooms (survivingRooms,forceConnectMainRoom);
		}
		
		void ProcessObjectsOnMap (float wallHeight)
		{
			int amount;
			if (place != null) {
				foreach (ObjectPlaceableOnMap obj in place.listObjects) {
					amount = obj.GetAmount(GenerateRandom(obj.DontUseSeededMapRandom?true:useRandomSeed),this);
					for (int count = 1; count <= amount; count++) {
						// Try to mutate the object
						obj.Mutate(GenerateRandom(obj.DontUseSeededMapRandom?true:useRandomSeed));
						// Try to find the objects
						List<Coord> listCoords = FindPlaceObjectInsideRoom (ref obj.patternToPlace);
						// Check if was possible to find a positions
						if(listCoords != null) {
							Coord c;
							if(obj.usePlaceBehaviour) {
								if(obj.placebehaviour != null) {
									c = obj.placebehaviour.ChoosePlacement(this,listCoords);
								} else {
									c = GetRandomPlace(ref listCoords,obj.DontUseSeededMapRandom?true:useRandomSeed);
								}
							} else {
								c = GetRandomPlace(ref listCoords,obj.DontUseSeededMapRandom?true:useRandomSeed);
							}
							if (c != null) {
								GameObject tempObject = Instantiate (obj.objectPrefab) as GameObject;
								tempObject.name = obj.objectPrefab.name + "#" + count;
								tempObject.transform.parent = transform;
								map [c.tileX, c.tileY] = obj.ID;
								tempObject.transform.position = CoordToWorldPoint (c.tileX, c.tileY, transform.position.z + wallHeight, mapOrientation);
								tempObject.transform.position += obj.deltaPosition;
								tempObject.transform.Rotate(obj.deltaRotation);
								MutateBehaviour[] mutations = tempObject.GetComponents<MutateBehaviour>();
								foreach(MutateBehaviour m in mutations) {
									m.DoMutate();
								}
							}
						} else {
							// There is no place the object so, forget it!
						}
					}
				}
			}
		}

		Coord GetRandomPlace (ref List<Coord> listCoords,bool useRealRandom)
		{
			System.Random pseudoRandom = GenerateRandom(useRealRandom,listCoords.Count);//new System.Random((seed+).GetHashCode());
			return listCoords[pseudoRandom.Next(0,listCoords.Count)];

		}
		
		void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {
			
			List<Room> roomListA = new List<Room> ();
			List<Room> roomListB = new List<Room> ();
			
			if (forceAccessibilityFromMainRoom) {
				foreach (Room room in allRooms) {
					if (room.isAccessibleFromMainRoom) {
						roomListB.Add (room);
					} else {
						roomListA.Add (room);
					}
				}
			} else {
				roomListA = allRooms;
				roomListB = allRooms;
			}
			
			int bestDistance = 0;
			Coord bestTileA = new Coord ();
			Coord bestTileB = new Coord ();
			Room bestRoomA = new Room ();
			Room bestRoomB = new Room ();
			bool possibleConnectionFound = false;
			
			foreach (Room roomA in roomListA) {
				if (!forceAccessibilityFromMainRoom) {
					possibleConnectionFound = false;
					if (roomA.connectedRooms.Count > 0) {
						continue;
					}
				}
				
				foreach (Room roomB in roomListB) {
					if (roomA == roomB || roomA.IsConnected(roomB)) {
						continue;
					}
					
					for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA ++) {
						for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB ++) {
							Coord tileA = roomA.edgeTiles[tileIndexA];
							Coord tileB = roomB.edgeTiles[tileIndexB];
							int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX-tileB.tileX,2) + Mathf.Pow (tileA.tileY-tileB.tileY,2));
							
							if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
								bestDistance = distanceBetweenRooms;
								possibleConnectionFound = true;
								bestTileA = tileA;
								bestTileB = tileB;
								bestRoomA = roomA;
								bestRoomB = roomB;
							}
						}
					}
				}
				if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
					CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				}
			}
			
			if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				ConnectClosestRooms(allRooms, true);
			}
			
			if (!forceAccessibilityFromMainRoom) {
				ConnectClosestRooms(allRooms, true);
			}
		}
		
		void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
			roomA.ConnectRoom(roomB);
			//	Debug.DrawLine (CoordToWorldPoint (tileA.tileX,tileA.tileY), CoordToWorldPoint (tileB.tileX,tileB.tileY), Color.green, 100);
			
			List<Coord> line = GetLine (tileA, tileB);
			foreach (Coord c in line) {
				DrawCircle (c, PassageSize);
			}
		}

		public Coord FindFirstCoordByID (int myID)
		{
			Coord returnValue = new Coord ();
			bool found = false;
			for (int x = 0; x < map.GetLength(0); x++) {
				for (int y = 0; y < map.GetLength(1); y++) {
					if(map[x,y] == myID) {
						returnValue.tileX = x;
						returnValue.tileY = y;
						found = true;
						break;
					}
				}
				if(found) break;
			}
			return returnValue;
		}
		
		void DrawCircle(Coord c, int r) {
			for (int x = -r; x <= r; x++) {
				for (int y = -r; y <= r; y++) {
					if (x*x + y*y <= r*r) {
						int drawX = c.tileX + x;
						int drawY = c.tileY + y;
						if (IsInMapRange(drawX, drawY)) {
							map[drawX,drawY] = 0;
						}
					}
				}
			}
		}
		
		List<Coord> GetLine(Coord from, Coord to) {
			List<Coord> line = new List<Coord> ();
			
			int x = from.tileX;
			int y = from.tileY;
			
			int dx = to.tileX - from.tileX;
			int dy = to.tileY - from.tileY;
			
			bool inverted = false;
			int step = Math.Sign (dx);
			int gradientStep = Math.Sign (dy);
			
			int longest = Mathf.Abs (dx);
			int shortest = Mathf.Abs (dy);
			
			if (longest < shortest) {
				inverted = true;
				longest = Mathf.Abs(dy);
				shortest = Mathf.Abs(dx);
				
				step = Math.Sign (dy);
				gradientStep = Math.Sign (dx);
			}
			
			int gradientAccumulation = longest / 2;
			for (int i =0; i < longest; i ++) {
				line.Add(new Coord(x,y));
				
				if (inverted) {
					y += step;
				}
				else {
					x += step;
				}
				
				gradientAccumulation += shortest;
				if (gradientAccumulation >= longest) {
					if (inverted) {
						x += gradientStep;
					}
					else {
						y += gradientStep;
					}
					gradientAccumulation -= longest;
				}
			}
			
			return line;
		}
		
		public List<Coord> FindPlaceObjectInsideRoom(ref List<LineIDsToPlace> linesIds) {
			List<Coord> listPossiblePoints = new List<Coord>();
			bool testValidPosition = true;
			for (int y = 0; y< height; y++) {
				for (int x = 0; x < width; x++) {
					// Check against the linesIds grid values if this position on the map is valid.
					testValidPosition = true; // Start with possible, if something goes wrong change to false
					int startXPostion = 0;
					int startYPostion = y -(linesIds.Count/2);
					if(startYPostion >= 0 && startYPostion < height) {
						for(int yID = 0; yID < linesIds.Count; yID++) {
							startXPostion = x -(linesIds[yID].line.Count/2);
							if(startXPostion >= 0 && startXPostion < width) {
								for(int xID = 0; xID < linesIds[yID].line.Count; xID++) {
									if (map [(startXPostion + xID), (startYPostion + yID)] != (linesIds [yID].line [xID])) {
										testValidPosition = false;
										break;
									}
								}
							} else {
								testValidPosition = false;
							}
							if(testValidPosition == false) break;
						}
					} else {
						testValidPosition = false;
					}
					if(testValidPosition == true) {
						listPossiblePoints.Add(new Coord(x,y));
					}
				}
			}
			if(listPossiblePoints.Count > 0) {
				return listPossiblePoints;
			}
			return null;
		}
		
		/**
		 * This function calculate the world positon inside the map
		 */
		private Vector3 CoordToWorldPoint(int x, int y,float depth = 0.0f,MapAxisOrientation axis=MapAxisOrientation.XwYhZd) {
			switch(axis) {
			case MapAxisOrientation.XwYhZd:
				return new Vector3 ((-width / 2 + .5f + x-borderSize)*tileScale, (-height / 2 + .5f + y-borderSize)*tileScale,depth);
			case MapAxisOrientation.XwYdZh:
				return new Vector3 ((-width / 2 + .5f + x-borderSize)*tileScale, depth, (-height / 2 + .5f + y-borderSize)*tileScale);
			case MapAxisOrientation.XdYwZh:
				return new Vector3 (depth, (-width / 2 + .5f + x-borderSize)*tileScale, (-height / 2 + .5f + y-borderSize)*tileScale);
			case MapAxisOrientation.XhYwZd:
				return new Vector3 ((-height / 2 + .5f + y-borderSize)*tileScale,(-width / 2 + .5f + x-borderSize)*tileScale,depth);
			case MapAxisOrientation.XhYdZw:
				return new Vector3 ((-height / 2 + .5f + y-borderSize)*tileScale, depth, (-width / 2 + .5f + x-borderSize)*tileScale);
			case MapAxisOrientation.XdYhZw:
			default:
				return new Vector3 (depth, (-height / 2 + .5f + y-borderSize)*tileScale, (-width / 2 + .5f + x-borderSize)*tileScale);
			}
		}
		
		List<List<Coord>> GetRegions(int tileType) {
			List<List<Coord>> regions = new List<List<Coord>> ();
			int[,] mapFlags = new int[width,height];
			
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
						List<Coord> newRegion = GetRegionTiles(x,y);
						regions.Add(newRegion);
						
						foreach (Coord tile in newRegion) {
							mapFlags[tile.tileX, tile.tileY] = 1;
						}
					}
				}
			}
			
			return regions;
		}
		
		List<Coord> GetRegionTiles(int startX, int startY) {
			List<Coord> tiles = new List<Coord> ();
			int[,] mapFlags = new int[width,height];
			int tileType = map [startX, startY];
			
			Queue<Coord> queue = new Queue<Coord> ();
			queue.Enqueue (new Coord (startX, startY));
			mapFlags [startX, startY] = 1;
			
			while (queue.Count > 0) {
				Coord tile = queue.Dequeue();
				tiles.Add(tile);
				
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
						if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)) {
							if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
								mapFlags[x,y] = 1;
								queue.Enqueue(new Coord(x,y));
							}
						}
					}
				}
			}
			return tiles;
		}
		
		bool IsInMapRange(int x, int y) {
			return x >= 0 && x < width && y >= 0 && y < height;
		}
		
		System.Random GenerateRandom(bool useRealRandom,int extraSeed = 0) {
			int randomNumberSeed = 0;
			if (useRealRandom) {
				randomNumberSeed = (int)Time.time + UnityEngine.Random.Range(int.MinValue,int.MaxValue);
			}
			return new System.Random (seed.GetHashCode () + (randomNumberSeed + extraSeed));
		}

		void RandomFillMap() {

			System.Random pseudoRandom = GenerateRandom(useRandomSeed);
			
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					if (x == 0 || x == width-1 || y == 0 || y == height -1) {
						map[x,y] = 1;
					}
					else {
						map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? 1: 0;
					}
				}
			}
		}
		
		void SmoothMap() {
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					int neighbourWallTiles = GetSurroundingWallCount(x,y);
					
					if (neighbourWallTiles > 4)
						map[x,y] = 1;
					else if (neighbourWallTiles < 4)
						map[x,y] = 0;
					
				}
			}
		}
		
		int GetSurroundingWallCount(int gridX, int gridY) {
			int wallCount = 0;
			for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
				for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
					if (IsInMapRange(neighbourX,neighbourY)) {
						if (neighbourX != gridX || neighbourY != gridY) {
							wallCount += map[neighbourX,neighbourY];
						}
					}
					else {
						wallCount ++;
					}
				}
			}
			
			return wallCount;
		}
		
		public float GetMapSize ()
		{
			return mapSizeWH;
		}
	}
	
	public class Coord {
		public int tileX;
		public int tileY;
		
		public Coord(int x=0, int y=0) {
			tileX = x;
			tileY = y;
		}

		public Vector2 ToVector2() {
			return new Vector2 (tileX, tileY);
		}

		public float DistanceEuclidean(Coord other, bool useTaxiCabe = false) {
			return Vector2.Distance (this.ToVector2 (), other.ToVector2 ());
		}

		public float DistanceTaxiCab(Coord other) {
			return Mathf.Abs (other.tileX - tileX) + Mathf.Abs (other.tileY - tileY);
		}
	}
	
	public class Room : IComparable<Room> {
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;
		
		public Room() {
			
		}
		
		public Room(List<Coord> roomTiles, int[,] map) {
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();
			
			edgeTiles = new List<Coord>();
			foreach (Coord tile in tiles) {
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if (x == tile.tileX || y == tile.tileY) {
							if (map[x,y] == 1) {
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}
		
		public void SetAccessibleFromMainRoom() {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}
		
		public void ConnectRoom(Room otherRoom) {
			if (this.isAccessibleFromMainRoom) {
				otherRoom.SetAccessibleFromMainRoom ();
			} else if (otherRoom.isAccessibleFromMainRoom) {
				this.SetAccessibleFromMainRoom();
			}
			this.connectedRooms.Add (otherRoom);
			otherRoom.connectedRooms.Add (this);
		}
		
		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}
		
		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo (roomSize);
		}
	}
}