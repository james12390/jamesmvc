using System.Collections.Generic;
namespace jamesmvc.Models
{
   

    public class DriverWithProfilesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<DriverProfile> Profiles { get; set; }
    }
}
