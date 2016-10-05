using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Diffusion : MonoBehaviour {
	
	public string message;
	public string url;

	public int platformsToHide;

	public string[] customPlatforms;

	/// <summary>
	/// The event receiver.
	/// </summary>
	public GameObject eventReceiver;
	
	/// <summary>
	/// Delegates for better performance or multiple listeners
	/// </summary>
	public delegate void OnCompleted(DiffusionPlatform platform);
	public delegate void OnCancelled();

	// Expose the delegates
	public OnCompleted onCompleted;
	public OnCancelled onCancelled;

	void Start() {
		Prewarm();
		foreach(string platform in customPlatforms) {
			AddCustomPlatform(platform);
		}
	}



#region Accounts

	public static bool isFacebookConnected() {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			#if UNITY_IPHONE
			return _isFacebookConnected();
			#endif
		}
		return false;
	}

	public static bool isTwitterConnected() {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			#if UNITY_IPHONE
			return _isTwitterConnected();
			#endif
		}
		return false;
	}

#endregion

	

#region Sharing

	/// <summary>
	/// Posts the default message and URL to Twitter.
	/// </summary>
	public void PostToTwitter() {
		PostToTwitter(this.message, this.url, null);
	}


	/// <summary>
	/// Posts to Twitter.
	/// </summary>
	/// <param name="message">Message.</param>
	public void PostToTwitter(string message) {
		PostToTwitter(message, null, null);
	}


	/// <summary>
	/// Posts to Twitter.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="imagePath">Image path.</param>
	public void PostToTwitter(string message, string imagePath) {
		PostToTwitter(message, null, imagePath);
	}


	/// <summary>
	/// Posts to Twitter.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="url">URL.</param>
	/// <param name="imagePath">Image path.</param>
	public void PostToTwitter(string message, string url, string imagePath) {

		if (string.IsNullOrEmpty(message)) {
			message = "";
		}

		if (string.IsNullOrEmpty(url)) {
			url = "";
		}

		if (string.IsNullOrEmpty(imagePath)) {
			imagePath = "";
		}

		Debug.Log("[Diffusion] Sharing message: " + message + " | " + url + " | " + imagePath);
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			#if UNITY_IPHONE
			_postToTwitter(message, url, imagePath);
			#endif
		}
	}


	/// <summary>
	/// Posts the default message and URL to Facebook.
	/// </summary>
	public void PostToFacebook() {
		PostToFacebook(this.message, this.url, null);
	}


	/// <summary>
	/// Posts to Facebook.
	/// </summary>
	/// <param name="message">Message.</param>
	public void PostToFacebook(string message) {
		PostToFacebook(message, null, null);
	}


	/// <summary>
	/// Posts to Facebook.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="imagePath">Image path.</param>
	public void PostToFacebook(string message, string imagePath) {
		PostToFacebook(message, null, imagePath);
	}


	/// <summary>
	/// Posts to Facebook.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="url">URL.</param>
	/// <param name="imagePath">Image path.</param>
	public void PostToFacebook(string message, string url, string imagePath) {

		if (string.IsNullOrEmpty(message)) {
			message = "";
		}
		
		if (string.IsNullOrEmpty(url)) {
			url = "";
		}
		
		if (string.IsNullOrEmpty(imagePath)) {
			imagePath = "";
		}

		Debug.Log("[Diffusion] Sharing message: " + message + " | " + url + " | " + imagePath);
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			#if UNITY_IPHONE
			_postToFacebook(message, url, imagePath);
			#endif
		}
	}


	/// <summary>
	/// Share the default message and url.
	/// </summary>
	public void Share() {
		Share(this.message, this.url, null, this.platformsToHide);
	}

	
	/// <summary>
	/// Share the specified message and file.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="filePath">File path.</param>
	public void Share(string message, string filePath) {
		Share(message, null, filePath, this.platformsToHide);
	}


	/// <summary>
	/// Share the specified message, url and file.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="url">URL.</param>
	/// <param name="filePath">File path.</param>
	public void Share(string message, string url, string filePath) {
		Share(message, url, filePath, this.platformsToHide);
	}


	/// <summary>
	/// Share the specified message, url, filePath and explicit platforms to hide.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="url">URL.</param>
	/// <param name="filePath">File path.</param>
	/// <param name="platforms">Platforms to hide.</param>
	public void Share(string message, string url, string filePath, int platforms) {

		if (message == null) {
			message = "";
		}

		if (string.IsNullOrEmpty(url)) {
			url = "";
		} else {
			url = AddPrefix("http://", url);
		}

		if (string.IsNullOrEmpty(filePath)) {
			filePath = "";
		} else { 
			filePath = AddPrefix("file://", filePath);
		}

		Debug.Log("[Diffusion] Sharing message: " + message + " | " + url + " | " + filePath);
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			#if UNITY_IPHONE
			_share(message, url, filePath, platforms);
			#endif
		}

		#if UNITY_EDITOR
		Completed("1");
		#endif
	}

#endregion



#region Callbacks

	private void Completed(string platformAsInt) {
		DiffusionPlatform platform = (DiffusionPlatform)int.Parse(platformAsInt);

		if (eventReceiver != null) {
			eventReceiver.SendMessage("OnCompleted", platform, SendMessageOptions.DontRequireReceiver);
		}

		if (onCompleted != null) {
			onCompleted(platform);
		}
	}

	private void Cancelled() {
		if (eventReceiver != null) {
			eventReceiver.SendMessage("OnCancelled", SendMessageOptions.DontRequireReceiver);
		}

		if (onCancelled != null) {
			onCancelled();
		}
	}

#endregion



#region Utils

	/// <summary>
	/// Initialize the native module at scene start
	/// </summary>
	private static void Prewarm() {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			#if UNITY_IPHONE
			_prewarm();
			#endif
		}
	}

	private static void AddCustomPlatform(string platformClass) {
		if (string.IsNullOrEmpty(platformClass)) {
			return;
		}

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			#if UNITY_IPHONE
			_addCustomActivity(platformClass);
			#endif
		}
	}

	private string AddPrefix(string prefix, string url) {
		if (url.Substring(0, prefix.Length) != prefix) {
			url = prefix+url;
		}
		return url;
	}

#endregion



#region Native 

	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void _share(string message, string url, string filePath, int platforms);

	[DllImport("__Internal")]
	private static extern void _postToTwitter(string message, string url, string imagePath);

	[DllImport("__Internal")]
	private static extern void _postToFacebook(string message, string url, string imagePath);
		
	[DllImport("__Internal")]
	private static extern void _prewarm();

	[DllImport("__Internal")]
	private static extern void _addCustomActivity(string activityClass);

	[DllImport("__Internal")]
	private static extern bool _isFacebookConnected();

	[DllImport("__Internal")]
	private static extern bool _isTwitterConnected();
	#endif
	
#endregion



}
