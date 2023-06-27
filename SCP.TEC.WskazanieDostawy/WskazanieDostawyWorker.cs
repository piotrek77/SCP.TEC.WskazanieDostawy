using SCP.TEC.WskazanieDostawy;
using Soneta.Business;
using Soneta.Business.UI;
using Soneta.Handel;
using Soneta.Magazyny;
using System;
using static Soneta.Core.KSeF.API.Models;

[assembly: Worker(typeof(WskazanieDostawyWorker), typeof(PozycjaDokHandlowego))]

namespace SCP.TEC.WskazanieDostawy
{
    public class WskazanieDostawyWorker
    {

        [Context]
        public PozycjaDokHandlowego Poz { get; set; }

        [Context]
        public Context context { get; set; }



        // TODO -> Należy podmienić podany opis akcji na bardziej czytelny dla uzytkownika
        [Action("WskazanieDostawyWorker/ToDo", Mode = ActionMode.SingleSession | ActionMode.ConfirmSave | ActionMode.Progress)]
        public MessageBoxInformation ToDo(/*Context context*/)
        {

            //this.context = context;
            if (context == null)
                throw new Exception ("context == null");


            if (context.Contains(typeof(PozycjaDokHandlowego)))
            {
                this.Poz = (PozycjaDokHandlowego)context[typeof(PozycjaDokHandlowego)];
            }

            return new MessageBoxInformation("Czy wykonać operację?")
            {
                Text = "Opis operacji",
                YesHandler = () => PrzypiszDostawe(),
                NoHandler = () => "Operacja przerwana"
            };


        }




        public string PrzypiszDostawe()
        {
            Log log = new Log("WskazanieDostawy", true);

            if (context == null)
                return "context == null";

            if (Poz == null)
                return "Poz == null";

            if (Poz.Towar == null)
                return "Towar == null";

            var dostawaWorker = new Soneta.Handel.Dostawy.DostawaWorker(
            context,
            new[] { Poz }
            );


            dostawaWorker.Pozycja = Poz;


            double ilosc = Poz.Ilosc.Value;


            foreach (Soneta.Magazyny.Zasob zasob in dostawaWorker.Zasoby)
            {
                using (var transaction = zasob.Session.Logout(true))
                {
                    var wybierzZasob = (Soneta.Magazyny.Dostawy.DostawaWorker)context
                    .CreateObject(null, typeof(Soneta.Magazyny.Dostawy.DostawaWorker), zasob);
                    //wybierzZasob.Pobrano = true;    //pobiera wszystkie zasoby, w razie potrzeby tworzy nowe pozycje dok han
                    // albo
                    //wybierzZasob.Ilość = wybierzZasob.IloscDostepna; //j.w.

                    wybierzZasob.Ilość = new Soneta.Towary.Quantity(System.Math.Min(ilosc, wybierzZasob.IloscDostepna.Value), wybierzZasob.IloscDostepna.Symbol);
                    ilosc -= wybierzZasob.Ilość.Value;
                    wybierzZasob.Info.ZasobInfo.FocusedZasob = zasob;
                    transaction.Commit();
                    //break;
                    if (ilosc <= 0)
                        break;
                }
            }



            var wybierzDostawe = (Soneta.Business.QueryContextInformation)
dostawaWorker.WybierzDostawe();
            wybierzDostawe.GetHandler().DynamicInvoke();


            if (ilosc>0)
            {
                UtworzPozDok(ilosc);
            }
            return "";
        }




        private void UtworzPozDok(double ilosc )
        {
            using (var transaction = Poz.Session.Logout(true))
            {
                var p = new PozycjaDokHandlowego(Poz.Dokument);

                Poz.Session.AddRow(p);

                p.Towar = Poz.Towar;

                p.Ilosc = new Soneta.Towary.Quantity(ilosc, Poz.Ilosc.Symbol);

                p.Cena = Poz.Cena;
                p.Rabat = Poz.Rabat;

                transaction.Commit();

            }
        }
    }


}
