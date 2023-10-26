using KuritaTest.Business;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace KuritaTest
{
    public class OportunityPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            OrganizationServiceContext serviceContext = new OrganizationServiceContext(service);
            ITracingService tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (!context.InputParameters.Contains("Target")) return;
            if (!(context.InputParameters["Target"] is Entity)) return;
            if (((Entity)context.InputParameters["Target"]).LogicalName != "opportunity") return;

            Entity target = (Entity)context.InputParameters["Target"];


            #region CREATE POST-OPERATION
            if (ExecuteCreatePostOperation(context))
            {
                // Set Estimated Close Date
                /* This function triggers on create of a new oportunity 
                 * record and sets the estimatedclosedate to 
                 * one month after the inserted date. */
                if (target.Contains("estimatedclosedate") && target.GetAttributeValue<DateTime>("estimatedclosedate") != null)
                {
                    OpportunityBusiness oppBusiness = new OpportunityBusiness(context, service, serviceContext, tracing);
                    try
                    {
                        oppBusiness.SetNewEstimatedCloseDate(target.GetAttributeValue<DateTime>("estimatedclosedate"), target);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException($"Set Estimated Close Date: {ex.Message}");
                    }
                }

                // New Estimated Revenue
                /* This function triggers either on create or update (estimatedvalue or totalamount fields)
                 * of an opportunity. When triggered, it will multiply both mentioned values and set a new 
                 * fields (rr_new_total_estimated_revenue) with it's value. */
                if (target.Contains("estimatedvalue") && target.GetAttributeValue<Money>("estimatedvalue") != null)
                {
                    if (target.Contains("totalamount"))
                    {
                        OpportunityBusiness oppBusiness = new OpportunityBusiness(context, service, serviceContext, tracing);
                        try
                        {
                            oppBusiness.NewEstimatedRevenue
                                (target.GetAttributeValue<Money>("estimatedvalue"),
                                target.GetAttributeValue<Money>("estimatedvalue"),
                                target);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidPluginExecutionException($"New Estimated Revenue: {ex.Message}");
                        }
                    }
                }

            }
            #endregion


            #region UPDATE PRE-OPERATION
            if (ExecuteUpdatePreOperation(context))
            {

                // New Estimated Revenue
                /* This function triggers either on create or update (estimatedvalue or totalamount fields)
                 * of an opportunity. When triggered, it will multiply both mentioned values and set a new 
                 * fields (rr_new_total_estimated_revenue) with it's value. */
                if (target.Contains("estimatedvalue") && target.GetAttributeValue<Money>("estimatedvalue") != null)
                {
                    if (target.Contains("totalamount") && target.GetAttributeValue<Money>("totalamount") != null)
                    {
                        if(context.Depth > 2)
                        {
                            return;
                        }
                        OpportunityBusiness oppBusiness = new OpportunityBusiness(context, service, serviceContext, tracing);
                        try
                        {

                            Entity preImage = context.PreEntityImages.Contains("preImageOpportunity") ?
                                context.PreEntityImages["preImageOpportunity"] : null;

                            oppBusiness.NewEstimatedRevenue
                                (preImage.GetAttributeValue<Money>("estimatedvalue"),
                                preImage.GetAttributeValue<Money>("totalamount"),
                                target);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidPluginExecutionException($"New Estimated Revenue: {ex.Message}");
                        }
                    }
                }

            }
            #endregion

        }

        private bool ExecuteUpdatePreOperation(IPluginExecutionContext context)
        {
            if (context.MessageName.ToUpper() != "UPDATE") return false;
            if (context.Stage != 20) return false;

            return true;
        }

        private bool ExecuteUpdatePostOperation(IPluginExecutionContext context)
        {
            if (context.MessageName.ToUpper() != "UPDATE") return false;
            if (context.Stage != 40) return false;

            return true;
        }

        private bool ExecuteCreatePostOperation(IPluginExecutionContext context)
        {
            if (context.MessageName.ToUpper() != "CREATE") return false;
            if (context.Stage != 40) return false;

            return true;
        }

        private bool ExecuteCreatePreOperation(IPluginExecutionContext context)
        {
            if (context.MessageName.ToUpper() != "CREATE") return false;
            if (context.Stage != 20) return false;

            return true;
        }

    }
}
