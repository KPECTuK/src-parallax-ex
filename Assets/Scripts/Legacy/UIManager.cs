using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

/// <summary> this script manages UI buttons and functions </summary>
[Guid("AFA987A8-B08D-4F82-9FA3-41F227D783CA")]
public class UIManager : MonoBehaviour
{
	public GameObject SettingsPanel;
	public SceneManager sceneManager;

	public GameObject deviceSettingsPanel;
	public Slider IPDSlider;
	public Text eyeInfoText;
	public Text IPDValueText;
	//public Text IPDLabelText;
	//public GameObject HighIPDRangeToggle;

	public HeadTrackManager headTrackManager;
	public CameraManager camManager;

	public GameObject RequireIPhoneXPanel;
	public GameObject HelpPanel;

	public Button Scene0; // box scene
	public Button Scene1; // void
	public Button Scene2; // beeple
	public Button Scene3;
	public Button Scene4;

	public GameObject theVoidBrightnessSlider;
	public GameObject theVoidBrightnessLabel;

	public GameObject ErrorPanel;
	public Text ErrorText;

	private bool _settingsVisible;
	private int _activeScene;

	private void Start()
	{
		var deviceIsIphoneX =
			Device.generation == DeviceGeneration.iPhoneX ||
			Device.generation == DeviceGeneration.iPhoneXS ||
			Device.generation == DeviceGeneration.iPhoneXR;

		if(!deviceIsIphoneX)
		{
			RequireIPhoneXPanel.SetActive(true);
		}

		var sceneButtons = new Button[5];
		sceneButtons[0] = Scene0;
		sceneButtons[1] = Scene1;
		sceneButtons[2] = Scene2;
		sceneButtons[3] = Scene3;
		sceneButtons[4] = Scene4;

		var noScenes = sceneManager.GetNoScenes();
		for(var index = 0; index < sceneButtons.Length; index++)
		{
			if(index < noScenes)
			{
				sceneButtons[index].GetComponentInChildren<Text>().text = sceneManager.GetSceneName(index);
			}
			else
			{
				sceneButtons[index].gameObject.SetActive(false);
			}
		}
	}

	private void Update()
	{
		if(headTrackManager.ARError != null)
		{
			ErrorPanel.SetActive(true);
			ErrorText.text = $"AR Session Error: {headTrackManager.ARError}";

			if(headTrackManager.ARError.StartsWith("Camera access"))
			{
				ErrorText.text += "\n\nGo into the phone's Settings. Scroll down to TheParallaxView and enable camera access.";
			}
		}
		else
		{
			ErrorPanel.SetActive(false);
		}

		if(_settingsVisible)
		{
			IPDValueText.text = $"{(int)headTrackManager.IPD} mm";
			eyeInfoText.text = headTrackManager.eyeInfoText;

			if(camManager.DeviceCamUsed)
			{
				deviceSettingsPanel.SetActive(true);
				/*IPDSlider.gameObject.SetActive (true);
				IPDValueText.enabled = true;
				IPDLabelText.enabled = true;
				HighIPDRangeToggle.SetActive (true);*/
			}
			else
			{
				deviceSettingsPanel.SetActive(false);
				/*IPDSlider.gameObject.SetActive (false);
				IPDValueText.enabled = false;
				IPDLabelText.enabled = false;
				HighIPDRangeToggle.SetActive (false);*/
			}

			if(_activeScene == 1)
			{
				// the void
				theVoidBrightnessLabel.SetActive(true);
				theVoidBrightnessSlider.SetActive(true);
			}
			else
			{
				theVoidBrightnessLabel.SetActive(false);
				theVoidBrightnessSlider.SetActive(false);
			}
		}
	}

	public void ToggleSettingsVisible()
	{
		_settingsVisible = !_settingsVisible;
		SettingsPanel.SetActive(_settingsVisible);
	}

	public void HighIPDRangeToggleFunction(bool active)
	{
		if(active)
		{
			IPDSlider.maxValue = 200f;
			IPDSlider.minValue = 0f;
		}
		else
		{
			IPDSlider.maxValue = 80f;
			IPDSlider.minValue = 50f;
		}
	}

	public void ReadArticle()
	{
		Application.OpenURL("http://anxious-bored.com/TPV");
	}

	public void SetScene0()
	{
		sceneManager.SetActiveScene(0);
		_activeScene = 0;
	}

	public void SetScene1()
	{
		sceneManager.SetActiveScene(1);
		_activeScene = 1;
	}

	public void SetScene2()
	{
		sceneManager.SetActiveScene(2);
		_activeScene = 2;
	}

	public void SetScene3()
	{
		sceneManager.SetActiveScene(3);
		_activeScene = 3;
	}

	public void SetScene4()
	{
		sceneManager.SetActiveScene(4);
		_activeScene = 4;
	}

	//public void SetBoxScene() {
	//	BoxScene.SetActive (true);
	//	TheVoidScene.SetActive (false);
	//	BeepleScene.SetActive (false);
	//	RenderSettings.ambientLight = new Color (0.23f, 0.23f, 0.23f, 1.0f);
	//}

	//public void SetTheVoidScene() {
	//	BoxScene.SetActive (false);
	//	TheVoidScene.SetActive (true);
	//	BeepleScene.SetActive (false);
	//	RenderSettings.ambientLight = new Color (0.83f, 0.83f, 0.83f, 1.0f);
	//}

	//public void SetBeepleScene() {
	//	BoxScene.SetActive (false);
	//	TheVoidScene.SetActive (false);
	//	BeepleScene.SetActive (true);
	//	RenderSettings.ambientLight = new Color (0.23f, 0.23f, 0.23f, 1.0f);

	public void TryAnyway()
	{
		RequireIPhoneXPanel.SetActive(false);
	}

	public void DismissHelp()
	{
		HelpPanel.SetActive(false);
	}

	public void DisableSleep(bool disableSleep)
	{
		Screen.sleepTimeout = disableSleep
			? SleepTimeout.NeverSleep
			: SleepTimeout.SystemSetting;
	}
}
