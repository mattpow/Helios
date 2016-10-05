using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlUIManager : MonoBehaviour
{
	public void buttonHeld () //Detects if canvas UI Button is clicked. Checks which button was pressed and marks its click as true
	{
		if (!PlayerController.PlayerMovementIsPaused) {
			PlayerController.buttonIsHeld = true;
		}
	}

	public void buttonUnHeld () //Detects when canvas UI Button is no longer being clicked. Checks which button to turn boolean Held to false
	{
		PlayerController.buttonIsHeld = false;
	}
}