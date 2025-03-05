namespace Menu.State
{
    public class MainMenu : MenuState
    {
        //Specific for this state
        
        #region Main Menu states
        public override void InitState(MenuController menuController)
        {
            base.InitState(menuController);

            state = MenuController.MenuState.Main;
        }
        public void SwitchToSettings()
        {
            MenuController.SetActiveState(MenuController.MenuState.Settings);
        }
        public void SwitchToHelp()
        {
            MenuController.SetActiveState(MenuController.MenuState.Help);
        }
        public void PlayGame()
        {
            MenuController.PlayGame();
        }
        public void QuitGame()
        {
            MenuController.QuitGame();
        }
        #endregion
    }
}
