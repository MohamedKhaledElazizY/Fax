namespace FaxSystem.ViewModels
{
    public class UserRolesViewModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
