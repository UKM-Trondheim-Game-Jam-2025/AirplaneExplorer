namespace Menu.State
{
    public class SettingsMenu : MenuState
    {

        #region InGameUI States

        public override void InitState(MenuController menuController)
        {
            base.InitState(menuController);

            state = MenuController.MenuState.GameUIPanel;
        }

        public void SwitchToSettings()
        {
            MenuController.SetActiveState(MenuController.MenuState.Settings);
        }

        public void SwitchToConfirm()
        {
            MenuController.SetActiveState(MenuController.MenuState.ConfirmPanel);
        }

        public void SwitchToGameUI()
        {
            MenuController.SetActiveState(MenuController.MenuState.GameUIPanel);
        }

        public void SwitchToControlPanel()
        {
            MenuController.SetActiveState(MenuController.MenuState.Settings);
        }

        public void SwitchToAudioPanel()
        {
            MenuController.SetActiveState(MenuController.MenuState.Settings);
        }

        #endregion

    }
}