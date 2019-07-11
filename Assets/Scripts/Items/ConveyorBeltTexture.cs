using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltTexture : MonoBehaviour
{
    // Scroll main texture based on time

    public float scrollSpeed = 0.5f;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = scrollSpeed * Time.time;
        rend.materials[1].SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }

}
