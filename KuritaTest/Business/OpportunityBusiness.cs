using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using System.Globalization;

namespace KuritaTest.Business
{
    public class OpportunityBusiness
    {
        private readonly IPluginExecutionContext _context;
        private readonly IOrganizationService _service;
        private readonly OrganizationServiceContext _serviceContext;
        private readonly ITracingService _tracing;

        public OpportunityBusiness(IPluginExecutionContext context, IOrganizationService service, OrganizationServiceContext serviceContext, ITracingService tracing)
        {
            _context = context;
            _service = service;
            _serviceContext = serviceContext;
            _tracing = tracing;
        }
        public void SetNewEstimatedCloseDate(DateTime date, Entity opportunity)
        {

            try
            {
                DateTime newDate = date.AddMonths(1);
                opportunity["estimatedclosedate"] = newDate;
                _service.Update(opportunity);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($@"Failure while defining a new close date: {ex}");
            }

        }

        public void NewEstimatedRevenue(Money estimatedvalue, Money totalAmount, Entity opportunity)
        {
            try
            {
                decimal totalRevenue = estimatedvalue.Value * totalAmount.Value;
                opportunity["rr_new_total_estimated_revenue"] = new Money(totalRevenue);

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($@"Failure while defining a new Total Estimated Revenue: {ex}");
            }
        }
    }
}