using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WingTipCommon.Services;
using WingTipToysWebApplication.ViewModels.Report;

namespace WingTipToysWebApplication.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IGraphService _graphService;

        public ReportController(IGraphService graphService)
        {
            _graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));
        }

        public async Task<IActionResult> B2CAuthenticationCountSummaryIndex()
        {
            var report = await _graphService.GetReportAsync("b2cAuthenticationCountSummary", GetCurrentUserId());
            var reportEntries = report["value"].Value<JArray>();
            var reportEntryViewModels = new List<B2CAuthenticationCountSummaryReportEntryViewModel>();

            foreach (var reportEntry in reportEntries)
            {
                var reportEntryViewModel = new B2CAuthenticationCountSummaryReportEntryViewModel
                {
                    Date = reportEntry["Date"].Value<DateTime>(),
                    AuthenticationCount = reportEntry["AuthenticationCount"].Value<int>(),
                    //ImplicitAuthenticationCount = reportEntry["AuthFlowTypeBreakDown"]["implicit"].Value<int>()
                };

                var authorizationCodeAsPublicClientAuthenticationCount = reportEntry["AuthFlowTypeBreakDown"]["publicclient-authcode"];

                if (authorizationCodeAsPublicClientAuthenticationCount != null)
                {
                    reportEntryViewModel.AuthorizationCodeAsPublicClientAuthenticationCount = authorizationCodeAsPublicClientAuthenticationCount.Value<int>();
                }

                var authorizationCodeAsConfidentialClientAuthenticationCount = reportEntry["AuthFlowTypeBreakDown"]["confclient-authcode"];

                if (authorizationCodeAsConfidentialClientAuthenticationCount != null)
                {
                    reportEntryViewModel.AuthorizationCodeAsConfidentialClientAuthenticationCount = authorizationCodeAsConfidentialClientAuthenticationCount.Value<int>();
                }

                var hybridAuthenticationCount = reportEntry["AuthFlowTypeBreakDown"]["hybrid"];

                if (hybridAuthenticationCount != null)
                {
                    reportEntryViewModel.HybridAuthenticationCount = hybridAuthenticationCount.Value<int>();
                }

                var implicitAuthenticationCount = reportEntry["AuthFlowTypeBreakDown"]["implicit"];

                if (implicitAuthenticationCount != null)
                {
                    reportEntryViewModel.ImplicitAuthenticationCount = implicitAuthenticationCount.Value<int>();
                }

                reportEntryViewModels.Add(reportEntryViewModel);
            }

            var viewModel = new B2CAuthenticationCountSummaryIndexViewModel
            {
                ReportEntries = reportEntryViewModels
            };

            return View(viewModel);
        }

        public async Task<IActionResult> B2CMfaRequestCountSummaryIndex()
        {
            var report = await _graphService.GetReportAsync("b2cMfaRequestCountSummary", GetCurrentUserId());
            var reportEntries = report["value"].Value<JArray>();
            var reportEntryViewModels = new List<B2CMfaRequestCountSummaryReportEntryViewModel>();

            foreach (var reportEntry in reportEntries)
            {
                var reportEntryViewModel = new B2CMfaRequestCountSummaryReportEntryViewModel
                {
                    Date = reportEntry["Date"].Value<DateTime>(),
                    MfaCount = reportEntry["MfaCount"].Value<int>(),
                    SmsMfaCount = reportEntry["MfaTypeBreakDown"]["SmsMfa"].Value<int>()
                };

                reportEntryViewModels.Add(reportEntryViewModel);
            }

            var viewModel = new B2CMfaRequestCountSummaryIndexViewModel
            {
                ReportEntries = reportEntryViewModels
            };

            return View(viewModel);
        }

        public async Task<IActionResult> B2CUserJourneySummaryEventsIndex()
        {
            var report = await _graphService.GetReportAsync("b2cUserJourneySummaryEvents", GetCurrentUserId());
            var reportEntries = report["value"].Value<JArray>();
            var reportEntryViewModels = new List<B2CUserJourneySummaryEventsReportEntryViewModel>();

            foreach (var reportEntry in reportEntries)
            {
                var reportEntryViewModel = new B2CUserJourneySummaryEventsReportEntryViewModel
                {
                    Id = reportEntry["Id"].Value<int>(),
                    BasePolicyUniqueId = reportEntry["BasePolicyUniqueId"].Value<string>(),
                    UserJourneyId = reportEntry["UserJourneyId"].Value<string>(),
                    ResourceId = reportEntry["UserJourneyId"].Value<string>(),
                    SuccessCount = reportEntry["SuccessCount"].Value<int>(),
                    FailureCount = reportEntry["FailureCount"].Value<int>(),
                    CallerErrorCount = reportEntry["CallerErrorCount"].Value<int>()
                };

                reportEntryViewModels.Add(reportEntryViewModel);
            }

            var viewModel = new B2CUserJourneySummaryEventsIndexViewModel
            {
                ReportEntries = reportEntryViewModels
            };

            return View(viewModel);
        }

        public async Task<IActionResult> TenantUserCountSummaryIndex()
        {
            var report = await _graphService.GetReportAsync("tenantUserCount", GetCurrentUserId());
            var reportEntries = report["value"].Value<JArray>();
            var reportEntryViewModels = new List<TenantUserCountSummaryReportEntryViewModel>();

            foreach (var reportEntry in reportEntries)
            {
                var reportEntryViewModel = new TenantUserCountSummaryReportEntryViewModel
                {
                    Date = reportEntry["TimeStamp"].Value<DateTime>(),
                    TotalUserCount = reportEntry["TotalUserCount"].Value<int>(),
                    LocalUserCount = reportEntry["LocalUserCount"].Value<int>(),
                    InternalUserCount = reportEntry["OtherUserCount"].Value<int>(),
                    ExternalUserCount = reportEntry["AlternateIdUserCount"].Value<int>()
                };

                if (reportEntry["AlternateIdUserBreakDown"]["amazon_com"] != null)
                {
                    reportEntryViewModel.AmazonExternalUserCount = reportEntry["AlternateIdUserBreakDown"]["amazon_com"].Value<int>();
                }

                if (reportEntry["AlternateIdUserBreakDown"]["facebook_com"] != null)
                {
                    reportEntryViewModel.FacebookExternalUserCount = reportEntry["AlternateIdUserBreakDown"]["facebook_com"].Value<int>();
                }

                reportEntryViewModels.Add(reportEntryViewModel);
            }

            var viewModel = new TenantUserCountSummaryIndexViewModel
            {
                ReportEntries = reportEntryViewModels
            };

            return View(viewModel);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
        }
    }
}
