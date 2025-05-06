using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using UserService.Interface;

namespace UserService.Models
{
    public class UserModel : IGenericObject
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }

        //TODO: Add hashing to password
        public string Password { get; set; }
        //[JsonIgnore]
        public string RoleCsv { get; private set; } = UserRole.User.ToString();
        //[NotMapped]
        //[JsonIgnore]
        //public UserRole UserRole
        //{
        //    get => Enum.TryParse<UserRole>(RoleCsv, out var role) ? role : UserRole.User;
        //    set => RoleCsv = value.ToString();
        //}

        private Regex _regex;

        #region PATTERNS TO VALIDATE USER INPUTS 

        const string EmailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        const string PHONE_PATTERN = @"^(\d{8,12})$";
        //const string PASSWORD_PATTERN = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,50}$";
        const string PASSWORD_PATTERN = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{1,50}$";



        // Danish names will be used for password so the user can't use the same name as password
        //Link: https://familieretshuset.dk/navne/navne/godkendte-fornavne 
        private readonly string DANISH_NAMES;

        #endregion


        public UserModel()
        {
            DANISH_NAMES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DanishName\\danishNames.txt");
            // Ensure RoleCsv is always "User"
            RoleCsv = UserRole.User.ToString();

        }

        /// <summary>
        /// This constructor is used for creating a new user with email and password only i.e. login
        /// </summary>
        /// <param name="email">Given email the existing email</param>
        /// <param name="password">Given existing password</param>
        public UserModel(string email, string password)
        {
            Email = email;
            Password = password;
            // Ensure RoleCsv is always "User"
            RoleCsv = UserRole.User.ToString();
        }



        public override bool Equals(object? obj)
        {
            return obj is UserModel model &&
                   Id == model.Id &&
                   FirstName == model.FirstName &&
                   LastName == model.LastName &&
                   Email == model.Email &&
                   PhoneNumber == model.PhoneNumber &&
                   UserName == model.UserName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName, Email, PhoneNumber, UserName);
        }

        public override string ToString()
        {
            return $"{Id.ToString()}, {FirstName}, {LastName}, {Email}, {PhoneNumber}, {UserName}";
        }

        public void ValidateFirstName()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(FirstName))
            {
                throw new ArgumentNullException(nameof(FirstName), "First name is required.");
            }
            if (FirstName.Length < 2)
            {
                throw new ArgumentNullException(nameof(FirstName), "First name must be at least 2 characters long.");
            }

        }
        public void ValidateLastName()
        {
            if (string.IsNullOrEmpty(LastName) || string.IsNullOrWhiteSpace(LastName))
            {
                throw new ArgumentNullException(nameof(LastName), "Last name is required.");
            }
            if (LastName.Length < 2)
            {
                throw new ArgumentNullException(nameof(LastName), "Last name must be at least 2 characters long.");
            }
        }

        public void ValidateEmail()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentNullException(nameof(Email), "Email is required.");
            }
            if (!Regex.IsMatch(Email, EmailPattern))
            {
                throw new ArgumentNullException(nameof(Email), "Invalid email format.");
            }
        }

        public void ValidatePhoneNumber()
        {
            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                throw new ArgumentNullException(nameof(PhoneNumber), "Phone number is required.");
            }
            if (!Regex.IsMatch(PhoneNumber, PHONE_PATTERN))
            {
                throw new ArgumentNullException(nameof(PhoneNumber), "Invalid phone number format.");
            }
        }

        public void ValidateUserName()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrWhiteSpace(UserName))
            {
                throw new ArgumentNullException(nameof(UserName), "User name is required.");
            }
            if (UserName.Length < 2)
            {
                throw new ArgumentNullException(nameof(UserName), "User name must be at least 2 characters long.");
            }
        }

        public void ValidatePassword()
        {
            if (string.IsNullOrEmpty(Password) || string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException(nameof(Password), "Password is required.");
            }
            if (Password.Length < 8)
            {
                throw new ArgumentException(nameof(Password), "Password must be at least 8 characters long.");
            }
            if (Password.Length > 50)
            {
                throw new ArgumentException(nameof(Password), "Password must be at most 50 characters long.");
            }

            // Ensure that the password won't contain the user name, first name or last name
            if (!string.IsNullOrEmpty(UserName) &&
                Password.ToLower().Contains(UserName.ToLower()) ||
                !string.IsNullOrEmpty(FirstName) &&
                Password.ToLower().Contains(FirstName.ToLower()) ||
                !string.IsNullOrEmpty(LastName) &&
                Password.ToLower().Contains(LastName.ToLower()))
            {
                throw new ArgumentException("Password cannot contain the user name, first name or last name.");
            }

            try
            {
                //C:\Users\shero\source\repos\DLS_Mandatory_Project\UserService\DanishName\danishNames.txt
                if (File.Exists(DANISH_NAMES))
                {
                    //read alle names from the file
                    string[] names = File.ReadAllLines(DANISH_NAMES);
                    foreach (var name in names)
                    {
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            // for debugging purpose only
                            System.Diagnostics.Debug.WriteLine(name);

                            // escaping the name to avoid regex errors
                            string escapedName = Regex.Escape(name.Trim());

                            // for debugging purpose only
                            System.Diagnostics.Debug.WriteLine(escapedName);

                            // check if the password contains the name 
                            // TimeSpan is preventing ReDoS 
                            if (Regex.IsMatch(Password, escapedName, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
                            {
                                // Danish names will be used for password so the user can't use the same name as password
                                throw new ArgumentException($"Password cannot contain the name: {name.Trim()}");
                            }
                        }
                    }
                }

                if (!Regex.IsMatch(Password, PASSWORD_PATTERN, RegexOptions.None, TimeSpan.FromSeconds(1)))
                {
                    throw new ArgumentException(nameof(Password), "Invalid password format.");
                }

            }
            catch (RegexMatchTimeoutException e)
            {
                // for debugging purpose only
                Console.WriteLine("Regex match timed out: " + e.Message);

                // We don't inform the user about this error
                throw new ArgumentException(nameof(Password), "Password not valid please try again.");

            }
        }

        public virtual void Validate()
        {
            ValidateFirstName();
            ValidateLastName();
            ValidateEmail();
            ValidatePassword();
            ValidatePhoneNumber();
            ValidateUserName();
            
        }
    }
}


public enum UserRole
{
    User
}