using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class GameOverScreenManager : MonoBehaviour
{
	private GameObject BlackCoverScreen;
	public static bool newHighScore;
	Animator gameOverMenuAnim;

	// Use this for initialization
	void Start ()
	{
		/*
		if (!PlayerPrefs.HasKey ("AdCount")) {
			PlayerPrefs.SetInt ("AdCount", 0);
		}
		PlayerPrefs.SetInt ("AdCount", PlayerPrefs.GetInt ("AdCount") + 1);
		*/
		BlackCoverScreen = GameObject.Find ("BlackTransitionCover");
		BlackCoverScreen.GetComponent<Animator> ().Play ("TransitionFadeOut");
		gameOverMenuAnim = GameObject.Find ("GameOverMenuUIGroup").GetComponent<Animator> ();
		gameOverMenuAnim.Play ("UIFadeIn");
		GameScreenManager.playerHasCrashed = false;
		GameScreenManager.deathAnimationAlreadyPlaying = false;
		/*if (PlayerPrefs.GetInt ("RemoveAds") == 0) {
			Debug.Log (PlayerPrefs.GetInt ("AdCount"));
			if (PlayerPrefs.GetInt ("AdCount") >= 8) {
				ShowAd ();
				PlayerPrefs.SetInt ("AdCount", 0);
			}
		}
		if (PlayerPrefs.GetInt ("RemoveAds") == 1) {
			GameObject.Find ("NoAdsButton").GetComponent<Button> ().interactable = false;
		} else if (PlayerPrefs.GetInt ("RemoveAds") == 0) {
			GameObject.Find ("NoAdsButton").GetComponent<Button> ().interactable = true;
		}*/
		if (newHighScore) {
			GameObject.Find ("GameOverHighScoreText").GetComponent<Text> ().color = new Color32 (209, 158, 6, 255);
		}
	}

	public void ReplayGame ()
	{
		SoundFXPlayer.PlayClickSFX ();

		Reset ();
		StartCoroutine (TransitionToGameScene ());
		gameOverMenuAnim.Play ("UIFastFadeOut");
		BlackCoverScreen.GetComponent<Animator> ().Play ("TransitionFastFadeIn");
	}

	IEnumerator TransitionToGameScene ()
	{
		yield return new WaitForSeconds (.4f);
		PauseController.UnPauseGame ();
		SceneManager.LoadScene ("Game");
	}

	public void ReturnToHomeScreen ()
	{
		SoundFXPlayer.PlayClickSFX ();

		Reset ();
		PauseController.UnPauseGame ();
		StartCoroutine (TransitionToHomeScene ());
		gameOverMenuAnim.Play ("UIFastFadeOut");
		BlackCoverScreen.GetComponent<Animator> ().Play ("TransitionFastFadeIn");
	}

	public void Reset ()
	{
		ScoreController.scoreInt = 0;
		SpawnManager.peaceSpawnCount = 0;
		PlayerController.currentPos = 0;
		PlayerController.playerHasExploded = false;
		GameScreenManager.playerHasCrashed = false;
		GameScreenManager.deathAnimationAlreadyPlaying = false;
		PlayerController.isOrbiting = false;
		PlayerController.buttonIsHeld = false;
		SpawnManager.isFirstSpawn = true;
		newHighScore = false;
	}

	IEnumerator TransitionToHomeScene ()
	{
		yield return new WaitForSeconds (.4f);
		SceneManager.LoadScene ("Home");
	}
}
