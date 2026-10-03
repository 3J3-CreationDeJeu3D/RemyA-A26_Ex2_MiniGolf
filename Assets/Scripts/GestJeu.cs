using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum EtatJeu
{
    Debut,
    Jeu,
    Fin
}

public class GestJeu : MonoBehaviour
{
    public static GestJeu instance;
    public EtatJeu etat;

    public ActiverRigidBody activerRigidBodyGrosMoulin;
    public TourneMoulin tourneMoulin;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        etat = EtatJeu.Jeu;
    }

    
    void Update()
    {
        
    }

    public IEnumerator FinJeu()
    {
        etat = EtatJeu.Fin;

        activerRigidBodyGrosMoulin.FaireTomberMoulin();
        tourneMoulin.TournePlusVite();

        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Intro");
    }
}
