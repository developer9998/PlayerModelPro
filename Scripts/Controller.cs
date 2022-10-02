using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using DitzelGames.FastIK;
using System;
using Steamworks;
using GorillaLocomotion;

namespace PlayerModelPro.Scripts
{
    public class Controller : MonoBehaviour
    {
        public static Controller Instance;

        public GameObject misc_orb;

        public string[] player_info;
        public string playermodel_head = "playermodel.head";
        public string playermodel_torso = "playermodel.torso";
        public string playermodel_lefthand = "playermodel.lefthand";
        public string playermodel_righthand = "playermodel.righthand";
        public string player_info_stream;
        public string playermodel_name;
        public string playermodel_author;
        public string namegui;

        public bool CustomColors;
        public bool GameModeTextures;
        public bool customSpot = false;

        public float rotationY = -60f;
        public float localPositionY = 0.15f;

        public Vector3 pos;

        public GameObject player_preview;
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
        public GameObject prevLeft;
        public GameObject prevRight;
        public GameObject otherPreview;
        public Quaternion headoffset;

        void Start()
        {
            Instance = this;
        }

        public void PreviewThatOtherShit(int index)
        {
            if (otherPreview != null)
                Destroy(otherPreview);

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

                otherPreview = new GameObject("playemodel.previewHouse");

                var meshFilter = otherPreview.AddComponent<MeshFilter>();
                Mesh originalMesh = player_body.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                meshFilter.mesh = originalMesh;

                MeshRenderer rend = otherPreview.AddComponent<MeshRenderer>();//easy code really // nacho be quiet
                rend.materials = material_array;

                pos = Plugin.Instance.misc_preview.transform.position;
                otherPreview.transform.localScale = player_body.transform.localScale;

                if (GameObject.Find("Level/forest").activeInHierarchy)
                {
                    otherPreview.transform.position = new Vector3(-68.6798f, 11.8345f, -81.7803f);
                    otherPreview.transform.localScale *= 0.85f;
                }
                else if (GameObject.Find("Level/mountain").activeInHierarchy)
                {
                    otherPreview.transform.position = new Vector3(-26.1865f, 17.9029f, -94.3946f);
                    otherPreview.transform.localScale *= 0.85f;
                }
                else
                {
                    otherPreview.transform.position = Vector3.one * 1000;
                    return;
                }

                otherPreview.AddComponent<PreviewRoom>();

                Quaternion rot = Quaternion.Euler(-90f, -60f, 0f);
                otherPreview.transform.rotation = rot;

                Plugin.Instance.model_text.text = playermodel_name.ToUpper(); ;
                Plugin.Instance.author_text.text = playermodel_author.ToUpper();

                Plugin.Instance.mat_preview[0] = otherPreview.GetComponent<MeshRenderer>().material;
                Plugin.Instance.ChangeMainButton();

                //otherPreview.AddComponent<Spin>();

                otherPreview.GetComponent<MeshRenderer>().material = Plugin.Instance.mat_preview[Plugin.Instance.currentPreviewMaterial];

                Destroy(parentAsset);
            }
        }

        public void PreviewModel(int index)
        {
            if (player_preview != null)
                Destroy(player_preview);

            if (prevLeft != null)
                Destroy(prevLeft);

            if (prevRight != null)
                Destroy(prevRight);

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

                MeshRenderer rend = player_preview.AddComponent<MeshRenderer>();//easy code really // nacho be quiet
                rend.materials = material_array;


                pos = Plugin.Instance.misc_preview.transform.position;
                player_preview.transform.localScale = player_body.transform.localScale;

                player_preview.transform.position = Plugin.Instance.misc_preview.transform.position;
                player_preview.transform.localScale = player_body.transform.localScale;

                Quaternion rot = Quaternion.Euler(-90f, -60f, 0f);
                player_preview.transform.rotation = rot;

                Plugin.Instance.model_text.text = playermodel_name.ToUpper(); ;
                Plugin.Instance.author_text.text = playermodel_author.ToUpper();

                Plugin.Instance.mat_preview[0] = player_preview.GetComponent<MeshRenderer>().material;
                Plugin.Instance.ChangeMainButton();

                player_preview.AddComponent<Spin>();

                player_preview.GetComponent<MeshRenderer>().material = Plugin.Instance.mat_preview[Plugin.Instance.currentPreviewMaterial];

                Destroy(parentAsset);
            }

            //PreviewThatOtherShit(index);

            if (Plugin.Instance.fileName.Length >= 3)
                PreviewSides(index);
        }

        public void PreviewSides(int index)
        {

            int ind = index;

            if (ind == Plugin.Instance.fileName.Length - 1)
                ind = -1;

            string path = Path.Combine(Plugin.Instance.playerpath, Plugin.Instance.fileName[ind + 1]);

            AssetBundle playerbundle;
            GameObject assetplayer;

            playerbundle = AssetBundle.LoadFromFile(path);
            assetplayer = playerbundle.LoadAsset<GameObject>("playermodel.ParentObject");

            if (playerbundle != null && assetplayer != null)
            {
                // LEFT
                var parentAsset = Instantiate(assetplayer);

                playerbundle.Unload(false);

                parentAsset.GetComponent<Text>().enabled = false;

                player_body = parentAsset.transform.GetChild(0).gameObject.transform.Find("playermodel.body").gameObject;
                List<Material> material_list = player_body.GetComponent<SkinnedMeshRenderer>().materials.ToList();

                Material[] material_array = material_list.ToArray();

                prevLeft = new GameObject("playemodel.previewLeft");

                var meshFilter = prevLeft.AddComponent<MeshFilter>();
                Mesh originalMesh = player_body.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                meshFilter.mesh = originalMesh;

                MeshRenderer rend = prevLeft.AddComponent<MeshRenderer>();//easy code really // nacho be quiet
                rend.materials = material_array;

                prevLeft.transform.localScale = player_body.transform.localScale;

                pos = Plugin.Instance.misc_preview.transform.position;

                prevLeft.transform.position = Plugin.Instance.misc_preview.transform.position;

                Quaternion rot = Quaternion.Euler(-90f, -60f, 0f);
                prevLeft.transform.rotation = rot;

                prevLeft.AddComponent<Spin>();

                Destroy(parentAsset);

                prevLeft.transform.localScale *= 0.35f;
                prevLeft.transform.localPosition += Plugin.Instance.misc_preview.transform.right * -1 * 1.25f;
                prevLeft.transform.localPosition += new Vector3(0, 0.25f, 0);
            }

           

            int ind2 = index - 2;

            if (index == 0)
                ind2 = Plugin.Instance.fileName.Length - 2;

            string path2 = Path.Combine(Plugin.Instance.playerpath, Plugin.Instance.fileName[ind2 + 1]);

            AssetBundle playerbundle2;
            GameObject assetplayer2;

            playerbundle2 = AssetBundle.LoadFromFile(path2);
            assetplayer2 = playerbundle2.LoadAsset<GameObject>("playermodel.ParentObject");

            if (playerbundle2 != null && assetplayer2 != null)
            {
                // LEFT
                var parentAsset = Instantiate(assetplayer2);

                playerbundle2.Unload(false);

                parentAsset.GetComponent<Text>().enabled = false;

                player_body = parentAsset.transform.GetChild(0).gameObject.transform.Find("playermodel.body").gameObject;
                List<Material> material_list = player_body.GetComponent<SkinnedMeshRenderer>().materials.ToList();

                Material[] material_array = material_list.ToArray();

                prevRight = new GameObject("playemodel.previewRight");

                var meshFilter = prevRight.AddComponent<MeshFilter>();
                Mesh originalMesh = player_body.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                meshFilter.mesh = originalMesh;

                MeshRenderer rend = prevRight.AddComponent<MeshRenderer>();//easy code really // nacho be quiet
                rend.materials = material_array;

                prevRight.transform.localScale = player_body.transform.localScale;

                pos = Plugin.Instance.misc_preview.transform.position;

                prevRight.transform.position = Plugin.Instance.misc_preview.transform.position;

                Quaternion rot = Quaternion.Euler(-90f, -60f, 0f);
                prevRight.transform.rotation = rot;

                prevRight.AddComponent<Spin>();

                Destroy(parentAsset);

                prevRight.transform.localScale *= 0.35f;
                prevRight.transform.localPosition += Plugin.Instance.misc_preview.transform.right * 1 * 1.25f;
                prevRight.transform.localPosition += new Vector3(0, 0.25f, 0);
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

            try
            {
                // left hand

                offsetL = new GameObject("offsetL");

                poleL = new GameObject("poleL");
                poleL.transform.SetParent(root.transform, false);
                // root, the playermodel's root
                poleL.transform.localPosition = new Vector3(-6f, -5f, -10);

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

                poleR.transform.localPosition = new Vector3(6f, -5f, -10);

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
                HandRight.transform.localRotation = rotR; ;
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
                root.transform.localRotation = Quaternion.identity;

                // head
                headbone.transform.localRotation = headtarget.transform.localRotation;
                headoffset = headtarget.transform.localRotation;
                headbone.transform.SetParent(headtarget.transform, true);
                headbone.transform.localRotation = Quaternion.Euler(headoffset.x - 8, headoffset.y, headoffset.z);

                poleL.AddComponent<Pole>().hand = Player.Instance.leftHandTransform;
                poleR.AddComponent<Pole>().hand = Player.Instance.rightHandTransform;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Failed to assign rig:" + e.Message + " " + e.Source);
                return;
            }
        }
    }
}
