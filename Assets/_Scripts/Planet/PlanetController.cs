using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour
{
	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.name == "Player") {
			SoundFXPlayer.PlayCrashSFX ();
			PlayerController.PlayerExplosion ();
			GameScreenManager.playerHasCrashed = true;
		}
	}
}
