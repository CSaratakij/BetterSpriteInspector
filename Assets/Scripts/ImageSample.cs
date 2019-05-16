using UnityEngine;

public class ImageSample : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite[] images;

    public Sprite[] Images => images;


    void Update()
    {
        ShowFirstSprite();
    }

    void ShowFirstSprite()
    {
        if (images.Length > 0)
        {
            spriteRenderer.sprite = images[0];
        }
    }
}
