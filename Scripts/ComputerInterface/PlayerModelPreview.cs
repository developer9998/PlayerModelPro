using ComputerInterface.ViewLib;
using ComputerInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PlayerModelPro.Scripts.ComputerInterface
{
    public class PlayerModelPreview : ComputerView
    {
        private CancellationTokenSource cancelToken;

        // This is called when you view is opened
        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            // changing the Text property will fire an PropertyChanged event
            // which lets the computer know the text has changed and update it
            Redraw();
        }

        List<PlayerModelData> GetData()
        {
            List<PlayerModelData> pmNamesLocal = new List<PlayerModelData>();

            if (PlayerModelLogic.playerModelData.Count == 0)
                PlayerModelLogic.GetPlayerModelData();

            foreach (PlayerModelData pmd in PlayerModelLogic.playerModelData)
                pmNamesLocal.Add(pmd);

            return pmNamesLocal;
        }

        private void Redraw() => ShowMainMenu();

        public bool CheckIfHave(string n)
        {
            List<string> files = new List<string>();
            foreach (var file in Plugin.Instance.fileName)
                files.Add(file);

            if (files.Contains(n + ".gtmodel"))
                return true;
            return false;
        }

        void ShowMainMenu()
        {
            var str = new StringBuilder();

            List<PlayerModelData> pmNamesLocal = GetData();
            PlayerModelData n = pmNamesLocal[PlayerModelLogic.index];

            str.BeginCenter()
                .AppendLine()
                .Append($"{n.name} information")
                .EndAlign()
                .AppendLines(3);

            str.AppendLine($"<color=#{PrimaryColor}>Name:</color> {n.file_name.ToUpper()}");
            str.AppendLine($"<color=#{PrimaryColor}>Author:</color> {n.author.ToUpper()}").AppendLine();
            str.AppendLine($"<color=#{PrimaryColor}>Custom Colours:</color> {(n.detail_colour.ToString().ToUpper() == "TRUE" ? "<color=#80FF80ff>ENABLED</color>" : "<color=#FF5454ff>DISABLED</color>")}");
            str.AppendLine($"<color=#{PrimaryColor}>Gamemode Textures:</color> {(n.detail_gamemode.ToString().ToUpper() == "TRUE" ? "<color=#80FF80ff>ENABLED</color>" : "<color=#FF5454ff>DISABLED</color>")}");

            str.AppendLines(2)
                .BeginCenter()
                .Append($"{(!CheckIfHave(n.file_name) ? "<color=#ffffff10>Press enter to download this model</color>" : "<color=#FF5454ff>You already have this model</color>")}")
                .EndAlign();

            SetText(str);
        }

        void ShowInstallMenu()
        {
            var str = new StringBuilder();

            List<PlayerModelData> pmNamesLocal = GetData();
            PlayerModelData n = pmNamesLocal[PlayerModelLogic.index];

            str.BeginCenter()
                .AppendLine()
                .Append($"{n.name} information")
                .EndAlign()
                .AppendLines(3);

            str.AppendLine($"<color=#{PrimaryColor}>Name:</color> {n.file_name.ToUpper()}");
            str.AppendLine($"<color=#{PrimaryColor}>Author:</color> {n.author.ToUpper()}").AppendLine();
            str.AppendLine($"<color=#{PrimaryColor}>Custom Colours:</color> {(n.detail_colour.ToString().ToUpper() == "TRUE" ? "<color=#80FF80ff>ENABLED</color>" : "<color=#FF5454ff>DISABLED</color>")}");
            str.AppendLine($"<color=#{PrimaryColor}>Gamemode Textures:</color> {(n.detail_gamemode.ToString().ToUpper() == "TRUE" ? "<color=#80FF80ff>ENABLED</color>" : "<color=#FF5454ff>DISABLED</color>")}");

            str.AppendLines(2)
                .BeginCenter()
                .Append($"<color=#80FF80ff>Successfully downloaded {n.name}</color>")
                .EndAlign();

            SetText(str);
        }

        // you can do something on keypresses by overriding "OnKeyPressed"
        // it get's an EKeyboardKey passed as a parameter which wraps the old character string
        public async override void OnKeyPressed(EKeyboardKey key)
        {
            switch (key)
            {
                case EKeyboardKey.Enter:

                    List<PlayerModelData> pmNamesLocal = GetData();
                    PlayerModelData n = pmNamesLocal[PlayerModelLogic.index];

                    if (CheckIfHave(n.file_name))
                        return;

                    Plugin.Instance.DownloadPlayerModel(n.download_url, n.file_name);

                    cancelToken?.Cancel();
                    cancelToken?.Dispose();
                    cancelToken = new CancellationTokenSource();

                    ShowInstallMenu();

                    await WaitForABitAndThenGoBackToDoingWhatever(cancelToken.Token);

                    break;
                case EKeyboardKey.Back:
                    // "ReturnToMainMenu" will basically switch to the main menu again
                    ShowView<PlayerModelExplorer>();
                    break;
            }
        }


        private async Task WaitForABitAndThenGoBackToDoingWhatever(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token);
                ShowMainMenu();
            }
            catch (OperationCanceledException) { }
        }
    }
}
