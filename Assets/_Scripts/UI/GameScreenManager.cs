using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using GooglePlayGames;

public class GameScreenManager : MonoBehaviour
{
	public static bool playerHasCrashed;
	public static GameObject BlackCoverScreen;
	public static bool deathAnimationAlreadyPlaying;
	private Animator playerControlMenuAnim;
	public static GameObject background;
	public static GameObject Stars;
	public static GameObject OutOfBoundsLineLeft;
	public static GameObject OutOfBoundsLineRight;
	public static GameObject leftWall;
	public static GameObject rightWall;
	public static GameObject bottomWall;
	public static GameObject earth;
	public static GameObject lastScoreLine;
	public static GameObject lastScoreClock;
	public static GameObject highScoreLine;
	public static GameObject highScoreLineStar;
	private Animator tutorialUIAnim;
	private CanvasGroup tutorialUI;
	private CanvasGroup playerControlUI;
	public static int highestPosition;
	public static float dynamicMiddleOfScreen;

	// Use this for initialization
	void Start ()
	{
		highestPosition = 0;
		BlackCoverScreen = GameObject.Find ("BlackTransitionCover");
		BlackCoverScreen.GetComponent<Animator> ().Play ("TransitionFadeOut");
		playerControlMenuAnim = GameObject.Find ("PlayerControlUIGroup").GetComponent<Animator> ();
		background = GameObject.Find ("Background");
		leftWall = GameObject.Find ("LeftWall");
		rightWall = GameObject.Find ("RightWall");
		bottomWall = GameObject.Find ("BottomWall");
		Stars = GameObject.Find ("Stars");
		OutOfBoundsLineLeft = GameObject.Find ("OutOfBoundsLineLeft");
		OutOfBoundsLineRight = GameObject.Find ("OutOfBoundsLineRight");
		lastScoreLine = GameObject.Find ("LastScoreLine");
		lastScoreClock = GameObject.Find ("LastScoreClock");
		highScoreLine = GameObject.Find ("HighScoreLine");
		highScoreLineStar = GameObject.Find ("HighScoreStar");
		earth = GameObject.Find ("Earth");
		tutorialUIAnim = GameObject.Find ("TutorialUIGroup").GetComponent<Animator> ();
		tutorialUI = GameObject.Find ("TutorialUIGroup").GetComponent<CanvasGroup> ();
		playerControlUI = GameObject.Find ("PlayerControlUIGroup").GetComponent<CanvasGroup> ();
		playerHasCrashed = false;
		deathAnimationAlreadyPlaying = false;
		ScaleCameraAndStationaryObjects ();

		if (PlayerPrefs.GetInt ("ReturningPlayer") == 0) {
			StartCoroutine (ActivateTutorial ());
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (playerHasCrashed && !deathAnimationAlreadyPlaying) {
			deathAnimationAlreadyPlaying = true;
			PlayerDied ();
		}
		if (ScoreController.dontChangeScoreText) {
			ScoreController.dontChangeScoreText = false;
		}

		if (PlayerController.currentPos > highestPosition && !PlayerController.isOutOfBounds) {
			highestPosition = (int)PlayerController.currentPos;
		}
	}

	/*public void DisplayPlayerControlUI ()
	{
		playerControlMenuAnim.Play ("UIFadeIn");
		playerControl.interactable = true;
		playerControl.blocksRaycasts = true;
		inGameScoreText.enabled = true;
	}*/
	
	public void PlayerDied ()
	{
		StartCoroutine (TransitionToGameOverScene ());
		SpawnController.SpawnPlanetsIsPaused = true;
		PlayerPrefs.SetInt ("totalScore", PlayerPrefs.GetInt ("totalScore") + ScoreController.scoreInt);

		#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated ()) {
			if (!PlayGamesPlatform.Instance.GetAchievement ("CgkIja6x2owHEAIQCg").IsUnlocked) {
				PlayGamesPlatform.Instance.IncrementAchievement ("CgkIja6x2owHEAIQCg", ScoreController.scoreInt, (bool success) => {
				});
			}
			if (!PlayGamesPlatform.Instance.GetAchievement ("CgkIja6x2owHEAIQCw").IsUnlocked) {
				PlayGamesPlatform.Instance.IncrementAchievement ("CgkIja6x2owHEAIQCw", ScoreController.scoreInt, (bool success) => {
				});
			}
			if (!PlayGamesPlatform.Instance.GetAchievement ("CgkIja6x2owHEAIQCQ").IsUnlocked) {
				PlayGamesPlatform.Instance.IncrementAchievement ("CgkIja6x2owHEAIQCQ", ScoreController.scoreInt, (bool success) => {
				});
			}

			PlayGamesPlatform.Instance.ReportScore (PlayerPrefs.GetInt ("totalScore"), "CgkIja6x2owHEAIQDg", (bool success) => {
			});
		}
		#elif UNITY_IOS
		if (Social.localUser.authenticated) {
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner (true);
			Social.ReportProgress ("xyz.mattpowell.helios.totalscore1000", (PlayerPrefs.GetInt ("totalScore") / 1000) * 100f, (bool success) => {
			});
			Social.ReportProgress ("xyz.mattpowell.helios.totalscore3000", (PlayerPrefs.GetInt ("totalScore") / 3000) * 100f, (bool success) => {
			});
			Social.ReportProgress ("xyz.mattpowell.helios.totalscore5000", (PlayerPrefs.GetInt ("totalScore") / 5000) * 100f, (bool success) => {
			});
			Social.ReportScore (PlayerPrefs.GetInt ("totalScore"), "xyz.mattpowell.helios.leaderboardtopscore", (bool success) => {
			});
		}
		#endif
	}

	public void ScaleCameraAndStationaryObjects ()
	{
		Camera.main.transform.position = new Vector3 (dynamicMiddleOfScreen, 960, -10);
		GameObject.Find ("Earth").transform.position = new Vector3 (dynamicMiddleOfScreen, 30, 0);
		GameObject.Find ("Player").transform.position = new Vector3 (dynamicMiddleOfScreen, 360, 0);
		GameObject.Find ("LeftWall").GetComponent<BoxCollider2D> ().offset = new Vector2 (-540, 0);
		GameObject.Find ("RightWall").GetComponent<BoxCollider2D> ().offset = new Vector2 (1920 * Camera.main.aspect - 540, 0);
		GameObject.Find ("Background").transform.position = new Vector3 (dynamicMiddleOfScreen, 960, 10);

		highScoreLine.transform.position = new Vector3 (dynamicMiddleOfScreen, PlayerPrefs.GetInt ("highScore") * 500, 1);
		highScoreLineStar.transform.position = new Vector3 (50, highScoreLine.transform.position.y + 35, 1);
		highScoreLineStar.transform.localScale = new Vector3 (dynamicMiddleOfScreen * 2 / 1440, dynamicMiddleOfScreen * 2 / 1440, 1);

		if (PlayerPrefs.GetInt ("LastScore") <= (PlayerPrefs.GetInt ("highScore") * 500) - 150) {
			lastScoreLine.transform.position = new Vector3 (dynamicMiddleOfScreen, PlayerPrefs.GetInt ("LastScore"), 1);
			lastScoreClock.transform.localScale = new Vector3 (dynamicMiddleOfScreen * 2 / 1440, dynamicMiddleOfScreen * 2 / 1440 - .1f, 1);
			lastScoreClock.transform.position = new Vector3 (50, lastScoreLine.transform.position.y + 35, 1);
		}
	}

	public static void MoveColliders ()
	{
		Camera.main.transform.position = new Vector3 ((PlayerController.player.transform.position.x + GameScreenManager.dynamicMiddleOfScreen) / 2, PlayerController.player.position.y + 460, -10);
		leftWall.transform.position = new Vector3 (leftWall.transform.position.x, PlayerController.player.position.y + 460, 0);
		rightWall.transform.position = new Vector3 (rightWall.transform.position.x, PlayerController.player.position.y + 460, 0);
		background.transform.position = new Vector3 (background.transform.position.x, PlayerController.player.position.y + 460, 10);
		BlackCoverScreen.transform.position = new Vector3 (PlayerController.player.position.x, PlayerController.player.position.y, 0);
		Stars.transform.position = new Vector3 (540, PlayerController.player.position.y + 1520, 0);
		OutOfBoundsLineLeft.transform.position = new Vector3 (-12.5f, PlayerController.currentPos, 9);
		OutOfBoundsLineRight.transform.position = new Vector3 ((1920 * Camera.main.aspect) + 12.5f, PlayerController.currentPos, 9);
	}

	IEnumerator TransitionToGameOverScene ()
	{
		PlayerPrefs.SetInt ("LastScore", highestPosition);
		yield return new WaitForSeconds (.5f);
		BlackCoverScreen.GetComponent<Animator> ().Play ("TransitionFadeIn");
		playerControlMenuAnim.Play ("UIFadeOut");
		yield return new WaitForSeconds (.8f);
		SceneManager.LoadScene ("GameOver");
	}

	IEnumerator ActivateTutorial ()
	{
		yield return new WaitForSeconds (.70f);
		DisplayTutorialUI ();
		PlayerPrefs.SetInt ("ReturningPlayer", 1);
	}

	public void DisplayTutorialUI ()
	{
		PauseController.PauseGame ();
		tutorialUIAnim.Play ("UIFadeIn");
		tutorialUI.interactable = true;
		tutorialUI.blocksRaycasts = true;
		playerControlUI.blocksRaycasts = false;
		playerControlUI.interactable = false;
	}

	public void HideTutorialUI ()
	{
		PauseController.UnPauseGame ();
		tutorialUIAnim.Play ("UIFastFadeOut");
		tutorialUI.interactable = false;
		tutorialUI.blocksRaycasts = false;
		playerControlUI.blocksRaycasts = true;
		playerControlUI.interactable = true;
	}
}