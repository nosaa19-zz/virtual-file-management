using Microsoft.VisualStudio.TestTools.UnitTesting;
using UsrControl;
using UsrControl.Service;
using UsrControl.Domain;

namespace UsrControlUnitTest
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        public void RegisterUserTest()
        {
            UserService userService = new UserService();

            string Command1 = "Register User1";

            string result = userService.RegisterUser(Command1);
            string expected = "Error - user already existing";
            string actual = userService.RegisterUser(Command1);

            // Assert
            Assert.AreEqual(expected, actual, "The Register Method Not Correct");
        }
    }
}
