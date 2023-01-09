using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverDirector : MonoBehaviour
{
    // 効果音関連
    AudioSource audioSource;
    public AudioClip vTabetakatta;

    // Start is called before the first frame update
    void Start()
    {
        // 効果音のコンポーネントを取得
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
