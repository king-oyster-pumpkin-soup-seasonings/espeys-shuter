using System.Collections;
using UnityEngine;

public class GameCompleteFilter : MonoBehaviour
{
    [SerializeField] private Material filterMaterial;
    [SerializeField] private float fadeDuration;

    void Start()
    {
        if (fadeDuration == 0) fadeDuration = 2f;
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
        if (filterMaterial != null)
        {
            filterMaterial.SetFloat("_FilterStrength", 0f);
        }
    }
}