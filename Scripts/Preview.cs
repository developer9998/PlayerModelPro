using UnityEngine;
using System.Collections;

namespace PlayerModelPlus.Scripts
{
    public class Preview : MonoBehaviour
    {
        float to;
        Vector3 originalPos;
        void Start()
        {
            originalPos = transform.position;
        }

        void LateUpdate()
        {
            to = Mathf.Lerp(to, Controller.Instance.localPositionY * 2f, 0.35f * 0.1f * Time.deltaTime);

            //transform.position = originalPos + new Vector3(0, to, 0);
        }
    }
}
