using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using GooglePlayGames;

public class ScoreController : MonoBehaviour
{
	private Text inGameScoreText;
	private Text gameOverScoreText;
	private Text gameOverHighScoreText;
	//private Text gameOverTotalScoreText;
	public static bool dontChangeScoreText;
	private int highScore;
	//private int totalScore;
	public static int scoreInt;
	// Use this for initialization
	void Start ()
	{	
		GameCenterPlatform.ShowDefaultAchievementCompletionBanner (true);
		if (!PlayerPrefs.HasKey ("highScore")) { //If player doesn't have a high score then set it to zero
			PlayerPrefs.SetInt ("highScore", 0);
		}

		dontChangeScoreText = false;

		if (SceneManager.GetActiveScene ().name == "Game") {
			inGameScoreText = GameObject.Find ("InGameScoreText").GetComponent<Text> ();
		} else if (SceneManager.GetActiveScene ().name == "GameOver") {
			gameOverScoreText = GameObject.Find ("GameOverScoreText").GetComponent<Text> ();
			gameOverHighScoreText = GameObject.Find ("GameOverHighScoreText").GetComponent<Text> ();
			//gameOverTotalScoreText = GameObject.Find ("GameOverTotalScoreText").GetComponent<Text> ();
		}
		scoreInt = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		scoreInt = GameScreenManager.highestPosition / 500; //Current score in-game
		if (scoreInt < 0) { //Game does not allow negative scores. If goes into negative, set score to zero.
			scoreInt = 0;
		}
		if (SceneManager.GetActiveScene ().name == "Game") { //While in game scene, make score text equal to the current score
			inGameScoreText.text = " " + scoreInt.ToString ();	
		}

		highScore = PlayerPrefs.GetInt ("highScore"); //Gets the high score and stores it in an int
		//totalScore = PlayerPrefs.GetInt ("totalScore");

		if (SceneManager.GetActiveScene ().name == "GameOver") {
			if (!dontChangeScoreText) {
				gameOverScoreText.text = "Score: " + scoreInt.ToString ();
				if (GameOverScreenManager.newHighScore) {
					gameOverHighScoreText.text = "★ Best: " + highScore.ToString () + " ★";
				} else {
					gameOverHighScoreText.text = "Best: " + highScore.ToString ();
				}
				//gameOverTotalScoreText.text = "Total: " + totalScore.ToString ();

				dontChangeScoreText = true;
			}
		}

		if (scoreInt > highScore) {
			PlayerPrefs.SetInt ("highScore", scoreInt);
			GameOverScreenManager.newHighScore = true;
			highScore = scoreInt;
			//****************************LEADERBOARD REPORTING CODE FOR IOS / ANDROID****************************\\ 
			if (Social.localUser.authenticated || PlayGamesPlatform.Instance.IsAuthenticated ()) {
				#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ReportScore (PlayerPrefs.GetInt ("highScore"), "CgkIja6x2owHEAIQAQ", (bool success) => {
				});
				#elif UNITY_IOS
				Social.ReportScore (PlayerPrefs.GetInt ("highScore"), "xyz.mattpowell.helios.leaderboardscore", (bool success) => {
				});
				#endif
			}
			//****************************END OF LEADERBOARD REPORTING CODE FOR IOS / ANDROID****************************\\ 
		}
			
		//****************************ACHIEVEMENT REPORTING CODE FOR IOS / ANDROID****************************\\ 
		if (Social.localUser.authenticated || PlayGamesPlatform.Instance.IsAuthenticated ()) {
			if (scoreInt >= 10) {
				#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ReportProgress ("CgkIja6x2owHEAIQDw", 100.0f, (bool success) => {
				});
				#elif UNITY_IOS
			Social.ReportProgress ("xyz.mattpowell.helios.score10", 100.0f, (bool success) => {
			});
				#endif
			}
			if (scoreInt >= 25) {
				#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ReportProgress ("CgkIja6x2owHEAIQBQ", 100.0f, (bool success) => {
				});
				#elif UNITY_IOS
			Social.ReportProgress ("xyz.mattpowell.helios.score25", 100.0f, (bool success) => {
			});
				#endif
			}
			if (scoreInt >= 50) {
				#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ReportProgress ("CgkIja6x2owHEAIQBg", 100.0f, (bool success) => {
				});
				#elif UNITY_IOS
			Social.ReportProgress ("xyz.mattpowell.helios.score50", 100.0f, (bool success) => {
			});
				#endif
			}
			if (scoreInt >= 100) {
				#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ReportProgress ("CgkIja6x2owHEAIQBw", 100.0f, (bool success) => {
				});
				#elif UNITY_IOS
			Social.ReportProgress ("xyz.mattpowell.helios.score100", 100.0f, (bool success) => {
			});
				#endif
			}
			if (scoreInt >= 200) {
				#if UNITY_ANDROID
				PlayGamesPlatform.Instance.ReportProgress ("CgkIja6x2owHEAIQCA", 100.0f, (bool success) => {
				});
				#elif UNITY_IOS
			Social.ReportProgress ("xyz.mattpowell.helios.score200", 100.0f, (bool success) => {
			});
				#endif
			}
		}
		//****************************END OF ACHIEVEMENT REPORTING CODE FOR IOS / ANDROID****************************\\


	}
}
