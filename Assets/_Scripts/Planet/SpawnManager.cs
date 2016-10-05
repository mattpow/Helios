using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
	public static bool SpawnPlanetsIsPaused;
	private float planetSpawnYPosition;
	private float planetSpawnXPosition;
	private Object[] planets;
	private int planetToSpawn;
	private bool peaceSpawn;
	public static int peaceSpawnCount;
	private float playerHeightCheckpoint;
	private float YPositionMin;
	public static bool isFirstSpawn;

	// Use this for initialization
	void Start ()
	{
		isFirstSpawn = true;
		peaceSpawn = true;
		peaceSpawnCount = 0;
		planets = Resources.LoadAll ("Planets");
		playerHeightCheckpoint = 3750;
		YPositionMin = playerHeightCheckpoint + 1750;
		SpawnNewAsteroids ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (PlayerController.currentPos >= playerHeightCheckpoint) {
			SpawnNewAsteroids ();
			playerHeightCheckpoint += 2500;
		}
	}

	void SpawnNewAsteroids ()
	{
		int firstSpawnHeight = 3000;
		for (int i = 0; i < 5; i++) {
			if (!peaceSpawn) {
				planetSpawnXPosition = Random.Range (100f, (GameScreenManager.dynamicMiddleOfScreen * 2) - 150f);
			} else {
				float value = Random.value;
				if (value < .5f) {
					planetSpawnXPosition = Random.Range (100f, GameScreenManager.dynamicMiddleOfScreen - 100f);
				} else {
					planetSpawnXPosition = Random.Range (690f, (GameScreenManager.dynamicMiddleOfScreen * 2) - 100f);
				}
				peaceSpawnCount++;
				if (peaceSpawnCount >= 5) {
					peaceSpawn = false;
				}
			}

			if (isFirstSpawn) {
				planetSpawnYPosition = Random.Range (PlayerController.currentPos + firstSpawnHeight, PlayerController.currentPos + firstSpawnHeight + 200);
				firstSpawnHeight += 500;
			} else {
				planetSpawnYPosition = Random.Range (YPositionMin, YPositionMin + 200);
				YPositionMin += 500;
			}
			float randomRotation = Random.Range (0, 361);
			planetToSpawn = Random.Range (0, planets.Length);
			Object asteroid = Instantiate (planets [planetToSpawn], new Vector3 (planetSpawnXPosition, planetSpawnYPosition, 0), Quaternion.Euler (new Vector3 (0, 0, randomRotation)));
			asteroid.name = planets [planetToSpawn].name;
		}
		if (isFirstSpawn) {
			isFirstSpawn = false;
		}
	}
}
