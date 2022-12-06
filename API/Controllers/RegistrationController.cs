using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using registration.Models;
using System.Data;
using System.Data.SqlClient;

namespace registration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public string registration(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Social").ToString());
            SqlCommand cmd = new SqlCommand("INSERT INTO registration(FirstName,LastName,UserName,Email,Password,ConfirmPassword,DateOfBirth,Gender,PhoneNo,IsActive) VALUES('" + registration.FirstNamee + "','" + registration.LastNamee + "','" + registration.UserNamee + "','" + registration.Email + "','" + registration.Password + "','" + registration.ConfirmPassword + "','" + registration.DateOfBirth + "','" + registration.Gender + "','" + registration.PhoneNo + "','" + registration.IsActive + "' )",con);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i > 0)   
            {
                return "Data inserted";
            }
            else
            {
                return "Error";
            }


        }
        [HttpPost]
        [Route("login")]
        public string Login(Registration registration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Social").ToString());
            SqlDataAdapter da =new SqlDataAdapter("SELECT * FROM Registration WHERE Email='" + registration.Email+"' AND Password ='" + registration.Password+"'AND IsActive =1 ", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                return "valid user";
            }
            else
            {
                return "invalid user ";
            }
        }
    }
}
