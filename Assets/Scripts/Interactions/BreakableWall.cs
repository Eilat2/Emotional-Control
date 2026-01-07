using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public ParticleSystem wallFragmentsPrefab;
    public string angerPlayerTag = "AngerPlayer";

    private bool isBroken = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBroken) return;

        if (!collision.gameObject.CompareTag(angerPlayerTag))
            return;

        isBroken = true;

        Vector2 hitPoint = collision.GetContact(0).point;

        if (wallFragmentsPrefab != null)
        {
            ParticleSystem ps = Instantiate(
                wallFragmentsPrefab,
                hitPoint,
                Quaternion.identity
            );

            ps.Play();

            Destroy(
                ps.gameObject,
                ps.main.duration + ps.main.startLifetime.constantMax + 0.2f
            );
        }

        Destroy(gameObject);
    }
}
