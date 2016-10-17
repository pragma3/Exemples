using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace ServiceJournalisation.Controllers
{
    public class JournalisationController : ApiController
    {
        
        
        // POST api/<controller>
        public void Post(string applicationName,[FromBody]string message)
        {
            //Simulation d'un délai d'enregistrement.
            //Théoriquement, l'information devrait être enregistrée dans une base de données,
            //le journal des événements, des fichiers plats, xml,...
            Debug.WriteLine("{0}:{1}", applicationName, message);
            Thread.Sleep(250);
        }

        
    }
}