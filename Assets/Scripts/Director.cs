using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class Director : MonoBehaviour
{
    PlayableDirector pd;
    public Camera targetCam;

    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayableDirector>();

        pd.Play();
       
    }

    // Update is called once per frame
    void Update()
    {
        if(pd.time >= pd.duration)
        {
            if(Camera.main == targetCam)
            {
                targetCam.GetComponent<CinemachineBrain>().enabled = false;
                
            }

            targetCam.gameObject.SetActive(false);

            gameObject.SetActive(false);
        }

        
    }
}
