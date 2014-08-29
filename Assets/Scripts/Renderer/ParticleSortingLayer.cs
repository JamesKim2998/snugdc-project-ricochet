using UnityEngine;
using System.Collections;

public class ParticleSortingLayer : MonoBehaviour {

	public string sortingLayer = "Foreground";

	void Start () 
	{
		particleSystem.renderer.sortingLayerName = sortingLayer;
	}
	
}
