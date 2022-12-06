// From New Branch
namespace registration.Models
{
    public class Registration
    {
        public int Id { get; set; }


        public string FirstNamee { get; set; }

        public string LastNamee { get; set; }

        public string UserNamee { get; set; }

        public string Email { get; set; }

        public int Password { get; set; }

        public int ConfirmPassword { get; set; }

        public int DateOfBirth { get; set; }

        public string Gender { get; set; }

        public int PhoneNo { get; set; }

        public int IsActive { get; set; }
    }
}
