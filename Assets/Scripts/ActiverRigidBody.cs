using UnityEngine;

public class ActiverRigidBody : MonoBehaviour
{
    public static Rigidbody rigidbodyAutreBalle;
    public Rigidbody rigidbodyGrosMoulin;

    void Start()
    {
        if (rigidbodyAutreBalle)
        {
            rigidbodyAutreBalle = GetComponent<Rigidbody>();
        }
        if (rigidbodyGrosMoulin)
        {
            rigidbodyGrosMoulin = GetComponent<Rigidbody>();
            rigidbodyGrosMoulin.isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && rigidbodyAutreBalle)
        {
            Invoke(nameof(ActiverRigidbody), 6f); // Balle tombe si tu prends trop de temps
        }
    }

    void ActiverRigidbody()
    {
        if (rigidbodyAutreBalle)
        {
            rigidbodyAutreBalle.isKinematic = false;
        }
    }

    public void FaireTomberMoulin()
    {
        rigidbodyGrosMoulin.isKinematic = false;
    }
}
