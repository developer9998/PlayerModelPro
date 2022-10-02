using ComputerInterface;
using ComputerInterface.ViewLib;
using PlayerModelPro.Scripts;
using PlayerModelPro.Scripts.ComputerInterface;
using System.Text;

namespace PlayerModelPro
{
    public class PlayerModelView : ComputerView
    {
        private readonly UISelectionHandler _selectionHandler;

        private PlayerModelView()
        {
            _selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
            _selectionHandler.ConfigureSelectionIndicator($"<color=#{PrimaryColor}>></color> ", "", "  ", "");
            _selectionHandler.OnSelected += OnEntrySelected;
            _selectionHandler.MaxIdx = 1;
        }

        // This is called when you view is opened
        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            Controller.Instance.customSpot = false;
            // changing the Text property will fire an PropertyChanged event
            // which lets the computer know the text has changed and update it
            Redraw();
        }

        private void Redraw()
        {
            var str = new StringBuilder();

            str.BeginCenter().MakeBar('-', SCREEN_WIDTH, 0, "ffffff10");
            str.AppendClr(PluginInfo.Name, "656582")
                .EndColor()
                .Append(" v")
                .Append(PluginInfo.Version).AppendLine();

            str.Append("by ").AppendClr("dev9998", "3E30A3")
                .AppendLine();

            str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10").EndAlign().AppendLines(2);

            str.Append(_selectionHandler.GetIndicatedText(0, $"Choose Model")).AppendLine();
            str.Append(_selectionHandler.GetIndicatedText(1, $"Download Model")).AppendLine();

            SetText(str);
        }

        private void OnEntrySelected(int index)
        {
            switch (index)
            {
                case 0:
                    ShowView<PlayerModelPicker>();
                    break;
                case 1:
                    if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
                        return;

                    ShowView<PlayerModelExplorer>();
                    break;
            }
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
            }
        }
    }
}