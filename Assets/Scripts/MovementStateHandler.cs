namespace MovementHandling
{
    public abstract class MovementStateHandler
    {
        protected MovementStateHandler Next;

        public void SetNext(MovementStateHandler handler)
        {
            Next = handler;
        }

        public abstract bool HandleState(PlayerController controller);
    }

    public class CrouchingStateHandler : MovementStateHandler
    {
        public override bool HandleState(PlayerController controller)
        {
            if (!controller.IsCrouching)
                return Next is not null && Next.HandleState(controller);
            controller.SetMovementSpeed(controller.CrouchSpeed);
            return true;
        }
    }

    public class SprintingStateHandler : MovementStateHandler
    {
        public override bool HandleState(PlayerController controller)
        {
            if (!controller.IsGrounded || !controller.IsSprinting)
                return Next is not null && Next.HandleState(controller);
            controller.SetMovementSpeed(controller.SprintSpeed);
            return true;
        }
    }

    public class WalkingStateHandler : MovementStateHandler
    {
        public override bool HandleState(PlayerController controller)
        {
            if (!controller.IsGrounded)
                return Next is not null && Next.HandleState(controller);
            controller.SetMovementSpeed(controller.WalkSpeed);
            return true;
        }
    }

    public class AirStateHandler : MovementStateHandler
    {
        public override bool HandleState(PlayerController controller)
        {
            return true;
        }
    }
}
