using UnityEngine;

public class DebrisAutoDestroy : MonoBehaviour
{
    [SerializeField] float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
//האובייקט שמוחק את החתיכות של השברים 