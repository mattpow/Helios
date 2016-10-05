using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{

	private AudioClip[] music;
	//Music playlist
	private Button musicToggle;
	//Music toggle button on start scene
	private int musicNumber;
	//Track number of music playlist
	private AudioSource musicAudio;
	//Audio source from music manager
	static MusicPlayer instance = null;
	//Music playing instance
	public Sprite musicOn;
	//Music on sprite
	public Sprite musicOff;
	//Music off sprite
	private int musicPref;
	//Music preferences
	
	///static int instance = 0;
	// Use this for initialization
	
	void Awake ()
	{
		music = Resources.LoadAll<AudioClip> ("Sounds/Music"); //Finds all music and stores it in an array
		musicNumber = 0;
		
		if (instance != null) { //if there are multiple music players running, delete any extras
			Destroy (gameObject);
		} else { //else let the single music player run entirely and mark it as the single instance
			instance = this;
			GameObject.DontDestroyOnLoad (gameObject);
		}
		if (!PlayerPrefs.HasKey ("allowMusicToPlay")) { //If no music playing preference is set, then play music by default
			PlayerPrefs.SetInt ("allowMusicToPlay", 1);
		} 	 
	}

	void Start ()
	{
		musicAudio = GetComponent<AudioSource> (); //Gets audio source component and stores it in musicAudio variable
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (musicToggle == null) {
			if (SceneManager.GetActiveScene ().name == "Home") {
				musicToggle = GameObject.Find ("MusicToggleButton").GetComponent<Button> ();
				musicToggle.onClick.AddListener (() => ToggleMusic ());
			}
		}
		musicPref = PlayerPrefs.GetInt ("allowMusicToPlay"); //Determines whether or not music is allowed to play
		if (musicPref == 1) { // if so
			if (SceneManager.GetActiveScene ().name == "Home") {
				musicToggle.GetComponent<Image> ().sprite = musicOn; //Render the Music on sprite
			}
			musicAudio.volume = .40f; //set volume to 50%
			if (!musicAudio.isPlaying) { //if there is no music playing
				if (musicNumber < music.Length) { //and if music track is less than the allowed tracks
					musicAudio.clip = music [musicNumber]; //select the music that matches the selected track number
					musicAudio.Play (); //Play it 
					musicNumber++; //Imcrement track number for next go through
				} else {  //If music track number exceeds amount of music, reset track number to zero
					musicNumber = 0;
				}
			}
		} else {
			musicAudio.volume = 0; //If music is not allowed to play then turn volume to 0%
			if (SceneManager.GetActiveScene ().name == "Home") {
				musicToggle.GetComponent<Image> ().sprite = musicOff; //Render the Music off sprite
			}
		}
	}

	public void ToggleMusic () //When music toggle button is clicked
	{
		if (musicPref == 1) { //If the music preference was previously true
			PlayerPrefs.SetInt ("allowMusicToPlay", 0); //set it to zero/false
			musicToggle.GetComponent<Image> ().sprite = musicOff; //Render Music off sprite
			
		} else if (musicPref == 0) { //If the music preferebce was previously false
			PlayerPrefs.SetInt ("allowMusicToPlay", 1); //Set it to 1/true
			musicToggle.GetComponent<Image> ().sprite = musicOn; //Render Music on sprite
		}

		SoundFXPlayer.PlayClickSFX ();
	}
}