using GorillaLocomotion;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlayerModelPro.Scripts
{
    public class Pole : MonoBehaviour
    {
        public Transform hand;
        public Transform body;
        public Transform head;
        public float original;

        void Start()
        {
            head = Player.Instance.headCollider.transform;
            body = Player.Instance.bodyCollider.transform;
            original = transform.localPosition.x;
        }

        void FixedUpdate()
        {
            float headY = head.position.y;
            float handOffset = headY - hand.transform.position.y - 0.5f;
            float newOffset = handOffset * 100;

            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(newOffset, -50, -5f), transform.localPosition.z); ;
        }
    }
}
