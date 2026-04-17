using System;
using UnityEditor;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float coneAngle = 60;
    public GameObject cannonBall;
    public float shootAngle = 0.4f;
    public float maxDistance = 10f;
    public float coolDown = 1f;
    private float _lastShot = 0;

    public void Update()
    {
        if(_lastShot > 0) _lastShot -= Time.deltaTime;
        float minDist = maxDistance;
        GameObject target = null;
        foreach(Collider ship in Physics.OverlapSphere(transform.position, maxDistance, LayerMask.GetMask("Enemy")))
        {
            if (ship.gameObject == gameObject) continue;
            Vector3 dir = (ship.transform.position- transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if(angle <= coneAngle / 2)
            {
                float dist = Vector3.Distance(transform.position, ship.transform.position);
                if(dist < maxDistance && dist <= minDist)
                {
                   minDist = dist;
                   target = ship.gameObject; 
                }
            }
        }

        if(target == null) return;
        Shoot(target);
    }

    public Vector3 CalculateVelocity(Vector3 start, Vector3 target, float height)
    {
        float gravity = Physics.gravity.y;

        Vector3 displacement = target - start;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        float timeUp = Mathf.Sqrt(-2 * height / gravity);
        float timeDown = Mathf.Sqrt(2 * (displacement.y - height) / gravity);

        float totalTime = timeUp + timeDown;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }

    public void Shoot(GameObject target)
    {
        if(_lastShot > 0) return;
        _lastShot = coolDown;
        var projectile = Instantiate(cannonBall, transform);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 velocity = CalculateVelocity(
            transform.position,
            target.transform.position,
            shootAngle
        );

        rb.linearVelocity = velocity;
    }

        void OnDrawGizmos()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Handles.color = new Color(1, 1, 0, 0.1f);
        Handles.DrawSolidArc(
            origin,
            Vector3.up, 
            Quaternion.Euler(0, -coneAngle / 2, 0) * direction,
            coneAngle,
            maxDistance
        );

        Handles.color = Color.yellow;

        Vector3 left = Quaternion.Euler(0, -coneAngle / 2, 0) * direction * maxDistance;
        Vector3 right = Quaternion.Euler(0, coneAngle / 2, 0) * direction * maxDistance;

        Handles.DrawLine(origin, origin + left);
        Handles.DrawLine(origin, origin + right);
    }

}
