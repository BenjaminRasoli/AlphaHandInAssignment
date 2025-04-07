using Domain.Models;

namespace WebApp.Models;

public class ClientsViewModel
{
    public IEnumerable<Client> Clients { get; set; } = [];

    public AddClientViewModel AddClientViewModel { get; set; } = new AddClientViewModel();


}
