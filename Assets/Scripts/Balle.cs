using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class Balle : MonoBehaviour
{

    // [Header("État de jeu")]
    public float angle = 0;




    [Header("Paramètres de tir")]
    [SerializeField] float forceTir = 0f;
    [SerializeField] Rigidbody rigidbody;
    LineRenderer lineRendererBalle;


    [Header("Gauge de force")]
    [SerializeField] float incrementForce = 0.1f;
    // [SerializeField] GameObject jaugeForceGO;
    [SerializeField] Slider jaugeForce;
    [SerializeField] float forceMin = 0f;
    [SerializeField] float forceMax = 100f;

    [Header("Input Actions")]
    [SerializeField] InputAction tirAction;
    [SerializeField] InputAction angleAction;


    // [Header("Composant")]


    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        lineRendererBalle = GetComponent<LineRenderer>();
    }

    void Update()
    {

        angle += angleAction.ReadValue<float>();
        Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        lineRendererBalle.SetPosition(0, transform.position);
        lineRendererBalle.SetPosition(1, transform.position + direction);



        
 




        if (tirAction.WasPressedThisFrame())
        {
            //Quand la touche est enfoncée une fois au début
            forceTir = 0f;
            jaugeForce.value = forceTir;
        }

        if (tirAction.IsPressed())
        {
            //Quand la touche est enfoncée en continu
            forceTir += incrementForce;
            forceTir = Mathf.Clamp(forceTir, forceMin, forceMax);
            jaugeForce.value = forceTir;
        }

        if (tirAction.WasReleasedThisFrame())
        {
            //Quand la touche est relâchée
            rigidbody.AddForce(direction * forceTir * Time.deltaTime, ForceMode.Impulse);


            forceTir = 0;
            jaugeForce.value = forceTir;

        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "terrain") {
            Debug.Log("ouch ur out");
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "trou") {
            Debug.Log("in da hole");
        }
    }

    // ===================
    void FrapperBalle()
    {

    }

    void MettreAJourUI()
    {
    }

    // IEnumerator FinJeu()
    // {

    // }

    void SauvegarderScore()
    {

    }

    //=================================
    // Gestion des inputs actions
    void OnEnable()
    {
        tirAction.Enable();
        angleAction.Enable();
    }

    void OnDisable()
    {
        tirAction.Disable();
        angleAction.Disable();
    }
}
