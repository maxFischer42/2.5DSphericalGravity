using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ObjectController obj;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        if (x != 0)
        {
            obj.Move((int)x);
        }
    }
}
