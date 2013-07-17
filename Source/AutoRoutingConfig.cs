using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using WebStart;

namespace $rootnamespace$.App_Start
{
    public class AutoRoutingConfg : Config
    {
        private List<string> _usedRoutesNames = new List<string>();

        public override void Setup()
        {
            var routes = RouteTable.Routes;
            //Since this method is being run after Application_Start there is need to remove the already created "Default" route:
            routes.Remove(routes["Default"]);
            //Reading all types available in assembly
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in types)
            {
                //Selecting only those that inherit from Controller class and are not abstract
                if (t.IsSubclassOf(typeof(Controller)) && !t.IsAbstract)
                {
                    string controllerName = t.Name.Replace("Controller", "");
                    var methods = t.GetMethods();
                    //Retrieving only methods that were declared for a given controller
                    foreach (var method in methods.Where(x => x.DeclaringType == t).ToList())
                    {
                        //Only public methods are taken into account
                        if (method.IsPublic)
                        {
                            //POST actions don't need mapping
                            if (method.GetCustomAttribute(typeof(HttpPostAttribute)) == null)
                            {
                                ParameterInfo[] parameters = method.GetParameters();
                                if (parameters.Length > 0)
                                {
                                    //No need to register default action:
                                    if (parameters.Length == 1 && parameters[0].Name.ToLower() == "id")
                                        continue;
                                    string actionName = method.Name;
                                    string routeName = String.Format("{0}-{1}-{2}", controllerName, actionName, Guid.NewGuid().ToString("N"));
                                    string routeUrl = String.Format("{0}/{1}/", controllerName, actionName);
                                    string parametersPart = GetParametersPart(parameters);
                                    object routeObject = GetRouteObject(controllerName, actionName, parameters);
                                    routeUrl = String.Format("{0}{1}", routeUrl, parametersPart);
                                    
                                    routes.MapRoute(routeName, routeUrl, routeObject);
                                }
                            }
                        }
                    }
                }
            }
            //Restoring default route as the last one:
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        private object GetRouteObject(string controllerName, string actionName, ParameterInfo[] parameters)
        {
            var result = new ExpandoObject() as IDictionary<string, object>;

            result.Add("controller", controllerName);
            result.Add("action", actionName);
            foreach (var parameter in parameters)
            {
                result.Add(parameter.Name, UrlParameter.Optional);
            }

            return result;
        }

        private string GetParametersPart(ParameterInfo[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var parameter in parameters)
            {
                sb.AppendFormat("{{{0}}}/", parameter.Name);
            }
            return sb.ToString();
        }
    }
}