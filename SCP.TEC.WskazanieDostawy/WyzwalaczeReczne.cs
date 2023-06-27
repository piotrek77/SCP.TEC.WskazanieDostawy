using Soneta.Business;
using Soneta.Handel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Soneta.Handel.HandelModule;

namespace SCP.TEC.WskazanieDostawy
{
    public class WyzwalaczeReczneWskazanieDostawy
    {

        //konstruktor statyczny jest lepszy, gdyż jest wołany tylko raz
        static WyzwalaczeReczneWskazanieDostawy()
        {
            //Tools.GetInstance().Loguj("WyzwalaczeReczne - konstruktor statyczny");


            HandelModule.PozycjaDokHandlowegoSchema.AddIloscAfterEdit(WybierzDostawy);


        }

        private static void WybierzDostawy(HandelModule.PozycjaDokHandlowegoRow row)
        {
            if (row == null) { return; }
            PozycjaDokHandlowego poz = row as PozycjaDokHandlowego;


            if (poz == null) { return; }


            if (poz.Ilosc.Value == 0)
                return;

            if (poz.Dostawa != null)
                return;

            Context cx = Context.Empty.Clone(poz.Session);
            cx[typeof(PozycjaDokHandlowego)] = poz;
            cx[typeof(DokumentHandlowy)] = poz.Dokument;

            WskazanieDostawyWorker wskazanieDostawyWorker = new WskazanieDostawyWorker();
            wskazanieDostawyWorker.Poz = poz;
            wskazanieDostawyWorker.context = cx;


            string wynik = wskazanieDostawyWorker.PrzypiszDostawe();

            //Tools.GetInstance().Loguj($"Ilosc={poz.Ilosc}");
        }
    }
}
