using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetData : MonoBehaviour
{
    public enum _type { sphere };
    public Transform center_of_mass;
    public float GravitationalConstant = 1f;
    public float Mass = 5f;

    public float GetDistance(PlayerPhysics _obj)
    {
        float _dis;
        _dis = (center_of_mass.position - _obj.Position).magnitude / Mass;
        return _dis;
    }

    public Vector3 calculateObjForce(PlayerPhysics _obj)
    {
        Vector3 position_1 = _obj.Position;
        Vector3 position_2 = center_of_mass.position;
        float distance = Mathf.Sqrt(Mathf.Pow(position_1.x - position_2.x, 2) + Mathf.Pow(position_1.y - position_2.y, 2) + Mathf.Pow(position_1.z - position_2.z, 2));
        float G = calculateGravitationalForce(_obj.Mass, distance);
        Vector3 direction = position_2 - position_1;
        direction.Normalize();
        direction *= G;
        return direction;
    }

    // F = G (m1m2 / r^2)
    public float calculateGravitationalForce(float mass, float distance)
    {
        float force;
        float masses = mass * this.Mass;
        force = masses / (distance * distance);
        force = GravitationalConstant * force;
        return force;
    }

}
