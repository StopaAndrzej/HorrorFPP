using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainJoiner : MonoBehaviour
{
    void Awake()
    {
        this.GetComponent<CharacterJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
    }
}
