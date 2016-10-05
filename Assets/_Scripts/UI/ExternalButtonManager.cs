using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExternalButtonManager : MonoBehaviour
{
	public Sprite IOSShareSprite;
	public Texture2D _logo;
	public Diffusion diffusion;

	void Start ()
	{
		#if UNITY_IOS
		if (SceneManager.GetActiveScene ().name == "GameOver") {
			GameObject.Find ("ShareButton").GetComponent<Button> ().GetComponent<Image> ().sprite = IOSShareSprite;
			diffusion.eventReceiver = gameObject;
		}
		#endif
	}

	public void RateLink ()
	{
		#if UNITY_ANDROID
		Application.OpenURL ("http://www.play.google.com/store/apps/details?id=xyz.mattpowell.helios");
		#elif UNITY_IOS
		Application.OpenURL ("https://itunes.apple.com/us/app/helios-orbital-escape/id1073325324?ls=1&mt=8");
		#endif
	}


	public void shareText ()
	{
		SoundFXPlayer.PlayClickSFX ();

		//execute the below lines if being run on a Android device
		#if UNITY_ANDROID
		string subject = "Helios";
		string body = "I scored " + ScoreController.scoreInt + " points in Helios! Get it on the play store and try to beat me. \n\n http://www.play.google.com/store/apps/details?id=xyz.mattpowell.helios";
		
		//Refernece of AndroidJavaClass class for intent
		AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
		//Refernece of AndroidJavaObject class for intent
		AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
		//call setAction method of the Intent object created
		intentObject.Call<AndroidJavaObject> ("setAction", intentClass.GetStatic<string> ("ACTION_SEND"));
		//set the type of sharing that is happening
		intentObject.Call<AndroidJavaObject> ("setType", "text/plain");
		//add data to be passed to the other activity i.e., the data to be sent
		intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_SUBJECT"), subject);
		intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_TEXT"), body);
		//get the current activity
		AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
		//start the activity by sending the intent data
		currentActivity.Call ("startActivity", intentObject);
		#elif UNITY_IOS
		diffusion.Share ("I scored " + ScoreController.scoreInt + " points in Helios! Can you beat me?", "https://itunes.apple.com/us/app/helios-orbital-escape/id1073325324?ls=1&mt=8", null);
		#endif
	}
}
