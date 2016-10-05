using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using GooglePlayGames;

public class SocialManager : MonoBehaviour
{
	public void ShowAchievements ()
	{
		#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated () == true) {
			PlayGamesPlatform.Instance.ShowAchievementsUI ();
			PlayerPrefs.SetInt ("LoggedIn", 1);
		} else {
			PlayGamesPlatform.Instance.localUser.Authenticate ((bool success) => {
				if (PlayGamesPlatform.Instance.IsAuthenticated () == true) {
					PlayGamesPlatform.Instance.ShowAchievementsUI ();
					PlayerPrefs.SetInt ("LoggedIn", 1);
				}
			});
		}
		#elif UNITY_IOS
		Social.ShowAchievementsUI ();
		#endif

		SoundFXPlayer.PlayClickSFX ();
	}

	public void ShowLeaderboard ()
	{
		#if UNITY_ANDROID
		if (PlayGamesPlatform.Instance.IsAuthenticated () == true) {
			PlayGamesPlatform.Instance.ShowLeaderboardUI ();
			PlayerPrefs.SetInt ("LoggedIn", 1);
		} else {
			PlayGamesPlatform.Instance.localUser.Authenticate ((bool success) => {
				if (PlayGamesPlatform.Instance.IsAuthenticated () == true) {
					PlayGamesPlatform.Instance.ShowLeaderboardUI ();
					PlayerPrefs.SetInt ("LoggedIn", 1);
				}
			});
		}
		#elif UNITY_IOS
		Social.ShowLeaderboardUI ();
		#endif

		SoundFXPlayer.PlayClickSFX ();
	}
}

