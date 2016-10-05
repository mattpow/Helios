using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Diffusion))]
public class DiffusionInspector : Editor {

	//private Diffusion diffusion;

	private SerializedProperty message, url, platform, customPlatforms, eventReceiver;

	// just to simplify the dropdown
	private DiffusionPlatform newPlatform;
	private bool twitterHidden;
	private bool urlEntered;

	void OnEnable() {
		message = serializedObject.FindProperty("message");
		url = serializedObject.FindProperty("url");

		platform = serializedObject.FindProperty("platformsToHide");
		newPlatform = (DiffusionPlatform)platform.intValue;
	
		customPlatforms = serializedObject.FindProperty("customPlatforms");

		eventReceiver = serializedObject.FindProperty("eventReceiver");
	}


	public override void OnInspectorGUI() {
		EditorStyles.textField.wordWrap = true;
		serializedObject.Update();
		twitterHidden = ((newPlatform & DiffusionPlatform.Twitter) == DiffusionPlatform.Twitter);
		urlEntered = !string.IsNullOrEmpty(url.stringValue);

		// Message box
		EditorGUILayout.LabelField("Message");
		message.stringValue = EditorGUILayout.TextField(message.stringValue, GUILayout.Height(60f) );

		// Warnings and char count
		if (string.IsNullOrEmpty(message.stringValue)) {
			EditorGUILayout.HelpBox("This is the default message, it will be attached when sharing, " +
			                        "unless it is explicitly overridden.", MessageType.Info);
		} else {
			int length = message.stringValue.Length;
			MessageType type = MessageType.None;
			if (!twitterHidden && urlEntered && length > 118) {
				type = MessageType.Warning;
			}
			EditorGUILayout.HelpBox(length.ToString(), type);
			// warn the user about twitter URL length
			if (!twitterHidden && urlEntered) {
				EditorGUILayout.HelpBox("URLs will be treated as 22 characters on Twitter due to automatic " +
				                        "URL 'shortening', even if the URL is already shorter than 22 characters.", MessageType.Warning);
			}
		}


		// URL box 
		EditorGUILayout.LabelField("URL");
		url.stringValue = EditorGUILayout.TextField(url.stringValue);
		if (!string.IsNullOrEmpty(url.stringValue)) {

			EditorGUILayout.HelpBox("The URL will be attached to the default message. This may modify " +
			                        "which platforms appear in the menu on the device.", MessageType.Info);

		}


		EditorGUILayout.Separator();
		EditorGUILayout.Separator();


		// Platform hide selector
		newPlatform = (DiffusionPlatform)EditorGUILayout.EnumMaskField("Platforms to hide", newPlatform);
		platform.intValue = (int)newPlatform;
		
		EditorGUILayout.HelpBox("The menu is generated automatically based on which platforms can " +
		                        "handle the data you want to share. This selection is to explicitly hide some options, " +
		                        "even if they are supported.", MessageType.None);


		// Custom platforms
		EditorGUILayout.PropertyField(customPlatforms, true);
		if (customPlatforms.arraySize < 1) {
			EditorGUILayout.HelpBox("Custom platforms are additional UIActivies that should be included. These are " +
				"specified by their class name.", MessageType.None);
		} else {
			EditorGUILayout.HelpBox("Make sure you create categories that include the headers for these UIActivities!", MessageType.Warning);
		}

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();


		// Event receiver
		eventReceiver.objectReferenceValue = EditorGUILayout.ObjectField("Event receiver", eventReceiver.objectReferenceValue, typeof(GameObject), true);


		serializedObject.ApplyModifiedProperties();

	}
}
