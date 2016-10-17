using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ActionFilterMVC.Models;

namespace ActionFilterMVC.Controllers
{
    public class ClientsController : Controller
    {
        // GET: Clients
        public ActionResult Index()
        {
            return View(SampleDBContext.GetClientList());
        }

        // GET: Clients/Details/5
        public ActionResult Details(string number)
        {
            if (number == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = SampleDBContext.GetClient(number);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientID,Name,StreetAddress,PostCode,State,Country,Email")] Client client)
        {
            if (ModelState.IsValid)
            {
                SampleDBContext.AddClient(client);
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(string number)
        {
            if (number == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = SampleDBContext.GetClient(number);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [JournalisationFilter(Message = "L'utilisateur {utilisateur} vient de modifier le client {number} ({StreetAddress}{PostCode}).",
            ProprietesAJournaliser = "StreetAddress,PostCode" )]
        public ActionResult Edit([Bind(Include = "ClientID,Name,StreetAddress,PostCode,State,Country,Email")] Client client)
        {
            if (ModelState.IsValid)
            {
                SampleDBContext.Modify(client);
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(string number)
        {
            if (number == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = SampleDBContext.GetClient(number);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string number)
        {
            SampleDBContext.RemoveClient(number);
            return RedirectToAction("Index");
        }

    }
}
