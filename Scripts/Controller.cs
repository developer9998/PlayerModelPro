using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DitzelGames.FastIK;
using System;

namespace PlayerModelPlus.Scripts
{
    public class Controller : MonoBehaviour
    {
        public static Controller Instance;

        public GameObject misc_orb;
        public string playermodel_head = "playermodel.head";
        public string playermodel_torso = "playermodel.torso";
        public string playermodel_lefthand = "playermodel.lefthand";
        public string playermodel_righthand = "playermodel.righthand";
        public string player_info_stream;
        public string[] player_info;
        public string playermodel_name;
        public string playermodel_author;
        public bool CustomColors;
        public bool GameModeTextures;
        public GameObject player_preview;
        public string namegui;
        public float rotationY = -60f;
        public float localPositionY = 0.15f;
        public Vector3 pos;
        public GameObject player_body;
        public GameObject offsetL;
        public GameObject offsetR;
        public GameObject HandLeft;
        public GameObject HandRight;
        public GameObject root;
        public GameObject headbone;
        public GameObject headtarget;
        public GameObject poleR;
        public GameObject poleL;
        public Quaternion headoffset;

        void Start()
        {
            Instance = this;
        }

        public void PreviewModel(int index)
        {
            if (player_preview != null)
                Destroy(player_preview);

            string path = Path.Combine(Plugin.Instance.playerpath, Plugin.Instance.fileName[index]);

            AssetBundle playerbundle;
            GameObject assetplayer;

            try
            {
                playerbundle = AssetBundle.LoadFromFile(path);
                assetplayer = playerbundle.LoadAsset<GameObject>("playermodel.ParentObject");
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Failed to retrive new playermodel:" + e.Message);
                return;
            }

            if (playerbundle != null && assetplayer != null)
            {
                var parentAsset = Instantiate(assetplayer);

                playerbundle.Unload(false);

                player_info_stream = parentAsset.GetComponent<Text>().text;
                player_info = player_info_stream.Split('$');

                parentAsset.GetComponent<Text>().enabled = false;

                playermodel_name = player_info[0];
                playermodel_author = player_info[1];

                player_body = parentAsset.transform.GetChild(0).gameObject.transform.Find("playermodel.body").gameObject;
                List<Material> material_list = player_body.GetComponent<SkinnedMeshRenderer>().materials.ToList();

                Material[] material_array = material_list.ToArray();

                player_preview = new GameObject("playemodel.preview");
                var meshFilter = player_preview.AddComponent<MeshFilter>();
                Mesh originalMesh = player_body.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                meshFilter.mesh = originalMesh;
                MeshRenderer rend = player_preview.AddComponent<MeshRenderer>();//easy code really
                rend.materials = material_array;
                player_preview.transform.localScale = player_body.transform.localScale;
                pos = Plugin.Instance.misc_preview.transform.position;
                player_preview.transform.position = Plugin.Instance.misc_preview.transform.position;

                Quaternion rot = Quaternion.Euler(-90f, -60f, 0f);
                player_preview.transform.rotation = rot;

                Plugin.Instance.model_text.text = playermodel_name.ToUpper(); ;
                Plugin.Instance.author_text.text = playermodel_author.ToUpper();

                Plugin.Instance.mat_preview[0] = player_preview.GetComponent<MeshRenderer>().material;
                player_preview.AddComponent<Spin>();

                Destroy(parentAsset);
            }
        }

        public void UnloadModel()
        {
            GorillaTagger.Instance.transform.Find("TurnParent/LeftHandTriggerCollider").GetComponent<SphereCollider>().enabled = true;
            GorillaTagger.Instance.transform.Find("TurnParent/RightHandTriggerCollider").GetComponent<SphereCollider>().enabled = true;

            GameObject playermodel = GameObject.Find("playermodel.ParentObject(Clone)");

            if (playermodel != null)
            {
                try
                {
                    GameObject headbone = GameObject.Find(playermodel_head);
                    GameObject HandRight = GameObject.Find(playermodel_lefthand);
                    GameObject HandLeft = GameObject.Find(playermodel_righthand);
                    GameObject offsetL = GameObject.Find("offsetL");
                    GameObject offsetR = GameObject.Find("offsetR");
                    GameObject root = GameObject.Find(playermodel_torso);
                    GameObject LeftTarget = GameObject.Find("playermodel.lefthandpos" + " Target");
                    GameObject RightTarget = GameObject.Find("playermodel.righthandpos" + " Target");
                    GameObject poleR = GameObject.Find("poleR");
                    GameObject poleL = GameObject.Find("poleL");

                    Destroy(poleR);
                    Destroy(poleL);
                    Destroy(LeftTarget);
                    Destroy(RightTarget);
                    Destroy(root);
                    Destroy(HandLeft);
                    Destroy(offsetL);
                    Destroy(offsetR);
                    Destroy(HandRight);
                    Destroy(headbone);
                    Destroy(playermodel);
                }
                catch (InvalidCastException e)
                {
                    Debug.LogError("Failed to destroy leftover assets: " + e.Message);
                }
            }
        }

        public void LoadModel(int index)
        {

            AssetBundle playerbundle;
            GameObject assetplayer;

            try
            {
                playerbundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.Instance.playerpath, Plugin.Instance.fileName[index]));
                assetplayer = playerbundle.LoadAsset<GameObject>("playermodel.ParentObject");
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Failed to retrive new preview:" + e.Message);
                return;
            }

            if (playerbundle != null && assetplayer != null)
            {
                var parentAsset = Instantiate(assetplayer);

                playerbundle.Unload(false);

                player_info_stream = parentAsset.GetComponent<Text>().text;
                player_info = player_info_stream.Split('$');

                parentAsset.GetComponent<Text>().enabled = false;

                CustomColors = bool.Parse(player_info[2]);

                GameModeTextures = bool.Parse(player_info[3]);

                HandLeft = GameObject.Find(playermodel_lefthand);

                HandRight = GameObject.Find(playermodel_righthand);

                root = GameObject.Find(playermodel_torso);

                headbone = GameObject.Find(playermodel_head);
                headtarget = GorillaTagger.Instance.offlineVRRig.headMesh.gameObject;

                Plugin.Instance.player_main_material = GameObject.Find("playermodel.body").GetComponent<SkinnedMeshRenderer>().material; //saves playermodel material
            }
        }

        public void AssignModel()
        {
            // makes a new object used to press buttons with a new one in the playermodel and disables the gorilla one
            GameObject left_finger = GameObject.Find("playermodel.left_finger");
            GameObject right_finger = GameObject.Find("playermodel.right_finger");

            /* 
            if (left_finger != null && right_finger != null)
            {
                left_finger.AddComponent<SphereCollider>().radius = 0.01f;
                right_finger.AddComponent<SphereCollider>().radius = 0.01f;

                left_finger.layer = 10;
                right_finger.layer = 10;

                GameObject.Find("RightHandTriggerCollider").GetComponent<SphereCollider>().enabled = false;
                GameObject.Find("LeftHandTriggerCollider").GetComponent<SphereCollider>().enabled = false;
            }*/

            // left hand

            offsetL = new GameObject("offsetL");

            poleL = new GameObject("poleL");

            poleL.transform.SetParent(root.transform, false);
            // root, the playermodel's root
            poleL.transform.localPosition = new Vector3(-5f, -5f, -10);

            GameObject hand_l = GorillaTagger.Instance.offlineVRRig.leftHandTransform.parent.gameObject;
            // hand_l, the gorillalocomotion's left hand

            offsetL.AddComponent<TransformFollow>().transformToFollow = hand_l.transform;

            GameObject lefthandpos = new GameObject("playermodel.lefthandpos");
            // lefthandpos, the global position of the gorilla's left forearm

            GameObject lefthandparent = HandLeft.transform.parent.gameObject;
            // lefthandparent, the gorilla's forearm
            lefthandpos.transform.SetParent(lefthandparent.transform, false);
            lefthandpos.transform.SetPositionAndRotation(HandLeft.transform.position, HandLeft.transform.rotation);
            HandLeft.transform.SetPositionAndRotation(hand_l.transform.position, hand_l.transform.rotation);
            // HandLeft, the playermodel's left hand

            Quaternion rotL = Quaternion.Euler(HandLeft.transform.localRotation.eulerAngles.x, HandLeft.transform.localRotation.eulerAngles.y, HandLeft.transform.localRotation.eulerAngles.z + 20f);
            // rotL, an edited version of HandLeft
            HandLeft.transform.position = hand_l.transform.position;
            HandLeft.transform.localRotation = rotL;
            HandLeft.transform.SetParent(hand_l.transform, true);
            HandLeft = lefthandpos;

            // right hand

            offsetR = new GameObject("offsetR");

            poleR = new GameObject("poleR");
            poleR.transform.SetParent(root.transform, false);
            // root, the playermodel's root

            poleR.transform.localPosition = new Vector3(5f, -5f, -10);

            GameObject hand_r = GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent.gameObject;
            // hand_r, the gorillalocomotion's right hand

            offsetR.AddComponent<TransformFollow>().transformToFollow = hand_r.transform;

            GameObject righthandpos = new GameObject("playermodel.righthandpos");
            // righthandpos, the global position of the gorilla's right forearm

            GameObject righthandparent = HandRight.transform.parent.gameObject;
            // righthandparent, the gorilla's right forearm
            righthandpos.transform.SetParent(righthandparent.transform, false);
            righthandpos.transform.SetPositionAndRotation(HandRight.transform.position, HandRight.transform.rotation);
            HandRight.transform.SetPositionAndRotation(hand_r.transform.position, hand_r.transform.rotation);
            // HandRight, the playermodel's right hand

            Quaternion rotR = Quaternion.Euler(HandRight.transform.localRotation.eulerAngles.x, HandRight.transform.localRotation.eulerAngles.y, HandRight.transform.localRotation.eulerAngles.z - 20f);
            // rotR, an edited version of HandRight
            HandRight.transform.position = hand_r.transform.position;
            HandRight.transform.localRotation = rotR;;
            HandRight.transform.SetParent(hand_r.transform, true);
            HandRight = righthandpos;

            // left hand ik
            FastIKFabric leftHandFabric = HandLeft.AddComponent<FastIKFabric>();
            leftHandFabric.Target = offsetL.transform;
            leftHandFabric.Pole = poleL.transform;

            // right hand ik
            FastIKFabric rightHandFabric = HandRight.AddComponent<FastIKFabric>();
            rightHandFabric.Target = offsetR.transform;
            rightHandFabric.Pole = poleR.transform;

            // body
            root.transform.SetParent(GorillaTagger.Instance.offlineVRRig.mainSkin.transform.parent.Find("rig/body").transform, false);
            root.transform.localRotation = Quaternion.Euler(0f, 0.0f, 0.0f);

            headbone.transform.localRotation = headtarget.transform.localRotation;
            headoffset = headtarget.transform.localRotation;
            headbone.transform.SetParent(headtarget.transform, true);
            headbone.transform.localRotation = Quaternion.Euler(headoffset.x - 8, headoffset.y, headoffset.z);

        }
    }
}
