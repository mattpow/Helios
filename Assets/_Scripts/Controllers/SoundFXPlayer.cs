using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundFXPlayer : MonoBehaviour
{
	private Button SFXToggle;
	//SFX toggle button on start scene
	public Sprite SFXOn;
	//SFX on sprite
	public Sprite SFXOff;
	//SFX off sprite
	private int SFXPref;
	//SFX preferences
	public static AudioClip clickSFX;
	public static AudioClip SwooshSFX;
	public static AudioClip crashSFX;
	public static AudioSource audioSource;
	static SoundFXPlayer instance = null;

	///static int instance = 0;
	// Use this for initialization
	
	void Awake ()
	{
		if (!PlayerPrefs.HasKey ("allowSFXToPlay")) { //If no SFX playing preference is set, then play SFX by default
			PlayerPrefs.SetInt ("allowSFXToPlay", 1);
		} 	 
	}

	void Start ()
	{
		if (instance != null) { //if there are multiple music players running, delete any extras
			Destroy (gameObject);
		} else { //else let the single music player run entirely and mark it as the single instance
			instance = this;
			GameObject.DontDestroyOnLoad (gameObject);
		}

		crashSFX = Resources.Load <AudioClip> ("Sounds/SFX/Crash");
		clickSFX = Resources.Load <AudioClip> ("Sounds/SFX/UIClick");
		SwooshSFX = Resources.Load <AudioClip> ("Sounds/SFX/Swoosh");
		audioSource = instance.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (SceneManager.GetActiveScene ().name == "Home") {
			if (SFXToggle == null) {
				SFXToggle = GameObject.Find ("SFXToggleButton").GetComponent<Button> ();
				SFXToggle.onClick.AddListener (() => ToggleSFX ());
			}
			SFXPref = PlayerPrefs.GetInt ("allowSFXToPlay"); //Determines whether or not SFX is allowed to play
			if (SFXPref == 1) { // if so
				SFXToggle.GetComponent<Image> ().sprite = SFXOn; //Render the SFX off sprite
			} else {
				SFXToggle.GetComponent<Image> ().sprite = SFXOff; //Render the SFX off sprite
			}
		}
	}

	public void ToggleSFX () //When SFX toggle button is clicked
	{
		if (SFXPref == 1) { //If the SFX preference was previously true
			PlayerPrefs.SetInt ("allowSFXToPlay", 0); //set it to zero/false
			SFXToggle.GetComponent<Image> ().sprite = SFXOff; //Render SFX off sprite
			
		} else if (SFXPref == 0) { //If the SFX preferebce was previously false
			PlayerPrefs.SetInt ("allowSFXToPlay", 1); //Set it to 1/true
			SFXToggle.GetComponent<Image> ().sprite = SFXOn; //Render SFX on sprite
		}

		PlayClickSFX ();
	}

	public static void PlayCrashSFX ()
	{
		audioSource.clip = crashSFX;
		audioSource.volume = 1f;
		if (PlayerPrefs.GetInt ("allowSFXToPlay") == 1) {
			audioSource.Play ();
		}
	}

	public static void PlayClickSFX ()
	{
		audioSource.clip = clickSFX;
		audioSource.volume = .6f;
		if (PlayerPrefs.GetInt ("allowSFXToPlay") == 1) {
			audioSource.Play ();
		}
	}

	public static void PlaySwooshSFX ()
	{
		audioSource.clip = SwooshSFX;
		audioSource.volume = .75f;
		if (PlayerPrefs.GetInt ("allowSFXToPlay") == 1) {
			audioSource.Play ();
		}
	}
}