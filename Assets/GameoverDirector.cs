using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverDirector : MonoBehaviour
{
    // ���ʉ��֘A
    AudioSource audioSource;
    public AudioClip vTabetakatta;

    // Start is called before the first frame update
    void Start()
    {
        // ���ʉ��̃R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(vTabetakatta);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("OpeningScene");
        }
    }
}
