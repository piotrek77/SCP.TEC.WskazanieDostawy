using SCP.TEC.WskazanieDostawy;
using Soneta.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[assembly: ProgramInitializer(typeof(AutostartWskazanieDostawy))]
namespace SCP.TEC.WskazanieDostawy
{
    public class AutostartWskazanieDostawy : IProgramInitializer
    {
        public void Initialize()
        {
            //Tools singleton = Tools.GetInstance();

            //singleton.Loguj(this.GetType().Name);



            _ = new WyzwalaczeReczneWskazanieDostawy();
            
        }
    }
}
