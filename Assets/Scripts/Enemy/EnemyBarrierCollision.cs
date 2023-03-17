using UnityEngine;

public class EnemyBarrierCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Enemy barrier collision with " + collision.gameObject.tag);
    }
}