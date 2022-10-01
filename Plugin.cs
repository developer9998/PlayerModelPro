using BepInEx;
using System;
using UnityEngine;
using Utilla;
using System.Reflection;
using System.IO;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Net;
using PlayerModelPlus.Scripts;
using HarmonyLib;
using UnityEngine.XR;
using Valve.VR;
using Bepinject;
using PlayerModelPlus.Scripts.ComputerInterface;

namespace PlayerModelPlus
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    // you better have newtonsoft you stinker.
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        string rootPath;
        public string playerpath;
        public string[] files;
        public string[] fileName;
        public string[] page;

        public int mat_index;
        public int playerIndex = 0;
        public int assignedIndex = 0; // index of array, 
        public int currentPreviewMaterial = 0;

        public float currentTime = 0;

        public bool IsGorilla = true;
        public bool selectflag = false;
        public bool leftflag = false;
        public bool rightflag = false;
        public bool clone_body_flag = false;
        public bool flag_inroom = true;
        public bool ModStart = false;

        public GameObject[] misc_orbs;
        public GameObject misc_preview;
        public GameObject SelectButton;
        public GameObject RightButton;
        public GameObject LeftButton;
        public GameObject playermodel;
        public GameObject clone_body;

        public Transform nachoEngineText;

        public Material[] mat_preview;
        public Material defMat;
        public Material invisibleMaterial;
        public Material chestMaterial;
        public Material player_main_material;

        public Text model_text;
        public Text author_text;

        public bool isPriorModel = false;
        public string priorName = "";
        public int priorIndex = 0;
        public WebClient downloadClient;

        void Start()
        {
            Instance = this;
            Events.GameInitialized += OnGameInitialized;

            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll();

            Zenjector.Install<MainInstaller>().OnProject();

            PlayerModelLogic.GetPlayerModelData();
        }

        public void RefreshPM() => StartCoroutine(RefreshPlayerModel());

        IEnumerator RefreshPlayerModel()
        {
            if (!Directory.Exists(playerpath))
            {
                Directory.CreateDirectory(playerpath);
                yield return new WaitForSeconds(0.1f);
            }

            if (Directory.GetFiles(playerpath, "*.gtmodel").Length == 0)
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://drive.google.com/uc?export=download&id=1tz41u9au0TxWjRFQ5sYAqp2rnoIaTmP4", playerpath + @"\" + "Kyle The Robot" + ".gtmodel");

                yield return new WaitForSeconds(0.2f);
            }

            // get files
            files = Directory.GetFiles(playerpath, "*.gtmodel");//cant Path.GetFileName - cant convert string[] to string
            fileName = new string[files.Length]; //creating new array with same length as files array

            for (int i = 0; i < fileName.Length; i++)
                fileName[i] = Path.GetFileName(files[i]); //getting file names from directories

            if (playerIndex > fileName.Length)
                playerIndex = fileName.Length - 1;

            if (Controller.Instance != null)
                Controller.Instance.PreviewModel(playerIndex);

            yield break;
        }

        public void DownloadPlayerModel(string downloadURL, string playerName) => StartCoroutine(DownloadModel(downloadURL, playerName));

        IEnumerator DownloadModel(string downloadURL, string playerName)
        {
            if (downloadClient == null)
                downloadClient = new WebClient();

            downloadClient.DownloadFile(@downloadURL, playerpath + @"\" + playerName + ".gtmodel");

            yield return new WaitForSeconds(0.2f);

            RefreshPM();

            yield return new WaitForEndOfFrame();

            yield break;
        }

        IEnumerator StartPlayerModel()
        {
            // creates the controller and appearance components
            gameObject.AddComponent<Controller>();
            gameObject.AddComponent<Appearance>();

            // setting up materials
            invisibleMaterial = new Material(Shader.Find("Standard"));
            invisibleMaterial.SetColor("_Color", Color.clear);
            invisibleMaterial.SetFloat("_Mode", 2);
            invisibleMaterial.SetFloat("_Glossiness", 0.0f);
            invisibleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            invisibleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
            invisibleMaterial.EnableKeyword("_ALPHABLEND_ON");
            invisibleMaterial.renderQueue = 3000;

            chestMaterial = GorillaTagger.Instance.offlineVRRig.mainSkin.transform.parent.Find("rig/body/gorillachest").GetComponent<MeshRenderer>().material;

            // creates the local folder
            rootPath = Directory.GetCurrentDirectory();
            playerpath = Path.Combine(rootPath, "BepInEx", "Plugins", "PlayerModelPlus", "CustomModels");

            if (!Directory.Exists(playerpath))
            {
                Directory.CreateDirectory(playerpath);
                yield return new WaitForSeconds(0.1f);
            }

            if (Directory.GetFiles(playerpath, "*.gtmodel").Length == 0)
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://drive.google.com/uc?export=download&id=1tz41u9au0TxWjRFQ5sYAqp2rnoIaTmP4", playerpath + @"\" + "Kyle The Robot" + ".gtmodel");

                yield return new WaitForSeconds(0.2f);
            }

            // get files
            files = Directory.GetFiles(playerpath, "*.gtmodel");//cant Path.GetFileName - cant convert string[] to string
            fileName = new string[files.Length]; //creating new array with same length as files array

            for (int i = 0; i < fileName.Length; i++)
                fileName[i] = Path.GetFileName(files[i]); //getting file names from directories

            // get misc bundle
            AssetBundle bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("PlayerModelPlus.Resources.PlayerModelStand"));
            GameObject localasset = Instantiate(bundle.LoadAsset<GameObject>("misc"));

            // get the gameobject and add the components
            GameObject misc = localasset.transform.GetChild(0).gameObject;

            misc.transform.position = new Vector3(-53.3f, 16.216f, -124.6f);
            misc.transform.localRotation = Quaternion.Euler(0f, -60f, 0f);

            SelectButton = misc.transform.Find("misc.selector").gameObject;
            SelectButton.AddComponent<PlayerButton>().button = 1;

            RightButton = misc.transform.Find("misc.rightpage").gameObject;
            RightButton.AddComponent<PlayerButton>().button = 2;

            LeftButton = misc.transform.Find("misc.leftpage").gameObject;
            LeftButton.AddComponent<PlayerButton>().button = 3;

            GameObject canvasText = misc.transform.Find("Canvas").gameObject;

            GameObject modelText = canvasText.transform.Find("model.text").gameObject;
            GameObject authorText = canvasText.transform.Find("author.text").gameObject;

            model_text = modelText.GetComponent<Text>();
            author_text = authorText.GetComponent<Text>();

            misc_preview = misc.transform.Find("misc.preview").gameObject;

            misc_orbs = new GameObject[4];
            misc_orbs[0] = misc.transform.Find("misc.fur").gameObject;
            misc_orbs[1] = misc.transform.Find("misc.lava").gameObject;
            misc_orbs[2] = misc.transform.Find("misc.rock").gameObject;
            misc_orbs[3] = misc.transform.Find("misc.ice").gameObject;

            for (int i = 0; i < misc_orbs.Length; i++)
            {
                misc_orbs[i].AddComponent<PlayerButton>().button = 4 + i;
                misc_orbs[i].GetComponent<PlayerButton>().setColour = false;
            }

            mat_preview = new Material[misc_orbs.Length];

            for (int i = 0; i < mat_preview.Length; i++)
                mat_preview[i] = misc_orbs[i].GetComponent<MeshRenderer>().material;

            // creates empty hand objects, not sure what they're for though
            GameObject left_empty = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject right_empty = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            left_empty.GetComponent<SphereCollider>().enabled = false;
            right_empty.GetComponent<SphereCollider>().enabled = false;
            left_empty.transform.localScale = new Vector3(.03f, .5f, .03f);
            right_empty.transform.localScale = new Vector3(.03f, .5f, .03f);
            left_empty.name = "LeftEmpty";
            right_empty.name = "RightEmpty";

            // gets the text mesh and adds its 
            nachoEngineText = misc.transform.Find("nachoengine_playermodelmod");
            nachoEngineText.gameObject.AddComponent<Preview>();

            // moves the object down
            misc.transform.Find("displaytext").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.fur").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.ice").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.rock").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.lava").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.rightpage").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.leftpage").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.display").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("misc.selector").localPosition -= new Vector3(0, 0.135f, 0);
            misc.transform.Find("Canvas").localPosition -= new Vector3(0, 0.135f, 0);

            // makes the update function work

            yield return new WaitForSeconds(1);

            ModStart = true;

            yield break;
        }

        public void ChangeMainButton() => misc_orbs[0].GetComponent<MeshRenderer>().material = mat_preview[0];

        void OnGameInitialized(object sender, EventArgs e) => StartCoroutine(StartPlayerModel());

        IEnumerator PressChangeButton()
        {
            nachoEngineText.GetComponent<MeshRenderer>().forceRenderingOff = true;

            if (IsGorilla == true)//switching from gorilla to playermodel
            {
                Controller.Instance.LoadModel(playerIndex);
                playermodel = GameObject.Find("playermodel.body");
                assignedIndex = playerIndex;
                IsGorilla = false;
                Controller.Instance.AssignModel();

                PlayerPrefs.SetInt("PlayerModelProIsPriorModel", 1);
                PlayerPrefs.SetString("PlayerModelProPriorName", fileName[playerIndex]);
                PlayerPrefs.SetInt("PlayerModelProPriorIndex", playerIndex);
            }
            else
            {
                if (assignedIndex == playerIndex)//playermodel to gorilla 
                {
                    Controller.Instance.UnloadModel();
                    IsGorilla = true;
                    player_main_material = null;

                    PlayerPrefs.SetInt("PlayerModelProIsPriorModel", 0);
                    PlayerPrefs.SetString("PlayerModelProPriorName", fileName[playerIndex]);
                    PlayerPrefs.SetInt("PlayerModelProPriorIndex", playerIndex);
                }
                else//playermodel to playermodel
                {
                    Controller.Instance.UnloadModel();
                    IsGorilla = false;
                    player_main_material = null;
                    assignedIndex = playerIndex;

                    PlayerPrefs.SetInt("PlayerModelProIsPriorModel", 1);
                    PlayerPrefs.SetString("PlayerModelProPriorName", fileName[playerIndex]);
                    PlayerPrefs.SetInt("PlayerModelProPriorIndex", playerIndex);
                }
            }

            yield break;
        }

        public void ButtonPress(int button) => ButtonToPress(button);

        private void ButtonToPress(int button)
        {
            switch (button)
            {
                case 1:
                    StartCoroutine(PressChangeButton());
                    break;

                case 2:
                    playerIndex++;

                    if (playerIndex > fileName.Length - 1)
                        playerIndex = 0;

                    Controller.Instance.PreviewModel(playerIndex);

                    break;

                case 3:
                    playerIndex--;

                    if (playerIndex < 0)
                        playerIndex = fileName.Length - 1;//10 items but starts from 0 so 0 to 9 = 10 items

                    Controller.Instance.PreviewModel(playerIndex);

                    break;

                case 4:
                    currentPreviewMaterial = 0;
                    Controller.Instance.player_preview.GetComponent<MeshRenderer>().material = mat_preview[0];
                    break;

                case 5:
                    currentPreviewMaterial = 1;
                    Controller.Instance.player_preview.GetComponent<MeshRenderer>().material = mat_preview[1];
                    break;

                case 6:
                    currentPreviewMaterial = 2;
                    Controller.Instance.player_preview.GetComponent<MeshRenderer>().material = mat_preview[2];
                    break;

                case 7:
                    currentPreviewMaterial = 3;
                    Controller.Instance.player_preview.GetComponent<MeshRenderer>().material = mat_preview[3];
                    break;

            }
        }

        void LateUpdate()
        {
            if (!ModStart)
                return;

            if (Controller.Instance == null)
                return;

            if (Time.time < currentTime)
                return;

            currentTime = Time.time + 1;

            if (Controller.Instance.localPositionY == 1f)
                Controller.Instance.localPositionY = -1f;
            else
                Controller.Instance.localPositionY = 1f;
        }

        void Update()
        {
            if (!ModStart)
                return;

            /* Code here runs every frame when the mod is enabled */

            if (Keyboard.current.jKey.wasPressedThisFrame)
                ButtonToPress(1);
            if (Keyboard.current.kKey.wasPressedThisFrame)
                ButtonToPress(2);
            if (Keyboard.current.hKey.wasPressedThisFrame)
                ButtonToPress(3);

            if (Keyboard.current.vKey.wasPressedThisFrame)
                ButtonToPress(4);
            if (Keyboard.current.bKey.wasPressedThisFrame)
                ButtonToPress(5);
            if (Keyboard.current.nKey.wasPressedThisFrame)
                ButtonToPress(6);
            if (Keyboard.current.mKey.wasPressedThisFrame)
                ButtonToPress(7);

            if (Keyboard.current.rKey.wasPressedThisFrame)
                RefreshPM();

            if (playermodel != null)
            {
                if (XRSettings.loadedDeviceName == "Oculus")
                    playermodel.GetComponent<SkinnedMeshRenderer>().enabled = OVRManager.hasInputFocus;
                if (OpenVR.Overlay != null)
                    playermodel.GetComponent<SkinnedMeshRenderer>().enabled = !OpenVR.Overlay.IsDashboardVisible();
            }

            Controller.Instance.rotationY -= 0.5f;

            if (PhotonNetwork.InRoom)
            {
                if (IsGorilla) //in a room, is gorilla model
                {
                    flag_inroom = true;
                    Appearance.Instance.flag1 = true;

                    if (!Appearance.Instance.ModelShown)
                    {
                        Appearance.Instance.ShowOnlineRig();
                        Appearance.Instance.ShowOfflineRig();
                    }

                    clone_body = null;
                }
                else//in a room, is playermodels
                {
                    if (clone_body != null && playermodel != null)
                    {
                        if (Appearance.Instance.ModelShown)
                        {
                            Appearance.Instance.HideOnlineRig();
                            Appearance.Instance.HideOfflineRig();
                        }

                        if (Controller.Instance.CustomColors)
                            Appearance.Instance.AssignColor(playermodel);

                        if (Controller.Instance.GameModeTextures)
                            Appearance.Instance.AssignMaterial(clone_body, playermodel);
                    }

                    if (clone_body == null)
                        clone_body = GorillaParent.instance.vrrigs[0].mainSkin.gameObject;

                    if (playermodel == null)
                    {
                        Controller.Instance.LoadModel(assignedIndex);
                        while (playermodel == null)
                            playermodel = GameObject.Find("playermodel.body");

                        Controller.Instance.AssignModel();
                    }
                }
            }
            else if (!PhotonNetwork.InRoom)
            {

                flag_inroom = false;
                clone_body = null;

                if (IsGorilla) //not in a room, is gorilla model
                {
                    Appearance.Instance.flag1 = true;
                    Appearance.Instance.ShowOfflineRig();
                }
                else//not in a room, is playermodel
                {
                    playermodel = GameObject.Find("playermodel.body");
                    if (playermodel != null)
                    {
                        Appearance.Instance.ResetMaterial(playermodel);

                        Appearance.Instance.HideOfflineRig();

                        if (Controller.Instance.CustomColors)
                            Appearance.Instance.AssignColor(playermodel);
                    }
                    else
                    {
                        Controller.Instance.LoadModel(assignedIndex);
                        while (playermodel == null)
                            playermodel = GameObject.Find("playermodel.body");

                        Controller.Instance.AssignModel();
                    }
                }

            }
        }
    }
}
