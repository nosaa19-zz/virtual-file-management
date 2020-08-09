using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UsrControl.Domain;

namespace UsrControl.Service
{
    public class UserService
    {
        private List<User> MasterUsers = new List<User>();

        public List<User> GetUsers() {
            return MasterUsers;
        }

        public String RegisterUser(String command)
        {
            String result = String.Empty;

            var regularExpression = new Regex(@"(Register)\s(.+)", RegexOptions.IgnoreCase);
            var match = regularExpression.Match(command);

            String username = match.Groups[2].Value;

            if (username.Split(" ").Count() > 1)
            {
                result = "Warning - username contains whitespace, please consider another name";
            }
            else
            {
                if (username.Equals(String.Empty))
                {
                    result = "Error - wrong format!";
                }
                else
                {
                    User newUser = MasterUsers.Find(newUser => newUser.Username.ToLower() == username.ToLower());
                    if (newUser != null)
                    {
                        result= "Error - user already existing";
                    }
                    else
                    {
                        newUser = new User();
                        newUser.Id = Guid.NewGuid().ToString();
                        newUser.Username = username;

                        MasterUsers.Add(newUser);
                        result = "Success";
                    }
                }
            }
            return result;
        }

    }
}
