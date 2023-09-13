using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed;

    private void Update()
    {
        this.gameObject.transform.Rotate( new Vector3(rotateSpeed , rotateSpeed , rotateSpeed) * Time.deltaTime );
    }
}
