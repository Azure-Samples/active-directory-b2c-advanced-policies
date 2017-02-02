using System.Threading.Tasks;

namespace B2CAppMobile
{
    public interface IMainPage
    {
        Task EditProfileAsync();

        Task LogInAsync();

        Task LogOutAsync();
    }
}
