using ComputerInterface.ViewLib;
using ComputerInterface;
using System.Text;

namespace PlayerModelPro.Scripts.ComputerInterface
{
    public class PlayerModelPicker : ComputerView
    {
        // This is called when you view is opened
        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            Controller.Instance.customSpot = true;
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

            str.AppendLine("Change PlayerModel");

            str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10").EndAlign().AppendLines(2);

            str.BeginCenter();
            str.AppendLine($"Name: {Controller.Instance.playermodel_name.ToUpper()}\nAuthor: {Controller.Instance.playermodel_author.ToUpper()}").AppendLines(2);
            str.AppendLine($"<  {Plugin.Instance.playerIndex}  >").AppendLine().EndAlign();

            SetText(str);
        }

   
        // you can do something on keypresses by overriding "OnKeyPressed"
        // it get's an EKeyboardKey passed as a parameter which wraps the old character string
        public override void OnKeyPressed(EKeyboardKey key)
        {

            switch (key)
            {
                case EKeyboardKey.Enter:
                    Plugin.Instance.ButtonPress(1);
                    Redraw();
                    break;
                case EKeyboardKey.Left:
                    Plugin.Instance.ButtonPress(3);
                    Redraw();
                    break;
                case EKeyboardKey.Right:
                    Plugin.Instance.ButtonPress(2);
                    Redraw();
                    break;
                case EKeyboardKey.Back:
                    // "ReturnToMainMenu" will basically switch to the main menu again
                    ShowView<PlayerModelView>();
                    break;
            }
        }
    }
}
