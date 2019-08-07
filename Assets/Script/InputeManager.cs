using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputeManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { //检测鼠标左键是否点击

            RaycastHit2D hit;

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
            if (hit.collider.tag == "Player" || hit.collider.tag == "Enamy")
            {
                hit.collider.gameObject.GetComponent<Role>().onTouched();
            }

        }
    }
}
