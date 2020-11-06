using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public List<PlanetData> _planets = new List<PlanetData>();
    public List<float> _distances = new List<float>();

    private float x_pos, y_pos, z_pos;
    private float x_rot, y_rot, z_rot;

    private Vector3 previousPosition;
    private Vector3 previousRotation;

    public Vector3 Position;
    public Vector3 Rotation;

    private float rotation_speed = 1.5f;

    public PlanetData mainGravity;

    public float Mass = 1.5f;

    [HideInInspector]
    public Rigidbody m_rigidbody;

    private Vector3 myUp;

    public Transform upTransform;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        Setup();
    }

    void FixedUpdate()
    {
        UpdatePosition();
        UpdateDistance();
        UpdateRotation();
    }

    void Setup()
    {
        SetAllPlanets();
    }

    void SetAllPlanets()
    {
        GameObject[] planetArray = GameObject.FindGameObjectsWithTag("Planetoid");
        foreach (GameObject element in planetArray)
        {
            this._planets.Add(element.GetComponent<PlanetData>());
        }
    }

    void UpdateDistance()
    {
        List<float> distances = new List<float>();
        PlanetData[] planetData = this._planets.ToArray();
        foreach(PlanetData p in planetData)
        {
            distances.Add(p.GetDistance(this));
        }
        this._distances = distances;
        findMainGrav();
    }

    void findMainGrav()
    {
        float[] distance_array = _distances.ToArray();
        int recall_index = 0;
        float store = distance_array[0];
        for (int i = 1; i < distance_array.Length; i++)
        {
            if (distance_array[i] < store)
            {
                recall_index = i;
                store = distance_array[i];
            }
        }
        mainGravity = _planets[recall_index];
    }

    void UpdatePosition()
    {
        ApplyGravity();
        Vector3 transformPos = transform.position;
        float x = transformPos.x,
            y = transformPos.y,
            z = transformPos.z;
        x_pos = x;
        y_pos = y;
        z_pos = z;
        Vector3 tempPosition = new Vector3(x_pos, y_pos, z_pos);
        this.previousPosition = this.Position;
        this.Position = tempPosition;
    }

    void UpdateRotation()
    {
        previousRotation = Rotation;
        /*Rotation = myUp;
        Quaternion euler = Quaternion.Euler(0,0,Rotation.z);
        transform.rotation = euler;*/
        Vector3 p1 = transform.position;
        Vector3 p2 = mainGravity.gameObject.transform.position;
        float z = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * 180 / Mathf.PI;
        z += 90;
        transform.rotation = Quaternion.Euler(0, 0, z);
    }

    void ApplyGravity()
    {
        if (!mainGravity)
            return;
        Vector3 force = mainGravity.calculateObjForce(this);
        m_rigidbody.velocity = force;
        myUp = -force.normalized;  
    }

}
