using UnityEngine;

namespace PlayerModelPlus.Scripts
{
    public class Spin : MonoBehaviour
    {
        float to;
        Vector3 originalPos;

        void Start()
        {
            originalPos = transform.position;
        }

        void LateUpdate()
        {
            to = Mathf.Lerp(to, Controller.Instance.localPositionY, 0.35f * 0.1f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(-90f, Controller.Instance.rotationY, 0);
            transform.position = originalPos + new Vector3(0, to, 0);
        }
    }
}
