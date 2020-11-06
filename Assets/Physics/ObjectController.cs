using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private PlayerPhysics phy;
    public float movementCap = 1;
    public float speed;
    public Transform forward;
    public Transform back;

    void Start()
    {
        phy = GetComponent<PlayerPhysics>();
    }

    public void Move(int dir)
    {
        Vector3 direction = Vector3.zero;
        if(dir == 0){
            direction = transform.position - forward.position;
        } else {
            direction = transform.position - back.position;
        } 

        Vector3 force = direction * speed;
        Vector3 vel = phy.m_rigidbody.velocity;
        Vector3 newVel = vel + force;

        phy.m_rigidbody.velocity = newVel;
    }

    public void Jump()
    {

    }
}
