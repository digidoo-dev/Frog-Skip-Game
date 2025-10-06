using UnityEngine;

public class FrogVisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer frogBodyRenderer;



    public void SetFrogColor(Color color)
    {
        frogBodyRenderer.color = color;
    }
}
