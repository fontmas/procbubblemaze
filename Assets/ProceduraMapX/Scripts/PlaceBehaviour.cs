/**
 * Created by Mario Madureira Fontes 
 * Procedural Game Jam 2015
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils.Map
{
	public abstract class PlaceBehaviour : MonoBehaviour	{

		virtual public Coord ChoosePlacement(MapGenerator mapGenerator, List<Coord> listPossiblePlaces) {
			return new Coord();
		}
	}
}