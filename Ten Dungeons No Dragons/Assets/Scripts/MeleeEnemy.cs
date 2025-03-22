using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public class MeleeEnemy : EnemyBase
{
    public int meleeDamage;

    public float meleeDistance = 1f;
    public float meleeRadius = 1f;

    public UnityEvent OnMelee;

    protected override void Attack()
    {
        OnMelee.Invoke();
        Debug.Log(gameObject.name +  " Melee Attack");
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + (directionVector * meleeDistance), meleeRadius); //Melee attack range

        foreach (Collider2D collider in colliders)
        {
            if (collider.transform.TryGetComponent<IHittable>(out IHittable hittable))
            {
                hittable.Hit(meleeDamage);
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((transform.position + (Vector3)(directionVector * meleeDistance)), meleeRadius);
    }
}
