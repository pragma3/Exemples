using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ActionFilterMVC
{
    public class JournalisationFilterAttribute : ActionFilterAttribute
    {
        public string Message { get; set; }
        public string ProprietesAJournaliser { get; set; }
        private Dictionary<string, string> InitialiserDictionnaireProprietes(object parametreAction)
        {

            //Gérer le fait que le dictionnaire doit être par filterContext
            var dictionnaireProprietes = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(ProprietesAJournaliser))
            {
                return null;
            }

            var splitted = ProprietesAJournaliser.Split(',');
            foreach (string nomPropriete in splitted)
            {
                var valeur = parametreAction.GetType().GetProperty(nomPropriete).GetValue(parametreAction, null) ?? string.Empty;

                dictionnaireProprietes.Add(nomPropriete, valeur.ToString());
            }
            return dictionnaireProprietes;
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var parametreAction = filterContext.ActionParameters.Values.FirstOrDefault();
            if (parametreAction != null)
            {
                var dic = InitialiserDictionnaireProprietes(parametreAction);
                filterContext.HttpContext.Items.Add("dic", dic);
            }
        }


        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var message = ReplaceValues(filterContext, Message);

            Journal.Instance.Log(message);
        }

        private string ReplaceValues(ActionExecutedContext filterContext, string message)
        {
            if (filterContext.RequestContext.RouteData.Values.ContainsKey("number"))
            {
                var number = filterContext.RequestContext.RouteData.Values["number"].ToString();
                message = message.Replace("{number}", number);
            }
            var dic = filterContext.HttpContext.Items["dic"] as Dictionary<string, string>;
            if (dic!= null && dic.Any())
            {
                foreach (string nomPropriete in dic.Keys)
                {
                    message = message.Replace("{" + nomPropriete + "}", dic[nomPropriete]);
                }
            }

            message = message.Replace("{utilisateur}", HttpContext.Current.User.Identity.Name);

            return message;
        }
    }
}