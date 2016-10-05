using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour
{
	public static bool SpawnPlanetsIsPaused;
	private float planetSpawnYPosition;
	private float planetSpawnXPosition;
	private float spawnTimer;
	private Object[] planets;
	private int planetToSpawn;
	public static float highestPosition;
	public static bool peaceSpawn;
	public static int peaceSpawnCount;
	// Use this for initialization
	void Start ()
	{
		peaceSpawn = true;
		planetSpawnYPosition = PlayerController.currentPos + 1750;
		peaceSpawnCount = 0;
		planets = Resources.LoadAll ("Planets");
		StartCoroutine (SpawnPlanets ());
		highestPosition = 0;
		spawnTimer = .3f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (PlayerController.isOrbiting) {
			if (PlayerController.currentPos > highestPosition) {
				highestPosition = PlayerController.currentPos;
			}
		}
	}

	IEnumerator SpawnPlanets ()
	{
		while (true) {
			if (!peaceSpawn) {
				planetSpawnXPosition = Random.Range (100f, (GameScreenManager.dynamicMiddleOfScreen * 2) - 100f);
			} else if (peaceSpawn) {
				float value = Random.value;
				if (value < .5f) {
					planetSpawnXPosition = Random.Range (100f, GameScreenManager.dynamicMiddleOfScreen - 140f);
				} else {
					planetSpawnXPosition = Random.Range (680f, (GameScreenManager.dynamicMiddleOfScreen * 2) - 100f);
				}

				peaceSpawnCount++;
				if (peaceSpawnCount >= 5) {
					peaceSpawn = false;
				}
			}
			spawnTimer = Random.Range (.25f, .45f);
			float randomRotation = Random.Range (0, 361);
			yield return new WaitForSeconds (spawnTimer);
			planetSpawnYPosition = PlayerController.currentPos + 1750;
			planetToSpawn = Random.Range (0, planets.Length);
			if (!SpawnPlanetsIsPaused && !PlayerController.isOrbiting && PlayerController.currentPos > highestPosition) {
				Object asteroid = Instantiate (planets [planetToSpawn], new Vector3 (planetSpawnXPosition, planetSpawnYPosition, 0), Quaternion.Euler (new Vector3 (0, 0, randomRotation)));
				asteroid.name = planets [planetToSpawn].name;
			} 
		}
	}
}
