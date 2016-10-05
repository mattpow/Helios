using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using GooglePlayGames;

public class PlayerController : MonoBehaviour
{
	public static bool PlayerMovementIsPaused;
	public static bool buttonIsHeld;
	public static bool isOrbiting;
	public static float currentPos;
	public static List<GameObject> activePlanets;
	public static GameObject explodedPlayerPrefab;
	public static GameObject self;
	private GameObject currentPlanet;
	private GameObject closestPlanet1;
	private GameObject closestPlanet2;
	private float relativeAngle;
	private float relativeDistance;
	private float[] planetDistance;
	private bool closestPlanetCollisionApproach;
	private bool isOnCorrectHeading;
	private bool outOfBoundsWithoutOrbit;
	private bool coRoutineInProgress;
	private bool playerCantOrbit;
	private bool sfxHasPlayed;
	public static bool isOutOfBounds;
	public static bool playerHasExploded;
	private Vector3 angleCross;
	private static Vector3 playerPosition;
	public static Transform player;
	private static TrailRenderer trail;

	// Use this for initialization
	void Start ()
	{
		player = this.transform;
		activePlanets = new List<GameObject> ();
		explodedPlayerPrefab = Resources.Load <GameObject> ("PlayerDestroyed");
		isOnCorrectHeading = false;
		closestPlanetCollisionApproach = false;
		outOfBoundsWithoutOrbit = true;
		playerHasExploded = false;
		isOutOfBounds = false;
		coRoutineInProgress = false;
		playerCantOrbit = false;
		sfxHasPlayed = false;
		self = this.gameObject;
		trail = GameObject.Find ("Trail").GetComponent<TrailRenderer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!PlayerMovementIsPaused && SceneManager.GetActiveScene ().name == "Game") {
			AddPlanets ();
			SortPlanetsByDistance ();
			ChooseCurrentPlanet ();
	
			if (buttonIsHeld) {
				if (!GameScreenManager.playerHasCrashed && !playerCantOrbit) {
					OrbitalMovement ();
				} else {
					NormalMovement ();
				}
			} else if (!buttonIsHeld) {
				isOrbiting = false;
				isOnCorrectHeading = false;
				outOfBoundsWithoutOrbit = true;
				NormalMovement ();
				SortPlanetsByDistance ();
				if (currentPlanet != null) {
					ChangeSprite (false);
				}
			}
			currentPos = player.position.y;

			if (currentPos > 500) {
				if (!GameScreenManager.playerHasCrashed) {
					GameScreenManager.MoveColliders ();
				}
			}
			playerPosition = self.transform.position;

		} 
	}

	void OnCollisionStay2D (Collision2D col)
	{
		if (outOfBoundsWithoutOrbit) {
			if (col.gameObject == GameScreenManager.leftWall || col.gameObject == GameScreenManager.rightWall || col.gameObject == GameScreenManager.bottomWall) {
				if (!coRoutineInProgress) {
					StartCoroutine (OutOfBoundsEndGameCheck ());
					coRoutineInProgress = true;
				}
			}
			if (col.gameObject == GameScreenManager.earth) {
				PlayerExplosion ();
				SoundFXPlayer.PlayCrashSFX ();
				GameScreenManager.playerHasCrashed = true;
				isOutOfBounds = true;
				#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ReportProgress ("CgkIja6x2owHEAIQDA", 100.0f, (bool success) => {
				});
				#elif UNITY_IOS
				GameCenterPlatform.ShowDefaultAchievementCompletionBanner (true);
				Social.ReportProgress ("xyz.mattpowell.helios.earthcrash", 100.0f, (bool success) => {
				});
				#endif
			}
		}
	}

	IEnumerator OutOfBoundsEndGameCheck ()
	{
		playerCantOrbit = true;
		yield return new WaitForSeconds (.33f);
		if (player.position.x < GameScreenManager.dynamicMiddleOfScreen / 2 || player.position.x > GameScreenManager.dynamicMiddleOfScreen * 2) {
			GameScreenManager.playerHasCrashed = true;
			isOutOfBounds = true;
			if (!sfxHasPlayed) {
				SoundFXPlayer.PlaySwooshSFX ();
				sfxHasPlayed = true;
			}
		} 
		playerCantOrbit = false;
		coRoutineInProgress = false;
	}

	public void NormalMovement ()
	{
		player.position += player.up * 1600 * Time.deltaTime; //Flys in direction that object is facing
	}

	public void OrbitalMovement ()
	{
		ChooseCurrentPlanet ();
		if (relativeDistance < 750 && relativeDistance > 100) {
			if (relativeAngle > 89 && relativeAngle < 170) { //Once on correct heading
				if (angleCross.z < 0) { // Clockwise Rotation
					if (!isOnCorrectHeading) {
						if (relativeAngle > 91) {
							player.Rotate (new Vector3 (0, 0, -(relativeAngle - 90)));
						} /*else if (relativeAngle < 89) {
							player.Rotate (new Vector3 (0, 0, 90 - relativeAngle));
						}*/
						isOnCorrectHeading = true;
					}
					closestPlanetCollisionApproach = false;
					player.transform.RotateAround (currentPlanet.transform.position, Vector3.back, (750 / relativeDistance) * Time.deltaTime * 155);
					isOrbiting = true;
				} else if (angleCross.z > 0) { // CounterClockwise Rotation
					if (!isOnCorrectHeading) {
						if (relativeAngle > 91) {
							player.Rotate (new Vector3 (0, 0, relativeAngle - 90));
						} /*else if (relativeAngle < 89) {
							player.Rotate (new Vector3 (0, 0, -(90 - relativeAngle)));
						}*/
						isOnCorrectHeading = true;
					}
					closestPlanetCollisionApproach = false;
					player.transform.RotateAround (currentPlanet.transform.position, Vector3.forward, (750 / relativeDistance) * Time.deltaTime * 155);
					isOrbiting = true;
				}
				ChangeSprite (true);
				outOfBoundsWithoutOrbit = false;
			} else if (relativeAngle > 10 && relativeAngle < 89) {
				NormalMovement ();
				outOfBoundsWithoutOrbit = false;
			} else {
				NormalMovement ();
				closestPlanetCollisionApproach = true;
				outOfBoundsWithoutOrbit = true;
			}
		} else {
			NormalMovement ();
			outOfBoundsWithoutOrbit = true;
		}
	}

	public void AddPlanets ()
	{
		foreach (GameObject planets in GameObject.FindGameObjectsWithTag ("Planet")) {
			if (!activePlanets.Contains (planets)) {
				if (Vector3.Distance (planets.transform.position, player.position) < 1500) {
					activePlanets.Add (planets);
				}
			} else if (activePlanets.Contains (planets) && Vector3.Distance (planets.transform.position, player.position) > 1500) {
				activePlanets.Remove (planets);
			}
		}
	}

	public void SortPlanetsByDistance ()
	{
		activePlanets.Sort (delegate(GameObject x, GameObject y) {
			return Vector3.Distance (player.position, x.transform.position).CompareTo (Vector3.Distance (player.position, y.transform.position));
		});
	}

	public void CalculateDistanceAndAngles (GameObject planet)
	{
		relativeAngle = Vector2.Angle (planet.transform.position - player.position, player.up);
		angleCross = Vector3.Cross (player.position - planet.transform.position, player.up);
		relativeDistance = Vector3.Distance (player.position, planet.transform.position);
	}

	public void ChooseCurrentPlanet ()
	{
		if (activePlanets.Count > 0) {
			closestPlanet1 = activePlanets [0];
			if (activePlanets.Count > 1) {
				closestPlanet2 = activePlanets [1];
			}
		}
		if ((!closestPlanetCollisionApproach || currentPlanet == null) && activePlanets.Count > 0 && !isOrbiting) {
			if (closestPlanet1 != null) {
				currentPlanet = closestPlanet1;
				CalculateDistanceAndAngles (currentPlanet);
			}
		} else if (closestPlanetCollisionApproach && activePlanets.Count > 1 && !isOrbiting) {
			if (closestPlanet2 != null) {
				currentPlanet = closestPlanet2;
				CalculateDistanceAndAngles (currentPlanet);
			}
		}
	}

	public void ChangeSprite (bool changeToGlow)
	{
		if (changeToGlow) {
			currentPlanet.GetComponent<SpriteRenderer> ().sprite = Resources.Load <Sprite> ("Planets_img_Glow/" + currentPlanet.name);
		} else {
			currentPlanet.GetComponent<SpriteRenderer> ().sprite = Resources.Load <Sprite> ("Planets_img/" + currentPlanet.name);
		}
	}

	public static void PlayerExplosion ()
	{
		if (!playerHasExploded) {
			GameObject exploded = Instantiate (explodedPlayerPrefab, player.position, player.rotation) as GameObject;
			exploded.GetComponentInChildren<Rigidbody2D> ().AddForce (player.up * 40000 * Time.deltaTime, ForceMode2D.Impulse);
			trail.transform.SetParent (null, false);
			trail.gameObject.transform.position = playerPosition;
			trail.autodestruct = true;
			Destroy (self);
			playerHasExploded = true;
		}
	}
}