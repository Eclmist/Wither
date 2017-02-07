using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Special effects for the camera
/// </summary>
public class Effects : MonoBehaviour {

    public static Effects instance;

    // Default camera shake properties
    private const float SHAKE_DURATION = 0.5f;
    private const float SHAKE_INTENSITY = 0.2f;

    void Awake()
    {
        instance = this;
    }

    public void ShakeCamera()
    {
        StartCoroutine("Shake");
    }

    // Overloaded camera shake
    public void ShakeCamera(float shakeDuration, float shakeIntensity)
    {
        StartCoroutine(Shake(shakeDuration, shakeIntensity));
    }

    public void ShakeCameraRelative(float shakeDuration, float shakeIntensity)
    {
        StartCoroutine(ShakeRelative(shakeDuration, shakeIntensity));
    }

    // Default shake
    IEnumerator Shake()
    {

        float timeElapsed = 0.0f;

        // Store the original camera position
        Vector3 originalCamPos = Camera.main.transform.position;


        while (timeElapsed < SHAKE_DURATION)
        {

            timeElapsed += Time.deltaTime;


            // As the shake approaches the end, the smooth value increases
            // The smooth value will be multiplied to the shake intensity
            // This will slowly take away the shake intensty over time so it doesn't look like it snaps
            float percentComplete = timeElapsed / SHAKE_DURATION;
            float smooth = 1.0f - Mathf.Clamp(percentComplete, 0.0f, 1.0f);

            // Multiplying by ( *2 - 1 ) gets a range from -1 to 1
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;

            x *= SHAKE_INTENSITY * smooth;
            y *= SHAKE_INTENSITY * smooth;

            transform.position = new Vector3(x + originalCamPos.x, y + originalCamPos.y, originalCamPos.z);


            yield return null;
        }

        transform.position = originalCamPos;
    }

    IEnumerator Shake(float shakeDuration, float shakeIntensity)
    {

        float timeElapsed = 0.0f;

        // Store the original camera position
        Vector3 originalCamPos = Camera.main.transform.position;


        while (timeElapsed < shakeDuration)
        {

            timeElapsed += Time.deltaTime;


            // As the shake approaches the end, the smooth value increases
            // The smooth value will be multiplied to the shake intensity
            // This will slowly take away the shake intensty over time so it doesn't look like it snaps
            float percentComplete = timeElapsed / shakeDuration;
            float smooth = 1.0f - Mathf.Clamp(percentComplete, 0.0f, 1.0f);

            // Multiplying by ( *2 - 1 ) gets a range from -1 to 1
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;

            x *= shakeIntensity * smooth;
            y *= shakeIntensity * smooth;

            transform.position = new Vector3(x + originalCamPos.x, y + originalCamPos.y, originalCamPos.z);


            yield return null;
        }

        transform.position = originalCamPos;
    }
   
    IEnumerator ShakeRelative(float shakeDuration, float shakeIntensity)
    {

        float timeElapsed = 0.0f;

        
        while (timeElapsed < shakeDuration)
        {

            timeElapsed += Time.deltaTime;


            // As the shake approaches the end, the smooth value increases
            // The smooth value will be multiplied to the shake intensity
            // This will slowly take away the shake intensty over time so it doesn't look like it snaps
            float percentComplete = timeElapsed / shakeDuration;
            float smooth = 1.0f - Mathf.Clamp(percentComplete, 0.0f, 1.0f);

            // Multiplying by ( *2 - 1 ) gets a range from -1 to 1
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            float z = Random.value * 2.0f - 1.0f;

            x *= shakeIntensity * smooth;
            y *= shakeIntensity * smooth;
            z *= shakeIntensity * smooth;

            Vector3 temp = new Vector3(x,y,z);

            GetComponent<CameraFollow>().SetShakeVector(temp * 30);


            yield return null;
        }

        // Set it to a zero vector
        GetComponent<CameraFollow>().SetShakeVector(Vector3.zero);
    }
}
