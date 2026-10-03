using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class Balle : MonoBehaviour
{

    // [Header("État de jeu")]
    Vector3 positionBalle;
    public GameObject startPoint;
    int coups = 0;

    [SerializeField] bool peutJouer;

    private int collisionsHorsParcours = 0;
    private float tempsPremiereCollision = 0f; // Pour essayer d'empêcher la balle de convulser en spam hors parcours


    [SerializeField] private Transform drapeauVertEnHaut;
    private Vector3 positionDrapeauVertEnHaut;


    public float angle = 0;

    [Header("Paramètres de tir")]
    [SerializeField] float forceTir = 100f;
    [SerializeField] Rigidbody rigidbodyBalle;
    LineRenderer lineRendererBalle;
    [SerializeField] bool penteAbrupte = false;


    [Header("Gauge de force")]
    [SerializeField] float incrementForce = 2f;
    // [SerializeField] GameObject jaugeForceGO;
    [SerializeField] Slider jaugeForce;
    [SerializeField] float forceMin = 0f;
    [SerializeField] float forceMax = 400f;

    [Header("Input Actions")]
    [SerializeField] InputAction tirAction;
    [SerializeField] InputAction angleAction;


    [Header("Sons")]
    AudioSource audiosourceBalle;
    [SerializeField] AudioClip sonFin;
    [SerializeField] AudioClip sonErreur;
    public AudioClip nouvelleMusique;

    [Header("UI")]
    [SerializeField] TMP_Text texteCoups;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineFollow follow;
    [SerializeField] private float cameraSmooth = 1f;
    [SerializeField] private float cameraAngleOffset = 90f;
    private Vector3 baseOffset;
    private Vector3 targetOffset;




    void Start()
    {
        rigidbodyBalle = GetComponent<Rigidbody>();
        lineRendererBalle = GetComponent<LineRenderer>();
        audiosourceBalle = GetComponent<AudioSource>();
        coups = 0;
        texteCoups.text = $"{coups} coup";
        peutJouer = true;
        rigidbodyBalle.mass = 0.22f;

        positionBalle = startPoint.transform.position;
        transform.position = positionBalle;

        baseOffset = follow.FollowOffset;
        targetOffset = follow.FollowOffset;

        collisionsHorsParcours = 0;
        tempsPremiereCollision = 0f;

        penteAbrupte = false;

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
            lineRendererBalle.widthMultiplier = 0.1f; // Ajustement du lineRenderer
            angle += angleAction.ReadValue<float>() * 3f;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            lineRendererBalle.SetPosition(0, transform.position);
            lineRendererBalle.SetPosition(1, transform.position + direction);


            follow.FollowOffset = Vector3.Lerp(follow.FollowOffset, targetOffset, cameraSmooth * Time.deltaTime); // L'angle du Cinemachine qui change en continu selon la direction du mouvement de la balle


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


                    // Caméra regardant la direction du tir
                    targetOffset = Quaternion.Euler(0, angle + cameraAngleOffset, 0) * baseOffset;

                    if (!penteAbrupte)
                    {
                        StartCoroutine(VerifierApresTir());
                    }
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

        if (collision.gameObject.CompareTag("horsParcours"))
        {
            float tempsActuel = Time.time;

            if (tempsActuel - tempsPremiereCollision > 2f) // Si pendant plus de 2 secs, tu ne touches pas le gazon
            {
                collisionsHorsParcours = 0;
                tempsPremiereCollision = tempsActuel;
            }

            collisionsHorsParcours++; // Incrémente

            rigidbodyBalle.linearVelocity = Vector3.zero;
            rigidbodyBalle.angularVelocity = Vector3.zero;

            if (collisionsHorsParcours >= 2)
            {
                transform.position = startPoint.transform.position; // Sinon retour au départ si on tombe en continu
                collisionsHorsParcours = 0;
                targetOffset = baseOffset;
                coups = 0;
            }
            else
            {
                transform.position = positionBalle; // Retour dernière position enregistrée préférablement
            }

            audiosourceBalle.PlayOneShot(sonErreur, 5f);
            //Debug.Log("ouch ball's out");
        }


        if (collision.gameObject.tag == "montee") // Test d'ascension surprise (du drapeau random bleu en bas)
        {
            positionDrapeauVertEnHaut = drapeauVertEnHaut.position; // Au drapeau vert en haut
            positionBalle = positionDrapeauVertEnHaut + new Vector3(-1f, 2f, 2f); // Shortcut pour la balle
        }

        if (collision.gameObject.tag == "abrupte")
        {
            penteAbrupte = true; // J'empêche la coroutine lorsque la pente est abrupte, pour + de tirs
            incrementForce = 7f;
            forceTir = 150f;
            rigidbodyBalle.linearDamping = 0.02f;
        }
        if (collision.gameObject.tag == "anglecamera")
        {
            cameraAngleOffset = 270; // Meilleur angle pour monter l'ultime pente?
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "abrupte")
        {
            penteAbrupte = false;
            incrementForce = 3f;
            forceTir = 100f;
            rigidbodyBalle.linearDamping = 0.1f;
        }

        if (collision.gameObject.tag == "anglecamera")
        {
            cameraAngleOffset = 90f;
            forceTir = 200f;
            targetOffset = baseOffset;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "horsParcours") // Tomber dans le vide 
        {
            transform.position = startPoint.transform.position;
            targetOffset = baseOffset;

            audiosourceBalle.clip = nouvelleMusique;
            audiosourceBalle.Play();
        }

        if (collision.gameObject.tag == "trou") // Atteindre le trou final!
        {
            rigidbodyBalle.linearVelocity = Vector3.zero;
            rigidbodyBalle.useGravity = false;
            transform.position = collision.transform.position;
            lineRendererBalle.enabled = false;
            cameraAngleOffset = 180; // Cam

            // Déclenchement du son
            audiosourceBalle.PlayOneShot(sonFin, 10f);

            PlayerPrefs.SetInt("NbCoups", coups);

            //PlayerPrefs.DeleteKey("positionBalle");

            StartCoroutine(GestJeu.instance.FinJeu());
        }
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
