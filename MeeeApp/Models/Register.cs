using System;
namespace MeeeApp.Models
{
	public class Register
	{
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string SourcePlatform { get; set; }
        public string SourceVersion { get; set; }
    }
}

