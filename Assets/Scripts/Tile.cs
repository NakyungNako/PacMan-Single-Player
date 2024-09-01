using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Tile : MonoBehaviour {

	public bool isPortal;

	public bool isPellet;
	public bool isSuperPellet;
	public bool didConsume;

	public bool isGhostHouseEntrance;
	public bool isGhostHouse;

	public bool isKey;

	public bool isBonusItem;
	public int pointValue;
	public bool isPowerUp;
	public bool isPowerLoss;
	public GameObject portalReceiver;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
