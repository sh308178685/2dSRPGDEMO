using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate()
        {
            //Debug.Log("hehehe");
            GameObject canvas = transform.parent.gameObject;
            GameObject player = canvas.transform.parent.gameObject;
            if(player)
            {
               // player.GetComponent<PlayerControl>().onAttack();
                player.GetComponent<PlayerControl>().onMove();
            }

            
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
