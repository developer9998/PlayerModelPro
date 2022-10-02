using ComputerInterface.ViewLib;
using ComputerInterface;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using OVR.OpenVR;
using Viveport;

namespace PlayerModelPro.Scripts.ComputerInterface
{
    public class PlayerModelExplorer : ComputerView
    {
        private readonly UISelectionHandler _selectionHandler;
        List<string> pmNamesLocal = new List<string>();

        public int pageOffset;
        public int howManySoFar = 0;
        public int page = 1;
 
        private PlayerModelExplorer()
        {
            _selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
            _selectionHandler.ConfigureSelectionIndicator($"<color=#{PrimaryColor}>></color> ", "", "  ", "");
            _selectionHandler.OnSelected += OnEntrySelected;
            _selectionHandler.MaxIdx = 4;
        }

        // This is called when you view is opened
        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            // changing the Text property will fire an PropertyChanged event
            // which lets the computer know the text has changed and update it
            pmNamesLocal = GetNames();
            howManySoFar = 0;
            pageOffset = 0;
            PageFull = false;

            Redraw();
        }

        List<string> GetNames()
        {
            List<string> pmNamesLocal = new List<string>();

            if (PlayerModelLogic.playerModelData.Count == 0)
                PlayerModelLogic.GetPlayerModelData();

            foreach(PlayerModelData pmd in PlayerModelLogic.playerModelData)
                pmNamesLocal.Add(pmd.name);

            return pmNamesLocal;
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

        bool PageFull = false;

        private void Redraw()
        {
            var str = new StringBuilder();

            PageFull = false;

            howManySoFar = 0;

            str.BeginCenter()
                .AppendLine()
                .Append($"{pmNamesLocal.Count} models loaded from the web")
                .AppendLines(2)
                .EndAlign();

            if (page == 1)
            {
                int pmNameIndex = -1;
                pageOffset = 0;
                _selectionHandler.MaxIdx = -1;
                foreach (string pmName in pmNamesLocal)
                {
                    howManySoFar++;

                    _selectionHandler.MaxIdx++;

                    if (howManySoFar == 10)
                    {
                        _selectionHandler.MaxIdx -= 1;
                        PageFull = true;
                    }

                    if (PageFull)
                        break;

                    pmNameIndex++;
                    str.AppendLine(_selectionHandler.GetIndicatedText(pmNameIndex, pmName));
                }
            }
            else if (page == 2)
            {
                int pmNameIndex = -1;
                pageOffset = 10 - 1;
                _selectionHandler.MaxIdx = -1;
                foreach (string pmName in pmNamesLocal)
                {
                    howManySoFar++;

                    if (howManySoFar >= 10)
                    {
                        _selectionHandler.MaxIdx++;

                        int howManyAdd = 10;
                        if (howManySoFar == 10 + howManyAdd - 1)
                        {
                            _selectionHandler.MaxIdx -= 1;
                            PageFull = true;
                        }

                        if (PageFull)
                            break;

                        pmNameIndex++;
                        str.AppendLine(_selectionHandler.GetIndicatedText(pmNameIndex, pmName));
                    }

                }
            }
            else if (page == 3)
            {
                int pmNameIndex = -1;
                pageOffset = 20 - 2;
                _selectionHandler.MaxIdx = -1;
                foreach (string pmName in pmNamesLocal)
                {
                    howManySoFar++;

                    if (howManySoFar >= 20 - 1)
                    {
                        _selectionHandler.MaxIdx++;

                        int howManyAdd = 20;
                        if (howManySoFar == 10 + howManyAdd - 1)
                        {
                            _selectionHandler.MaxIdx -= 1;
                            PageFull = true;
                        }

                        if (PageFull)
                            break;

                        pmNameIndex++;
                        str.AppendLine(_selectionHandler.GetIndicatedText(pmNameIndex, pmName));
                    }

                }
            }

            if (!PageFull)
            {
                str.AppendLine();

                str.BeginCenter()
                    .Append($"<color=#ffffff10>Page {page}/3</color>")
                    .EndAlign();
            }
            else
            {
                str.BeginCenter()
                    .AppendLine($"<color=#ffffff10>Page {page}/3</color>")
                    .EndAlign();
            }

            SetText(str);
        }

        private void OnEntrySelected(int index)
        {
            PlayerModelLogic.index = index + pageOffset;
            ShowView<PlayerModelPreview>();
        }

        // you can do something on keypresses by overriding "OnKeyPressed"
        // it get's an EKeyboardKey passed as a parameter which wraps the old character string
        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (_selectionHandler.HandleKeypress(key))
            {
                Redraw();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    // "ReturnToMainMenu" will basically switch to the main menu again
                    ReturnToMainMenu();
                    break;
                case EKeyboardKey.Left:
                    int oldPage = page;
                    page = Mathf.Clamp(page - 1, 1, 3);

                    if (page != oldPage)
                        _selectionHandler.CurrentSelectionIndex = 0;

                    Redraw();
                    break;
                case EKeyboardKey.Right:
                    int oldPage2 = page;
                    page = Mathf.Clamp(page + 1, 1, 3);

                    if (page != oldPage2)
                        _selectionHandler.CurrentSelectionIndex = 0;

                    Redraw();
                    break;

            }
        }
    }
}
