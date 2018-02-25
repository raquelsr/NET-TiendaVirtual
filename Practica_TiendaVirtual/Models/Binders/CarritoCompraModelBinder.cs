using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Practica_TiendaVirtual.Models.Binders
{
    public class CarritoCompraModelBinder : IModelBinder
    {
        private string id_session = "ID_session";
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            CarritoCompra cc = (CarritoCompra)controllerContext.HttpContext.Session[id_session];

            if (cc == null)
            {
                cc = new CarritoCompra();
                controllerContext.HttpContext.Session[id_session] = cc;
            }

            return cc;
        }
    }
}