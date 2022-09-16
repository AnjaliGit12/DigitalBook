using AuthService.Models;
using Microsoft.EntityFrameworkCore;

public class UserValidationModel
{
    private readonly DigitalBooksContext _context = new DigitalBooksContext();
    public string? UserName { get; set; }
    public string? Password { get; set; }


    public UserTable ValidateCredentials(string UserName, string Password)
    {
        UserTable userMaster1 = null;

        if (_context.UserTables == null)
        {
            return userMaster1;
            // return false;
        }

        //var userMaster = (from x in _context.UserMasters
        //                   where x.UserName == UserName && x.Password == EncryptionDecryption.EncodePasswordToBase64(Password)
        //                   select x.UserId).SingleOrDefault();

        userMaster1 = (from x in _context.UserTables
                       where x.UserName == UserName && x.Password == Password
                       select x).SingleOrDefault();

        //if (userMaster > 0)
        //{
        //    return true;
        //}

        //return false;
        return userMaster1;

    }
    public static string EncodePasswordToBase64(string password)
    {
        try
        {
            byte[] encData_byte = new byte[password.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }
        catch (Exception ex)
        {
            throw new Exception("Error in base64Encode" + ex.Message);
        }
    }
    //this function Convert to Decord your Password
    public string DecodeFrom64(string encodedData)
    {
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        System.Text.Decoder utf8Decode = encoder.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(encodedData);
        int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        string result = new String(decoded_char);
        return result;
    }
}