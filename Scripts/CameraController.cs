using UnityEngine;
using Cinemachine;

/// <summary>
/// This script was made to manage camera shaking effects.
/// </summary>
public class CameraController : MonoBehaviour
{
    // These variables were made public to be accessed by scripts and modified from the editor
    [Range(0, 10f)]
    public float cameraShakeFrequency = 1f;

    [Range(0, 10f)]
    public float cameraShakeAmplitude = 1f;

    [Range(0, 5f)]
    public float cameraShakeDuration = 1f;

    // Boolean used to time shake
    bool isShaking;
    float shakeTimeElaspsed = 0;

    // Perlin noise used to create random shake effect
    CinemachineVirtualCamera vCam;
    CinemachineBasicMultiChannelPerlin perlinNoise;

    // Initialize perlin noise and vcam variables
    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        perlinNoise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Keep track of passage of time and stop shake when duration is finished
    private void Update()
    {
        shakeTimeElaspsed += Time.deltaTime;

        if (shakeTimeElaspsed > cameraShakeDuration)
        {
            StopShake();
        }
    }

    // Function used to create camera shakes with custom amplitude and frequency
    public void ShakeCamera(float ShakeAmplitude, float ShakeFrequency)
    {
        if (!isShaking)
        {
            shakeTimeElaspsed = 0;
            perlinNoise.m_AmplitudeGain = ShakeAmplitude;
            perlinNoise.m_FrequencyGain = ShakeFrequency;
            isShaking = true;
        }
    }

    // Resets shake amplitude and frequency to zero
    public void StopShake()
    {
        perlinNoise.m_AmplitudeGain = 0;
        perlinNoise.m_FrequencyGain = 0;
        isShaking = false;
    }
}
