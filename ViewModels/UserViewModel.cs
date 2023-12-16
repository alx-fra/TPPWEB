using System;

public class UserViewModel
{
    public string IdUser { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public IList<string> Roles { get; set; }
    public bool inativo { get; set; }

 
}
