using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Multiplayer : NetworkBehaviour
{


    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
