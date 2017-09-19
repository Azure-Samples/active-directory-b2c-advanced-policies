using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WingTipCommon.Services;
using WingTipToysWebApplication.ViewModels.Audit;

namespace WingTipToysWebApplication.Controllers
{
    [Authorize]
    public class AuditController : Controller
    {
        private readonly IGraphService _graphService;

        public AuditController(IGraphService graphService)
        {
            if (graphService == null)
            {
                throw new ArgumentNullException(nameof(graphService));
            }

            _graphService = graphService;
        }

        public async Task<IActionResult> ApplicationIndex()
        {
            const int Top = 50;

            var audit = await _graphService.GetAuditAsync("Application", Top, GetCurrentUserId());
            var auditEntries = audit["value"].Value<JArray>();
            var auditEntryViewModels = new List<AuditEntryViewModel>();

            foreach (var auditEntry in auditEntries)
            {
                var auditEntryViewModel = new AuditEntryViewModel
                {
                    Activity = auditEntry["activity"].Value<string>(),
                    ActivityDate = auditEntry["activityDate"].Value<DateTime>(),
                    ActorType = auditEntry["actorType"].Value<string>()
                };

                if (auditEntryViewModel.ActorType == "Application")
                {
                    auditEntryViewModel.ActorName = auditEntry["actor"]["name"].Value<string>();
                }
                else if (auditEntryViewModel.ActorType == "User")
                {
                    auditEntryViewModel.ActorName = auditEntry["actor"]["userPrincipalName"].Value<string>();
                }

                auditEntryViewModel.CorrelationId = auditEntry["correlationId"].Value<string>();
                var targetResources = auditEntry["targets"].Value<JArray>();
                var targetResource = targetResources[0];
                auditEntryViewModel.TargetResourceType = targetResource["targetResourceType"].Value<string>();

                if (auditEntryViewModel.TargetResourceType == "Application")
                {
                    auditEntryViewModel.TargetResourceName = targetResource["name"].Value<string>();
                }
                else if (auditEntryViewModel.TargetResourceType == "ServicePrincipal")
                {
                    auditEntryViewModel.TargetResourceName = targetResource["name"].Value<string>();
                }
                else if (auditEntryViewModel.TargetResourceType == "User")
                {
                    auditEntryViewModel.TargetResourceName = targetResource["userPrincipalName"].Value<string>();
                }

                auditEntryViewModels.Add(auditEntryViewModel);
            }

            var viewModel = new IndexViewModel
            {
                AuditEntries = auditEntryViewModels,
                Top = Top
            };

            return View(viewModel);
        }

        public async Task<IActionResult> UserIndex()
        {
            const int Top = 50;

            var audit = await _graphService.GetAuditAsync("User", Top, GetCurrentUserId());
            var auditEntries = audit["value"].Value<JArray>();
            var auditEntryViewModels = new List<AuditEntryViewModel>();

            foreach (var auditEntry in auditEntries)
            {
                var auditEntryViewModel = new AuditEntryViewModel
                {
                    Activity = auditEntry["activity"].Value<string>(),
                    ActivityDate = auditEntry["activityDate"].Value<DateTime>(),
                    ActorType = auditEntry["actorType"].Value<string>()
                };

                if (auditEntryViewModel.ActorType == "Application")
                {
                    auditEntryViewModel.ActorName = auditEntry["actor"]["name"].Value<string>();
                }
                else if (auditEntryViewModel.ActorType == "User")
                {
                    auditEntryViewModel.ActorName = auditEntry["actor"]["userPrincipalName"].Value<string>();
                }

                auditEntryViewModel.CorrelationId = auditEntry["correlationId"].Value<string>();
                var targetResources = auditEntry["targets"].Value<JArray>();
                var targetResource = targetResources[0];
                auditEntryViewModel.TargetResourceType = targetResource["targetResourceType"].Value<string>();

                if (auditEntryViewModel.TargetResourceType == "ServicePrincipal")
                {
                    auditEntryViewModel.TargetResourceName = targetResource["name"].Value<string>();
                }
                else if (auditEntryViewModel.TargetResourceType == "User")
                {
                    auditEntryViewModel.TargetResourceName = targetResource["userPrincipalName"].Value<string>();
                }

                auditEntryViewModels.Add(auditEntryViewModel);
            }

            var viewModel = new IndexViewModel
            {
                AuditEntries = auditEntryViewModels,
                Top = Top
            };

            return View(viewModel);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
        }
    }
}
