namespace Menu.State
{
    public class CreditsPanel : MenuState
    {
        //Specific for this state
        public override void InitState(MenuController menuController)
        {
            base.InitState(menuController);

            state = MenuController.MenuState.Help;
        }
    }
}
