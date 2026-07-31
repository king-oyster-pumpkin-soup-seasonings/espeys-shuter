using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float speed;
    [SerializeField] private Renderer bgRenderer;

    void Start()
    {
        if (speed == 0) speed = 3;
    }

    // Update is called once per frame
    void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(0, speed * Time.deltaTime);
    }
}
