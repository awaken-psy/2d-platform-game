using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BackgroundType
{
    Blue,
    Brown,
    Gray,
    Green,
    Pink,
    Purple,
    Yellow
}

public class AnimatedBackground : MonoBehaviour
{
    [SerializeField] private Vector2 movementDir;
    private MeshRenderer mesh;

    [Header("Color")]
    [SerializeField] private BackgroundType backgroundType;
    [SerializeField] private Texture2D[] textures;

    private void Awake() {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.mainTextureOffset = new Vector2(0, 0);
        UpdateBackgroundTexture();
    }

    private void Update() {
        mesh.material.mainTextureOffset += movementDir * Time.deltaTime;
    }

    [ContextMenu("Update Background Texture")]
    private void UpdateBackgroundTexture() {
        if (mesh == null)
            mesh = GetComponent<MeshRenderer>();
        mesh.sharedMaterial.mainTexture = textures[(int)backgroundType];
    }
}
