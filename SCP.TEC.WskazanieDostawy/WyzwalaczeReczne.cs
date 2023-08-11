using Soneta.Business;
using Soneta.Handel;
using Soneta.Magazyny;
using Soneta.Towary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Soneta.Core.KSeF.API.Models;
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
            //HandelModule.PozycjaDokHandlowegoSchema.AddIloscBeforeEdit(WybierzDostawy);

        }

        private static void WybierzDostawy(PozycjaDokHandlowegoRow row, ref Quantity value)
        {



            //https://dok.enova365.pl/Developers/33472
            /*
             * porównywać starą ilość z nową
             * starą ilość pobieramy z jakiejść zmiennej
             * 
             * staą ilość updateujemy po sprrawdzeniu/ ustawieniu nowej ilości
             * 
             * 
             * 
             */ 
            if (row == null) { return; }
            PozycjaDokHandlowego poz = row as PozycjaDokHandlowego;


            if (poz == null) { return; }


            Log log = new Log("wskazanie dostawy", true);

            log.WriteLine($"poz.ilosc={poz.Ilosc}, value={value}");
            if (poz.Ilosc == value)
                return;
            poz.Ilosc = value;

            //if (value.Value == 0)
            //    return;

            //if (poz.Dostawa != null)
            //    return;

            Context cx = Context.Empty.Clone(poz.Session);
            cx[typeof(PozycjaDokHandlowego)] = poz;
            cx[typeof(DokumentHandlowy)] = poz.Dokument;
            cx[typeof(PozycjaDokHandlowego[])] = new PozycjaDokHandlowego[] { poz };

            WskazanieDostawyWorker wskazanieDostawyWorker = new WskazanieDostawyWorker();
            wskazanieDostawyWorker.Poz = poz;
            wskazanieDostawyWorker.context = cx;


            string wynik = wskazanieDostawyWorker.PrzypiszDostawe();

            //Tools.GetInstance().Loguj($"Ilosc={poz.Ilosc}");
        }

        
        static Dictionary<Guid, DateTime> guids = new Dictionary<Guid, DateTime>();
        private static void WybierzDostawy(HandelModule.PozycjaDokHandlowegoRow row)
        {
            Log log = new Log("wskazanie dostawy", true);
            log.WriteLine($"row.Session={row.Session}");

            
            if (row == null) { return; }
            PozycjaDokHandlowego poz = row as PozycjaDokHandlowego;


            if (poz == null) { return; }


            Guid guid = row.Dokument.Guid;
            if (guids.ContainsKey(guid))
            {
                //guids.Remove(guid);

                if (guids[guid].AddSeconds(1) > DateTime.Now)
                    return;
            }

            guids[guid] = DateTime.Now;

            //if (poz.Ilosc.Value == 0)
            //    return;

            //if (poz.Dostawa != null)
            //    return;
            Context cx = Context.Empty.Clone(poz.Session);
            cx[typeof(PozycjaDokHandlowego)] = poz;
            cx[typeof(DokumentHandlowy)] = poz.Dokument;




            //if (poz.Dostawa != null)
            //{
            //    Log log = new Log("wskazanie dostawy", true);
            //    foreach(Zasob zasob in poz.Zasoby)
            //    {
            //        log.WriteLine("zasob={0}", zasob);
            //    }

            //    //var dostawaWorker = new Soneta.Handel.Dostawy.DostawaWorker(
            //    //   cx,
            //    //   new[] { poz }
            //    //   );


            //    //dostawaWorker.Pozycja = poz;

            //    //foreach(Zasob zasob in dostawaWorker.Zasoby)
            //    //{

            //    //    if (poz.Ilosc == zasob.Ilosc && poz.Zasoby == zasob)
            //    //        return;

            //    //    //Soneta.Magazyny.Dostawy.DostawaWorker dostWork = new Soneta.Magazyny.Dostawy.DostawaWorker(cx);
            //    //    //dostWork.Zasób = zasob;
            //    //    //if (poz.Ilosc == dostWork.Ilość)
            //    //    //    return;
            //    //}

                
            //}
            //return;
            

            WskazanieDostawyWorker wskazanieDostawyWorker = new WskazanieDostawyWorker();
            wskazanieDostawyWorker.Poz = poz;
            wskazanieDostawyWorker.context = cx;


            string wynik = wskazanieDostawyWorker.PrzypiszDostawe();

            //Tools.GetInstance().Loguj($"Ilosc={poz.Ilosc}");
        }
    }
}
