using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ExempleCache
{
    public partial class OutputCache0 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            lblTexte.Text = "L'id vaut " + id;
            lblHeure.Text = DateTime.Now.ToString();
        }
        public static string ObtenirHeure(HttpContext context)
        {
            return DateTime.Now.ToString();
        }
    }
}