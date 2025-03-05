namespace Menu.State
{
    public class GameUIPanel: MenuState
    {
        //Specific for this state
        public override void InitState(MenuController menuController)
        {
            base.InitState(menuController);

            state = MenuController.MenuState.Game;
        }
    }
}
