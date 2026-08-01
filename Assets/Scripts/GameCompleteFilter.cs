using System.Collections;
using UnityEngine;

public class GameCompleteFilter : MonoBehaviour
{
    public Material filterMaterial;
    public float fadeDuration = 2f;

    void Start()
    {
        // Start the fade as soon as GameCompleteScene loads
        StartCoroutine(FadeInFilter());
    }

    IEnumerator FadeInFilter()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            filterMaterial.SetFloat("_FilterStrength", strength);
            yield return null;
        }

        filterMaterial.SetFloat("_FilterStrength", 1f);
    }

    void OnDestroy()
    {
        // Reset back to 0 when leaving the scene so other scenes stay normal
        if (filterMaterial != null)
        {
            filterMaterial.SetFloat("_FilterStrength", 0f);
        }
    }
}