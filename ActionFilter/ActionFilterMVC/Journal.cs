using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Web;

namespace ActionFilterMVC
{
    public class Journal
    {
        //Basé sur http://stackoverflow.com/questions/1181561/how-to-effectively-log-asynchronously

        #region membres
        private static volatile Journal singletonInstance;
        private static object singletonLock = new Object();

        Queue<Action> requestQueue = new Queue<Action>();

        ManualResetEvent newElementPresent = new ManualResetEvent(false);
        ManualResetEvent waitingForRequest = new ManualResetEvent(false);

        Thread thread;

        #endregion

        #region constructeur
        /// <summary>
        /// Constructeur privé. Il faut passer par la propriété Journaliseur.Instance pour accéder à la fonctionnalité de journalisation.
        /// </summary>
        private Journal()
        {
            StartThread();
        }
        #endregion
        #region méthodes publiques & internal
        /// <summary>
        /// Permet d'initialiser les valeurs de fonctionnement pour les appels au service de journalisation
        /// </summary>
        /// <param name="applicationName">Nom de l'application</param>
        /// <param name="webServiceUrl">L'url du web service de journalisation</param>
        public void Initialize(string webServiceUrl)
        {
            this.ApplicationName = GetAssemblyTitle();
            this.ServiceWebUrl = webServiceUrl;
        }

        private string GetAssemblyTitle()
        {
            // Get the Assembly object to access its metadata.
            Assembly assy = Assembly.GetCallingAssembly();
            string title="";
            // Iterate through the attributes for the assembly.
            foreach (Attribute attr in Attribute.GetCustomAttributes(assy))
            {
                // Check for the AssemblyTitle attribute.
                if (attr.GetType() == typeof(AssemblyTitleAttribute))
                {
                    title = ((AssemblyTitleAttribute)attr).Title;
                    break;
                }
            }
            return title;
        }

        /// <summary>
        /// Envoyer une demande de journalisation de message.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ApplicationException">Levée si le nom de l'application n'est pas renseigné. (Soit par la propriété, soit par la méthode Initialiser)</exception>
        public void Log(string message)
        {

            if (string.IsNullOrEmpty(ApplicationName))
            {
                throw new ApplicationException("La propriété ApplicationName n'a pas été initialisée.");
            }

            lock (requestQueue)
            {
                requestQueue.Enqueue(() => InternalLog(message));
            }

            newElementPresent.Set();
        }

        /// <summary>
        /// Envoyer une demande de journalisation de message en faisant une mise en forme avec String.format.
        /// </summary>
        /// <param name="messageFormat">Chaîne de format composite.</param>
        /// <param name="args">Tableau d'objets contenant aucun ou plusieurs objets à mettre en forme.</param>
        /// <exception cref="ApplicationException">Si le nom de l'application n'est pas renseigné. (A renseigner par la propriété ou par la méthode Initialiser)</exception>
        public void Log(String messageFormat, params object[] args)
        {
            Log(CreateFormattedMessage(messageFormat, args));
        }

        /// <summary>
        /// Vider la liste des demandes en attente.
        /// </summary>
        public void Flush()
        {
            waitingForRequest.WaitOne();
        }


        #endregion

        #region méthodes privées

        private string CreateFormattedMessage(string messageFormat, object[] args)
        {
            if (args == null)
            {
                return messageFormat;
            }
            else
            {
                return string.Format(messageFormat, args);

            }
        }

        private void StartThread()
        {
            thread = new Thread(new ThreadStart(ManageQueue));

            thread.IsBackground = false;

            thread.Start();
        }

        private void ManageQueue()
        {
            while (true)
            {
                waitingForRequest.Set();

                newElementPresent.WaitOne();

                newElementPresent.Reset();

                waitingForRequest.Reset();

                ExecuteQueueActions();
            }
        }

        private void ExecuteQueueActions()
        {
            Queue<Action> copieQueue;

            lock (requestQueue)
            {
                copieQueue = new Queue<Action>(requestQueue);
                requestQueue.Clear();
            }

            foreach (var actionJournalisation in copieQueue)
            {
                actionJournalisation.Invoke();
            }

        }

        private void InternalLog(string message)
        {
            if (string.IsNullOrEmpty(ServiceWebUrl))
                return;

            using (var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true }))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Pour simplifier l'analyse de l'utilisation du système de journalisation, nous remplaçons le user-agent
                //avec le nom de l'application. Cette information est disponible dans les logs d'IIS
                client.DefaultRequestHeaders.Add("User-Agent", ApplicationName);

                client.PostAsJsonAsync( ServiceWebUrl + ApplicationName, message).Wait();
            }
        }

        #endregion

        #region propriétés

        public string ServiceWebUrl { get; set; }
        public string ApplicationName { get; set; }

        public static Journal Instance
        {
            get
            {
                if (singletonInstance == null)
                {
                    lock (singletonLock)
                    {
                        if (singletonInstance == null)
                            singletonInstance = new Journal();
                    }
                }

                return singletonInstance;
            }
        }
        #endregion
    }
}