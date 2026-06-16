namespace WebApiPtmHana.Models.AccountModel
{
    //[{\"STATUS\":\"200\",\"TIPOUSUARIO\":\"Administrador\",\"EMPLEADOID\":177,\"NOMBRECOMPLETO\":\"JULIO GARCIA ESPINOZA\"}]
    public class LoginResultModel
    {
        public string STATUS { get; set; }
        public string TIPOUSUARIO { get; set; }
        public int EMPLEADOID { get; set; }
        public string NOMBRECOMPLETO { get; set; }
        public int PLANTA { get; set; }
    }
}
