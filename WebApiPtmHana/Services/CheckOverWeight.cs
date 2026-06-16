using System.Diagnostics;

namespace WebApiPtmHana.Services
{
    public class CheckOverWeight
    {
        public int OverWeight(float? sobrePeso, string proceso)
        {
            if (proceso == "pvc")
            {
                if (sobrePeso < 0)
                {
                    return 2;
                }
                else if (sobrePeso > 3.5)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (sobrePeso == 0)
                {
                    return 1;
                }
                else if (sobrePeso > 0)
                {
                    return 3;
                }
                else
                {
                    return 2;
                }
            }
        }
    }
}
