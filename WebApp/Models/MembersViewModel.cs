using Domain.Models;

namespace WebApp.Models;

public class MembersViewModel
{
    public IEnumerable<User> Users{ get; set; } = [];

    public AddMemberviewModel AddMemberviewModel { get; set; } = new AddMemberviewModel();
}
