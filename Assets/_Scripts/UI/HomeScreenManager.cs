using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeScreenManager : MonoBehaviour
{
	Animator homeMenuAnim;
	private GameObject earth;
	private GameObject player;
	private float currentVelocity = 0.5f;
	public static bool playButtonPressed;
	private GameObject background;
	private GameObject Stars;
	private GameObject BlackCoverScreen;
	private CanvasGroup HomeMenuUIGroup;
	private CanvasGroup AboutMenuUIGroup;
	bool aboutMenuIsOpen;

	// Use this for initialization
	void Start ()
	{
		homeMenuAnim = GameObject.Find ("HomeMenuUIGroup").GetComponent<Animator> ();
		BlackCoverScreen = GameObject.Find ("BlackTransitionCover");
		BlackCoverScreen.GetComponent<Animator> ().Play ("TransitionFadeOut");
		player = GameObject.Find ("Player");
		earth = GameObject.Find ("Earth");
		background = GameObject.Find ("Background");
		Stars = GameObject.Find ("Stars");
		HomeMenuUIGroup = GameObject.Find ("HomeMenuUIGroup").GetComponent<CanvasGroup> ();
		AboutMenuUIGroup = GameObject.Find ("AboutMenuUIGroup").GetComponent<CanvasGroup> ();
		playButtonPressed = false;
		aboutMenuIsOpen = false;

		#if !UNITY_IOS
		GameObject.Find ("RestorePurchaseButton").SetActive (false);
		#endif
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!playButtonPressed) {
			PreGameOrbitAnimation ();
		} else {
			StartGameEscapeOrbit ();
		}	
		if (Input.GetKeyUp (KeyCode.Escape) || Input.GetKeyUp (KeyCode.Return)) {
			if (aboutMenuIsOpen) {
				HideAboutUI ();
			}
		}
	}

	IEnumerator TransitionToGameScene ()
	{
		SoundFXPlayer.PlayClickSFX ();

		yield return new WaitForSeconds (1f);
		SceneManager.LoadScene ("Game");
	}

	public void PreGameOrbitAnimation ()
	{
		background.transform.Rotate (new Vector3 (0, 0, 0.5f));
		Camera.main.transform.Rotate (new Vector3 (0, 0, 0.5f));
		earth.transform.Rotate (new Vector3 (0, 0, 0.5f));
		BlackCoverScreen.transform.Rotate (new Vector3 (0, 0, 0.5f));
		Stars.transform.Rotate (new Vector3 (0, 0, 0.5f));
	}

	public void StartGameEscapeOrbit ()
	{
		background.transform.Rotate (new Vector3 (0, 0, currentVelocity));
		Camera.main.transform.Rotate (new Vector3 (0, 0, currentVelocity));
		earth.transform.Rotate (new Vector3 (0, 0, currentVelocity));
		BlackCoverScreen.transform.Rotate (new Vector3 (0, 0, currentVelocity));
		Stars.transform.Rotate (new Vector3 (0, 0, currentVelocity));
		currentVelocity += 0.2f;
	}

	public void HideHomeUI ()
	{
		StartCoroutine (TransitionToGameScene ());
		homeMenuAnim.Play ("UISlowFadeOut");
		BlackCoverScreen.GetComponent<Animator> ().Play ("TransitionSlowFadeIn");
		playButtonPressed = true;
	}

	/*public void DisplaySettingsUI ()
	{
		SettingsMenuUIGroup.GetComponent<Animator> ().Play ("HalfSlideUp");
		homeMenuAnim.Play ("HomeMenuFadeOut");
		earth.GetComponent<Animator> ().Play ("HalfColor");
		player.GetComponent<Animator> ().Play ("HalfColor");
		HomeMenuUIGroup.interactable = false;
		HomeMenuUIGroup.blocksRaycasts = false;
		SettingsMenuUIGroup.interactable = true;
		SettingsMenuUIGroup.blocksRaycasts = true;

		settingsMenuIsOpen = true;
	}

	public void HideSettingsUI ()
	{
		SettingsMenuUIGroup.GetComponent<Animator> ().Play ("HalfSlideDown");
		homeMenuAnim.Play ("HomeMenuFadeIn");
		earth.GetComponent<Animator> ().Play ("FullColor");
		player.GetComponent<Animator> ().Play ("FullColor");
		HomeMenuUIGroup.interactable = true;
		HomeMenuUIGroup.blocksRaycasts = true;
		SettingsMenuUIGroup.interactable = false;
		SettingsMenuUIGroup.blocksRaycasts = false;

		settingsMenuIsOpen = false;
	}*/

	public void DisplayAboutUI ()
	{
		AboutMenuUIGroup.GetComponent<Animator> ().Play ("SlideUp");
		homeMenuAnim.Play ("HomeMenuFadeOut");
		earth.GetComponent<Animator> ().Play ("HalfColor");
		player.GetComponent<Animator> ().Play ("HalfColor");
		HomeMenuUIGroup.interactable = false;
		HomeMenuUIGroup.blocksRaycasts = false;
		AboutMenuUIGroup.interactable = true;
		AboutMenuUIGroup.blocksRaycasts = true;

		aboutMenuIsOpen = true;
		SoundFXPlayer.PlayClickSFX ();	
	}

	public void HideAboutUI ()
	{
		AboutMenuUIGroup.GetComponent<Animator> ().Play ("SlideDown");
		homeMenuAnim.Play ("HomeMenuFadeIn");
		earth.GetComponent<Animator> ().Play ("FullColor");
		player.GetComponent<Animator> ().Play ("FullColor");
		HomeMenuUIGroup.interactable = true;
		HomeMenuUIGroup.blocksRaycasts = true;
		AboutMenuUIGroup.interactable = false;
		AboutMenuUIGroup.blocksRaycasts = false;

		aboutMenuIsOpen = false;

		SoundFXPlayer.PlayClickSFX ();
	}
}
