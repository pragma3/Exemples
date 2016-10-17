using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ActionFilterMVC
{
    public class JournalisationFilterAttribute : ActionFilterAttribute
    {

        public string Message { get; set; }
        public string ProprietesAJournaliser { get; set; }
        public Dictionary<string, string> DictionnaireProprietes { get; private set; }
        private void InitialiserDictionnaireProprietes(object parametreAction)
        {
            DictionnaireProprietes = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(ProprietesAJournaliser))
            {
                return;
            }

            var splitted = ProprietesAJournaliser.Split(',');
            foreach (string nomPropriete in splitted)
            {
                var valeur = parametreAction.GetType().GetProperty(nomPropriete).GetValue(parametreAction, null) ?? string.Empty;

                DictionnaireProprietes.Add(nomPropriete, valeur.ToString());
            }

        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var parametreAction = filterContext.ActionParameters.Values.FirstOrDefault();
            if (parametreAction != null)
            {
                InitialiserDictionnaireProprietes(parametreAction);
            }
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            Message = ReplaceValues(filterContext, Message);

            Journal.Instance.Log(Message);
        }

        private string ReplaceValues(ActionExecutedContext filterContext, string message)
        {
            if (filterContext.RequestContext.RouteData.Values.ContainsKey("number"))
            {
                var number = filterContext.RequestContext.RouteData.Values["number"].ToString();
                message = message.Replace("{number}", number);
            }
            if (DictionnaireProprietes != null && DictionnaireProprietes.Any())
            {
                foreach (string nomPropriete in DictionnaireProprietes.Keys)
                {
                    message = message.Replace("{" + nomPropriete + "}", DictionnaireProprietes[nomPropriete]);
                }
            }

            message = message.Replace("{utilisateur}", HttpContext.Current.User.Identity.Name);

            return message;
        }
    }
}