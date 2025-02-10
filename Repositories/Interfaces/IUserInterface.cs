namespace Repositories;

public interface IUserInterface
{
    Task<int> Register(t_User user);
    Task<t_User> Login(vm_Login user);
}
