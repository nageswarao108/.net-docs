﻿using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{
#pragma warning disable CS8604 // Possible null reference argument.
    #region snippet
    public IActionResult Index()
    {
        var url = Url.Action("Buy", "Products", new { id = 17, color = "red" });
        return Content(url!);
    }
    #endregion
    #region snippet2
    public IActionResult Index2()
    {
        var url = Url.Action("Buy", "Products", new { id = 17 }, protocol: Request.Scheme);
        // Returns https://localhost:5001/Products/Buy/17
        return Content(url);
    }
    #endregion
#pragma warning restore CS8604 // Possible null reference argument.
}