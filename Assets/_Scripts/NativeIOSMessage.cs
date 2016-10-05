using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class NativeIOSMessage
{
	#if UNITY_IOS
	[DllImport ("__Internal")]
	extern static public void showNativeAlert (string title, string message);


	public static void showMessage (string title, string message)
	{
		showNativeAlert (title, message);
	}
	#endif
}