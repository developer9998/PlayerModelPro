using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlayerModelPro.Scripts
{
    public class PreviewRoom : MonoBehaviour
    {
        GameObject forest;
        GameObject mountain;

        void Start()
        {
            forest = GameObject.Find("Level/forest");
            mountain = GameObject.Find("Level/mountain");
            gameObject.transform.localScale *= 0.35f;
        }

        void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(-90f, Controller.Instance.rotationY, 0);

            if (Controller.Instance.customSpot)
            {
                if (forest.activeSelf)
                    gameObject.transform.position = new Vector3(-69.1398f, 12.1145f, -82.7203f);
                else if (mountain.activeSelf)
                    gameObject.transform.position = new Vector3(-27.1598f, 18.1145f, -94.2802f);
            }
            else
            {
                gameObject.transform.position = Vector3.one * 100000;
            }
        }
    }
}
