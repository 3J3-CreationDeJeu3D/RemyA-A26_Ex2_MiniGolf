using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuIntro : MonoBehaviour
{
    [SerializeField] TMP_Text texteCoups;
    void Start()
    {
        int nbCoups = PlayerPrefs.GetInt("NbCoups", 0);
        if (nbCoups != 0)
        {
            texteCoups.text = $"Dernier match :<br>{nbCoups} coups";

        }
        else
        {
            texteCoups.text = $"Un match presque impossible!";
        }
    }

    public void DemarrerJeu()
    {
        SceneManager.LoadScene("Jeu");
    }

}
