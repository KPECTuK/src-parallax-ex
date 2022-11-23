using UnityEngine;

public class Vibranium : MonoBehaviour
{
	public GameObject sphere;
	public int noSpheresX = 20;
	public int noSpheresY = 3;
	public int noSpheresZ = 3;
	public float size = 1.0f;
	public float sizeRnd = 0.5f;
	public float sizePerlin = 0.5f;
	public float ampPerlin = 0.5f;
	public float freqPerlin = 0.5f;
	public float speedPerlin = 1.0f;
	public float ampVibrate = 0.5f;
	public float speedVibrate = 0.5f;
	public float phaseVibrate = 0.1f;

	public Material mat1;
	public Material mat2;
	public Material mat3;

	private GameObject[] _spheres;

	private void Start()
	{
		_spheres = new GameObject[noSpheresX * noSpheresY * noSpheresZ];
		var i = 0;
		for(var x = 0; x < noSpheresX; x++)
		{
			for(var y = 0; y < noSpheresY; y++)
			{
				for(var z = 0; z < noSpheresZ; z++)
				{
					_spheres[i] = Instantiate(sphere, transform);
					_spheres[i].transform.localPosition = new Vector3(x, y, z);
					_spheres[i].transform.localScale = new Vector3(size, size, size);

					if(z < 2)
					{
						_spheres[i].GetComponent<Renderer>().material = mat3;
					}
					else if(z < 5)
					{
						_spheres[i].GetComponent<Renderer>().material = mat2;
					}
					else
					{
						_spheres[i].GetComponent<Renderer>().material = mat1;
					}

					i++;
				}
			}
		}
	}

	private void Update()
	{
		var i = 0;

		for(var x = 0; x < noSpheresX; x++)
		{
			for(var y = 0; y < noSpheresY; y++)
			{
				for(var z = 0; z < noSpheresZ; z++)
				{
					var pos = new Vector3(x, y, z);

					pos.x += ampPerlin * VfxPerlin.Noise(x * freqPerlin + speedPerlin * Time.time, y * freqPerlin, z * freqPerlin);
					pos.y += ampPerlin * VfxPerlin.Noise(x * freqPerlin + speedPerlin * Time.time, y * freqPerlin, z * freqPerlin + 13.2f);
					pos.z += ampPerlin * VfxPerlin.Noise(x * freqPerlin + speedPerlin * Time.time, y * freqPerlin, z * freqPerlin + 49.9f);

					var scaleP = 1f + sizePerlin * VfxPerlin.Noise(x * freqPerlin + speedPerlin * Time.time, y * freqPerlin, z * freqPerlin + 121.3f);

					pos.x += (0.9f * scaleP + 0.1f) * ampVibrate * Mathf.Sin(speedVibrate * Time.time + phaseVibrate * i);
					pos.y += (0.9f * scaleP + 0.1f) * ampVibrate * Mathf.Cos(speedVibrate * Time.time + phaseVibrate * i);
					pos.z += (0.9f * scaleP + 0.1f) * ampVibrate * Mathf.Cos(speedVibrate * Time.time + 0.5f * Mathf.PI + phaseVibrate * i);

					_spheres[i].transform.localPosition = pos;

					var scale = new Vector3(size, size, size);
					scale *= scaleP;

					_spheres[i].transform.localScale = scale;

					i++;
				}
			}
		}
	}
}
