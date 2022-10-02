using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
using System;

namespace PlayerModelPro.Scripts
{
    public class Appearance : MonoBehaviourPunCallbacks
    {
        public static Appearance Instance;

        public List<GameObject> playerGameObjects = new List<GameObject>();
        public GameObject serverGameObjectD;
        public GameObject gorillabody;

        public GameObject face;
        public GameObject chest;
        public GameObject body;

        public List<string> MainGameMat = new List<string>() { "infected (Instance)", "It (Instance)", "ice (Instance)" };
        public List<string> BrawlOutMat = new List<string>() { "paintsplattersmallblue (Instance)", "paintsplattersmallorange (Instance)" };
        public List<string> BrawlInMat = new List<string>() { "bluealive (Instance)", "bluehit (Instance)", "bluestunned (Instance)", "orangealive (Instance)", "orangehit (Instance)", "orangestunned (Instance)" };

        public bool flag1 = true;
        public bool ModelShown = true;

        public Renderer rendGorilla;

        public Material mat;

        Color gorillacolor;

        void Start() => StartCoroutine(Begin());

        public void ShowOnlineRig() => StartCoroutine(ShowOrHideOnlineRig(true));

        public void HideOnlineRig() => StartCoroutine(ShowOrHideOnlineRig(false));

        public void HideOfflineRig() => StartCoroutine(ShowOrHideOffline(false));

        public void ShowOfflineRig() => StartCoroutine(ShowOrHideOffline(true));

        IEnumerator Begin()
        {
            Instance = this;

            face = GorillaTagger.Instance.offlineVRRig.mainSkin.transform.parent.Find("rig/body/head/gorillaface").gameObject;
            chest = GorillaTagger.Instance.offlineVRRig.mainSkin.transform.parent.Find("rig/body/gorillachest").gameObject;
            body = GorillaTagger.Instance.offlineVRRig.mainSkin.gameObject;

            yield break;
        }
            
        IEnumerator ShowOrHideOffline(bool show)
        {
            try
            {
                face.layer = show ? 0 : 7;
                body.layer = show ? 0 : 7;
                chest.GetComponent<Renderer>().material = show ? Plugin.Instance.chestMaterial : Plugin.Instance.invisibleMaterial;
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

        public void AssignMaterial(GameObject clone_body, GameObject playermodel)
        {
            try
            {
                if (clone_body != null && playermodel != null)
                {
                    Renderer renderer = clone_body.GetComponent<Renderer>();
                    string matName = renderer.material.name;

                    Renderer skinnedMeshRenderer = playermodel.GetComponent<SkinnedMeshRenderer>();

                    skinnedMeshRenderer.material = Plugin.Instance.player_main_material;

                    if (MainGameMat.Contains(matName) || BrawlOutMat.Contains(matName))
                    {
                        skinnedMeshRenderer.material = renderer.material;
                        skinnedMeshRenderer.material.mainTextureScale = renderer.material.mainTextureScale * 0.5f;
                    }
                    else if (BrawlInMat.Contains(matName))
                    {
                        skinnedMeshRenderer.material = Plugin.Instance.player_main_material;
                        skinnedMeshRenderer.material.SetColor("_Color", renderer.material.color * 0.5f);
                    }
                    else
                        skinnedMeshRenderer.material = Plugin.Instance.player_main_material;
                }
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Failed to set Playermodel gamemode material: " + e.Message);
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
