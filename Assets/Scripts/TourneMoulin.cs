using UnityEngine;

public class TourneMoulin : MonoBehaviour
{
    [SerializeField] float vitesse = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * vitesse * Time.deltaTime, Space.Self);
    }
}
