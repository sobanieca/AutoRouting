AutoRouting
===========

AutoRouting module for ASP.NET MVC 4 applications


Are you tired of manually entering .MapRoute() for each non-default controller action? AutoRouting is a solution.

Installation
------------

Simply install AutoRouting package from NuGet to your ASP.NET MVC project:

PM> Install-Package AutoRouting


How does it work?
-----------------

Let's say that you are adding a following controller to your MVC project:

public class SampleController : Controller
{
  public ActionResult SampleMethod(string sampleParameter1, int sampleParameter2)
  {
    return View();
  }
}

Normally, you would have to add following instruction to RouteConfig:

routes.MapRoute(
  name: "SampleControllerSampleMethod",
  url: "SampleController/SampleMethod/{sampleParameter1}/{sampleParameter2}"
  defaults: new { controller = "SampleController", action = "SampleMethod", sampleParameter1 = UrlParameter.Optional, sampleParameter2 = UrlParameter.Optional }
);

With AutoRouting you will not have to perform it as it automatically detects all actions defined by you and maps all routes as soon as MVC application starts.

You can check the magic under "Source" folder.



Remarks
-------
AutoRouting is not currently working with Areas. If you need it to work with it feel free to "git clone" and contribute.

