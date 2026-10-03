using UnityEngine;

public class ActiverRigidBody : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbodyAutreBalle;
    [SerializeField] Rigidbody rigidbodyGrosMoulin;

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
            Invoke(nameof(ActiverRigidbody), 3f); // Balles tombent si tu prends trop de temps
        }
    }

    void ActiverRigidbody()
    {
        rigidbodyAutreBalle.isKinematic = false;
    }

    public void FaireTomberMoulin()
    {
        rigidbodyGrosMoulin.isKinematic = false;
    }
}
