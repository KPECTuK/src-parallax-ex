using System.Collections.Generic;
using UnityEngine;

/// <summary> report the scene names to the UI, and set active scene </summary>
public class SceneManager : MonoBehaviour
{
	public Light headLight;
	public Material TheVoidMaterial;
	public Camera EyeCam;
	public VfxPostBlur pblur;

	private List<GameObject> _scenes;

	private void Awake()
	{
		_scenes = new List<GameObject>();

		foreach(Transform child in transform)
		{
			var info = child.GetComponent<SceneInfo>();
			if(info.use)
			{
				_scenes.Add(child.gameObject);
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	public int GetNoScenes()
	{
		return _scenes.Count;
	}

	public string GetSceneName(int sceneNo)
	{
		return _scenes[sceneNo].GetComponent<SceneInfo>().sceneName;
	}

	public void SetActiveScene(int sceneNo)
	{
		for(var index = 0; index < _scenes.Count; index++)
		{
			var info = _scenes[index].GetComponent<SceneInfo>();
			if(index == sceneNo)
			{
				_scenes[index].SetActive(true);
				RenderSettings.ambientLight = info.ambientLight;
				headLight.gameObject.SetActive(info.headLight);
				EyeCam.backgroundColor = info.bgColor;

				//! this should be part of scene info
				if(sceneNo == 3)
				{
					pblur.enabled = true;
					pblur.Activate();
				}
				else
				{
					pblur.DeActivate();
					pblur.enabled = false;
				}
			}
			else
			{
				_scenes[index].SetActive(false);
			}
		}
	}

	/// <summary> kinda hacky, just sets the brightness of one scene: "the void" </summary>
	public void TheVoidSetBrightness(float value)
	{
		// 0.33 is default, means white albedo

		var b = 3.0f * value;
		var col = new Color(b, b, b, 1f);
		TheVoidMaterial.color = col;
	}
}
