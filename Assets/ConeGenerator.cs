using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _radius = 100;
    [SerializeField] private int _count = 100;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _count; i++)
        {
            var cone = Instantiate<GameObject>(_prefab);

            var randomCircle = Random.insideUnitCircle * _radius;
            Vector3 pos = transform.position + new Vector3(randomCircle.x, transform.position.y, randomCircle.y);
            RaycastHit hit;
            if (Physics.Raycast(new Ray(pos, Vector3.down), out hit, 100))
            {
                cone.transform.position = hit.point;
                cone.transform.up = hit.normal;
            }
            else
            {
                cone.transform.position = pos;
            }




        }
        
    }

}
