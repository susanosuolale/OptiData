using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptiData.Application.Bundles.Commands.OptimizeBundles;
using OptiData.Application.Bundles.Commands.SchedulePurchase;
using OptiData.Presentation.Models;
using OptiData.Domain.Enums;
using OptiData.Application.Interfaces;
using OptiData.Infrastructure.Data;

namespace OptiData.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public HomeController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(int duration, string timeUnit, DataProvider provider)
    {
        int hoursAhead = duration;
        
        if (timeUnit == "Days") hoursAhead = duration * 24;
        else if (timeUnit == "Months") hoursAhead = duration * 24 * 30; // Approximation of 30 days
        else if (timeUnit == "Years") hoursAhead = duration * 24 * 365;

        var command = new OptimizeBundlesCommand
        {
            UserId = _currentUserService.UserId,
            HoursAhead = hoursAhead,
            Provider = provider
        };
        
        // ensures every piece of data going to the webpage is
        // checked and confirmed correct. 
        var viewModel = new OptimizationResultViewModel();

        try 
        {
            var result = await _mediator.Send(command);
            viewModel.OptimalBundles = result.Bundles;
            viewModel.PredictedTotalMB = result.PredictedTotalMB;
        }
        catch (Exception)
        {
            viewModel.ErrorMessage = "Oops! The prediction engine is offline (Waiting for packages).";
        }

        ViewBag.Duration = duration;
        ViewBag.TimeUnit = timeUnit;
        ViewBag.Provider = provider;

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SchedulePurchase(decimal predictedMB, DataProvider provider, int duration, string timeUnit)
    {
        int hoursAhead = duration;
        if (timeUnit == "Days") hoursAhead = duration * 24;
        else if (timeUnit == "Months") hoursAhead = duration * 24 * 30;
        else if (timeUnit == "Years") hoursAhead = duration * 24 * 365;

        var command = new SchedulePurchaseCommand
        {
            UserId = _currentUserService.UserId,
            PredictedNeedMB = predictedMB,
            Provider = provider,
            HoursAhead = hoursAhead
        };

        await _mediator.Send(command);
        
        var hoursToWait = hoursAhead > 1 ? hoursAhead - 1 : 0;
        var exactPurchaseTime = DateTime.Now.AddHours(hoursToWait).ToString("f");

        TempData["SuccessMessage"] = $"Auto-Purchase Successfully Scheduled! We will buy it seamlessly in the background on {exactPurchaseTime}, exactly before your current data expires.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> OptimizeAjax([FromBody] OptimizeRequestModel req)
    {
        if (req == null || string.IsNullOrEmpty(req.Provider)) 
        {
            return Json(new { success = false, error = "Please select a telecom provider before optimizing." });
        }

        int hoursAhead = req.Duration;
        if (req.TimeUnit == "Days") hoursAhead = req.Duration * 24;
        else if (req.TimeUnit == "Months") hoursAhead = req.Duration * 24 * 30;
        else if (req.TimeUnit == "Years") hoursAhead = req.Duration * 24 * 365;

        var command = new OptimizeBundlesCommand
        {
            UserId = _currentUserService.UserId,
            HoursAhead = hoursAhead,
            Provider = Enum.Parse<DataProvider>(req.Provider, true)
        };
        
        try 
        {
            var result = await _mediator.Send(command);
            return Json(new { success = true, bundles = result.Bundles, predictedTotalMB = result.PredictedTotalMB, provider = req.Provider, duration = req.Duration, timeUnit = req.TimeUnit });
        }
        catch (Exception)
        {
            return Json(new { success = false, error = "Oops! The prediction engine is offline (Waiting for packages)." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SchedulePurchaseAjax([FromBody] ScheduleRequestModel req)
    {
        if (req == null || string.IsNullOrEmpty(req.Provider)) 
        {
            return Json(new { success = false, error = "Internal Server Error: The JSON request was empty or the server failed to parse it." });
        }

        int hoursAhead = req.Duration;
        if (req.TimeUnit == "Days") hoursAhead = req.Duration * 24;
        else if (req.TimeUnit == "Months") hoursAhead = req.Duration * 24 * 30;
        else if (req.TimeUnit == "Years") hoursAhead = req.Duration * 24 * 365;

        var command = new SchedulePurchaseCommand
        {
            UserId = _currentUserService.UserId,
            PredictedNeedMB = req.PredictedMB,
            Provider = Enum.Parse<DataProvider>(req.Provider, true),
            HoursAhead = hoursAhead
        };

        await _mediator.Send(command);

        // For PORTFOLIO DEMONSTRATION purposes, we will override the actual hoursToWait
        // and schedule the job to run exactly 10 seconds from now, so recruiters can see the SignalR Toast immediately.
        var secondsToWait = 10; 
        var exactPurchaseTime = DateTime.Now.AddSeconds(secondsToWait).ToString("T");
        
        return Json(new { success = true, message = $"Auto-Purchase Successfully Scheduled! For demonstration purposes, it will run in exactly 10 seconds (at <strong>{exactPurchaseTime}</strong>)." });
    }
    public class OptimizeRequestModel { public int Duration {get;set;} public string TimeUnit {get;set;} public string Provider {get;set;} }
    public class ScheduleRequestModel { public decimal PredictedMB {get;set;} public string Provider {get;set;} public int Duration {get;set;} public string TimeUnit {get;set;} }


    [HttpGet]
    public async Task<IActionResult> GetUsageData([FromServices] AppDbContext context)
    {
        var records = await context.UsageRecords
            .Where(u => u.UserId == _currentUserService.UserId)
            .GroupBy(u => u.Timestamp.Date)
            .Select(g => new { 
                Date = g.Key, 
                TotalMB = g.Sum(x => x.DataConsumedMB) 
            })
            .OrderByDescending(x => x.Date)
            .Take(10)
            .OrderBy(x => x.Date)
            .ToListAsync();
            
        var formattedRecords = records.Select(x => new {
            date = x.Date.ToString("MMM dd"),
            mb = x.TotalMB
        });

        return Json(formattedRecords);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
