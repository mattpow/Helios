using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public class PauseController : MonoBehaviour
{
	public static int AdCount;

	void Awake ()
	{
		#if UNITY_IOS
		Social.localUser.Authenticate (success => {
			PlayerPrefs.SetInt ("LoggedIn", 1);
		});
		#endif

		if (PlayerPrefs.GetInt ("LoggedIn") == 1) {
			#if UNITY_ANDROID
			PlayGamesPlatform.Instance.localUser.Authenticate ((bool success) => {

			});
			#endif
		}
	}

	void Start ()
	{
		GameScreenManager.dynamicMiddleOfScreen = 960 * Camera.main.aspect;

		if (!PlayerPrefs.HasKey ("LoggedIn")) {
			PlayerPrefs.SetInt ("LoggedIn", 0);
		}
		if (!PlayerPrefs.HasKey ("totalScore")) {
			PlayerPrefs.SetInt ("totalScore", 0);
		}
		//if (!PlayerPrefs.HasKey ("RemoveAds")) {
		//	PlayerPrefs.SetInt ("RemoveAds", 0);
		//}
		if (!PlayerPrefs.HasKey ("ReturningPlayer")) {
			PlayerPrefs.SetInt ("ReturningPlayer", 0);
		}
		if (!PlayerPrefs.HasKey ("LastScore")) {
			PlayerPrefs.SetInt ("LastScore", -1000);
		}

		Application.targetFrameRate = 60;
	}

	public static void PauseGame ()
	{
		SpawnController.SpawnPlanetsIsPaused = true;
		PlayerController.PlayerMovementIsPaused = true;
	}

	public static void UnPauseGame ()
	{
		SpawnController.SpawnPlanetsIsPaused = false;
		PlayerController.PlayerMovementIsPaused = false;
	}
}