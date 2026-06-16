namespace WebApiPtmHana.Models.Email
{
    public class EmailModel
    {
        public string tiporeporte { get; set; }
        public string base64PDF { get; set; }
        public string email { get; set; }
    }

    public class SapEmail
    {
        public string NOMBRECOMPLETO { get; set; }
        public string EMAIL { get; set; }
        public string NAME { get; set; }
    }
}
