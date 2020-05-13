﻿/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 * 
 * This file was inspired on the ideas Unity Tutorial:
 * http://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Map {
	public class MeshGenerator : MonoBehaviour {
		public SquareGrid squareGrid;
		public Material wallsMaterial;
		public Material mapBorderMaterial;
		public Material mapInsideMaterial;
		public Vector3 localRotationMap;
		private GameObject mapInsideWalls = null;
		private GameObject mapBorder = null;
		private GameObject mapInside = null;
		public bool generateInsideMap;
		public bool generateInsideWalls = true;
		public bool generateBorderMap;
		public bool is2D;
		public bool generateMeshCollider = true;
		public UnityEngine.Rendering.ShadowCastingMode shadowModeInside;
		public UnityEngine.Rendering.ShadowCastingMode shadowModeBorder;
		public UnityEngine.Rendering.ShadowCastingMode shadowModeWall;
		public float wallHeight = 5;


		List<Vector3> vertices;
		List<int> triangles;
		
		Dictionary<int,List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>> ();
		List<List<int>> outlines = new List<List<int>> ();
		HashSet<int> checkedVertices = new HashSet<int>();
		
		void GenerateObjectMap (ref GameObject obj, string nameObject,Material mat)
		{
			obj = new GameObject ();
			obj.AddComponent (typeof(MeshFilter));
			obj.AddComponent (typeof(MeshRenderer));
			obj.GetComponent<MeshRenderer> ().material = mat;
			obj.name = nameObject;
			obj.transform.Rotate(localRotationMap);
			obj.transform.parent = gameObject.transform;
		}
		
		void GenerateFloors (int[,] map, float squareSize,GameObject mapObj,Material mapMaterial,int side,UnityEngine.Rendering.ShadowCastingMode shadowMode)
		{
			triangleDictionary.Clear ();
			outlines.Clear ();
			checkedVertices.Clear ();
			squareGrid = new SquareGrid (map, squareSize,side);
			vertices = new List<Vector3> ();
			triangles = new List<int> ();
			for (int x = 0; x < squareGrid.squares.GetLength (0); x++) {
				for (int y = 0; y < squareGrid.squares.GetLength (1); y++) {
					TriangulateSquare (squareGrid.squares [x, y]);
				}
			}
			Mesh mesh = new Mesh ();
			mapObj.GetComponent<MeshFilter> ().mesh = mesh;
			mesh.vertices = vertices.ToArray ();
			mesh.triangles = triangles.ToArray ();
			mesh.RecalculateNormals ();
			Vector2[] uvs = new Vector2[vertices.Count];
			float percentX;
			float percentY;
			for (int i = 0; i < vertices.Count; i++) {
				percentX = Mathf.InverseLerp (-map.GetLength (0) / 2 * squareSize, map.GetLength (0) / 2 * squareSize, vertices [i].x) * (mapMaterial.mainTexture.width/100);
				percentY = Mathf.InverseLerp (-map.GetLength (1) / 2 * squareSize, map.GetLength (1) / 2 * squareSize, vertices [i].z) * (mapMaterial.mainTexture.height/100);
				uvs [i] = new Vector2 (percentX, percentY);
			}
			mesh.uv = uvs;
			mesh.RecalculateBounds ();
//			mesh.Optimize ();
			mapObj.GetComponent<MeshRenderer> ().shadowCastingMode = shadowMode;
		}
		
		public void GenerateMesh(int[,] map, float squareSize) {
			if(generateInsideWalls) GenerateObjectMap (ref mapInsideWalls,"MapWalls",wallsMaterial);
			
			if(generateBorderMap) GenerateObjectMap (ref mapBorder,"MapBorders",mapBorderMaterial);
			if(generateInsideMap) GenerateObjectMap (ref mapInside,"MapInside",mapInsideMaterial);
			
			if(generateBorderMap) GenerateFloors (map, squareSize, mapBorder, mapBorderMaterial, 1,shadowModeBorder);
			if(generateInsideMap) GenerateFloors (map, squareSize, mapInside, mapInsideMaterial, 0,shadowModeInside);
			if(generateInsideMap) mapInside.transform.position = new Vector3 (mapInside.transform.position.x, mapInside.transform.position.y, mapInside.transform.position.z + wallHeight);
			
			CalculateMeshOutlines ();
			
			if (is2D) {
				if (generateMeshCollider) {
					Generate2DColliders ();
				}
			} 
			
			if(generateInsideWalls) {
				CreateWallMesh ();
			}
		}
		
		void CreateWallMesh() {
			
			List<Vector3> wallVertices = new List<Vector3> ();
			List<int> wallTriangles = new List<int> ();

			foreach (List<int> outline in outlines) {
				for (int i = 0; i < outline.Count - 1; i ++) {
					int startIndex = wallVertices.Count;
					wallVertices.Add(vertices[outline[i]]); // left
					wallVertices.Add(vertices[outline[i+1]]); // right
					wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
					wallVertices.Add(vertices[outline[i+1]] - Vector3.up * wallHeight); // bottom right

					wallTriangles.Add(startIndex + 0);
					wallTriangles.Add(startIndex + 1);
					wallTriangles.Add(startIndex + 3);

					wallTriangles.Add(startIndex + 3);
					wallTriangles.Add(startIndex + 2);
					wallTriangles.Add(startIndex + 0);
					
				}
			}
			Mesh mesh = new Mesh ();
			mesh.vertices = wallVertices.ToArray ();
			mesh.triangles = wallTriangles.ToArray ();
			Vector2[] uvs = new Vector2[wallVertices.Count];
			float percentX=0;
			float percentY=0;
			mesh.RecalculateNormals ();
			int countWidth = 0;
			for (int i = 0; i < wallVertices.Count; i=i+4) {
				countWidth++;
				percentX = (1/wallHeight)*(countWidth-1);
				percentY = 0;
				uvs [i+0] = new Vector2 (percentX, percentY);
				percentX = (1/wallHeight)*countWidth;
				percentY = 0;
				uvs [i+1] = new Vector2 (percentX, percentY);
				percentX = (1/wallHeight)*countWidth;
				percentY = 1;
				uvs [i+2] = new Vector2 (percentX, percentY);
				percentX = (1/wallHeight)*(countWidth-1);
				percentY = 1;
				uvs [i+3] = new Vector2 (percentX, percentY);
				if(countWidth >= wallHeight) {
					countWidth = 0;
				}
			}
			mesh.uv = uvs;
			mesh.RecalculateBounds ();
			// mesh.Optimize ();
			mapInsideWalls.GetComponent<MeshFilter>().mesh = mesh;
			
			if (generateMeshCollider) {
				MeshCollider wallCollider = mapInsideWalls.gameObject.AddComponent<MeshCollider> ();
				wallCollider.sharedMesh = mesh;
			}
			mapInsideWalls.GetComponent<MeshRenderer> ().shadowCastingMode = shadowModeWall;
		}
		
		void Generate2DColliders() {
			EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D> ();
			
			for (int i = 0; i < currentColliders.Length; i++) {
				Destroy(currentColliders[i]);
			}
			
			foreach (List<int> outline in outlines) {
				EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
				Vector2[] edgePoints = new Vector2[outline.Count];
				
				for (int i = 0; i < outline.Count; i ++) {
					edgePoints[i] = new Vector2(vertices[outline[i]].x,vertices[outline[i]].z);
				}
				edgeCollider.points = edgePoints;
			}
		}
		
		void TriangulateSquare(Square square) {
			switch (square.configuration) {
			case 0:
				break;
				
				// 1 points:
			case 1:
				MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
				break;
			case 2:
				MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
				break;
			case 4:
				MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
				break;
			case 8:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
				break;
				
				// 2 points:
			case 3:
				MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
				break;
			case 6:
				MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
				break;
			case 9:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
				break;
			case 12:
				MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
				break;
			case 5:
				MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
				break;
			case 10:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
				break;
				
				// 3 point:
			case 7:
				MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
				break;
			case 11:
				MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
				break;
			case 13:
				MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
				break;
			case 14:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
				break;
				
				// 4 point:
			case 15:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
				checkedVertices.Add(square.topLeft.vertexIndex);
				checkedVertices.Add(square.topRight.vertexIndex);
				checkedVertices.Add(square.bottomRight.vertexIndex);
				checkedVertices.Add(square.bottomLeft.vertexIndex);
				break;
			}
			
		}
		
		void MeshFromPoints(params Node[] points) {
			AssignVertices(points);
			
			if (points.Length >= 3)
				CreateTriangle(points[0], points[1], points[2]);
			if (points.Length >= 4)
				CreateTriangle(points[0], points[2], points[3]);
			if (points.Length >= 5) 
				CreateTriangle(points[0], points[3], points[4]);
			if (points.Length >= 6)
				CreateTriangle(points[0], points[4], points[5]);
			
		}
		
		void AssignVertices(Node[] points) {
			for (int i = 0; i < points.Length; i ++) {
				if (points[i].vertexIndex == -1) {
					points[i].vertexIndex = vertices.Count;
					vertices.Add(points[i].position);
				}
			}
		}
		
		void CreateTriangle(Node a, Node b, Node c) {
			triangles.Add(a.vertexIndex);
			triangles.Add(b.vertexIndex);
			triangles.Add(c.vertexIndex);
			
			Triangle triangle = new Triangle (a.vertexIndex, b.vertexIndex, c.vertexIndex);
			AddTriangleToDictionary (triangle.vertexIndexA, triangle);
			AddTriangleToDictionary (triangle.vertexIndexB, triangle);
			AddTriangleToDictionary (triangle.vertexIndexC, triangle);
		}
		
		void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
			if (triangleDictionary.ContainsKey (vertexIndexKey)) {
				triangleDictionary [vertexIndexKey].Add (triangle);
			} else {
				List<Triangle> triangleList = new List<Triangle>();
				triangleList.Add(triangle);
				triangleDictionary.Add(vertexIndexKey, triangleList);
			}
		}
		
		void CalculateMeshOutlines() {
			
			for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex ++) {
				if (!checkedVertices.Contains(vertexIndex)) {
					int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
					if (newOutlineVertex != -1) {
						checkedVertices.Add(vertexIndex);
						List<int> newOutline = new List<int>();
						newOutline.Add(vertexIndex);
						outlines.Add(newOutline);
						FollowOutline(newOutlineVertex, outlines.Count-1);
						outlines[outlines.Count-1].Add(vertexIndex);
					}
				}
			}
		}
		
		void FollowOutline(int vertexIndex, int outlineIndex) {
			outlines [outlineIndex].Add (vertexIndex);
			checkedVertices.Add (vertexIndex);
			int nextVertexIndex = GetConnectedOutlineVertex (vertexIndex);
			
			if (nextVertexIndex != -1) {
				FollowOutline(nextVertexIndex, outlineIndex);
			}
		}
		
		int GetConnectedOutlineVertex(int vertexIndex) {
			List<Triangle> trianglesContainingVertex = triangleDictionary [vertexIndex];
			
			for (int i = 0; i < trianglesContainingVertex.Count; i ++) {
				Triangle triangle = trianglesContainingVertex[i];
				for (int j = 0; j < 3; j ++) {
					int vertexB = triangle[j];
					if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) {
						if (IsOutlineEdge(vertexIndex, vertexB)) {
							return vertexB;
						}
					}
				}
			}
			return -1;
		}
		
		bool IsOutlineEdge(int vertexA, int vertexB) {
			List<Triangle> trianglesContainingVertexA = triangleDictionary [vertexA];
			int sharedTriangleCount = 0;
			
			for (int i = 0; i < trianglesContainingVertexA.Count; i ++) {
				if (trianglesContainingVertexA[i].Contains(vertexB)) {
					sharedTriangleCount ++;
					if (sharedTriangleCount > 1) {
						break;
					}
				}
			}
			return sharedTriangleCount == 1;
		}
		
		struct Triangle {
			public int vertexIndexA;
			public int vertexIndexB;
			public int vertexIndexC;
			int[] vertices;
			
			public Triangle (int a, int b, int c) {
				vertexIndexA = a;
				vertexIndexB = b;
				vertexIndexC = c;
				
				vertices = new int[3];
				vertices[0] = a;
				vertices[1] = b;
				vertices[2] = c;
			}
			
			public int this[int i] {
				get {
					return vertices[i];
				}
			}
			
			
			public bool Contains(int vertexIndex) {
				return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
			}
		}
		
		public class SquareGrid {
			public Square[,] squares;
			
			public SquareGrid(int[,] map, float squareSize,int side) {
				int nodeCountX = map.GetLength(0);
				int nodeCountY = map.GetLength(1);
				float mapWidth = nodeCountX * squareSize;
				float mapHeight = nodeCountY * squareSize;
				
				ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];
				
				for (int x = 0; x < nodeCountX; x ++) {
					for (int y = 0; y < nodeCountY; y ++) {
						Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, 0, -mapHeight/2 + y * squareSize + squareSize/2);
						controlNodes[x,y] = new ControlNode(pos,map[x,y] == side, squareSize);
					}
				}
				
				squares = new Square[nodeCountX -1,nodeCountY -1];
				for (int x = 0; x < nodeCountX-1; x ++) {
					for (int y = 0; y < nodeCountY-1; y ++) {
						squares[x,y] = new Square(controlNodes[x,y+1], controlNodes[x+1,y+1], controlNodes[x+1,y], controlNodes[x,y]);
					}
				}
				
			}
		}
		
		public class Square {
			
			public ControlNode topLeft, topRight, bottomRight, bottomLeft;
			public Node centerTop, centerRight, centerBottom, centerLeft;
			public int configuration;
			
			public Square (ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
				topLeft = _topLeft;
				topRight = _topRight;
				bottomRight = _bottomRight;
				bottomLeft = _bottomLeft;
				
				centerTop = topLeft.right;
				centerRight = bottomRight.above;
				centerBottom = bottomLeft.right;
				centerLeft = bottomLeft.above;
				
				if (topLeft.active)
					configuration += 8;
				if (topRight.active)
					configuration += 4;
				if (bottomRight.active)
					configuration += 2;
				if (bottomLeft.active)
					configuration += 1;
			}
			
		}
		
		public class Node {
			public Vector3 position;
			public int vertexIndex = -1;
			
			public Node(Vector3 _pos) {
				position = _pos;
			}
		}
		
		public class ControlNode : Node {
			
			public bool active;
			public Node above, right;
			
			public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
				active = _active;
				above = new Node(position + Vector3.forward * squareSize/2f);
				right = new Node(position + Vector3.right * squareSize/2f);
			}
			
		}
	}
}