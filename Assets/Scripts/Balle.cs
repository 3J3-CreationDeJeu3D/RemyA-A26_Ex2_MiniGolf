using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro.EditorUtilities;

public class Balle : MonoBehaviour
{

    // [Header("État de jeu")]
    Vector3 positionBalle;
    public GameObject startPoint;
    int coups = 0;

    [SerializeField] bool peutJouer;


    public float angle = 0;

    [Header("Paramètres de tir")]
    [SerializeField] float forceTir = 10f;
    [SerializeField] Rigidbody rigidbodyBalle;
    LineRenderer lineRendererBalle;


    [Header("Gauge de force")]
    [SerializeField] float incrementForce = 1f;
    // [SerializeField] GameObject jaugeForceGO;
    [SerializeField] Slider jaugeForce;
    [SerializeField] float forceMin = 0f;
    [SerializeField] float forceMax = 100f;

    [Header("Input Actions")]
    [SerializeField] InputAction tirAction;
    [SerializeField] InputAction angleAction;


    [Header("Sons")]
    AudioSource audiosourceBalle;
    [SerializeField] AudioClip sonFin;
    [SerializeField] AudioClip sonErreur;

    [Header("UI")]
    [SerializeField] TMP_Text texteCoups;




    void Start()
    {
        rigidbodyBalle = GetComponent<Rigidbody>();
        lineRendererBalle = GetComponent<LineRenderer>();
        audiosourceBalle = GetComponent<AudioSource>();
        coups = 0;
        texteCoups.text = $"{coups} coup";
        peutJouer = true;

        positionBalle = startPoint.transform.localPosition;

        //if (PlayerPrefs.HasKey("positionBalle"))
        //{
        //    string positionJSON = PlayerPrefs.GetString("positionBalle"); 
        //    transform.position = JsonUtility.FromJson<Vector3>(positionJSON); // whaaat
        //}
    }


    void Update()
    {
        if (peutJouer && GestJeu.instance.etat == EtatJeu.Jeu)
        {
            angle += angleAction.ReadValue<float>() * 3f;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            lineRendererBalle.SetPosition(0, transform.position);
            lineRendererBalle.SetPosition(1, transform.position + direction);


            if (lineRendererBalle.enabled)
            {

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
                    coups++;
                    if (coups == 1)
                    {
                        texteCoups.text = $"{coups} coup";
                    }
                    else
                    {
                        texteCoups.text = $"{coups} coups";
                    }

                    positionBalle = transform.position;

                    //Stocker une position avec JSON
                    //string positionJSON = JsonUtility.ToJson(positionBalle);
                    //PlayerPrefs.SetString("positionBalle", positionJSON);


                    //Quand la touche est relâchée
                    rigidbodyBalle.AddForce(direction * forceTir * Time.deltaTime, ForceMode.Impulse);

                    StartCoroutine(VerifierApresTir());
                    forceTir = 0;
                    jaugeForce.value = forceTir;
                }
            }
        }

    }

    IEnumerator VerifierApresTir()
    {
        peutJouer = false;

        yield return new WaitForFixedUpdate();

        while (rigidbodyBalle.linearVelocity.magnitude > 1f)
        {
            lineRendererBalle.enabled = false;
            yield return null;
        }

        peutJouer = true;
        Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        lineRendererBalle.SetPosition(0, transform.position);
        lineRendererBalle.SetPosition(1, transform.position + direction);
        lineRendererBalle.enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "horsParcours")
        {
            rigidbodyBalle.linearVelocity = Vector3.zero;
            transform.position = positionBalle;
            audiosourceBalle.PlayOneShot(sonErreur);
            //Debug.Log("ouch ball's out");
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "trou")
        {
            rigidbodyBalle.linearVelocity = Vector3.zero;
            rigidbodyBalle.useGravity = false;
            transform.position = collision.transform.position;
            //Debug.Log("in da hole");

            // Déclenchement du son final
            audiosourceBalle.PlayOneShot(sonFin);

            PlayerPrefs.SetInt("NbCoups", coups);

            //PlayerPrefs.DeleteKey("positionBalle");

            StartCoroutine(GestJeu.instance.FinJeu());
        }
    }


    void MettreAJourUI()
    {
    }


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
