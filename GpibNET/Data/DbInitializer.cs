namespace Gpib.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any())
            {
                return;
            }

            //TODO: Add default user

            //TODO: Add default device

            //TODO: Add samples program files

        }
    }
}
