using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
using System;

namespace PlayerModelPlus.Scripts
{
    public class Appearance : MonoBehaviourPunCallbacks
    {
        public static Appearance Instance;
        public List<GameObject> playerGameObjects = new List<GameObject>();
        public GameObject serverGameObjectD;
        public bool flag1 = true;
        public GameObject gorillabody;
        public Renderer rendGorilla;
        public Material mat;
        public bool ModelShown = true;
        Color gorillacolor;

        void Start() => Instance = this;

        public void ShowOnlineRig() => StartCoroutine(ShowOrHideOnlineRig(true));

        public void HideOnlineRig() => StartCoroutine(ShowOrHideOnlineRig(false));

        public void HideOfflineRig() => StartCoroutine(ShowOrHideOffline(false));

        public void ShowOfflineRig() => StartCoroutine(ShowOrHideOffline(true));
            
        IEnumerator ShowOrHideOffline(bool show)
        {
            try
            {
                GameObject gorillaface = GorillaTagger.Instance.offlineVRRig.mainSkin.transform.parent.Find("rig/body/head/gorillaface").gameObject;
                GameObject gorillachest = GorillaTagger.Instance.offlineVRRig.mainSkin.transform.parent.Find("rig/body/gorillachest").gameObject;
                GameObject gorillabody = GorillaTagger.Instance.offlineVRRig.mainSkin.gameObject;

                gorillaface.layer = show ? 0 : 7;
                gorillabody.layer = show ? 0 : 7;
                gorillachest.GetComponent<MeshRenderer>().material = show ? Plugin.Instance.chestMaterial : Plugin.Instance.invisibleMaterial;
                ModelShown = show;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Failed to show/hide offline rig: " + e.Message);
            }
            yield break;
        }

        IEnumerator ShowOrHideOnlineRig(bool show)
        {
            if (!PhotonNetwork.InRoom)
                yield break;

            try
            {
                GameObject gorillaface = GorillaParent.instance.vrrigs[0].mainSkin.transform.parent.Find("rig/body/head/gorillaface").gameObject;
                GameObject gorillachest = GorillaParent.instance.vrrigs[0].mainSkin.transform.parent.Find("rig/body/gorillachest").gameObject;
                GameObject gorillabody = GorillaParent.instance.vrrigs[0].mainSkin.gameObject;

                gorillaface.layer = show ? 0 : 7;
                gorillabody.layer = show ? 0 : 7;
                gorillachest.GetComponent<MeshRenderer>().material = show ? Plugin.Instance.chestMaterial : Plugin.Instance.invisibleMaterial;
                ModelShown = show;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Failed to show/hide online rig: " + e.Message);
            }
            yield break;
        }

        public void AssignColor(GameObject playermodel)
        {
            if (PhotonNetwork.InRoom)
                gorillabody = GorillaParent.instance.vrrigs[0].mainSkin.gameObject;
            else
                gorillabody = GorillaTagger.Instance.offlineVRRig.mainSkin.gameObject;

            try
            {
                rendGorilla = gorillabody.GetComponent<Renderer>();
                gorillacolor = rendGorilla.material.color;

                playermodel.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", gorillacolor);
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Failed to set Playermodel colour: " + e.Message);
            }
        }

        public void ResetMaterial(GameObject playermodel) => playermodel.GetComponent<SkinnedMeshRenderer>().material = Plugin.Instance.player_main_material;

        bool IsBattleMat(GameObject tbod)
        {
            if (tbod.GetComponent<Renderer>().material.name == "bluealive (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "bluehit (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "bluestunned (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "orangealive (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "orangehit (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "orangestunned (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "paintsplattersmallblue (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "paintsplattersmallorange (Instance)")
            {
                return true;
            }
            return false;
        }

        bool IsBattleMat2(GameObject tbod)
        {
            if (tbod.GetComponent<Renderer>().material.name == "paintsplattersmallblue (Instance)")
            {
                return true;
            }
            if (tbod.GetComponent<Renderer>().material.name == "paintsplattersmallorange (Instance)")
            {
                return true;
            }
            return false;
        }

        public void AssignMaterial(GameObject clone_body, GameObject playermodel)
        {
            if (clone_body != null && playermodel != null)
            {
                playermodel.GetComponent<SkinnedMeshRenderer>().material = Plugin.Instance.player_main_material;

                if (clone_body.GetComponent<Renderer>().material.name == "infected (Instance)")
                {
                    playermodel.GetComponent<SkinnedMeshRenderer>().material = Plugin.Instance.mat_preview[1];
                }
                else if (clone_body.GetComponent<Renderer>().material.name == "It (Instance)")
                {
                    playermodel.GetComponent<SkinnedMeshRenderer>().material = Plugin.Instance.mat_preview[2];
                }
                else if (clone_body.GetComponent<Renderer>().material.name == "ice (Instance)")
                {
                    playermodel.GetComponent<SkinnedMeshRenderer>().material = Plugin.Instance.mat_preview[3];
                }
                else
                {
                    if (IsBattleMat(clone_body) == true)
                    {
                        if (IsBattleMat2(clone_body) == true)
                        {
                            playermodel.GetComponent<SkinnedMeshRenderer>().material = clone_body.GetComponent<Renderer>().material;
                        }
                        else
                        {
                            playermodel.GetComponent<SkinnedMeshRenderer>().material = Plugin.Instance.player_main_material;
                            playermodel.GetComponent<SkinnedMeshRenderer>().material.color = clone_body.GetComponent<Renderer>().material.color;
                        }
                    }
                    else
                    {
                        playermodel.GetComponent<SkinnedMeshRenderer>().material = Plugin.Instance.player_main_material;
                    }
                }

            }
            else
            {
                if (clone_body == null)
                    Debug.LogError("clone_body is null");
                if (playermodel == null)
                    Debug.LogError("playermodel is null");
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            ModelShown = true;
            HideOfflineRig();
            HideOnlineRig();
        }
    }
}
