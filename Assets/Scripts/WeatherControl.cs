// author: Marcus Xie
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fog is always on
public class WeatherControl : MonoBehaviour
{
    public bool toRain = false;
    public bool toSnow = false;
    public float linearFogStart = 8.6f;
    public float linearFogEnd = 21.4f;
    public float exponentialFogDensity = 0.04f;
    public float darkestDirLightIntensity = 0.5f;
    public GameObject DirLightObject;
    public AnimationCurve lightningCurve;
    public AudioClip[] audioClips;
    public Material[] snowMats;

    private AudioSource audioSource;
    private Transform rainManager;
    private RainBox rainBox;
    private Light dirLight;
    private float originalDirLightIntensity;
    private Camera cam;
    private Material spppMat;// material of singlePixelPostProcess
    private Transform snowParticles;
    private const float startSnowTime = 8f;
    private const float startRainTime = 8f;
    private float fogThin;

    void Start()
    {
        rainManager = transform.Find("rainManager");
        rainBox = transform.GetComponentInChildren<RainBox>();
        dirLight = DirLightObject.GetComponent<Light>();
        originalDirLightIntensity = dirLight.intensity;
        cam = transform.GetComponent<Camera>();
        audioSource = transform.GetComponent<AudioSource>();
        spppMat = transform.Find("singlePixelPostProcess").GetComponent<Renderer>().sharedMaterial;
        snowParticles = transform.Find("snowParticles");

        if(toSnow)
            StartCoroutine(StartSnowing());
        if(toRain)
            StartCoroutine(StartRaining());
    }

    void Update()
    {
    }

    IEnumerator LightningControl()
    {
        while (true)
        {
            StartCoroutine(Lightning(6f));
            audioSource.PlayOneShot(audioClips[1]);
            yield return new WaitForSeconds(32.88f);
        }
    }

    //light up whole screen during the lightning
    IEnumerator Lightning(float time)
    {
        yield return new WaitForSeconds(2.375f - 0.2f);
        float i = 0f;
        float rate = 1f / time;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            // synchronize the lightning with its sound, and the random value is to make the lightning shiver
            float intense = lightningCurve.Evaluate(i) * (1f + Random.Range(-0.2f, 0.4f));
            // use this color to control lightning and darken the environment
            Color lightningColor = new Color(0.7f * intense, 0.75f * intense, 1.0f * intense, 0.5f);
            // spppMat is the material of the whole screen quad, here we change its output color
            // by the way, "sppp" stands for "Single Pixel Post Processing"
            spppMat.SetColor("_Color", lightningColor);
            yield return 0;
        }
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 0.5f));
    }

    IEnumerator StartSnowing()
    {
        // initialization
        audioSource.enabled = false;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = exponentialFogDensity;
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 0f);
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 1.0f));
        rainManager.gameObject.SetActive(false);
        snowParticles.gameObject.SetActive(true);
        int snowMatsLength = snowMats.Length;
        for (int n = 0; n < snowMatsLength; n++)
            snowMats[n].SetFloat("_SnowHeavy", 0f);

        // wait for the snow particles to reach the camera
        yield return new WaitForSeconds(7f);

        // progressively intensify the snow
        float i = 0f;
        float rate = 1f / startSnowTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, i);
            for (int n = 0; n < snowMatsLength; n++)
                snowMats[n].SetFloat("_SnowHeavy", i);
            yield return 0;
        }

        // terminal status
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 1f);
        for (int n = 0; n < snowMatsLength; n++)
            snowMats[n].SetFloat("_SnowHeavy", 1f);
    }

    IEnumerator StartRaining()
    {
        // initialization
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        rainManager.gameObject.SetActive(true);
        rainBox.rainSparsity = Mathf.Lerp(5f, 1f, 0f);
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 0f);
        fogThin = Mathf.Lerp(2f, 1f, 0f);
        RenderSettings.fogStartDistance = linearFogStart * fogThin;
        RenderSettings.fogEndDistance = linearFogEnd * fogThin;
        cam.farClipPlane = RenderSettings.fogEndDistance;
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 1.0f));

        int snowMatsLength = snowMats.Length;
        for (int n = 0; n < snowMatsLength; n++)
            snowMats[n].SetFloat("_SnowHeavy", 0f);

        audioSource.enabled = true;
        audioSource.PlayOneShot(audioClips[0]);
        audioSource.volume = 0f;

        // progressively intensify the rain
        float i = 0f;
        float rate = 1f / startRainTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            rainBox.rainSparsity = Mathf.Lerp(5f, 1f, i) ;
            dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, i);
            fogThin = Mathf.Lerp(2f, 1f, i);
            RenderSettings.fogStartDistance = linearFogStart * fogThin;
            RenderSettings.fogEndDistance = linearFogEnd * fogThin;
            spppMat.SetColor("_Color", new Color(0f, 0f, 0f, (0.5f + 0.5f * (1.0f - i))));
            audioSource.volume = i;
            yield return 0;
        }

        // terminal status
        rainBox.rainSparsity = Mathf.Lerp(5f, 1f, 1f);
        dirLight.intensity = Mathf.Lerp(originalDirLightIntensity, darkestDirLightIntensity, 1f);
        fogThin = Mathf.Lerp(2f, 1f, 1f);
        RenderSettings.fogStartDistance = linearFogStart * fogThin;
        RenderSettings.fogEndDistance = linearFogEnd * fogThin;
        spppMat.SetColor("_Color", new Color(0f, 0f, 0f, 0.5f));

        // start to repeatedly play lightning
        StartCoroutine(LightningControl());
    }
}
