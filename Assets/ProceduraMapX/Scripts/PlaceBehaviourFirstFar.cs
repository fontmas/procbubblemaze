/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils.Map;


namespace Utils.Map {
	public enum DistanceCoordMapToUse {RandomTaxiCabEuclidean,Euclidean,TaxiCab};
	
	public class PlaceBehaviourFirstFar : PlaceBehaviour {
		
		public int farID = 2;
		public DistanceCoordMapToUse typeDistance = DistanceCoordMapToUse.RandomTaxiCabEuclidean;
		private Coord farPoint = new Coord();
		
		public override Coord ChoosePlacement(MapGenerator mapGenerator, List<Coord> myListOfPossiblePlaces) {
			if (myListOfPossiblePlaces.Count > 0) {
				Coord farCoord = mapGenerator.FindFirstCoordByID(farID);
				float taxiCabDistance = GetDistance(myListOfPossiblePlaces[0],farCoord);
				int index = 1;
				farPoint = myListOfPossiblePlaces[0];
				float currentDistance = 0;
				while( index < myListOfPossiblePlaces.Count ) {
					currentDistance = GetDistance(myListOfPossiblePlaces[index],farCoord);
					if(taxiCabDistance < currentDistance ){
						taxiCabDistance = currentDistance;
						farPoint = myListOfPossiblePlaces[index];
					}
					index++;
				}
			}
			return farPoint;
		}
		
		private float GetDistance(Coord from, Coord to) {
			switch (typeDistance) {
			case DistanceCoordMapToUse.Euclidean:
				return from.DistanceEuclidean(to);
			case DistanceCoordMapToUse.TaxiCab:
				return from.DistanceTaxiCab(to);
			case DistanceCoordMapToUse.RandomTaxiCabEuclidean:
			default:
				return Random.Range(0,1000) < 500 ? from.DistanceEuclidean(to) : from.DistanceTaxiCab(to);
			}
		}
	}
	
}